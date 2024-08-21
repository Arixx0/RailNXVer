using Data;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace UI
{
    public class ContextMenuDetailItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI detailName;
        [SerializeField] private TextMeshProUGUI detailDescription;
        [SerializeField] private ContextMenuResourceItem contextMenuResourceItemPrefab;
        [SerializeField] private Transform contextMenuResourcePanel;

        private readonly List<ContextMenuResourceItem> m_ActiveContextMenuResourceItems = new(4);
        private ObjectPool<ContextMenuResourceItem> m_ContextMenuResourceItemPool;

        public string DetailName
        {
            get => detailName.text;
            set => detailName.text = value;
        }

        public string DetailDescription
        {
            get => detailDescription.text;
            set => detailDescription.text = value;
        }

        private void Awake()
        {
            m_ContextMenuResourceItemPool = ObjectPool<ContextMenuResourceItem>.CreateObjectPool(contextMenuResourceItemPrefab, transform);
        }

        public void SetResource(Dictionary<ResourceType, float> dictionary)
        {
            if (dictionary == null)
            {
                return;
            }
            m_ContextMenuResourceItemPool.ReturnObjects(m_ActiveContextMenuResourceItems);
            m_ActiveContextMenuResourceItems.Clear();
            contextMenuResourcePanel.gameObject.SetActive(dictionary.Count > 0);

            foreach (var resource in dictionary)
            {
                var resourceItem = m_ContextMenuResourceItemPool.GetOrCreate();
                resourceItem.CachedTransform.SetParent(contextMenuResourcePanel);
                resourceItem.ChangeResourceItem(resource.Key.ToString(), resource.Value);
                m_ActiveContextMenuResourceItems.Add(resourceItem);
            }
        }
    }
}