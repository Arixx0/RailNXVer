using System.Reflection;
using System.Text.RegularExpressions;
using UI;
using UnityEditor;
using UnityEngine;

namespace Data
{
    [CustomPropertyDrawer(typeof(ShopKeeperItemContext))]
    public class ShopKeeperItemContextPropertyDrawer : PropertyDrawer
    {
        private const float SPACING = 1f;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            var identifierProperty = property.FindPropertyRelative("identifier");
            var localProbabilityProperty = property.FindPropertyRelative("localProbability");

            var labelRect = new Rect(position.x, position.y + SPACING, position.width, EditorGUIUtility.singleLineHeight);
            var identifierRect = new Rect(position.x, labelRect.yMax + SPACING, position.width, EditorGUIUtility.singleLineHeight);
            var localProbabilityRect = new Rect(position.x, identifierRect.yMax + SPACING, position.width, EditorGUIUtility.singleLineHeight);

            EditorGUI.LabelField(labelRect, label);
            EditorGUI.PropertyField(identifierRect, identifierProperty);
            EditorGUI.PropertyField(localProbabilityRect, localProbabilityProperty);
            
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var elementCount = 3;
            return EditorGUIUtility.singleLineHeight * elementCount + SPACING * (elementCount + 1);
        }
    }

    [CustomPropertyDrawer(typeof(ShopKeeperSlotProperty))]
    public class ShopKeeperSlotPropertyDrawer : PropertyDrawer
    {
        private const float SPACING = 1f;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            label.text = $"{label.text} ({property.type})";
            
            var itemsContextProperty = property.FindPropertyRelative("itemsContext");
            var itemsContextRect = new Rect(position.x, position.y + SPACING, position.width, EditorGUIUtility.singleLineHeight);
            
            EditorGUI.PropertyField(itemsContextRect, itemsContextProperty, label, true);
            
            EditorGUI.EndProperty();
        }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return SPACING * 2 + EditorGUI.GetPropertyHeight(property.FindPropertyRelative("itemsContext"));
        }
    }
    
    [CustomEditor(typeof(ShopKeeperItemsDefinition))]
    public class ShopKeeperItemsDefinitionEditor : Editor
    {
        private ShopItemsListElement m_ShopItemsListElement;
        
        private ShopKeeperItemsDefinition Target { get; set; }

        private void OnEnable()
        {
            Target = (ShopKeeperItemsDefinition)target;
            
            DatabaseDefinitions.GetOrCreate().Load();
            m_ShopItemsListElement = new ShopItemsListElement();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            m_ShopItemsListElement.DoLayout();
        }
    }
}