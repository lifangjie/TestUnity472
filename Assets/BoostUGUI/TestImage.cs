using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace BoostUGUI {
    public class TestImage : Image {
        public SpriteAtlas SpriteAtlas;
        public string SpriteName;

        public override void OnAfterDeserialize() {
            var field = GetType().GetField("m_Sprite", BindingFlags.NonPublic | BindingFlags.Instance);
            Sprite temp = SpriteAtlas.GetSprite(SpriteName);
            field.SetValue(this, temp);
        }
    }
}