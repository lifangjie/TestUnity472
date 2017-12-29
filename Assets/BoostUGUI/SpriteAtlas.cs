using System;
using UnityEngine;
using _Scripts.Util;

namespace BoostUGUI {
    [Serializable]
    public class SpriteAtlas : ScriptableObject{
        public TextAsset TextureBytes;
        public SpriteData[] SpriteDatas;

        private void OnEnable() {
            Debug.Log("on enable");
            Texture2D texture2D = WebPDecoder.DecodeFromBytes(TextureBytes.bytes);
            texture2D.hideFlags = HideFlags.HideAndDontSave;
            SpriteManager.AddTexture(texture2D);

            for (int i = 0; i < SpriteDatas.Length; i++) {
                Sprite sprite = Sprite.Create(texture2D, SpriteDatas[i].Rect, SpriteDatas[i].Pivot);
                sprite.hideFlags = HideFlags.HideAndDontSave;
                sprite.name = SpriteDatas[i].Name;
                SpriteManager.AddSprite(sprite);
            }
        }
    }

    [Serializable]
    public struct SpriteData {
        public string Name;
        public Rect Rect;
        public Vector2 Pivot;
    }
}