using Data;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace UI
{
    public class NoticeWarningItem : MonoBehaviour, IEnumerator
    {
        [SerializeField] private TextMeshProUGUI noticeName;
        [SerializeField] private TextMeshProUGUI noticeDescription;

        [SerializeField] private Image noticeImage;
        [SerializeField] private Image timerImage;
        [SerializeField] private TextMeshProUGUI timerText;

        // TODO : Add notice image change capability
        // TODO : Timer Implemented as a Coroutine or Update, Currently a Coroutine

        private string m_NoticeIdentifier;
        private NoticeType m_NoticeType;
        private Transform m_CachedTransform;
        private RectTransform m_RectTransform;


        [HideInInspector] public float delay;
        private float elapsedTime;

        public event OnTaskUpdateDelegate OnTaskUpdate;
        public event OnTaskCompleteDelegate OnTaskComplete;
        public event Action<string> OnDeleteRequest;

        public string NoticeName
        {
            get => noticeName.text;
            set => noticeName.text = value;
        }

        public NoticeType NoticeType
        {
            get => m_NoticeType;
            set => m_NoticeType = value;
        }

        public string NoticeDescription
        {
            get => noticeDescription.text;
            set => noticeDescription.text = value;
        }

        public string NoticeIdentifier
        {
            get => m_NoticeIdentifier;
            set => m_NoticeIdentifier = value;
        }

        public Transform CachedTransform =>
            m_CachedTransform is null ? (m_CachedTransform = transform) : m_CachedTransform;

        public RectTransform RectTransform =>
            m_RectTransform is null ? (m_RectTransform = (RectTransform)CachedTransform) : m_RectTransform;

        public void SetData(float time)
        {
            delay = time;
            if (m_NoticeType == NoticeType.Caution)
            {
                timerImage.gameObject.SetActive(delay > 0);
                noticeImage.color = Color.yellow;
            }
            else
            {
                noticeImage.color = Color.red;
            }
        }

        private void UpdateTimer()
        {
            if (delay < 0)
            {
                return;
            }
            timerImage.fillAmount = elapsedTime / delay;
            timerText.text = elapsedTime.ToString("F0");
        }

        private void UpdateTimeText()
        {
            if (!DatabaseUtility.TryGetData(Database.TextData, m_NoticeIdentifier, out var TimetextData))
            {
                return;
            }
            noticeDescription.text = $"{TimetextData.korean} {elapsedTime.ToString("F0")}s";
        }

        public void Solving()
        {
            OnDeleteRequest?.Invoke(m_NoticeIdentifier);
        }

        bool IEnumerator.MoveNext()
        {
            var deltaTime = TimeScaleManager.Get.GetDeltaTimeOfTag("Global");
            elapsedTime += deltaTime;

            OnTaskUpdate?.Invoke(elapsedTime);

            if (m_NoticeType == NoticeType.Caution)
            {
                UpdateTimer();
            }
            else
            {
                UpdateTimeText();
            }

            if (elapsedTime >= delay)
            {
                OnTaskComplete?.Invoke();
            }


            return elapsedTime <= delay;
        }

        void IEnumerator.Reset()
        {
            delay = 0;
            elapsedTime = 0;
        }

        object IEnumerator.Current => null;

        public delegate void OnTaskCompleteDelegate();

        public delegate void OnTaskUpdateDelegate(float progress);
    }
}

