using System.Collections.Generic;
using Attributes;
using Environments;
using UI;
using UnityEngine;

namespace TrainScripts
{
    [DisallowMultipleComponent]
    public class ScrapperCompartment : Car
    {
        [Header("Scrapper Compartment Settings")]
        [SerializeField] private Units.Drones.ScrapperDrone scrapperDronePrefab;
        [SerializeField, Disabled] private List<Units.Drones.ScrapperDrone> scrapperDrones = new(4);
        [SerializeField] private int droneMaxStockQuantity = 4;
        [SerializeField] private float droneCatapultInterval = 1f;
        
        [Space]
        [SerializeField] protected AudioSource droneCatapultingSFXAudioSource;
        [SerializeField] protected AudioSource resourceDepositSFXAudioSource;
        [SerializeField] protected AudioSource droneHangarSFXAudioSource;
        
        protected List<ResourceVein> m_CurrentScrapTargets = new(32);
        private Utility.ObjectPool<Units.Drones.ScrapperDrone> m_ScrapperDronePool;
        private float m_CurrentDroneProduceElapsedTime;
        private float m_LastDroneCatapultedTime;
        
        public bool IsScrapModeEnabled { get; set; }

        private void Awake()
        {
            m_ScrapperDronePool = Utility.ObjectPool<Units.Drones.ScrapperDrone>.CreateObjectPool(scrapperDronePrefab, CachedTransform);
        }

        protected override void Update()
        {
            base.Update();
            
            ProduceScrapperDrone();
            CatapultDrone();
        }

        protected override void CarCircuitFailure()
        {
            base.CarCircuitFailure();
            DisableScrapMode();
        }

        private void ProduceScrapperDrone()
        {
            if (scrapperDrones.Count >= droneMaxStockQuantity)
            {
                return;
            }
            
            m_CurrentDroneProduceElapsedTime += TimeScaleManager.Get.GetDeltaTimeOfTag(tag);
            if (m_CurrentDroneProduceElapsedTime < scrapperDronePrefab.ForTimeForProduction)
            {
                return;
            }
            
            m_CurrentDroneProduceElapsedTime = 0f;

            var drone = m_ScrapperDronePool.GetOrCreate();
            drone.ParentCompartment = this;
            
            scrapperDrones.Add(drone);
            statComponent.AddChainedStatComponent(drone.StatComponent);

            if (IsScrapModeEnabled)
            {
                CatapultDrone();
            }
            else
            {
                StoreDrone(drone);
            }
        }

        public void OnMiningTargetSelected(List<ResourceVein> veins)
        {
            if (veins == null || veins.Count == 0)
            {
                return;
            }
            
            parentTrain.SetMovementState(false);
            
            m_CurrentScrapTargets.Clear();
            m_CurrentScrapTargets.AddRange(veins);

            IsScrapModeEnabled = true;
        }

        public bool TryGetResourceVein(out ResourceVein scrapTarget)
        {
            for (var i = 0; i < m_CurrentScrapTargets.Count;)
            {
                if (m_CurrentScrapTargets[i].IsValidToScrap)
                {
                    i++;
                    continue;
                }
                
                m_CurrentScrapTargets.RemoveAt(i);
            }
            
            if (m_CurrentScrapTargets.Count > 0)
            {
                scrapTarget = m_CurrentScrapTargets[0];
                return true;
            }
            
            scrapTarget = null;
            IsScrapModeEnabled = false;
            return false;
        }

        public void DisableScrapMode()
        {
            IsScrapModeEnabled = false;

            foreach (var drone in scrapperDrones)
            {
                if (!drone.gameObject.activeSelf)
                {
                    continue;
                }

                drone.CurrentScrapTargetVein = null;
                drone.ResetBehaviourState();
            }
        }

        private void CommandDroneToScrap(Units.Drones.ScrapperDrone drone)
        {
            if (!TryGetResourceVein(out var scrapTarget))
            {
                StoreDrone(drone);
                return;
            }

            if (!drone.gameObject.activeSelf)
            {
                drone.gameObject.SetActive(true);
                drone.CachedTransform.SetParent(null);
                drone.CachedTransform.SetPositionAndRotation(CachedTransform.position, Quaternion.identity);
            }
            
            drone.CurrentScrapTargetVein = scrapTarget;
            drone.ResetBehaviourState();
        }

        public void DepositScrappedResource(Data.ResourceType resourceType, int amount)
        {
            parentTrain.AddResourceToInventory(resourceType, amount);

            if (!resourceDepositSFXAudioSource.isPlaying)
            {
                resourceDepositSFXAudioSource.PlayOneShot(resourceDepositSFXAudioSource.clip);
            }
        }

        public void StoreDrone(Units.Drones.ScrapperDrone drone, bool playSFX = true)
        {
            drone.gameObject.SetActive(false);
            drone.CachedTransform.SetParent(CachedTransform);

            if (playSFX)
            {
                if (droneHangarSFXAudioSource.isPlaying)
                {
                    droneHangarSFXAudioSource.Stop();
                }
                
                droneHangarSFXAudioSource.PlayOneShot(droneHangarSFXAudioSource.clip);
            }
        }
        
        public void NotifyDroneIsDestroyed(Units.Drones.ScrapperDrone drone)
        {
            if (scrapperDrones.Contains(drone))
            {
                scrapperDrones.Remove(drone);
                m_ScrapperDronePool.ReturnObject(drone);
                
                statComponent.RemoveChainedStatComponent(drone.StatComponent);
            }
        }

        private void CatapultDrone()
        {
            if (!IsScrapModeEnabled)
            {
                return;
            }
            
            var nextCatapultTime = m_LastDroneCatapultedTime + droneCatapultInterval;
            if (nextCatapultTime > Time.unscaledTime)
            {
                return;
            }

            Units.Drones.ScrapperDrone catapultTarget = null;
            foreach (var drone in scrapperDrones)
            {
                if (drone.IsActing)
                {
                    continue;
                }
                
                catapultTarget = drone;
                break;
            }

            if (catapultTarget == null)
            {
                return;
            }
            
            m_LastDroneCatapultedTime = Time.unscaledTime;
            
            droneCatapultingSFXAudioSource.PlayOneShot(droneCatapultingSFXAudioSource.clip);
            CommandDroneToScrap(catapultTarget);
        }
        
        #region ContextMenu Actions
        
#if UNITY_EDITOR

        protected override void SetDefaultContextMenuData()
        {
            base.SetDefaultContextMenuData();
            
            contextMenuData.Add(new UI.ContextMenuData("Select Scrap Target", ShowMiningTargetSelector));
            
            UnityEditor.EditorUtility.SetDirty(this);
        }

#endif

        public void ShowMiningTargetSelector()
        {
            var miningTargetSelector = FindObjectOfType<MiningTargetSelector>();
            if (miningTargetSelector == null)
            {
                return;
            }
            
            miningTargetSelector.OnMiningTargetSelectedCallback = OnMiningTargetSelected;
            miningTargetSelector.Activate();
        }

        #endregion
    }
}