using UnityEditor;
using UnityEngine;

namespace Units.Stats
{
    [CustomPropertyDrawer(typeof(StatVariable))]
    public class StatVariablePropertyDrawer : PropertyDrawer
    {
        private const float Spacing = 2f;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, GUIContent.none, property);
            
            var baseValueProperty = property.FindPropertyRelative("baseValue");
            var additiveValueProperty = property.FindPropertyRelative("additiveValue");
            var multiplicativeValueProperty = property.FindPropertyRelative("multiplicativeValue");
            var errorFactorProperty = property.FindPropertyRelative("errorFactor");
            var finalValueProperty = property.FindPropertyRelative("finalValue");
            var currentValueProperty = property.FindPropertyRelative("currentValue");
            var isRangeDeltaValueProperty = property.FindPropertyRelative("isRangeDeltaValue");
            
            EditorGUI.DrawRect(position, new Color(0.226f, 0.246f, 0.359f));

            var contentRect = new Rect(position.x + Spacing, position.y + Spacing, position.width - Spacing * 2f, position.height - Spacing * 2f);
            var height = EditorGUIUtility.singleLineHeight;

            var labelRect = new Rect(contentRect.x, contentRect.y, 180f, height);
            var isRangeDeltaValueRect = new Rect(labelRect.xMax + Spacing, labelRect.y, contentRect.width - 180f - Spacing, height);
            
            EditorGUI.LabelField(labelRect, label, EditorStyles.boldLabel);
            isRangeDeltaValueProperty.boolValue = EditorGUI.ToggleLeft(isRangeDeltaValueRect, "Is Range Delta Value", isRangeDeltaValueProperty.boolValue);

            var propertyWidth = (contentRect.width - Spacing * 7 - 160) * 0.25f;

            var baseValueLabelRect = new Rect(contentRect.x, labelRect.yMax + Spacing, 40f, height);
            var baseValueRect = new Rect(baseValueLabelRect.xMax + Spacing, baseValueLabelRect.y, propertyWidth, height);
            EditorGUI.LabelField(baseValueLabelRect, "Base");
            EditorGUI.PropertyField(baseValueRect, baseValueProperty, GUIContent.none);
            
            var additiveValueLabelRect = new Rect(baseValueRect.xMax + Spacing, baseValueLabelRect.y, 40f, height);
            var additiveValueRect = new Rect(additiveValueLabelRect.xMax + Spacing, baseValueLabelRect.y, propertyWidth, height);
            EditorGUI.LabelField(additiveValueLabelRect, "Add");
            GUI.enabled = false;
            EditorGUI.PropertyField(additiveValueRect, additiveValueProperty, GUIContent.none);
            GUI.enabled = true;
            
            var multiplicativeValueLabelRect = new Rect(additiveValueRect.xMax + Spacing, baseValueLabelRect.y, 40f, height);
            var multiplicativeValueRect = new Rect(multiplicativeValueLabelRect.xMax + Spacing, baseValueLabelRect.y, propertyWidth, height);
            EditorGUI.LabelField(multiplicativeValueLabelRect, "Mul");
            GUI.enabled = false;
            EditorGUI.PropertyField(multiplicativeValueRect, multiplicativeValueProperty, GUIContent.none);
            GUI.enabled = true;
            
            var errorFactorLabelRect = new Rect(multiplicativeValueRect.xMax + Spacing, baseValueLabelRect.y, 40f, height);
            var errorFactorRect = new Rect(errorFactorLabelRect.xMax + Spacing, baseValueLabelRect.y, propertyWidth, height);
            EditorGUI.LabelField(errorFactorLabelRect, "Err");
            GUI.enabled = false;
            EditorGUI.PropertyField(errorFactorRect, errorFactorProperty, GUIContent.none);
            GUI.enabled = true;

            propertyWidth = (contentRect.width - Spacing * 3 - 80f) * 0.5f;
            
            var finalValueLabelRect = new Rect(contentRect.x, baseValueLabelRect.yMax + Spacing, 40f, height);
            var finalValueRect = new Rect(finalValueLabelRect.xMax + Spacing, finalValueLabelRect.y, propertyWidth, height);
            EditorGUI.LabelField(finalValueLabelRect, "Final");
            GUI.enabled = false;
            EditorGUI.PropertyField(finalValueRect, finalValueProperty, GUIContent.none);
            GUI.enabled = true;

            if (isRangeDeltaValueProperty.boolValue)
            {
                var currentValueLabelRect = new Rect(finalValueRect.xMax + Spacing, finalValueRect.y, 40f, height);
                var currentValueRect = new Rect(currentValueLabelRect.xMax + Spacing, currentValueLabelRect.y, propertyWidth, height);
                EditorGUI.LabelField(currentValueLabelRect, "Cur");
                EditorGUI.PropertyField(currentValueRect, currentValueProperty, GUIContent.none);
            }

            GUI.enabled = true;
            
            EditorGUI.EndProperty();
        }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var rowCount = 4f;
            var spacing = 2f;
            
            return EditorGUIUtility.singleLineHeight * rowCount +
                   spacing * (rowCount + 1);
        }
    }

    [CustomPropertyDrawer(typeof(StatModifier))]
    public class StatModifierPropertyDrawer : PropertyDrawer
    {
        private const float Spacing = 4f;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, GUIContent.none, property);
            
            var guidProperty = property.FindPropertyRelative("guid");
            var modifierTypeProperty = property.FindPropertyRelative("modifierType");
            var valueProperty = property.FindPropertyRelative("value");

            var height = EditorGUIUtility.singleLineHeight;
            var labelWidth = 40f;
            var propertyWidth = position.width - Spacing * 2 - 170f;

            var labelRect = new Rect(position.x, position.y, 130f, height);
            var guidLabelRect = new Rect(labelRect.xMax + Spacing, position.y, labelWidth, height);
            var guidRect = new Rect(guidLabelRect.xMax + Spacing, guidLabelRect.y, propertyWidth, height);
            
            EditorGUI.LabelField(labelRect, label);
            EditorGUI.LabelField(guidLabelRect, "guid:");
            EditorGUI.PropertyField(guidRect, guidProperty, GUIContent.none);

            propertyWidth = (position.width - Spacing * 3 - 80f) / 2;

            var modifierTypeLabelRect = new Rect(position.x, labelRect.yMax + Spacing, labelWidth, height);
            var modifierTypeRect = new Rect(modifierTypeLabelRect.xMax + Spacing, modifierTypeLabelRect.y, propertyWidth, height);
            var valueLabelRect = new Rect(modifierTypeRect.xMax + Spacing, modifierTypeLabelRect.y, labelWidth, height);
            var valueRect = new Rect(valueLabelRect.xMax + Spacing, modifierTypeLabelRect.y, propertyWidth, height);
            
            EditorGUI.LabelField(modifierTypeLabelRect, "Mod");
            EditorGUI.PropertyField(modifierTypeRect, modifierTypeProperty, GUIContent.none);
            EditorGUI.LabelField(valueLabelRect, "Val");
            EditorGUI.PropertyField(valueRect, valueProperty, GUIContent.none);
        }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var rowCount = 2;
            return EditorGUIUtility.singleLineHeight * rowCount +
                   (rowCount + 1) * Spacing;
        }
    }
}