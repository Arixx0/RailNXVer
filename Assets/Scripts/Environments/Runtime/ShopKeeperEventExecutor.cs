using Data;
using TrainScripts;
using UI;
using UnityEngine;

namespace Environments
{
    public class ShopKeeperEventExecutor : MonoBehaviour
    {
        private StageSectorEventTrigger m_Trigger;

        private ShopKeeperItemsDefinition m_ShopKeeperItemsDefinition;

        private void OnTriggerPerformed()
        {
            if (m_ShopKeeperItemsDefinition == null)
            {
                Debug.LogError($"{nameof(m_ShopKeeperItemsDefinition)} is not set.");
                return;
            }

            if (m_Trigger.TriggeringCollider.TryGetComponent(out Train train))
            {
            }
            else if (m_Trigger.TriggeringCollider.TryGetComponent(out Car car))
            {
                train = car.ParentTrain;
            }
            else
            {
                Debug.LogError("Triggering collider is not a train or a car.");
                return;
            }
            
            var shopKeeperUI = FindObjectOfType<ShopUI>();
            Debug.Assert(shopKeeperUI != null, $"{nameof(ShopUI)} is not found in the scene.");

            if (train != null && shopKeeperUI != null)
            {
                train.SetMovementState(false);
                shopKeeperUI.AssignProperties(m_ShopKeeperItemsDefinition, OnShopKeeperUIClosed);
                shopKeeperUI.Show(m_ShopKeeperItemsDefinition, OnShopKeeperUIClosed);
            }
        }

        private void OnShopKeeperUIClosed()
        {
            if (m_Trigger.TriggeringCollider.TryGetComponent(out Train train)) { }
            else if (m_Trigger.TriggeringCollider.TryGetComponent(out Car car))
            {
                train = car.ParentTrain;
            }
            else
            {
                Debug.LogError("Triggering collider is not a train or a car.");
                return;
            }
            
            train.SetMovementState(true);
        }

        public static void CreateFromTrigger(StageSectorEventTrigger trigger, ShopKeeperItemsDefinition definition)
        {
            var executor = trigger.gameObject.AddComponent<ShopKeeperEventExecutor>();
            executor.m_Trigger = trigger;
            executor.m_Trigger.onEnterTrigger += executor.OnTriggerPerformed;
            executor.m_ShopKeeperItemsDefinition = definition;
        }
    }
}