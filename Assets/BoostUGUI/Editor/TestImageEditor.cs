using UnityEditor;
using UnityEngine;

namespace BoostUGUI.Editor {
    [CustomEditor(typeof(TestImage))]
    public class TestImageEditor : UnityEditor.Editor {
        private SerializedProperty _spriteName;

        private void OnEnable() {
            _spriteName = serializedObject.FindProperty("SpriteName");
        }

        public override void OnInspectorGUI() {
            EditorGUILayout.PropertyField(_spriteName);
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}