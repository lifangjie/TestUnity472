using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEngine;
using _Scripts.Editor.Util;

namespace _Scripts.Editor {
    public static class CreateAssetBundles {
        [MenuItem("Build/Build Scene AssetBundles")]
        static void BuildScene() {
            try {
                const string path = "Assets/AssetBundles";
                if (!Directory.Exists(path)) {
                    Directory.CreateDirectory(path);
                }

                CommonUtil.ClearConsole();
                var objects = Selection.objects;
                var length = Selection.objects.Length;
                Array.Sort(objects, (o, o1) => string.Compare(o.name, o1.name, StringComparison.OrdinalIgnoreCase));
                for (int i = 0; i < length; i++) {
                    var temp = new[] {AssetDatabase.GetAssetOrScenePath(objects[i])};
                    var bundlePath = path + "/" + objects[i].name;
                    Debug.Log("Building bundle: " + bundlePath);
                    BuildPipeline.BuildStreamedSceneAssetBundle(temp, bundlePath, BuildTarget.Android);
                }

                FileStream hashInfoFileStream = File.Open(path + "/HashInfo.txt", FileMode.Create);
                StreamWriter streamWriter = new StreamWriter(hashInfoFileStream);
                streamWriter.WriteLine("Build Time: " + DateTime.Now);
                MD5 md5 = MD5.Create();
                for (int i = 0; i < length; i++) {
                    var bundlePath = path + "/" + objects[i].name;
                    Debug.Log("Calculating MD5......");
                    FileStream fileStream = File.Open(bundlePath, FileMode.Open);
                    byte[] data = md5.ComputeHash(fileStream);
                    StringBuilder stringBuilder = new StringBuilder();
                    for (int j = 0; j < data.Length; j++) {
                        stringBuilder.Append(data[j].ToString("x2"));
                    }

                    streamWriter.WriteLine(objects[i].name + "@" + stringBuilder);
                }

                streamWriter.Flush();
                streamWriter.Close();
                Debug.Log("Build Succeeded.");
            }
            catch (Exception e) {
                Debug.Log("Build Failed.");
                Debug.LogError(e);
            }
        }
    }
}