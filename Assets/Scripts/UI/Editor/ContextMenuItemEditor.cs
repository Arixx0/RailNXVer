using UnityEditor;
using UnityEditor.UI;

namespace UI
{
    [CustomEditor(typeof(ContextMenuItem))]
    public class ContextMenuItemEditor : ButtonEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();
            
            serializedObject.Update();
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty("itemImage"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("iconImage"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("itemName"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("outline"));
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}