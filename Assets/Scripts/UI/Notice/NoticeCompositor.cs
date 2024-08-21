using TrainScripts;
using UnityEngine;

namespace UI
{
    public class NoticeCompositor : MonoBehaviour, INoticeEventExecutor
    {
        private HUD m_HUD;

        private event INoticeEventExecutor.OnNoticeEventDelegate OnNoticeEvent;

        public Data.NoticeIdentifier NoticeIdentifier { get; set; }

        private void Awake()
        {
            FindHUD();
        }

        private void FindHUD()
        {
            if (m_HUD == null)
            {
                m_HUD = FindObjectOfType<HUD>();
            }
        }

        public void RegisterNoticeEvent()
        {
            FindHUD();
            m_HUD?.EventExecutorRegister(this);
        }

        public void UnRegisterNoticeEvent()
        {
            FindHUD();
            m_HUD?.EventExecutorUnRegister(this);
        }

        public void InvokeNoticeEvent(NoticeType noticeType, string subType, string notifier = "")
        {
            SetNoticeIdentifier(noticeType, subType, notifier);
            OnNoticeEvent?.Invoke(NoticeIdentifier);
        }

        #region INoticeEventExecutor

        public void RegisterNoticeEvent(INoticeEventExecutor.OnNoticeEventDelegate onNoticeEvent)
        {
            OnNoticeEvent += onNoticeEvent;
        }

        public void UnRegisterNoticeEvent(INoticeEventExecutor.OnNoticeEventDelegate onNoticeEvent)
        {
            OnNoticeEvent -= onNoticeEvent;
        }

        public void SetNoticeIdentifier(NoticeType noticeType, string subType, string notifier = "")
        {
            NoticeIdentifier ??= new Data.NoticeIdentifier();
            NoticeIdentifier.Set(noticeType.ToString(), subType, notifier);
        }

        #endregion
    }
}