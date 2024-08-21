using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class EventRewardItem : MonoBehaviour
    {
        [SerializeField] private Image rewardImage;
        [SerializeField] private TextMeshProUGUI rewardQuantityText;

        [SerializeField] private GameObject rewardHover;
        [SerializeField] private TextMeshProUGUI rewardNameText;
        [SerializeField] private TextMeshProUGUI rewardDescriptionText;

        private Transform m_CachedTransform;
        private RectTransform m_RectTransform;

        public string rewardQuantity
        {
            get => rewardQuantityText.text;
            set => rewardQuantityText.text = value;
        }

        public string rewardName
        {
            get => rewardNameText.text;
            set => rewardNameText.text = value;
        }

        public string rewardDescription
        {
            get => rewardDescriptionText.text;
            set => rewardDescriptionText.text = value;
        }

        public Transform CachedTransform =>
            m_CachedTransform is null ? (m_CachedTransform = transform) : m_CachedTransform;

        public RectTransform RectTransform =>
            m_RectTransform is null ? (m_RectTransform = (RectTransform)CachedTransform) : m_RectTransform;
    }

}
