using System.Reflection;
using Data;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace TrainScripts
{
    [CustomPropertyDrawer(typeof(Inventory))]
    public class InventoryDrawer : PropertyDrawer
    {
        private bool m_FoldoutInventoryList = false;
        private ReorderableList m_InventoryList;
        private SerializedObject m_InventoryHolderObject;
        private Inventory m_TargetInventory;
        private SerializedProperty m_ResourceTypes;
        private SerializedProperty m_ResourceAmounts;
        private Color[] m_ResourceColorScheme;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            CreateInventoryListElement(property);
            
            EditorGUI.BeginProperty(position, GUIContent.none, property);
            
            var labelRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            
            m_FoldoutInventoryList = EditorGUI.Foldout(labelRect, m_FoldoutInventoryList, label, true);
            if (m_FoldoutInventoryList)
            {
                var listElementRect = new Rect(position.x, labelRect.yMax + 2f, position.width, m_InventoryList.GetHeight());
                m_InventoryList.DoList(listElementRect);
            }
            
            EditorGUI.EndProperty();
        }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var height = EditorGUIUtility.singleLineHeight;
            
            if (m_InventoryList != null && m_FoldoutInventoryList)
            {
                height += 2 + m_InventoryList.GetHeight();
            }

            return height;
        }

        private void CreateInventoryListElement(SerializedProperty property)
        {
            if (m_InventoryList != null && m_InventoryList.serializedProperty != property)
            {
                return;
            }
            
            m_InventoryHolderObject = property.serializedObject;

            var serializedObjectType = m_InventoryHolderObject.targetObject.GetType();
            var inventoryField = serializedObjectType.GetField(property.name, BindingFlags.NonPublic | BindingFlags.Instance);
            Debug.Assert(inventoryField != null, $"Field {property.name} not found in {serializedObjectType}");
            m_TargetInventory = (Inventory)inventoryField.GetValue(m_InventoryHolderObject.targetObject);

            m_ResourceTypes = property.FindPropertyRelative("resourceTypes");
            m_ResourceAmounts = property.FindPropertyRelative("resourceAmount");

            m_InventoryList = new ReorderableList(m_InventoryHolderObject, m_ResourceAmounts,
                false, false, false, false);
            
            m_ResourceColorScheme = new Color[m_InventoryList.count];
            var fromColor = new Color(0.207f, 0.214f, 0.292f);
            var toColor = new Color(0.468f, 0.625f, 0.511f);
            for (var i = 0; i < m_ResourceColorScheme.Length; i += 1)
            {
                var time = Mathf.InverseLerp(0, m_InventoryList.count, i);
                m_ResourceColorScheme[i] = Color.Lerp(fromColor, toColor, time);
            }

            m_InventoryList.drawElementCallback = (rect, index, active, focused) =>
            {
                var resourceType =  (ResourceType)m_ResourceTypes.GetArrayElementAtIndex(index).enumValueIndex;
                var resourceAmount = m_ResourceAmounts.GetArrayElementAtIndex(index).intValue;

                var typeNameRect = new Rect(rect.x, rect.y, 100, rect.height - 2);
                var amountRect = new Rect(typeNameRect.xMax + 4, rect.y + 2, rect.width - 232, rect.height - 4);
                var addButtonRect = new Rect(amountRect.xMax + 4, rect.y + 2, 60, rect.height - 4);
                var subButtonRect = new Rect(addButtonRect.xMax + 4, rect.y + 2, 60, rect.height - 4);
                    
                EditorGUI.LabelField(typeNameRect, new GUIContent(resourceType.ToString()));
                    
                EditorGUI.BeginChangeCheck();
                resourceAmount = EditorGUI.IntField(amountRect, GUIContent.none, resourceAmount);
                if (EditorGUI.EndChangeCheck())
                {
                    m_TargetInventory[resourceType] = resourceAmount;
                    EditorUtility.SetDirty(m_InventoryHolderObject.targetObject);
                }

                if (GUI.Button(addButtonRect, new GUIContent("+ 100")))
                {
                    if (EditorApplication.isPlaying)
                    {
                        Train m_Train = Object.FindObjectOfType<Train>();
                        m_Train.AddResourceToInventory(resourceType, 100);
                    }
                    EditorUtility.SetDirty(m_InventoryHolderObject.targetObject);
                }

                if (GUI.Button(subButtonRect, new GUIContent("- 100")))
                {
                    if (EditorApplication.isPlaying)
                    {
                        Train m_Train = Object.FindObjectOfType<Train>();
                        m_Train.AddResourceToInventory(resourceType, -100);
                    }
                    EditorUtility.SetDirty(m_InventoryHolderObject.targetObject);
                }
            };
            
            m_InventoryList.drawElementBackgroundCallback = (rect, index, active, focused) =>
            {
                EditorGUI.DrawRect(rect, m_ResourceColorScheme[index]);
            };
            
            m_InventoryList.elementHeightCallback = index => EditorGUIUtility.singleLineHeight + 2;
                
            EditorUtility.SetDirty(m_InventoryHolderObject.targetObject);
        }
    }
}