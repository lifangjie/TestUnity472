using UnityEngine.UI;

namespace BoostUGUI {
    public class TestImage : Image {
        public SpriteAtlas SpriteAtlas;
        public string SpriteName;

        protected override void Awake() {
            sprite = SpriteAtlas.GetSprite(SpriteName);
        }
    }
}