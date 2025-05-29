# unity_openai_texture
[ENGLISH README](README.md)

![Image](https://github.com/user-attachments/assets/75a47d30-aa0c-476b-8ee5-d477d713e81c)

OPEN AIのAPIを利用してUnity Editor上でTextureが作成できる拡張です

## インストール方法
Window -> Package Manager -> Install package from git URL...を選択し、下記のURLを入力してインストールしてください。
```
https://github.com/tkada/unity_openai_texture.git
```

## 使用方法
1.メニューから`Tools -> OpenAI -> Shader Creator`を選択してウィンドウを開きます。

2.`API KEY`欄にOpenAIのAPIキーを入力します。

3.`Prompt`欄に生成したい画像の説明（プロンプト）を入力し、`Create Texture`ボタンをクリックすると画像が生成されます。

4.生成された画像は`Save Texture`ボタンをクリックすることで保存できます。

- `Model`：使用するAIモデルを選択します。

- `Size`：生成する画像の解像度を指定します。

- `Number of generations`：生成する画像の枚数を指定します。

## 関連プロジェクト
- [unity_openai_shader](https://github.com/tkada/unity_openai_shader)