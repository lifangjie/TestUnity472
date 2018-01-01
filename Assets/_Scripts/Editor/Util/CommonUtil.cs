/**
 * CommonUtil.cs
 * Created by: lfj20
 * Created on: 2017/12/21
 */

using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace _Scripts.Editor.Util {
    public static class CommonUtil {
        [MenuItem("Tools/Bake Lightmap Size 2048")]
        public static void ChangeLightmapSize() {
            LightmapEditorSettings.maxAtlasWidth = 2048;
            LightmapEditorSettings.maxAtlasHeight = 2048;
            Lightmapping.Clear();
            Lightmapping.Bake();
        }

        public static void ClearConsole() {
            var assembly = Assembly.GetAssembly(typeof(ActiveEditorTracker));
            var type = assembly.GetType("UnityEditorInternal.LogEntries");
            var method = type.GetMethod("Clear");
            if (method != null) method.Invoke(new object(), null);
        }

        public static void ProcessCommand(string command, string argument) {
            System.Diagnostics.ProcessStartInfo info =
                new System.Diagnostics.ProcessStartInfo(command, argument) {
                    CreateNoWindow = true,
                    ErrorDialog = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    StandardOutputEncoding = System.Text.Encoding.UTF8,
                    StandardErrorEncoding = System.Text.Encoding.UTF8
                };

            System.Diagnostics.Process process = System.Diagnostics.Process.Start(info);

            if (process != null) {
                while (!process.StandardOutput.EndOfStream) {
                    Debug.Log(process.StandardOutput.ReadLine());
                }

                while (!process.StandardError.EndOfStream) {
                    Debug.Log(process.StandardError.ReadLine());
                }

                process.WaitForExit();
                process.Close();
            }
        }
    }
}