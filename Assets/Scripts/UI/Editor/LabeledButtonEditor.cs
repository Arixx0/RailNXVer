using UnityEditor;
using UnityEditor.UI;

namespace UI
{
    [CustomEditor(typeof(LabeledButton))]
    public class LabeledButtonEditor : ButtonEditor
    {
        private SerializedProperty m_LabelComponent;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            
            m_LabelComponent = serializedObject.FindProperty("labelComponent");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            serializedObject.Update();
            
            EditorGUILayout.LabelField("Labeled Button Options", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(m_LabelComponent);
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}