using Data;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace UI
{
    public interface IContextMenuDataProvider
    {
        public List<ContextMenuData> GetContextMenuData();
        
        public ContextMenuCondition GetContextMenuConditionState();
        
        public void OnContextMenuClosed();

        public Dictionary<ResourceType, float> GetResourceData(ContextMenuCondition condition);
    }


    [System.Serializable]
    public class ContextMenuData
    {
        public string itemName;
        public TextIdentifier identifier;
        public Sprite itemIcon;
        public ContextMenuCondition exposeCondition;
        public UnityEvent onClickEvent = new();

        public ContextMenuData()
        {
        }

        public ContextMenuData(string itemName, UnityAction onClick, ContextMenuCondition condition = ContextMenuCondition.None)
        {
            this.itemName = itemName;
            exposeCondition = condition;
            
#if UNITY_EDITOR
            if (Application.isEditor && !Application.isPlaying)
            {
                UnityEditor.Events.UnityEventTools.AddPersistentListener(onClickEvent, onClick);
                return;
            }
#endif

            onClickEvent.AddListener(onClick);
        }
    }

    [System.Flags]
    public enum ContextMenuCondition
    {
        None = 0,
        OnCompartmentDamaged = 0b_0000_0001,
        OnCompartmentOperating = 0b_0000_0010,
        OnCompartmentCircuitFailed = 0b_0000_0100,
        OnCompartmentDestroyed = 0b_0000_1000,
    }
}