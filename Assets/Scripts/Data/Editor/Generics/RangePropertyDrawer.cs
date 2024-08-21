using UnityEditor;
using UnityEngine;

namespace Data
{
    [CustomPropertyDrawer(typeof(Range))]
    public class RangePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var variableRect = EditorGUI.PrefixLabel(position, label);
            var minRect = new Rect(variableRect.x, variableRect.y, variableRect.width * 0.45f, variableRect.height);
            var rangeRect = new Rect(minRect.xMax, variableRect.y, variableRect.width * 0.1f, variableRect.height);
            var maxRect = new Rect(rangeRect.xMax, variableRect.y, variableRect.width * 0.45f, variableRect.height);
            EditorGUI.LabelField(rangeRect, "~");
            EditorGUI.PropertyField(minRect, property.FindPropertyRelative("min"), GUIContent.none);
            EditorGUI.PropertyField(maxRect, property.FindPropertyRelative("max"), GUIContent.none);
            
            EditorGUI.EndProperty();
        }
    }
}