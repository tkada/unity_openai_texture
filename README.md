# unity_openai_texture

[日本語README](README_JP.md)

![Image](https://github.com/user-attachments/assets/75a47d30-aa0c-476b-8ee5-d477d713e81c)

This extension allows you to generate textures within the Unity Editor using the OpenAI API.

## Installation

Select `Window -> Package Manager -> Install package from git URL...` and enter the following URL to install:

```
https://github.com/tkada/unity_openai_texture.git
```

## Usage

1. Open the window from the menu: `Tools -> OpenAI -> Shader Creator`.
2. Enter your OpenAI API key in the `API KEY` field.
3. Enter a prompt describing the image you want to generate in the `Prompt` field, then click the `Create Texture` button to generate the image.
4. To save the generated image, click the `Save Texture` button.

- `Model`: Select the AI model to use.
- `Size`: Specify the resolution of the generated image.
- `Number of generations`: Specify the number of images to generate.

## Related Projects

- [unity_openai_shader](https://github.com/tkada/unity_openai_shader)
