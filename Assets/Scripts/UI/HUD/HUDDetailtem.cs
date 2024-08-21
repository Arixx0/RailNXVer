using Data;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class HUDDetailItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI detailName;
        [SerializeField] private TextMeshProUGUI detailValue;

        private Transform m_CachedTransform;
        private RectTransform m_RectTransform;

        public string DetailName
        {
            get => detailName.text;
            set => detailName.text = value;
        }

        public string DetailValue
        {
            get => detailValue.text;
            set => detailValue.text = value;
        }

        public Transform CachedTransform =>
            m_CachedTransform is null ? (m_CachedTransform = transform) : m_CachedTransform;

        public RectTransform RectTransform =>
            m_RectTransform is null ? (m_RectTransform = (RectTransform)CachedTransform) : m_RectTransform;
    }
}
