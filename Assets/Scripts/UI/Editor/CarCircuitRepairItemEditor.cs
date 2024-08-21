using UnityEditor;
using UnityEditor.UI;

namespace UI
{
    [CustomEditor(typeof(CarCircuitRepairItem))]
    public class CarCircuitRepairItemEditor : ButtonEditor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("Circuit Repair Item Properties", EditorStyles.boldLabel);

            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("repairableAmountColor"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("nonRepairableAmountColor"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("repairAmount"));


            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("UI.Button Base Properties", EditorStyles.boldLabel);

            base.OnInspectorGUI();
        }
    }
}

