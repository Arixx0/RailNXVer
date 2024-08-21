using UnityEditor;
using UnityEngine;

namespace Attributes.Editor
{
    [CustomPropertyDrawer(typeof(DisabledAttribute))]
    public class DisabledAttributePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
        
            var previousState = GUI.enabled;
            GUI.enabled = false;
        
            EditorGUI.PropertyField(position, property, label);
        
            GUI.enabled = previousState;
        
            EditorGUI.EndProperty();
        }
    }
}