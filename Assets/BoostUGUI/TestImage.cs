using UnityEngine;
using UnityEngine.UI;

namespace BoostUGUI {
    [ExecuteInEditMode]
    public class TestImage : Image {
        public SpriteAtlas SpriteAtlas;
        public string SpriteName;

        protected override void Awake() {
            sprite = SpriteManager.GetSprite(SpriteName);
        }

        public void OnValidate() {
            sprite = SpriteManager.GetSprite(SpriteName);
        }

        public void SetSprite(string spriteName) {
            SpriteName = spriteName;
            sprite = SpriteManager.GetSprite(spriteName);
        }
        

        private void Update() {
            if (Time.frameCount > 500) {
            }
        }
    }
}