using TMPro;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

namespace UI
{
    [CustomEditor(typeof(UnitUpgradeOptionCard))]
    public class UnitUpgradeOptionCardEditor : ButtonEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            serializedObject.Update();
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(new GUIContent("Unit Upgrade Option Card"), EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("upgradeName"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("upgradeDescription"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("resourcePanel"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("resourceItemPrefab"));

            serializedObject.ApplyModifiedProperties();
        }
    }
}