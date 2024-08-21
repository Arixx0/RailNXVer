using Data;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class AugmentIcon : Button
    {
        [SerializeField] private Image augmentIconImage;

        private string m_Identifier;
        private Transform m_CachedTransform;
        private RectTransform m_RectTransform;

        public Image AugmentIconImage
        {
            get => augmentIconImage;
            set => augmentIconImage = value;
        }

        public string Identifier
        {
            get => m_Identifier;
            set => m_Identifier = value;
        }
        public Transform CachedTransform =>
            m_CachedTransform is null ? (m_CachedTransform = transform) : m_CachedTransform;

        public RectTransform RectTransform =>
            m_RectTransform is null ? (m_RectTransform = (RectTransform)CachedTransform) : m_RectTransform;
    }
}