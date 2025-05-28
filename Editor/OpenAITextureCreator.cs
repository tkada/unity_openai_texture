using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Text;
using UnityEngine.Networking;
using Unity.EditorCoroutines.Editor;
using System.Linq;
using System.Collections.Generic;

namespace OpenAI
{
    public class OpenAITextureCreator : EditorWindow
    {
        [MenuItem("Tools/OpenAI/Texture Creator")]
        public static void ShowWindow()
        {
            GetWindow<OpenAITextureCreator>("OpenAI Texture Creator");
        }

        string apiKey = string.Empty;
        string prompt = "";

        
        const string PROMPT_PREFS = "TextureCreator_Prompt";

        readonly string[] modelOptions = new string[] { "dall-e-2", "dall-e-3", "gpt-image-1"};
        private int selectedModelIndex = 0;

        readonly string[] spinner = new string[] { "|", "/", "-", "\\" };
        int spinnerIndex = 0;
        bool isLoading = false;

        readonly string[] SizeStr = new string[] { "256x256", "512x512", "1024x1024", "2048x2048" };
        int selectedSizeIndex = 1; // 初期値（例：512x512）

        int generateNumber = 1; // 生成する画像の数

        const string OPEN_AI_API_KEY_PREF = "OpenAI_API_Key";

        private void OnEnable()
        {
            this.apiKey = EditorPrefs.GetString(OPEN_AI_API_KEY_PREF, string.Empty);
            this.prompt = EditorPrefs.GetString(PROMPT_PREFS, "A beautiful landscape with mountains and a river");
            EditorApplication.update += Spin;
        }

        private void OnDisable()
        {
            EditorPrefs.SetString(OPEN_AI_API_KEY_PREF, this.apiKey);
            EditorPrefs.SetString(PROMPT_PREFS, this.prompt);
            EditorApplication.update -= Spin;
        }

        void Spin()
        {
            if (isLoading)
            {
                spinnerIndex = (spinnerIndex + 1) % spinner.Length;
                Repaint(); // UIを毎フレーム再描画
            }
        }

        private Vector2 _scrollPosition = Vector2.zero;

        private void OnGUI()
        {
            GUILayout.Label("OpenAI Texture Creator", EditorStyles.boldLabel);

            this.apiKey = EditorGUILayout.TextField("API Key", this.apiKey);

            if (string.IsNullOrEmpty(this.apiKey))
            {
                GUILayout.Label("Input OpenAI API KEY", EditorStyles.helpBox);
                return;
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Prompt");
            var style = new GUIStyle( EditorStyles.textArea )
                    {
                        wordWrap = true
                    };
            this.prompt = EditorGUILayout.TextArea(this.prompt, style, GUILayout.Height(100));

            this.selectedModelIndex = EditorGUILayout.Popup("Model", this.selectedModelIndex, this.modelOptions);
            this.selectedSizeIndex = EditorGUILayout.Popup("Size", this.selectedSizeIndex, this.SizeStr);
            this.generateNumber = EditorGUILayout.IntField("Number of generations", this.generateNumber);

            EditorGUILayout.Space();
            // Add your GUI elements here
            if (GUILayout.Button("Create Texture"))
            {
                CreateTexture(prompt);
            }

            if (isLoading)
            {
                GUILayout.Label("Loading " + spinner[spinnerIndex]);
            }

            EditorGUILayout.Space();

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            foreach(var image in generatedTexture)
            {
                if (GUILayout.Button("Save Texture"))
                {
                    string path = EditorUtility.SaveFilePanel("Save Texture", "", "GeneratedTexture.png", "png");
                    if (!string.IsNullOrEmpty(path))
                    {
                        System.IO.File.WriteAllBytes(path, image.EncodeToPNG());
                        AssetDatabase.Refresh();
                        Debug.Log("Texture saved to: " + path);
                    }
                }
                // ウィンドウ幅に合わせて高さを自動計算（アスペクト比維持）
                float aspect = (float)image.height / image.width;
                float maxWidth = EditorGUIUtility.currentViewWidth - 20f; // 余白を考慮
                float height = maxWidth * aspect;

                // レイアウト管理下で描画領域を取得
                Rect textureRect = GUILayoutUtility.GetRect(maxWidth, height, GUILayout.ExpandWidth(true));

                // アスペクト比を維持しつつ描画
                GUI.DrawTexture(textureRect, image, ScaleMode.ScaleToFit);
                EditorGUILayout.Space();
            }

            EditorGUILayout.EndScrollView();
        }

        private void CreateTexture(string prompt)
        {
            // Placeholder for texture creation logic
            Debug.Log("Texture creation logic goes here.");
            this.StartCoroutine(GenerateImageCoroutine(prompt));
        }

        IEnumerator GenerateImageCoroutine(string prompt)
        {
            this.isLoading = true;

            //Note:https://platform.openai.com/docs/api-reference/images/create
            string url = "https://api.openai.com/v1/images/generations";

            // JSONボディ
            string json = JsonUtility.ToJson(new ImageGenerationRequest
            {
                model = this.modelOptions[this.selectedModelIndex],
                prompt = prompt,
                n = this.generateNumber,
                size = this.SizeStr[this.selectedSizeIndex],
            });

            Debug.Log("Request JSON: " + json);

            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

            UnityWebRequest request = new UnityWebRequest(url, "POST");
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + apiKey);

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("OpenAI API Error: " + request.error);
                this.isLoading = false;
            }
            else
            {
                var result = JsonUtility.FromJson<ImageGenerationResponse>(request.downloadHandler.text);
                this.StartCoroutine(DownloadImage(result.data.Select(data => data.url).ToArray()));
            }
        }

        List<Texture2D> generatedTexture = new List<Texture2D>();

        IEnumerator DownloadImage(string[] imageUrl)
        {
            this.generatedTexture.Clear();
            foreach (var url in imageUrl)
            {
                yield return DownloadSingleImage(url);
            }
            this.isLoading = false;
        }

        IEnumerator DownloadSingleImage(string imageUrl)
        {
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUrl);
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Image download failed: " + request.error);
            }
            else
            {
                this.generatedTexture.Add(DownloadHandlerTexture.GetContent(request));
                Debug.Log("Image successfully loaded and applied.");
            }
        }

        [Serializable]
        public class ImageGenerationRequest
        {
            public string model;
            public string prompt;
            public int n;
            public string size;
        }

        [Serializable]
        public class ImageGenerationResponse
        {
            public ImageData[] data;
        }

        [Serializable]
        public class ImageData
        {
            public string url;
        }
    }
}


