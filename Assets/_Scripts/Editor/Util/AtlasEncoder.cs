/**
 * AtlasEncoder.cs
 * Created by: lfj20
 * Created on: 2017/12/21
 */

using System.IO;
using UnityEditor;
using UnityEngine;

namespace _Scripts.Editor.Util {
    public static class AtlasEncoder {
        [MenuItem("Tools/Encode Selection PNG Into WebP")]
        static void Encode() {
            CommonUtil.ClearConsole();
            EditorUtility.DisplayProgressBar("Encoding...", "Loading original atlas", 0f);
            if (Selection.activeObject is Texture2D) {
                string path = AssetDatabase.GetAssetPath(Selection.activeObject);
                TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
                TextureImporterSettings textureSettings;
                if (textureImporter != null) {
                    textureSettings = new TextureImporterSettings {
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

                Texture2D texture2D = AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)) as Texture2D;

                EditorUtility.DisplayProgressBar("Encoding...", "Creating temp atlas file.", 0.1f);
                if (texture2D != null) {
                    Texture2D newTexture2D = new Texture2D(texture2D.width, texture2D.height);
                    for (int i = 0; i < texture2D.width; i++) {
                        for (int j = 0; j < texture2D.height; j++) {
                            newTexture2D.SetPixel(i, j, texture2D.GetPixel(i, texture2D.height - j - 1));
                        }
                    }

                    newTexture2D.Apply();
                    string tempPath = path.Split('.')[0] + "_temp.png";
                    FileStream fileStream = File.Open(tempPath, FileMode.Create);
                    byte[] bytes = newTexture2D.EncodeToPNG();
                    fileStream.Write(bytes, 0, bytes.Length);
                    fileStream.Flush();
                    fileStream.Close();

                    EditorUtility.DisplayProgressBar("Encoding...", "Encoding into webp.", 0.2f);
                    if (File.Exists(tempPath)) {
                        string defaultPrams = "-q 80 -mt -m 6 -alpha_q 80 -af -alpha_filter best -short -quiet ";
                        CommonUtil.ProcessCommand(Path.Combine("Tools", "cwebp.exe"),
                            defaultPrams + tempPath + " -o " + path.Split('.')[0] + ".bytes");

                        EditorUtility.DisplayProgressBar("Encoding...", "Cleaning up.", 0.9f);
                        File.Delete(tempPath);
                        textureSettings.maxTextureSize = 32;
                        textureSettings.mipmapEnabled = false;
                        textureSettings.textureFormat = TextureImporterFormat.AutomaticCompressed;
                        textureSettings.readable = false;
                        textureImporter.SetTextureSettings(textureSettings);
                        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
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
            } else {
                Debug.LogWarning("Selection is not a texture!");
                Debug.LogWarning("Encoding suspended!");
                EditorUtility.ClearProgressBar();
            }
        }
    }
}