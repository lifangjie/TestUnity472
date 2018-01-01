/**
 * AtlasEncoder.cs
 * Created by: lfj20
 * Created on: 2017/12/21
 */

using UnityEditor;
using UnityEngine;

namespace _Scripts.Editor.Util {
    public static class AtlasEncoder {
        [MenuItem("Tools/Encode Selection PNG Into WebP")]
        static void Encode() {
            CommonUtil.ClearConsole();
            EditorUtility.DisplayProgressBar("Encoding...", "Loading original atlas", 0f);
            Texture2D texture2D = Selection.activeObject as Texture2D;
            if (texture2D != null) {
                WebPEncoder.Encode(texture2D, AssetDatabase.GetAssetPath(texture2D).Split('.')[0] + ".bytes");
                TextureImporter textureImporter =
                    AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(texture2D)) as TextureImporter;
                if (textureImporter != null) {
                    var textureSettings = new TextureImporterSettings {
                        maxTextureSize = 32,
                        mipmapEnabled = false,
                        textureFormat = TextureImporterFormat.AutomaticCompressed,
                        readable = false
                    };
                    textureImporter.SetTextureSettings(textureSettings);
                    AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(texture2D), ImportAssetOptions.ForceUpdate);
                } else {
                    Debug.LogWarning("Selection is not a texture!");
                    Debug.LogWarning("Encoding suspended!");
                    EditorUtility.ClearProgressBar();
                }
            }
        }
    }
}