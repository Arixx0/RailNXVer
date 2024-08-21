using UnityEditor;
using UnityEditor.UI;

namespace UI
{
    [CustomEditor(typeof(ShopItem))]
    public class ShopItemEditor : ButtonEditor
    {
        private SerializedProperty m_ItemName;
        private SerializedProperty m_ItemCostText;
        private SerializedProperty m_ItemImage;


        protected override void OnEnable()
        {
            base.OnEnable();

            m_ItemName = serializedObject.FindProperty("itemNameText");
            m_ItemCostText = serializedObject.FindProperty("itemCostText");
            m_ItemImage = serializedObject.FindProperty("itemImage");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            serializedObject.Update();
            
            EditorGUILayout.LabelField("ShopItem Options", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(m_ItemName);
            EditorGUILayout.PropertyField(m_ItemCostText);
            EditorGUILayout.PropertyField(m_ItemImage);

            serializedObject.ApplyModifiedProperties();
        }
    }
}