/**
 * SpriteAtlasMaker.cs
 * Created by: lfj20
 * Created on: 2017/12/27
 */

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace BoostUGUI.Editor {
    public class SpriteAtlasMaker : EditorWindow {
        [MenuItem("Assets/UGUI/Sprite Atlas Maker", false, 0)]
        public static void OpenAtlasMaker() {
            GetWindow<SpriteAtlasMaker>(false, "Atlas Maker", true).Show();
        }

        private static SpriteAtlas _targetAtlas;
        private static string _selectSpriteName;
        private readonly List<string> _deleteList = new List<string>();

        private void OnGUI() {
            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Load All", GUILayout.MaxWidth(100))) {
                    Search();
                }

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

            List<Texture2D> textures = GetSelectedTextures();


            Dictionary<string, int> spriteList = GetSpriteList(textures);

            if (spriteList.Count > 0) {
                GUILayout.BeginHorizontal();
                GUILayout.Space(3f);
                GUILayout.BeginVertical();

                GUILayout.BeginScrollView(Vector2.zero);

                bool delete = false;
                bool update = false;
                int index = 0;
                if (_targetAtlas != null && spriteList.Count > _targetAtlas.SpriteDatas.Count) {
                    update = GUILayout.Button("Repack Textures");
                }

                foreach (KeyValuePair<string, int> iter in spriteList) {
                    ++index;

                    GUILayout.Space(-1f);
                    bool highlight = _selectSpriteName == iter.Key;
                    GUI.backgroundColor = highlight ? Color.white : new Color(0.8f, 0.8f, 0.8f);
                    GUILayout.BeginHorizontal("AS TextArea", GUILayout.MinHeight(20f));
                    GUI.backgroundColor = Color.white;
                    GUILayout.Label(index.ToString(), GUILayout.Width(24f));

                    if (GUILayout.Button(iter.Key, "OL TextField", GUILayout.Height(20f)))
                        _selectSpriteName = iter.Key;

                    if (iter.Value == 2) {
                        GUI.color = Color.green;
                        GUILayout.Label("Add", GUILayout.Width(27f));
                        GUI.color = Color.white;
                    } else if (iter.Value == 1) {
                        GUI.color = Color.cyan;
                        GUILayout.Label("Update", GUILayout.Width(45f));
                        GUI.color = Color.white;
                    } else {
                        if (_deleteList.Contains(iter.Key)) {
                            GUI.backgroundColor = Color.red;

                            if (GUILayout.Button("Delete", GUILayout.Width(60f))) {
                                delete = true;
                            }

                            GUI.backgroundColor = Color.green;
                            if (GUILayout.Button("X", GUILayout.Width(22f))) {
                                _deleteList.Remove(iter.Key);
                                delete = false;
                            }

                            GUI.backgroundColor = Color.white;
                        } else {
                            // If we have not yet selected a sprite for deletion, show a small "X" button
                            if (GUILayout.Button("X", GUILayout.Width(22f))) _deleteList.Add(iter.Key);
                        }
                    }

                    GUILayout.EndHorizontal();
                }

                GUILayout.EndScrollView();
                GUILayout.EndVertical();
                GUILayout.Space(3f);
                GUILayout.EndHorizontal();

                if (_targetAtlas != null) {
                    if (!string.IsNullOrEmpty(_selectSpriteName)) {
                        //Selection.activeObject = _targetAtlas;
                    }

                    if (delete) {
                        // just update atlas info, no need to repack
                        foreach (var spriteName in _deleteList) {
                            spriteList.Remove(spriteName);
                        }

                        _deleteList.Clear();
                        _targetAtlas.SpriteDatas.Clear();
                        foreach (var spriteData in _targetAtlas.SpriteDatas) {
                            if (spriteList.ContainsKey(spriteData.Name)) {
                                _targetAtlas.SpriteDatas.Add(spriteData);
                            }
                        }

                        EditorUtility.SetDirty(_targetAtlas);
                        Repaint();
                        SpriteManager.Reload();
                        foreach (var image in FindObjectsOfType<TestImage>()) {
                            image.OnValidate();
                        }
                    } else if (update) {
                        List<SpriteData> newSpriteDatas = new List<SpriteData>(_targetAtlas.SpriteDatas);
                        List<Texture2D> newTexture2Ds = new List<Texture2D>();
                        foreach (var spriteData in _targetAtlas.SpriteDatas) {
                            Texture2D texture2D =
                                AssetDatabase.LoadAssetAtPath(spriteData.Path, typeof(Texture2D)) as Texture2D;
                            newTexture2Ds.Add(texture2D);
                        }

                        foreach (var texture in textures) {
                            var path = AssetDatabase.GetAssetPath(texture);
                            var pathSplited = path.Split('.', '/');
                            SpriteData spriteData = new SpriteData() {
                                Name = pathSplited[pathSplited.Length - 2],
                                Path = path
                            };
                            newSpriteDatas.Add(spriteData);
                            newTexture2Ds.Add(texture);
                        }

                        Texture2D temp = new Texture2D(32, 32, TextureFormat.RGBA32, false);
                        Rect[] rects = temp.PackTextures(newTexture2Ds.ToArray(), 8, 2048);
                        for (int i = 0; i < newSpriteDatas.Count; i++) {
                            Rect rect = new Rect(rects[i].xMin * temp.width, rects[i].yMin * temp.height,
                                rects[i].width * temp.width, rects[i].height * temp.height);
                            newSpriteDatas[i].Rect = rect;
                        }

                        _targetAtlas.SpriteDatas = newSpriteDatas;
                        var outputPath = AssetDatabase.GetAssetPath(_targetAtlas).Split('.')[0] + ".bytes";
                        WebPEncoder.Encode(temp, outputPath);
                        _targetAtlas.TextureBytes =
                            AssetDatabase.LoadAssetAtPath(outputPath, typeof(TextAsset)) as TextAsset;
                        AssetDatabase.SaveAssets();
                    }
                }
            }
        }

        Dictionary<string, int> GetSpriteList(List<Texture2D> textures) {
            Dictionary<string, int> spriteList = new Dictionary<string, int>();

            // If we have textures to work with, include them as well
            if (textures.Count > 0) {
                List<string> texNames = new List<string>();
                foreach (Texture2D tex in textures) texNames.Add(tex.name);
                texNames.Sort();
                foreach (string tex in texNames) spriteList.Add(tex, 2);
            }

            if (_targetAtlas != null) {
                foreach (SpriteData spriteData in _targetAtlas.SpriteDatas) {
                    if (spriteList.ContainsKey(spriteData.Name)) spriteList[spriteData.Name] = 1;
                    else spriteList.Add(spriteData.Name, 0);
                }
            }

            return spriteList;
        }

        private void OnSelectionChange() {
            Repaint();
        }

        private void Search() {
            string[] path = AssetDatabase.GetAllAssetPaths();
            for (int i = 0; i < path.Length; i++) {
                if (!path[i].EndsWith(".asset", StringComparison.Ordinal)) {
                    continue;
                }

                AssetDatabase.LoadAssetAtPath(path[i], typeof(SpriteAtlas));
            }
        }

        List<Texture2D> GetSelectedTextures() {
            List<Texture2D> textures = new List<Texture2D>();

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