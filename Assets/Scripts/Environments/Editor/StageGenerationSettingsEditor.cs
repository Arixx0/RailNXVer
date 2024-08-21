using UnityEditor;
using UnityEngine;

namespace Environments
{
    [CustomPropertyDrawer(typeof(FixedRoomEventArgs))]
    public class FixedRoomEventArgsPropertyDrawer : PropertyDrawer
    {
        private const float Spacing = 2f;
        
        private static readonly Color BackgroundColor = new(0.156f, 0.328f, 0.293f);
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            EditorGUI.DrawRect(position, BackgroundColor);

            var roomEventType = property.FindPropertyRelative("roomEventType");
            var isAbsolutePosition = property.FindPropertyRelative("isAbsolutePosition");
            var positionValue = property.FindPropertyRelative("position");

            var contentRect = new Rect(
                position.x + Spacing, position.y + Spacing, position.width - Spacing * 2, position.height - Spacing * 2);

            var roomEventTypeRect = new Rect(
                contentRect.x, contentRect.y, contentRect.width, EditorGUIUtility.singleLineHeight);
            var isAbsolutePositionRect = new Rect(
                contentRect.x, roomEventTypeRect.yMax + Spacing, contentRect.width, EditorGUIUtility.singleLineHeight);
            var positionRect = new Rect(
                contentRect.x, isAbsolutePositionRect.yMax + Spacing, contentRect.width, EditorGUIUtility.singleLineHeight);

            EditorGUI.PropertyField(roomEventTypeRect, roomEventType);
            EditorGUI.PropertyField(isAbsolutePositionRect, isAbsolutePosition);
            EditorGUI.PropertyField(positionRect, positionValue);

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            const int elementCount = 3;
            return EditorGUIUtility.singleLineHeight * elementCount + Spacing * (elementCount + 1);
        }
    }

    [CustomPropertyDrawer(typeof(DynamicRoomEventArgs))]
    public class DynamicRoomEventArgsPropertyDrawer : PropertyDrawer
    {
        private const float Spacing = 2f;
        
        private static readonly Color BackgroundColor = new(0.156f, 0.328f, 0.293f);
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            EditorGUI.DrawRect(position, BackgroundColor);

            var roomEventType = property.FindPropertyRelative("roomEventType");
            var possibility = property.FindPropertyRelative("possibility");
            var maxCount = property.FindPropertyRelative("maxCount");

            var contentRect = new Rect(
                position.x + Spacing, position.y + Spacing, position.width - Spacing * 2, position.height - Spacing * 2);

            var roomEventTypeRect = new Rect(
                contentRect.x, contentRect.y, contentRect.width, EditorGUIUtility.singleLineHeight);
            var possibilityRect = new Rect(
                contentRect.x, roomEventTypeRect.yMax + Spacing, contentRect.width, EditorGUIUtility.singleLineHeight);
            var maxCountRect = new Rect(
                contentRect.x, possibilityRect.yMax + Spacing, contentRect.width, EditorGUIUtility.singleLineHeight);

            EditorGUI.PropertyField(roomEventTypeRect, roomEventType);
            EditorGUI.PropertyField(possibilityRect, possibility);
            EditorGUI.PropertyField(maxCountRect, maxCount);

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            const int elementCount = 3;
            return EditorGUIUtility.singleLineHeight * elementCount + Spacing * (elementCount + 1);
        }
    }

    [CustomPropertyDrawer(typeof(DisplacementArgs))]
    public class DisplacementArgsPropertyDrawer : PropertyDrawer
    {
        private const float Spacing = 2f;
        
        private static readonly Color BackgroundColor = new(0.156f, 0.328f, 0.293f);
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            EditorGUI.DrawRect(position, BackgroundColor);

            var roomEventType = property.FindPropertyRelative("roomEventType");
            var displacement = property.FindPropertyRelative("displacement");

            var contentRect = new Rect(
                position.x + Spacing, position.y + Spacing, position.width - Spacing * 2, position.height - Spacing * 2);

            var roomEventTypeRect = new Rect(
                contentRect.x, contentRect.y, contentRect.width, EditorGUIUtility.singleLineHeight);
            var displacementRect = new Rect(
                contentRect.x, roomEventTypeRect.yMax + Spacing, contentRect.width, EditorGUIUtility.singleLineHeight);

            EditorGUI.PropertyField(roomEventTypeRect, roomEventType);
            EditorGUI.PropertyField(displacementRect, displacement);

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            const int elementCount = 2;
            return EditorGUIUtility.singleLineHeight * elementCount + Spacing * (elementCount + 1);
        }
    }
}