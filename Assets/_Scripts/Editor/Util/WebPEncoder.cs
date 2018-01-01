/**
  * WebPEncoder.cs
  * Created by: lfj20
  * Created on: 2017/12/28
  */

using System.IO;
using UnityEditor;
using UnityEngine;

namespace _Scripts.Editor.Util {
    public static class WebPEncoder {
        public static void Encode(Texture2D texture2D, string outputPath, int quality = 80) {
            CommonUtil.ClearConsole();
            EditorUtility.DisplayProgressBar("Encoding...", "Loading original atlas", 0f);
            if (texture2D != null) {
                string path = AssetDatabase.GetAssetPath(texture2D);
                string tempPath = Application.temporaryCachePath + texture2D.name + "_temp.png";
                if (!string.IsNullOrEmpty(path)) {
                    TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
                    if (textureImporter != null) {
                        var textureSettings = new TextureImporterSettings {
                            maxTextureSize = 2048,
                            mipmapEnabled = false,
                            textureFormat = TextureImporterFormat.RGBA32,
                            readable = true
                        };
                        textureImporter.SetTextureSettings(textureSettings);
                        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                    } else {
                        Debug.LogError("TextureImporter is null!");
                        Debug.LogWarning("Encoding suspended!");
                        return;
                    }

                    EditorUtility.DisplayProgressBar("Encoding...", "Creating temp atlas file.", 0.1f);
                    Texture2D newTexture2D = new Texture2D(texture2D.width, texture2D.height);
                    for (int i = 0; i < texture2D.width; i++) {
                        for (int j = 0; j < texture2D.height; j++) {
                            newTexture2D.SetPixel(i, j, texture2D.GetPixel(i, texture2D.height - j - 1));
                        }
                    }

                    newTexture2D.Apply();
                    FileStream fileStream = File.Open(tempPath, FileMode.Create);
                    byte[] bytes = newTexture2D.EncodeToPNG();
                    fileStream.Write(bytes, 0, bytes.Length);
                    fileStream.Flush();
                    fileStream.Close();
                } else {
                    FileStream fileStream = File.Open(tempPath, FileMode.Create);
                    byte[] bytes = texture2D.EncodeToPNG();
                    fileStream.Write(bytes, 0, bytes.Length);
                    fileStream.Flush();
                    fileStream.Close();
                }

                EditorUtility.DisplayProgressBar("Encoding...", "Encoding into webp.", 0.2f);
                if (File.Exists(tempPath)) {
                    string defaultPrams = "-q " + quality + " -mt -m 6 -af -alpha_filter best -short -quiet ";
                    CommonUtil.ProcessCommand(Path.Combine("Tools", "cwebp.exe"),
                        defaultPrams + tempPath + " -o \"" + outputPath + "\"");

                    EditorUtility.DisplayProgressBar("Encoding...", "Cleaning up.", 0.9f);
                    File.Delete(tempPath);
                    AssetDatabase.Refresh();
                    EditorUtility.ClearProgressBar();
                } else {
                    Debug.LogError("Create temp png failed!");
                    Debug.LogWarning("Encoding suspended!");
                    EditorUtility.ClearProgressBar();
                }
            } else {
                Debug.LogError("Texture2D is null!");
                Debug.LogWarning("Encoding suspended!");
                EditorUtility.ClearProgressBar();
            }
        }
    }
}