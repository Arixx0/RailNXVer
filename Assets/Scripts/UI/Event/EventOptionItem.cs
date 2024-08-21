using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class EventOptionItem : Button
    {
        [SerializeField] private TextMeshProUGUI optionText;

        private Transform m_CachedTransform;
        private RectTransform m_RectTransform;

        public string Option
        {
            get => optionText.text;
            set => optionText.text = value;
        }

        public Transform CachedTransform =>
            m_CachedTransform is null ? (m_CachedTransform = transform) : m_CachedTransform;

        public RectTransform RectTransform =>
            m_RectTransform is null ? (m_RectTransform = (RectTransform)CachedTransform) : m_RectTransform;
    }
}

