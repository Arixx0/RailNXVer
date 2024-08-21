using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ShopItem : Button
    {
        [SerializeField]
        private TextMeshProUGUI itemNameText;
        
        [SerializeField]
        private TextMeshProUGUI itemCostText;
        
        [SerializeField]
        private Image itemImage;

        private Transform m_CachedTransform;
        
        private RectTransform m_RectTransform;

        public string ItemIdentifier
        {
            get;
            set;
        }
        
        public string ItemName
        {
            get => itemNameText.text;
            set
            {
                itemNameText.text = value;
                
#if UNITY_EDITOR
                gameObject.name = $"ShopItem_{value}";
#endif
            }
        }

        public string ItemCost
        {
            get => itemCostText.text;
            set => itemCostText.text = value;
        }

        public Image ItemImage
        {
            get => itemImage;
            set => itemImage = value;
        }

        public int IndexFromSlot
        {
            get;
            set;
        }

        public Transform CachedTransform => m_CachedTransform ??= transform;

        public RectTransform RectTransform => m_RectTransform ??= (RectTransform)CachedTransform;
    }
}