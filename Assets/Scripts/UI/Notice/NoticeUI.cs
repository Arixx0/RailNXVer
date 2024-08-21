using System.Collections.Generic;
using UnityEngine;
using Data;
using System.Linq;
using Utility;
using TrainScripts;

namespace UI
{
    public partial class HUD : MonoBehaviour
    {
        #region Notice References
        [Space, Header("Notice Panel")]
        [SerializeField] private NoticeWarningItem noticeWarningItemPrefab;
        [SerializeField] private NoticeItem noticeItem;

        [SerializeField] private Transform noticeWarningPanel;
        [SerializeField] private Transform noticePanel;

        [SerializeField] private float noticeDeleteDelay;

        private ObjectPool<NoticeWarningItem> noticeWarningItemPool;
        private readonly Dictionary<string, NoticeWarningItem> m_ActiveNoticeWarningItems = new();

        private INoticeEventExecutor noticeEventExecutor;

        #endregion

        #region Notice Implementations

        private void OnNoticeReceived(NoticeIdentifier noticeIdentifier, float time)
        {
            var noticeType = noticeIdentifier.GetNoticeType();

            if (noticeType == NoticeType.Notice)
            {
                if (noticeItem.Notifier != noticeIdentifier.NotifierName && DatabaseUtility.TryGetData(Database.TextData, noticeIdentifier.NotifierName, out var notifierTextData))
                {
                    noticeItem.NotifierNameChange(notifierTextData.korean);
                }
                //if (DatabaseUtility.TryGetData(Database.TextData, noticeIdentifier.Identifier, out var noticeDescTextData))
                //{
                    noticeItem.CreateNoticeDescription(noticeIdentifier.Identifier, noticeDeleteDelay);
                //}
            }
            else if (noticeType == NoticeType.Caution || noticeType == NoticeType.Warning)
            {
                if (DatabaseUtility.TryGetData(Database.TextData, noticeIdentifier.Identifier, "", "Desc", out var noticeNameTextData, out var noticeDescTextData))
                {
                    if (m_ActiveNoticeWarningItems.ContainsKey(noticeIdentifier.Identifier))
                    {
                        Debug.LogWarning("There is already a same warning/caution message.");
                        return;
                    }

                    NoticeWarningItem noticeWarningItem = noticeWarningItemPool.GetOrCreate();
                    noticeWarningItem.CachedTransform.SetParent(noticeWarningPanel);
                    noticeWarningItem.NoticeName = noticeNameTextData.korean;
                    noticeWarningItem.NoticeDescription = noticeDescTextData.korean;
                    noticeWarningItem.NoticeIdentifier = noticeIdentifier.Identifier;
                    noticeWarningItem.NoticeType = noticeType;
                    noticeWarningItem.SetData(time);
                    m_ActiveNoticeWarningItems.Add(noticeIdentifier.Identifier, noticeWarningItem);
                    if (time > 0)
                    {
                        StartCoroutine(noticeWarningItem);
                        noticeWarningItem.OnTaskComplete += () =>
                        {
                            m_ActiveNoticeWarningItems.Remove(noticeWarningItem.NoticeIdentifier);
                            noticeWarningItemPool.ReturnObject(noticeWarningItem);
                        };
                    }
                    else
                    {
                        noticeWarningItem.OnDeleteRequest += DeleteNoticeWarningItem;
                    }
                    ReOrderWarningNotice();
                }
            }
        }

        //public void OnNoticeReceived(NoticeIdentifier provider)
        //{
        //    if (!DatabaseUtility.TryGetData(Database.TextData, notifier, out var notifierTextData))
        //    {
        //        return;
        //    }
        //    if (!DatabaseUtility.TryGetData(Database.TextData, noticeIdentifier, "", "Desc", out var noticeNameTextData, out var noticeDescTextData))
        //    {
        //        return;
        //    }

        //    foreach(NoticeItem noticeItemInstance in m_ActiveNoticeItems)
        //    {
        //        noticeItemPool.ReturnObject(noticeItemInstance);
        //    }
        //    m_ActiveNoticeItems.Clear();

        //    // TODO : Get Notice Data (identifier)
        //    NoticeItem noticeItem = noticeItemPool.GetOrCreate();
        //    noticeItem.CachedTransform.SetParent(noticePanel);
        //    noticeItem.NotifierName = notifierTextData.korean;
        //    noticeItem.NoticeDescription = noticeDescTextData.korean;
        //    m_ActiveNoticeItems.Add(noticeItem);
        //    noticeItem.OnDeleteRequest += DeleteNoticeItem;
        //}

        public void DeleteNoticeWarningItem(string noticeIdentifier)
        {
            if (!m_ActiveNoticeWarningItems.TryGetValue(noticeIdentifier, out NoticeWarningItem noticeWarningItem))
            {
                Debug.LogError($"The Notice({noticeIdentifier}) does not exist(active) in NoticeUI.");
                return;
            }
            noticeWarningItem.OnDeleteRequest -= DeleteNoticeWarningItem;
            m_ActiveNoticeWarningItems.Remove(noticeIdentifier);
            noticeWarningItemPool.ReturnObject(noticeWarningItem);
        }

        public NoticeWarningItem GetNoticeWarningItem(string noticeIdentifier)
        {
            if (!m_ActiveNoticeWarningItems.TryGetValue(noticeIdentifier, out NoticeWarningItem noticeWarningItem))
            {
                Debug.LogError($"The Notice({noticeIdentifier}) does not exist(active) in NoticeUI.");
            }
            return noticeWarningItem;
        }

        private void ReOrderWarningNotice()
        {
            var sortedItem = m_ActiveNoticeWarningItems.OrderByDescending(item => item.Value.NoticeType).ToList();
            
            for (int i = 0; i < sortedItem.Count; i++)
            {
                sortedItem[i].Value.transform.SetSiblingIndex(i);
            }
        }

        #endregion

        public void EventExecutorRegister(NoticeCompositor noticeCompositor)
        {
            if (noticeCompositor.TryGetComponent(out noticeEventExecutor))
            {
                noticeEventExecutor.RegisterNoticeEvent(OnNoticeReceived);
            }
        }

        public void EventExecutorUnRegister(NoticeCompositor noticeCompositor)
        {
            if (noticeCompositor.TryGetComponent(out noticeEventExecutor))
            {
                noticeEventExecutor.UnRegisterNoticeEvent(OnNoticeReceived);
            }
        }
    }

    public enum NoticeType
    {
        None = -1,
        Notice,
        Caution,
        Warning,
        Story,
        Tutorial
    }

    public enum NPCName
    {
        Venus,
        Grace,
        Scott
    }
}