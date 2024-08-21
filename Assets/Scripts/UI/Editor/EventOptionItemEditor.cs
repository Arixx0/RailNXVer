using UnityEditor;
using UnityEditor.UI;

namespace UI
{
    [CustomEditor(typeof(EventOptionItem))]
    public class EventOptionItemEditor : ButtonEditor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("Event Option Item Properties", EditorStyles.boldLabel);

            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("optionText"));

            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("UI.Button Base Properties", EditorStyles.boldLabel);

            base.OnInspectorGUI();
        }
    }
}