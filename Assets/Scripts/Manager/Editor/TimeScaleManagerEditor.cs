using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Manager
{
    [CustomPropertyDrawer(typeof(TimeScaleManager.TimeScaleTable))]
    public class TimeScaleTablePropertyDrawer : PropertyDrawer
    {
        private ReorderableList m_TimeScaleTableList;
        private SerializedObject m_TimeTableHolderObject;
        private SerializedProperty m_GlobalTimeScale;
        private SerializedProperty m_Keys;
        private SerializedProperty m_Values;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (m_TimeScaleTableList == null)
            {
                m_TimeTableHolderObject = property.serializedObject;
                m_Keys = property.FindPropertyRelative("keysSerializedCache");
                m_Values = property.FindPropertyRelative("valuesSerializedCache");
                m_GlobalTimeScale = m_Values.GetArrayElementAtIndex(0).FindPropertyRelative("timeScale");

                m_TimeScaleTableList = new ReorderableList(m_TimeTableHolderObject, m_Values,
                    false, true, false, false);
            
                m_TimeScaleTableList.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "TimeScales");

                m_TimeScaleTableList.drawElementCallback = (rect, index, active, focused) =>
                {
                    var tag = m_Keys.GetArrayElementAtIndex(index);
                    var timeScaleContext = m_Values.GetArrayElementAtIndex(index);
                    var globalTimeScale = timeScaleContext.FindPropertyRelative("timeScale").floatValue * m_GlobalTimeScale.floatValue;

                    var timeScaleContextHeight = EditorGUI.GetPropertyHeight(timeScaleContext);

                    var tagRect
                        = new Rect(rect.x, rect.y + 2, 80, EditorGUIUtility.singleLineHeight);
                    var timeScaleContextRect
                        = new Rect(tagRect.xMax + 2, rect.y + 2, rect.width - 141, timeScaleContextHeight);
                    var targetGlobalTimeScaleLabelRect
                        = new Rect(timeScaleContextRect.xMax + 2, rect.y + 2, 15, EditorGUIUtility.singleLineHeight);
                    var targetGlobalTimeScaleRect
                        = new Rect(targetGlobalTimeScaleLabelRect.xMax + 2, rect.y + 2, 40, EditorGUIUtility.singleLineHeight);

                    var prevGUIEnabled = GUI.enabled;

                    GUI.enabled = false;
                    EditorGUI.PropertyField(tagRect, tag, GUIContent.none);
                
                    GUI.enabled = true;
                    EditorGUI.PropertyField(timeScaleContextRect, timeScaleContext, GUIContent.none);

                    if (index != 0)
                    {
                        GUI.enabled = false;
                        EditorGUI.LabelField(targetGlobalTimeScaleLabelRect, "g:");
                        EditorGUI.TextField(targetGlobalTimeScaleRect, $"{globalTimeScale:0.00}");
                    }
                
                    GUI.enabled = prevGUIEnabled;
                };

                m_TimeScaleTableList.elementHeightCallback = index =>
                {
                    if (m_TimeScaleTableList.count <= 0)
                    {
                        return EditorGUIUtility.singleLineHeight;
                    }

                    return EditorGUI.GetPropertyHeight(m_TimeScaleTableList.serializedProperty.GetArrayElementAtIndex(index));
                };
            
                EditorUtility.SetDirty(m_TimeTableHolderObject.targetObject);
                return;
            }

            EditorGUI.BeginProperty(position, GUIContent.none, property);
        
            m_TimeScaleTableList.DoList(position);
        
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return m_TimeTableHolderObject == null
                ? EditorGUIUtility.singleLineHeight
                : m_TimeScaleTableList.GetHeight();
        }
    }

    [CustomPropertyDrawer(typeof(TimeScaleManager.TimeScaleContext))]
    public class TimeScaleContextPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
        
            var timeScaleProperty = property.FindPropertyRelative("timeScale");
            var actualTimeScaleProperty = property.FindPropertyRelative("actualTimeScale");
            var timeProperty = property.FindPropertyRelative("time");
        
            var timeScaleLabelRect
                = new Rect(position.x, position.y, 15f, EditorGUIUtility.singleLineHeight);
            var timeScaleRect
                = new Rect(timeScaleLabelRect.xMax + 2f, position.y, position.width - 17f, EditorGUIUtility.singleLineHeight);
            var actualTimeScaleLabelRect
                = new Rect(position.x, timeScaleLabelRect.yMax + 2f, 15f, EditorGUIUtility.singleLineHeight);
            var actualTimeScaleRect
                = new Rect(actualTimeScaleLabelRect.xMax + 2f, timeScaleLabelRect.yMax + 2f, 40f, EditorGUIUtility.singleLineHeight);
            var timeLabelRect
                = new Rect(actualTimeScaleRect.xMax + 2f, timeScaleLabelRect.yMax + 2f, 15f, EditorGUIUtility.singleLineHeight);
            var timeRect
                = new Rect(timeLabelRect.xMax + 2f, timeScaleLabelRect.yMax + 2f, position.width - 76f, EditorGUIUtility.singleLineHeight);
        
            var guiEnabled = GUI.enabled;

            GUI.enabled = true;
            EditorGUI.BeginChangeCheck();
            EditorGUI.LabelField(timeScaleLabelRect, new GUIContent("t:"));
            EditorGUI.PropertyField(timeScaleRect, timeScaleProperty, GUIContent.none);
            if (EditorGUI.EndChangeCheck())
            {
                actualTimeScaleProperty.floatValue = timeScaleProperty.floatValue;
            }

            GUI.enabled = false;
            EditorGUI.LabelField(actualTimeScaleLabelRect, new GUIContent("c:"));
            EditorGUI.PropertyField(actualTimeScaleRect, actualTimeScaleProperty, GUIContent.none);

            GUI.enabled = false;
            EditorGUI.LabelField(timeLabelRect, new GUIContent("e:"));
            EditorGUI.PropertyField(timeRect, timeProperty, GUIContent.none);

            GUI.enabled = guiEnabled;
        
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // 2 lines and 3 spaces
            const float spacing = 3f;
            return (EditorGUIUtility.singleLineHeight * 2) + (spacing * 3);
        }
    }
}