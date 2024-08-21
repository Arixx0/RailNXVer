using Data;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class HUDResourceItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI resourceAmount;
        [SerializeField] private ChangingImageLoader resourceImage;

        private Transform m_CachedTransform;
        private RectTransform m_RectTransform;
        private ResourceType m_resourceType;
        
        public ResourceType ResourceType
        {
            get => m_resourceType;
            set => m_resourceType = value;
        }

        public string ResourceAmount
        {
            get => resourceAmount.text;
            set => resourceAmount.text = value;
        }

        public ChangingImageLoader ResourceImage => resourceImage;

        public Transform CachedTransform =>
            m_CachedTransform is null ? (m_CachedTransform = transform) : m_CachedTransform;

        public RectTransform RectTransform =>
            m_RectTransform is null ? (m_RectTransform = (RectTransform)CachedTransform) : m_RectTransform;
    }
}
