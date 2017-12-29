using UnityEditor;

namespace BoostUGUI.Editor {
    [CustomEditor(typeof(SpriteAtlas))]
    public class SpriteAtlasEditor : UnityEditor.Editor {
        private SerializedProperty _textureBytes;
        private SerializedProperty _spriteDatas;

        private void OnEnable() {
            _textureBytes = serializedObject.FindProperty("TextureBytes");
            _spriteDatas = serializedObject.FindProperty("SpriteDatas");
        }

        public override void OnInspectorGUI() {
            EditorGUILayout.PropertyField(_textureBytes);
            EditorGUILayout.PropertyField(_spriteDatas, true);

            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);
        }
    }
}