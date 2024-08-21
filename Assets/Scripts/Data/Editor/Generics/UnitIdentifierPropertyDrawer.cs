using UnityEditor;
using UnityEngine;

namespace Data
{
    [CustomPropertyDrawer(typeof(UnitIdentifier))]
    public class UnitIdentifierPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            var category = property.FindPropertyRelative("category");
            var objectType = property.FindPropertyRelative("objectType");
            var subType = property.FindPropertyRelative("subType");
            
            var identifier = $"{category.stringValue}_{objectType.stringValue}_{subType.stringValue}";
            var leveledIdentifier = $"{category.stringValue}_{objectType.stringValue}_{{Level}}_{subType.stringValue}";

            var rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

            EditorGUI.LabelField(rect, label);
            rect.y += EditorGUIUtility.singleLineHeight + 2;

            EditorGUI.indentLevel += 1;
            
            EditorGUI.PropertyField(rect, category);
            rect.y += EditorGUIUtility.singleLineHeight + 2;
            EditorGUI.PropertyField(rect, objectType);
            rect.y += EditorGUIUtility.singleLineHeight + 2;
            EditorGUI.PropertyField(rect, subType);
            rect.y += EditorGUIUtility.singleLineHeight + 2;
            EditorGUI.LabelField(rect, "Identifier", identifier);
            rect.y += EditorGUIUtility.singleLineHeight + 2;
            EditorGUI.LabelField(rect, "Leveled Identifier", leveledIdentifier);

            EditorGUI.indentLevel -= 1;
            
            EditorGUI.EndProperty();
        }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 6 + 14;
        }
    }
}