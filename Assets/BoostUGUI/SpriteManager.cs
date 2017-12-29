using System.Collections.Generic;
using UnityEngine;

namespace BoostUGUI {
    public class SpriteManager {
        private readonly HashSet<Texture2D> _textureSet = new HashSet<Texture2D>();
        private readonly Dictionary<string, Sprite> _spriteSet = new Dictionary<string, Sprite>();
        private static SpriteManager _instance;

        private static SpriteManager Instance {
            get { return _instance ?? (_instance = new SpriteManager()); }
        }

        public static Sprite GetSprite(string spriteName) {
            try {
                return Instance._spriteSet[spriteName];
            }
            catch (KeyNotFoundException) {
                return null;
            }
        }

        public static void AddTexture(Texture2D texture2D) {
            if (!Instance._textureSet.Add(texture2D)) {
                Object.DestroyImmediate(texture2D);
            }
        }

        public static void AddSprite(Sprite sprite) {
            Instance._spriteSet[sprite.name] = sprite;
        }
    }
}