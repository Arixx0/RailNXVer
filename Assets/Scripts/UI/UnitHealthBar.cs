using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UnitHealthBar : Slider
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Gradient healthColorGradient;
        [SerializeField] private float minWidth = 100;
        [SerializeField] private float maxWidth = 200;

        private RectTransform m_RectTransform;
        private Image m_FillRectImage;
        private int m_CurrentHealthPoint;

        protected override void OnEnable()
        {
            base.OnEnable();

            m_RectTransform = (RectTransform)transform;
            UpdateLayout();
            
            m_FillRectImage = fillRect.GetComponent<Image>();
        }

        protected override void Update()
        {
            base.Update();

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return;
            }
#endif

            value = Mathf.Lerp(value, m_CurrentHealthPoint, 0.1f);
        }

        private void LateUpdate()
        {
            var healthDelta = Mathf.InverseLerp(minValue, maxValue, value);
            m_FillRectImage.color = healthColorGradient.Evaluate(healthDelta);
        }

        public void SetHealthProperties(int maxHealthPoint, int currentHealthPoint)
        {
            m_CurrentHealthPoint = currentHealthPoint;
            value = currentHealthPoint;
            maxValue = maxHealthPoint;
            
            UpdateLayout();
        }
        
        public void SetHealthProperties(float maxHealthPoint, float currentHealthPoint)
        {
            canvasGroup.alpha = 1;

            m_CurrentHealthPoint = (int)currentHealthPoint;
            value = currentHealthPoint;
            maxValue = maxHealthPoint;
            
            UpdateLayout();
        }

        public void UpdateHealthPoint(int healthPoint)
        {
            m_CurrentHealthPoint = healthPoint;
        }

        public void UpdateHealthPoint(float healthPoint)
        {
            m_CurrentHealthPoint = (int)healthPoint;
        }

        private void UpdateLayout()
        {
            var targetWidth = Mathf.Clamp(maxValue, minWidth, maxWidth);
            if (m_RectTransform == null)
            {
                m_RectTransform = (RectTransform)transform;
            }
            m_RectTransform.sizeDelta = new Vector2(targetWidth, m_RectTransform.sizeDelta.y);
        }
    }
}