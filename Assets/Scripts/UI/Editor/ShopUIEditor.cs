using System.Collections.Generic;
using System.Linq;
using Data;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace UI
{
    [CustomEditor(typeof(ShopUI))]
    public class ShopUIEditor : Editor
    {
        private ShopKeeperItemsDefinition m_TestDefinition;

        private ShopItemsListElement m_ShopItemsListElement;

        private void OnEnable()
        {
            DatabaseDefinitions.GetOrCreate().Load();

            m_ShopItemsListElement = new ShopItemsListElement();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            m_ShopItemsListElement.DoLayout();
            
            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
            EditorGUILayout.LabelField("Test Options", EditorStyles.boldLabel);
            
            EditorGUILayout.Space();
            
            m_TestDefinition = (ShopKeeperItemsDefinition)EditorGUILayout.ObjectField(
                new GUIContent("Test Definition"), m_TestDefinition, typeof(ShopKeeperItemsDefinition), false);
            GUI.enabled = m_TestDefinition != null;
            if (GUILayout.Button("Try Show Shop"))
            {
                var shopUI = (ShopUI)target;
                shopUI.Show(m_TestDefinition);
            }
            GUI.enabled = true;
        }
    }

    public class ShopItemsListElement
    {
        private List<ShopItemData> m_ShopItems;

        private string m_ShopItemsSearchText;

        private bool m_FoldoutShopItemsListElement;
        
        private Vector2 m_ShopItemsListScrollPosition;
        
        private ReorderableList m_ShopItemsListElement;

        public ShopItemsListElement()
        {
            m_ShopItems = Database.ShopItemsData.Values.ToList();

            m_ShopItemsListElement = new ReorderableList(
                m_ShopItems, typeof(ShopItemData), false, false, false, false);

            m_ShopItemsListElement.elementHeight = EditorGUIUtility.singleLineHeight * 3 + 6;

            m_ShopItemsListElement.drawElementCallback = (rect, index, active, focused) =>
            {
                var data = (ShopItemData)m_ShopItemsListElement.list[index];

                var identifierRect = new Rect(rect.x, rect.y + 1, rect.width, EditorGUIUtility.singleLineHeight);
                EditorGUI.LabelField(identifierRect, new GUIContent($"{index} {data.Identifier}"));

                var itemTypeRect = new Rect(rect.x, identifierRect.yMax + 1, 80f, EditorGUIUtility.singleLineHeight);
                EditorGUI.EnumPopup(itemTypeRect, GUIContent.none, data.ItemType);

                var dropPossibilityRect = new Rect(itemTypeRect.xMax + 1f, itemTypeRect.y, 36f, EditorGUIUtility.singleLineHeight);
                EditorGUI.FloatField(dropPossibilityRect, data.DropPossibility);

                dropPossibilityRect.x = dropPossibilityRect.xMax + 1;
                dropPossibilityRect.width = 16f;
                EditorGUI.LabelField(dropPossibilityRect, new GUIContent("p"));

                var dropAmountRect = new Rect(dropPossibilityRect.xMax + 1f, itemTypeRect.y, 46f, EditorGUIUtility.singleLineHeight);
                EditorGUI.LabelField(dropAmountRect, new GUIContent(", Drops"));

                dropAmountRect.x = dropAmountRect.xMax + 1f;
                EditorGUI.IntField(dropAmountRect, data.DropAmount);
                
                var costRect = new Rect(dropAmountRect.xMax + 1f, itemTypeRect.y, 46f, EditorGUIUtility.singleLineHeight);
                EditorGUI.LabelField(costRect, new GUIContent(", Costs"));
                
                costRect.x = costRect.xMax + 1f;
                EditorGUI.IntField(costRect, data.Cost);

                var tierRect = new Rect(costRect.xMax + 2, itemTypeRect.y, 45f, EditorGUIUtility.singleLineHeight);
                EditorGUI.LabelField(tierRect, new GUIContent($", tier {data.Tier}"));

                var copyToClipboardButton = new Rect(rect.x, tierRect.yMax + 2, rect.width, EditorGUIUtility.singleLineHeight);
                if (GUI.Button(copyToClipboardButton, "Copy to Clipboard"))
                {
                    data.CopyToClipboard();
                }
            };
        }

        public void DoLayout()
        {
            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
            m_FoldoutShopItemsListElement = EditorGUILayout.Foldout(m_FoldoutShopItemsListElement, new GUIContent("Shop items(Read Only)"), true);
            if (m_FoldoutShopItemsListElement)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Search by ID", GUILayout.Width(80f));
                m_ShopItemsSearchText = EditorGUILayout.DelayedTextField(m_ShopItemsSearchText);
                if (GUILayout.Button("Search"))
                {
                    m_ShopItemsListElement.index = m_ShopItems.FindIndex(0,
                        data => string.Compare(data.Identifier, m_ShopItemsSearchText) == 0);
                }
                EditorGUILayout.EndHorizontal();
                
                m_ShopItemsListScrollPosition = EditorGUILayout.BeginScrollView(
                    m_ShopItemsListScrollPosition, false, true, GUILayout.Height(300f));
                m_ShopItemsListElement.DoLayoutList();
                EditorGUILayout.EndScrollView();
            }
        }
    }
}