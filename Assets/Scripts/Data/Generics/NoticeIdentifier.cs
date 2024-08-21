using UnityEngine;

namespace Data
{
    [System.Serializable]
    public class NoticeIdentifier : BaseIdentifier
    {
        private string m_NPCName;

        public string NotifierName => $"{category}_Name_{m_NPCName}";

        public UI.NoticeType GetNoticeType()
        {
            return System.Enum.TryParse<UI.NoticeType>(objectType, out var result) ? result : UI.NoticeType.None;
        }

    public void Set(string objectType, string subType, string notifier = "")
        {
            category = "Message";
            this.objectType = objectType;
            this.subType = subType;
            m_NPCName = notifier;
        }
    }
}