using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Data
{
    [Serializable]
    public class ShopKeeperItemContext
    {
        public string identifier;

        public float localProbability;
        
        [NonSerialized]
        public int stockedAmount;

        public override string ToString()
        {
            return $"identifier: {identifier}, probability: {localProbability}, stockedAmount: {stockedAmount}";
        }
    }

    [Serializable]
    public class ShopKeeperSlotProperty
    {
        public ShopKeeperItemContext[] itemsContext;
    }
    
    [CreateAssetMenu(menuName = "TrainCraft/ShopKeeperItemsDefinition")]
    public class ShopKeeperItemsDefinition : ScriptableObject
    {
        [SerializeField]
        private List<ShopKeeperSlotProperty> slotProperties = new();

        private void OnValidate()
        {
            for (var i = 0; i < slotProperties.Count; i++)
            {
                var slotProperty = slotProperties[i];
                var totalPossibility = slotProperty.itemsContext.Sum(item => item.localProbability);

                Debug.Assert(Mathf.Approximately(totalPossibility, 1f), $"Total possibility is not 1 in slot {i}.", this);
            }
        }

        public List<ShopKeeperItemContext> GetItems()
        {
            var rand = new System.Random();
            var items = new List<ShopKeeperItemContext>(slotProperties.Count);

            for (var i = 0; i < slotProperties.Count; i += 1)
            {
                items.Add(GetItem(ref slotProperties[i].itemsContext, (float)rand.NextDouble()));
            }

            return items;
        }

        private ShopKeeperItemContext GetItem(ref ShopKeeperItemContext[] context, float rand)
        {
            var accumulatedProbability = 0f;

            foreach (var t in context)
            {
                accumulatedProbability += t.localProbability;

                if (rand < accumulatedProbability)
                {
                    t.stockedAmount = Database.ShopItemsData[t.identifier].PurchaseCountPerEvent;
                    return t;
                }
            }

            context[0].stockedAmount = Database.ShopItemsData[context[0].identifier].PurchaseCountPerEvent;
            return context[0];
        }

#if UNITY_EDITOR
        public void AddItem(ShopItemData data)
        {
            /*
            if (data == null)
            {
                return;
            }

            var context = new ShopKeeperItemContext()
            {
                identifier = data.Identifier,
                localProbability = data.DropPossibility,
            };
            
            firstSlotItemContext.Add(context);
            
            // update local probability to make sure it's 1.
            var totalProbability = 1f;
            var unassignedProbabilityCount = 0;
            for (var i = 0; i < firstSlotItemContext.Count; i += 1)
            {
                var targetItem = Database.ShopItemsData[firstSlotItemContext[i].identifier];
                
                if (targetItem.DropPossibility > 0f)
                {
                    totalProbability -= targetItem.DropPossibility;
                    continue;
                }

                unassignedProbabilityCount += 1;
                firstSlotItemContext[i].localProbability = 0f;
            }

            // if the element's probability is 0, skip and set it later.
            var extrasProbability = totalProbability / (float)unassignedProbabilityCount;
            for (var i = 0; i < firstSlotItemContext.Count; i += 1)
            {
                if (firstSlotItemContext[i].localProbability > 0f)
                {
                    continue;
                }

                firstSlotItemContext[i].localProbability = extrasProbability;
            }
            */
        }
#endif
    }
}