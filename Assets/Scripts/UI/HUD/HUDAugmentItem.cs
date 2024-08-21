using Data;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class HUDAugmentItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI augmentNameText;
        [SerializeField] private TextMeshProUGUI augmentDescriptionText;
        [SerializeField] private Image augmentIconImage;

        private Transform m_CachedTransform;
        private RectTransform m_RectTransform;

        public string AugmentName
        {
            get => augmentNameText.text;
            set => augmentNameText.text = value;
        }

        public string AugmentDescription
        {
            get => augmentDescriptionText.text;
            set => augmentDescriptionText.text = value;
        }

        public Image AugmentIconImage
        {
            get => augmentIconImage;
            set => augmentIconImage = value;
        }

        public Transform CachedTransform =>
            m_CachedTransform is null ? (m_CachedTransform = transform) : m_CachedTransform;

        public RectTransform RectTransform =>
            m_RectTransform is null ? (m_RectTransform = (RectTransform)CachedTransform) : m_RectTransform;
    }
}
