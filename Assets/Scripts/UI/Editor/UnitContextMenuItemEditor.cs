using UnityEditor;
using UnityEditor.UI;

namespace UI
{
    [CustomEditor(typeof(UnitContextMenuItem))]
    public class UnitContextMenuItemEditor : ButtonEditor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("Unit Context Menu Item Properties", EditorStyles.boldLabel);
            
            serializedObject.Update();
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty("text"));
            
            serializedObject.ApplyModifiedProperties();
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("UI.Button Base Properties", EditorStyles.boldLabel);
            
            base.OnInspectorGUI();
        }
    }
}