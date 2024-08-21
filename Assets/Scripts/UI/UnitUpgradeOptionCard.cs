using Data;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace UI
{
    public class UnitUpgradeOptionCard : Button
    {
        [SerializeField] private TextMeshProUGUI upgradeName;
        [SerializeField] private TextMeshProUGUI upgradeDescription;
        [SerializeField] private Transform resourcePanel;
        [SerializeField] private ContextMenuResourceItem resourceItemPrefab;

        private Transform m_CachedTransform;
        private readonly List<ContextMenuResourceItem> m_ActiveContextMenuResourceItems = new(4);
        private ObjectPool<ContextMenuResourceItem> m_ContextMenuResourceItemPool;

        public Transform CachedTransform => m_CachedTransform ??= transform;

        public RectTransform RectTransform => (RectTransform)CachedTransform;

        protected override void Awake()
        {
            m_ContextMenuResourceItemPool = ObjectPool<ContextMenuResourceItem>.CreateObjectPool(resourceItemPrefab, transform);
        }

        public void SetData(Dictionary<ResourceType, float> resourceData, IUnitUpgradeSelectCallbackReceiver callbackReceiver)
        {
            if (resourceData != null && resourceData.Count > 0)
            {
                m_ContextMenuResourceItemPool.ReturnObjects(m_ActiveContextMenuResourceItems);
                m_ActiveContextMenuResourceItems.Clear();

                foreach (var resource in resourceData)
                {
                    var resourceItem = m_ContextMenuResourceItemPool.GetOrCreate();
                    resourceItem.CachedTransform.SetParent(resourcePanel);
                    var availabilityResource = callbackReceiver.CheckAvailabilityResource(resource.Key, resource.Value);
                    resourceItem.ChangeResourceItem(resource.Key.ToString(), resource.Value, availabilityResource);
                    m_ActiveContextMenuResourceItems.Add(resourceItem);
                }
            }
        }

        public string UpgradeName
        {
            get => upgradeName.text;
            set => upgradeName.text = value;
        }
        
        public string UpgradeDescription
        {
            get => upgradeDescription.text;
            set => upgradeDescription.text = value;
        }
    }
}