using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using System.Collections.Generic;

namespace UI
{
    [System.Serializable]
    public class ContextMenuItem : Button
    {
        [SerializeField] private Image itemImage;
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI itemName;
        [SerializeField] private Outline outline;

        private Transform m_CachedTransform;
        private string m_Identifier;
        private Dictionary<Data.ResourceType, float> m_ResourceData = new();

        public float AngularPosition { get; private set; }

        public Transform CachedTransform => m_CachedTransform ??= GetComponent<Transform>();

        public string Identifier => m_Identifier;

        public Dictionary<Data.ResourceType, float> ResourceData => m_ResourceData;

        public void SetData(ContextMenuData menuData, float angularPosition, UnityAction onClickCallback, Dictionary<Data.ResourceType, float> resourceData)
        {
            if (Utility.DatabaseUtility.TryGetData(Data.Database.TextData, menuData.identifier.Identifier, out var textData))
            {
                itemName.text = textData.korean;
                m_Identifier = menuData.identifier.Identifier;
            }       
            iconImage.sprite = menuData.itemIcon;
            AngularPosition = angularPosition;
            m_ResourceData = resourceData;
            Select(false);
            onClick.RemoveAllListeners();
            onClick.AddListener(onClickCallback);
            onClick.AddListener(menuData.onClickEvent.Invoke);
        }

        public void Select(bool selected)
        {
            targetGraphic.color = selected ? colors.selectedColor : colors.normalColor;
        }
    }
}