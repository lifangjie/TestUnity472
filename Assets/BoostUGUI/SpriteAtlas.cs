using System;
using System.Collections.Generic;
using UnityEngine;
using _Scripts.Util;

namespace BoostUGUI {
    [Serializable]
    public class SpriteAtlas : ScriptableObject, ISerializationCallbackReceiver {
        public TextAsset TextureBytes;
        public SpriteData[] SpriteDatas;

        [NonSerialized] private Texture2D _texture2D;
        [NonSerialized] private Dictionary<string, Sprite> _sprites;

        public void OnBeforeSerialize() {
        }
        public void OnAfterDeserialize() {
            _texture2D = WebPDecoder.DecodeFromBytes(TextureBytes.bytes);
            _sprites = new Dictionary<string, Sprite>();

            for (int i = 0; i < SpriteDatas.Length; i++) {
                Sprite sprite = Sprite.Create(_texture2D, SpriteDatas[i].Rect, SpriteDatas[i].Pivot);
                sprite.name = SpriteDatas[i].Name;
                _sprites.Add(sprite.name, sprite);
            }

            Debug.Log("instance" + GetInstanceID());
        }

        public Sprite GetSprite(string spriteName) {
            return _sprites[spriteName];
        }
    }

    [Serializable]
    public struct SpriteData {
        public string Name;
        public Rect Rect;
        public Vector2 Pivot;
    }
}