/**
 * SpriteAtlasMaker.cs
 * Created by: lfj20
 * Created on: 2017/12/27
 */

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BoostUGUI.Editor {
    public class SpriteAtlasMaker : EditorWindow {
        [MenuItem("Assets/UGUI/Sprite Atlas Maker", false, 0)]
        public static void OpenAtlasMaker() {
            GetWindow<SpriteAtlasMaker>(false, "Atlas Maker", true).Show();
        }

        private readonly HashSet<SpriteAtlas> _container = new HashSet<SpriteAtlas>();

        private bool _update = false;

        private static SpriteAtlas _targetAtlas;

        private void OnGUI() {
            EditorGUILayout.BeginHorizontal();
            {
                _targetAtlas =
                    EditorGUILayout.ObjectField(_targetAtlas, typeof(SpriteAtlas), false, GUILayout.ExpandWidth(true))
                        as SpriteAtlas;
                if (GUILayout.Button("New", GUILayout.MaxWidth(100))) {
                    _targetAtlas = CreateInstance<SpriteAtlas>();
                    string path = EditorUtility.SaveFilePanelInProject("Save As",
                        "New Atlas.asset", "asset", "Save atlas as...", Application.dataPath);

                    if (!string.IsNullOrEmpty(path)) {
                        AssetDatabase.CreateAsset(_targetAtlas, path);
                    }
                }
            }
            EditorGUILayout.EndHorizontal();


            // padding size, default 8

            HashSet<Texture2D> textures = GetSelectedTextures();
            if (GUILayout.Button("Pack") && _targetAtlas != null) {
                _targetAtlas.SpriteDatas = new SpriteData[textures.Count];
                int index = 0;
                foreach (var texture in textures) {
                    SpriteData spriteData = new SpriteData() {
                        Name = "test",
                        Rect = new Rect(0, 0, texture.width, texture.height),
                        Pivot = Vector2.zero
                    };
                    _targetAtlas.SpriteDatas[index++] = spriteData;
                    // update sprites data
                }

                // pack texture
                Texture2D texture2D = new Texture2D(32, 32);
                texture2D.Apply();
                var outputPath = AssetDatabase.GetAssetPath(_targetAtlas).Split('.')[0] + ".bytes";
                WebPEncoder.Encode(texture2D, outputPath);
                _targetAtlas.TextureBytes = AssetDatabase.LoadAssetAtPath(outputPath, typeof(TextAsset)) as TextAsset;
                AssetDatabase.SaveAssets();
            }

            if (textures.Count > 0) {
                _update = GUILayout.Button("Add/Update");
            } else if (GUILayout.Button("View Sprites")) {
                SpriteSelector.ShowSelected();
            }
        }

        private void OnSelectionChange() {
            //throw new System.NotImplementedException();
        }

        private void Search() {
            string[] path = AssetDatabase.GetAllAssetPaths();
            for (int i = 0; i < path.Length; i++) {
                if (!path[i].EndsWith(".asset", StringComparison.Ordinal)) {
                    continue;
                }

                SpriteAtlas spriteAtlas = AssetDatabase.LoadAssetAtPath(path[i], typeof(SpriteAtlas)) as SpriteAtlas;
                if (spriteAtlas == null || _container.Contains(spriteAtlas)) {
                    continue;
                }

                _container.Add(spriteAtlas);
            }
        }

        HashSet<Texture2D> GetSelectedTextures() {
            HashSet<Texture2D> textures = new HashSet<Texture2D>();

            if (Selection.objects != null && Selection.objects.Length > 0) {
                Object[] objects = EditorUtility.CollectDependencies(Selection.objects);

                foreach (Object o in objects) {
                    Texture2D tex = o as Texture2D;
                    if (tex != null) {
                        textures.Add(tex);
                    }

                    //if (tex == null || tex.name == "Font Texture") continue;
                    //if (NGUISettings.atlas == null || NGUISettings.atlas.texture != tex) textures.Add(tex);
                }
            }

            return textures;
        }
    }
}