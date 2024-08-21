namespace UI
{
    public interface INoticeEventExecutor
    {
        public void RegisterNoticeEvent(OnNoticeEventDelegate onNoticeEvent);

        public void UnRegisterNoticeEvent(OnNoticeEventDelegate onNoticeEvent);

        public delegate void OnNoticeEventDelegate(Data.NoticeIdentifier identifier, float value = -1);

        public Data.NoticeIdentifier NoticeIdentifier { get; set; }

        public void SetNoticeIdentifier(NoticeType noticeType, string subType, string notifier = "");
    }
}