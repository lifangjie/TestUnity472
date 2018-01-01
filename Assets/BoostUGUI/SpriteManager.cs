using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BoostUGUI {
    public class SpriteManager {
        private readonly HashSet<SpriteAtlas> _registedAtlases = new HashSet<SpriteAtlas>();
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

        public static void Reload() {
            foreach (var sprite in Instance._spriteSet.Values) {
                Object.DestroyImmediate(sprite);
            }

            Instance._spriteSet.Clear();

            foreach (var texture in Instance._textureSet) {
                Object.DestroyImmediate(texture);
            }

            Instance._textureSet.Clear();

            foreach (var spriteAtlas in Instance._registedAtlases) {
                spriteAtlas.OnEnable();
            }
        }

        public static bool AddAtlas(SpriteAtlas spriteAtlas, Texture2D texture2D) {
            Instance._registedAtlases.Add(spriteAtlas);
            if (Instance._textureSet.Add(texture2D)) {
                return true;
            }
            Object.DestroyImmediate(texture2D);
            return false;
        }

        public static void AddSprite(Sprite sprite) {
            Instance._spriteSet[sprite.name] = sprite;
        }
    }
}