using System;
using UnityEngine;

namespace _Scripts.UI {
    [Serializable]
    public class SpriteAtlas : ScriptableObject {
        public TextAsset TextureBytes;
        public SpriteData[] SpriteDatas;
    }

    [Serializable]
    public struct SpriteData {
        public Rect Rect;
        public Vector2 Pivot;

    }
}