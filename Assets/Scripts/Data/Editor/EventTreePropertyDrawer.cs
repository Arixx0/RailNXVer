using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Data
{
    [CustomPropertyDrawer(typeof(EventTree.EventContext))]
    public class EventNodeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var nextEventNodeIdentifier = property.FindPropertyRelative("nextEventNodeIdentifier");

            var nextEventNodeIdentifierRect = new Rect(position.x, position.y, position.width,
                EditorGUI.GetPropertyHeight(nextEventNodeIdentifier));

            EditorGUI.BeginProperty(position, GUIContent.none, property);
            EditorGUI.PropertyField(nextEventNodeIdentifierRect, nextEventNodeIdentifier, true);
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var nextEventNodeIdentifier = property.FindPropertyRelative("nextEventNodeIdentifier");
            var height = EditorGUIUtility.singleLineHeight + 6f + EditorGUI.GetPropertyHeight(nextEventNodeIdentifier, true);

            return height;
        }
    }

    [CustomPropertyDrawer(typeof(EventTree.EventTreeLUT))]
    public class EventTreeLUTDrawer : PropertyDrawer
    {
        private UnityEngine.Object m_Target;
        private SerializedProperty m_Keys;
        private SerializedProperty m_Values;
        private ReorderableList m_LUTList;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (m_Target != property.serializedObject.targetObject)
            {
                m_Target = property.serializedObject.targetObject;
                m_Keys = property.FindPropertyRelative("keysSerializedCache");
                m_Values = property.FindPropertyRelative("valuesSerializedCache");

                m_LUTList = new ReorderableList(property.serializedObject, m_Keys,
                    false, true, true, true);

                m_LUTList.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Event Tree LUT");

                m_LUTList.drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    var key = m_Keys.GetArrayElementAtIndex(index);
                    var keyRect = new Rect(rect.x, rect.y + 2, rect.width, EditorGUIUtility.singleLineHeight);

                    EditorGUI.BeginChangeCheck();
                    var newKey = EditorGUI.DelayedTextField(keyRect, GUIContent.none, key.stringValue);
                    if (EditorGUI.EndChangeCheck())
                    {
                        if (IsKeyExistsFromLUT(m_Keys, newKey))
                        {
                            return;
                        }

                        key.stringValue = newKey;
                        EditorUtility.SetDirty(m_Target);
                    }

                    if (index != m_LUTList.index)
                    {
                        return;
                    }

                    var value = m_Values.GetArrayElementAtIndex(index);
                    var valueRect = new Rect(rect.x + 18, keyRect.yMax + 2, rect.width - 18, EditorGUI.GetPropertyHeight(value));

                    EditorGUI.BeginChangeCheck();
                    EditorGUI.PropertyField(valueRect, value, GUIContent.none);
                    if (EditorGUI.EndChangeCheck())
                    {
                        EditorUtility.SetDirty(m_Target);
                    }
                };

                m_LUTList.elementHeightCallback = index =>
                {
                    if (index == m_LUTList.index)
                    {
                        var value = m_Values.GetArrayElementAtIndex(index);
                        return EditorGUI.GetPropertyHeight(value) + EditorGUIUtility.singleLineHeight + 6;
                    }

                    return EditorGUIUtility.singleLineHeight + 4;
                };

                m_LUTList.onAddCallback = list =>
                {
                    var index = m_Keys.arraySize;
                    m_Keys.arraySize++;
                    m_Values.arraySize++;

                    var key = list.serializedProperty.GetArrayElementAtIndex(index);
                    key.stringValue = DateTime.Now.GetHashCode().ToString();

                    EditorUtility.SetDirty(m_Target);
                };

                EditorUtility.SetDirty(m_Target);
            }

            EditorGUI.BeginProperty(position, GUIContent.none, property);
            m_LUTList.DoList(position);
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (m_LUTList == null)
            {
                return base.GetPropertyHeight(property, label);
            }

            return m_LUTList.GetHeight();
        }

        private static bool IsKeyExistsFromLUT(SerializedProperty property, string key)
        {
            for (var i = 0; i < property.arraySize; ++i)
            {
                if (string.CompareOrdinal(property.GetArrayElementAtIndex(i).stringValue, key) != 0)
                {
                    continue;
                }

                return true;
            }

            return false;
        }
    }

    [CustomEditor(typeof(EventTree))]
    public class EventTreeEditor : Editor
    {
        private EventTree m_Target;
        private string m_PreviewTargetIdentifier;
        private string m_PossibleEventTree;
        private int m_PossibleEventTreeLength;

        private void OnEnable()
        {
            m_Target = target as EventTree;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Debug", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            m_PreviewTargetIdentifier = EditorGUILayout.DelayedTextField(
                new GUIContent("Tree Preview Target Identifier"), m_PreviewTargetIdentifier);
            if (EditorGUI.EndChangeCheck())
            {
                var possibleEventTree = m_Target.FindPossibleEventTree(m_PreviewTargetIdentifier);
                m_PossibleEventTree = string.Empty;
                m_PossibleEventTreeLength = possibleEventTree.Count;

                foreach (var node in possibleEventTree)
                {
                    m_PossibleEventTree += $"{node} -> \n";
                }
            }

            if (!string.IsNullOrEmpty(m_PossibleEventTree))
            {
                EditorGUILayout.LabelField(
                    m_PossibleEventTree,
                    GUILayout.Height((EditorGUIUtility.singleLineHeight + 2) * m_PossibleEventTreeLength));
            }
        }
    }
}