using UnityEditor;
using UnityEditor.UI;

namespace UI
{
    [CustomEditor(typeof(AugmentIcon))]
    public class AugmentIconEditor : ButtonEditor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("Augment Icon Image Properties", EditorStyles.boldLabel);

            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("augmentIconImage"));

            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("UI.Button Base Properties", EditorStyles.boldLabel);

            base.OnInspectorGUI();
        }
    }
}