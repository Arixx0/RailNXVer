using UnityEditor;
using UnityEditor.UI;

namespace UI
{
    [CustomEditor(typeof(TexturedToggle))]
    public class TexturedToggleEditor : ToggleEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            serializedObject.Update();
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty("stateImage"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("onStateSprite"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("offStateSprite"));

            serializedObject.ApplyModifiedProperties();
        }
    }
}