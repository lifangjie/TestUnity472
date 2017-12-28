using UnityEditor;

namespace BoostUGUI.Editor {
    [CustomEditor(typeof(TestImage))]
    public class TestImageEditor : UnityEditor.Editor {
        private SerializedProperty _spriteAtlas;
        private SerializedProperty _spriteName;

        private void OnEnable() {
            _spriteAtlas = serializedObject.FindProperty("SpriteAtlas");
            _spriteName = serializedObject.FindProperty("SpriteName");
        }

        public override void OnInspectorGUI() {
            EditorGUILayout.PropertyField(_spriteAtlas);
            EditorGUILayout.PropertyField(_spriteName);
        }
    }
}