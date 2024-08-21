using UnityEditor;
using UnityEditor.UI;

namespace UI
{
    [CustomEditor(typeof(LabeledSlider))]
    public class LabeledSliderEditor : SliderEditor
    {
        private SerializedProperty m_ValueLabel;
        
        protected override void OnEnable()
        {
            base.OnEnable();

            m_ValueLabel = serializedObject.FindProperty("valueLabel");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            EditorGUILayout.PropertyField(m_ValueLabel);

            serializedObject.ApplyModifiedProperties();
        }
    }
}