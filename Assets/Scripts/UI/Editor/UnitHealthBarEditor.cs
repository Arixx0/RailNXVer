using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

namespace UI
{
    [CustomEditor(typeof(UnitHealthBar))]
    public class UnitHealthBarEditor : SliderEditor
    {
        private SerializedProperty m_CanvasGroup;
        private SerializedProperty m_HealthColorGradient;
        private SerializedProperty m_MinWidth;
        private SerializedProperty m_MaxWidth;

        protected override void OnEnable()
        {
            base.OnEnable();

            m_CanvasGroup = serializedObject.FindProperty("canvasGroup");
            m_HealthColorGradient = serializedObject.FindProperty("healthColorGradient");
            m_MinWidth = serializedObject.FindProperty("minWidth");
            m_MaxWidth = serializedObject.FindProperty("maxWidth");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            serializedObject.Update();

            EditorGUILayout.PropertyField(m_CanvasGroup, new GUIContent("Canvas Group"));
            EditorGUILayout.PropertyField(m_HealthColorGradient, new GUIContent("Health Bar Color by Delta"));

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Layout Width", GUILayout.Width(100));
            EditorGUILayout.LabelField("min:", GUILayout.Width(30));
            EditorGUILayout.PropertyField(m_MinWidth, GUIContent.none);
            EditorGUILayout.LabelField("max:", GUILayout.Width(30));
            EditorGUILayout.PropertyField(m_MaxWidth, GUIContent.none);
            EditorGUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }
    }
}