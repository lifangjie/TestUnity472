using UnityEngine;

namespace _Scripts.UI {
    public class SpriteAtlas : ScriptableObject {
        private TextAsset TextureBytes;
        private SpriteData[] SpriteDatas;

    }

    struct SpriteData {
        private Rect Rect;
        private Vector2 Pivot;

    }
}