using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace UI
{
    public class NoticeItem : MonoBehaviour
    {
        #region Notice Item References

        [SerializeField] private TextMeshProUGUI notifierName;
        [SerializeField] private NoticeItemDescription noticeDescriptionItem;
        [SerializeField] private Transform noticeDescriptionPanel;

        private Transform m_CachedTransform;
        private RectTransform m_RectTransform;

        private Utility.ObjectPool<NoticeItemDescription> noticeDescriptionTextPool;
        private readonly List<NoticeItemDescription> m_ActiveNoticeDescriptionTexts = new(5);

        public Transform CachedTransform =>
            m_CachedTransform is null ? (m_CachedTransform = transform) : m_CachedTransform;

        public RectTransform RectTransform =>
            m_RectTransform is null ? (m_RectTransform = (RectTransform)CachedTransform) : m_RectTransform;

        public string Notifier => notifierName.text;

        #endregion

        private void Awake()
        {
            noticeDescriptionTextPool = Utility.ObjectPool<NoticeItemDescription>.CreateObjectPool(noticeDescriptionItem, CachedTransform);
        }

        public void NotifierNameChange(string notifier)
        {
            notifierName.text = notifier;
        }

        public void CreateNoticeDescription(string textData, float time)
        {
            gameObject.SetActive(true);
            var descripitionItem = noticeDescriptionTextPool.GetOrCreate();
            descripitionItem.transform.SetParent(noticeDescriptionPanel);
            descripitionItem.text = $"{textData} ¾Ë¸²";
            descripitionItem.delay = time;
            m_ActiveNoticeDescriptionTexts.Add(descripitionItem);
            descripitionItem.OnTaskComplete += () =>
            {
                m_ActiveNoticeDescriptionTexts.Remove(descripitionItem);
                noticeDescriptionTextPool.ReturnObject(descripitionItem);
                if (m_ActiveNoticeDescriptionTexts.Count <= 0)
                {
                    gameObject.SetActive(false);
                }
            };
            StartCoroutine(descripitionItem);
        }

    }
}

