using UnityEditor;
using UnityEngine;

namespace InputComponents
{
    [CustomPropertyDrawer(typeof(AxisInputIntervalModule))]
    public class AxisInputIntervalModuleDrawer : PropertyDrawer
    {
        private static readonly Color RectColor = new Color(53f / 255f, 89f / 255f, 59f / 255f);

        private static readonly float Spacing = 2f;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            EditorGUI.DrawRect(position, RectColor);

            var contentRect = new Rect(position.x + Spacing, position.y + Spacing,
                position.width - Spacing * 2, position.height - Spacing * 2);

            var rect = new Rect(contentRect.x, contentRect.y, contentRect.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(rect, label);

            EditorGUI.indentLevel += 1;
            
            rect.y = rect.yMax + Spacing;
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("moveRepeatDelay"));

            rect.y = rect.yMax + Spacing;
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("moveRepeatRate"));

            EditorGUI.indentLevel -= 1;
            
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var propertyCount = 3;
            return EditorGUIUtility.singleLineHeight * propertyCount + Spacing * (propertyCount + 1);
        }
    }
}