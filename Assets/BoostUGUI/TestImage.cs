using UnityEngine;
using UnityEngine.UI;

namespace BoostUGUI {
    [ExecuteInEditMode]
    public class TestImage : Image {
        public string SpriteName;

        protected override void Awake() {
            sprite = SpriteManager.GetSprite(SpriteName);
        }

//        protected override void OnValidate() {
//            sprite = SpriteManager.GetSprite(SpriteName);
//            base.OnValidate();
//        }

        private void Update() {
            if (Time.frameCount > 500) {
            }
        }
    }
}