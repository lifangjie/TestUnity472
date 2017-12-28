using System;
using System.Collections.Generic;
using UnityEngine;

namespace BoostUGUI {
    [Serializable]
    public class SpriteAtlas : ScriptableObject {
        public TextAsset TextureBytes;
        public SpriteData[] SpriteDatas;

        [NonSerialized] private Texture2D _texture2D;
        [NonSerialized] private Dictionary<string, Sprite> _sprites;
        private void OnEnable() {
            _sprites = new Dictionary<string, Sprite>();
            
            // load texture
            
            // init sprites
            
        }

        public Sprite GetSprite(string spriteName) {
            return _sprites[spriteName];
        }
    }

    [Serializable]
    public struct SpriteData {
        public Rect Rect;
        public Vector2 Pivot;

    }
}