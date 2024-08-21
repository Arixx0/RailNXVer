using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class LabeledButton : Button
    {
        [SerializeField] private TextMeshProUGUI labelComponent;

        private Transform m_CachedTransform;
        private RectTransform m_RectTransform;
        
        public string Label
        {
            get => labelComponent.text;
            set => labelComponent.text = value;
        }

        public Transform CachedTransform =>
            m_CachedTransform is null ? (m_CachedTransform = transform) : m_CachedTransform;

        public RectTransform RectTransform =>
            m_RectTransform is null ? (m_RectTransform = (RectTransform)CachedTransform) : m_RectTransform;
    }
}