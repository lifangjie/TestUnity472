/**
 * SpriteAtlasMaker.cs
 * Created by: lfj20
 * Created on: 2017/12/27
 */

using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using _Scripts.UI;
using Object = UnityEngine.Object;

namespace _Scripts.Editor.UI {
    public class SpriteAtlasMaker : EditorWindow {
        [MenuItem("Assets/UGUI/Sprite Atlas Maker", false, 0)]
        public static void OpenAtlasMaker() {
            GetWindow<SpriteAtlasMaker>(false, "Atlas Maker", true).Show();
        }
        
        private HashSet<SpriteAtlas> _container = new HashSet<SpriteAtlas>();

        private bool _update = false;

        private SpriteAtlas _spriteAtlas;

        private void OnGUI() {
            // choose or new a atlas
            ComponentSelector.Draw("Atlas", _spriteAtlas, delegate(Object o) {
                if (_spriteAtlas != o) {
                    _spriteAtlas = o as SpriteAtlas;
                }
            }, true, GUILayout.MinWidth(80f));
            
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.ObjectField(_spriteAtlas, typeof(SpriteAtlas), false, GUILayout.ExpandWidth(true));
                if (GUILayout.Button("New")) {
                    EditorGUILayout.ObjectField("test", _spriteAtlas, typeof(SpriteAtlas), true,
                        GUILayout.MinWidth(80));
                    SpriteAtlas spriteAtlas = CreateInstance<SpriteAtlas>();
                    string path = EditorUtility.SaveFilePanelInProject("Save As",
                        "New Atlas.asset", "asset", "Save atlas as...", Application.dataPath);

                    if (!string.IsNullOrEmpty(path)) {
                        AssetDatabase.CreateAsset(spriteAtlas, path);
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
            
            
            
            // padding size, default 8

            HashSet<Texture> textures = GetSelectedTextures();
            if (GUILayout.Button("Pack")) {
                for (int i = 0; i < textures.Count; i++) {
                    // update sprites data
                    // pack texture
                }
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

        HashSet<Texture> GetSelectedTextures() {
            HashSet<Texture> textures = new HashSet<Texture>();

            if (Selection.objects != null && Selection.objects.Length > 0) {
                Object[] objects = EditorUtility.CollectDependencies(Selection.objects);

                foreach (Object o in objects) {
                    Texture tex = o as Texture;
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