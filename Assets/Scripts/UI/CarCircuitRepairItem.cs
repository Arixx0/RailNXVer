using Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class CarCircuitRepairItem : Button
    {
        [SerializeField] private Color repairableAmountColor;
        [SerializeField] private Color nonRepairableAmountColor;
        [SerializeField] private TextMeshProUGUI repairAmount;

        private Transform m_CachedTransform;
        private RectTransform m_RectTransform;

        public Transform CachedTransform =>
            m_CachedTransform is null ? (m_CachedTransform = transform) : m_CachedTransform;

        public RectTransform RectTransform =>
            m_RectTransform is null ? (m_RectTransform = (RectTransform)CachedTransform) : m_RectTransform;

        public string RepairAmount
        {
            get => repairAmount.text;
            set => repairAmount.text = value;
        }

        public void SetRepairAmountColor(bool isAvailable)
        {
            repairAmount.color = isAvailable ? repairableAmountColor : nonRepairableAmountColor;
        }

    }
}