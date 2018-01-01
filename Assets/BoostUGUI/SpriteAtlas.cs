using System;
using System.Collections.Generic;
using UnityEngine;
using _Scripts.Util;

namespace BoostUGUI {
    [Serializable]
    public class SpriteAtlas : ScriptableObject {
        public TextAsset TextureBytes;
        public List<SpriteData> SpriteDatas = new List<SpriteData>();

        internal void OnEnable() {
            if (TextureBytes == null) {
                SpriteManager.AddAtlas(this, null);
                return;
            }
            Texture2D texture2D = WebPDecoder.DecodeFromBytes(TextureBytes.bytes);
            texture2D.hideFlags = HideFlags.HideAndDontSave;
            if (SpriteManager.AddAtlas(this, texture2D)) {
                for (int i = 0; i < SpriteDatas.Count; i++) {
                    Sprite sprite = Sprite.Create(texture2D, SpriteDatas[i].Rect, SpriteDatas[i].Pivot);
                    sprite.hideFlags = HideFlags.HideAndDontSave;
                    sprite.name = SpriteDatas[i].Name;
                    SpriteManager.AddSprite(sprite);
                }
            }
        }
    }

    [Serializable]
    public class SpriteData {
        public string Name;
        public string Path;
        public Rect Rect;
        public Vector2 Pivot;
    }
}