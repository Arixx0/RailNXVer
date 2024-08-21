using System;
using System.Collections.Generic;
using UnityEngine;

using Data;

namespace TrainScripts
{
    [Serializable]
    public class Inventory : Dictionary<ResourceType, float>, ISerializationCallbackReceiver
    {
        [SerializeField, HideInInspector]
        private List<ResourceType> resourceTypes;
        
        [SerializeField, HideInInspector]
        private List<int> resourceAmount;
        
        public Inventory()
        {
            foreach (var type in Enum.GetValues(typeof(ResourceType)))
            {
                Add((ResourceType)type, 0);
            }
        }

        public void ConsumeResourceOfType(ResourceType type, float amount)
        {
            var destAmount = this[type] - amount;

            if (destAmount < 0)
            {
                throw new InvalidOperationException($"Cannot consume {amount} of {type} from inventory. This operation is not allowed.");
            }
            
            this[type] = destAmount;
        }
        
        public void AddResourceOfType(ResourceType type, int amount)
        {
            this[type] += amount;
        }

        public float GetResourceOfType(ResourceType type)
        {
            return this[type];
        }

        public void OnBeforeSerialize()
        {
            resourceTypes = new List<ResourceType>(Count);
            resourceAmount = new List<int>(Count);
            foreach (var pair in this)
            {
                if (Enum.IsDefined(typeof(ResourceType), pair.Key))
                {
                    resourceTypes.Add(pair.Key);
                    resourceAmount.Add(Convert.ToInt32(pair.Value));
                }
            }
        }

        public void OnAfterDeserialize()
        {
            for (var i = 0; i < resourceTypes.Count; i++)
            {
                if (TryAdd(resourceTypes[i], resourceAmount[i]))
                {
                    continue;
                }

                this[resourceTypes[i]] = resourceAmount[i];
            }
        }
    }

    [Serializable]
    public class InventoryItem
    {
        public string identifier;
    }
}