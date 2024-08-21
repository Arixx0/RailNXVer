// ReSharper disable CheckNamespace

using Data;
using UI;

using System.Collections.Generic;
using System.Linq;
using Units.Stats;
using UnityEngine;

namespace TrainScripts
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(UnitStatComponent))]
    public partial class Train : MonoBehaviour, ISerializationCallbackReceiver, IContextMenuDataProvider
    {
        #region Fields & Properties
        
        [Header("Train Base Fields")]
        [SerializeField] private Environments.RailPath railPath;
        [SerializeField] private UnitStatComponent statComponent;
        [SerializeField] private CameraUtility.CameraController cameraController;
        [SerializeField] private HUD hud;
        [SerializeField] private ContextMenuUI contextMenu;
        [SerializeField] private Inventory inventory;
        [SerializeField] private List<Augments.Augment> augments;
        [SerializeField] private List<ContextMenuData> contextMenuData;
        [SerializeField] private bool enableInputHandling = true;
        [SerializeField] private NoticeCompositor noticeCompositor;
        [SerializeField] private AudioSource trainAudioSource;

        private Transform m_CachedTransform;
        private int m_SelectedCarIndex = -1;
        private float m_TotalConsumePowerStone;
        private TrainDefaultInputHandler m_DefaultInputHandler;

        private Transform CachedTransform => m_CachedTransform ??= GetComponent<Transform>();

        public Inventory Inventory => inventory;
        
        public UnitStatComponent StatComponent => statComponent;

        public HUD HUD => hud;
        
        public int SelectedCarIndex => m_SelectedCarIndex;

        public float TotalConsumePowerStone => m_TotalConsumePowerStone;

        #endregion // Fields & Properties
        
        #region Event Delegates
        
        public event OnTrainStateChangedCallback OnTrainStateChanged;
        
        public event OnInventoryChangedCallback OnInventoryChanged;
        
        #endregion // Event Delegates

        #region MonoBehaviour Events

        private void Awake()
        {
            StatComponent.OnChangedStat += ChangedCarSafety;
            
            reorderOverlayUI.gameObject.SetActive(false);
        }

        private void Update()
        {
            // cameraController.SetZoomVelocity(inputComponent.ZoomInputAlpha);
            // cameraController.SetRotateVelocity(inputComponent.CameraRotateInputDelta);
            
            Move();
        }

        [ContextMenu("Force Validate")]
        private void OnValidate()
        {
            for (var i = cars.Count - 1; i >= 0; i -= 1)
            {
                if (cars[i] == null)
                {
                    cars.RemoveAt(i);
                }
            }
        }
        
        #endregion
        
        #region Train Management

        public void Setup()
        {
            if (carPrefabsForInitialSpawn.Count != 0)
            {
                SpawnCars(carPrefabsForInitialSpawn);
            }
            
            if (statComponent == null && !TryGetComponent(out statComponent))
            {
                Debug.LogError($"{nameof(statComponent)} is not assigned to the train.", gameObject);
            }
            else // statComponent != null (already assigned from inspector)
            {
                statComponent.Setup();
            }
            
            if (cameraController == null)
            {
                cameraController = FindObjectOfType<CameraUtility.CameraController>();
                if (cameraController == null)
                {
                    Debug.LogError($"{nameof(CameraUtility.CameraController)} does not exists in the scene.", gameObject);
                }
                else
                {
                    SetSelectedCar(cars.Count == 0 ? -1 : 0);
                }
            }
            else // cameraController != null (already assigned from inspector)
            {
                cameraController.FollowTarget = CachedTransform;
            }

            if (hud == null)
            {
                hud = FindObjectOfType<HUD>();
                if (hud == null)
                {
                    Debug.LogError($"{nameof(HUD)} does not exists in the scene.", gameObject);
                }
                else
                {
                    hud.TargetTrain = this;
                }
            }
            else // hud != null (already assigned from inspector)
            {
                hud.TargetTrain = this;
            }

            enableInputHandling = true;
            
            m_DefaultInputHandler = new TrainDefaultInputHandler(this);
            InputComponents.InputComponent.Get.RegisterHandler(m_DefaultInputHandler);
            
            UpdateTrainStats();
            
            // Try cache the rail path in current scene.
            railPath = FindObjectOfType<Environments.RailPath>();
            Debug.Assert(railPath != null, $"{nameof(railPath)} is not found.");
            m_TravelDistanceOnPath = AlignTrainFromDistanceOnPath(10, true);
            foreach (var car in cars)
            {
                m_TotalConsumePowerStone += car.StatComponent.FuelEfficiency;
            }
            CheckTrainElectricPower();
            noticeCompositor.RegisterNoticeEvent();
            VFXAndSFXLoad();
        }

        private void VFXAndSFXLoad()
        {
            LoadVFX(ref m_MoveVFX, "Move");
            LoadSFX(ref m_OverLoadSFX, "OverLoad");
            LoadSFX(ref m_RestoreSFX, "Restore");
        }

        private void LoadSFX(ref AudioClip sfxField, string identifierSuffix)
        {
            if (sfxField == null)
            {
                var identifier = $"{Database.SFXCommonCategory}_Train_{identifierSuffix}";
                if (Utility.DatabaseUtility.TryGetData(Database.SFXSettingsData, identifier, out var SFXData))
                {
                    sfxField = SFXData.sfxSource;
                }
            }
        }

        private void LoadVFX(ref VFX vfxField, string  identifierSuffix)
        {
            if (vfxField == null)
            {
                var identifier = $"{Database.VFXCommonCategory}_Train_{identifierSuffix}";
                if (Utility.DatabaseUtility.TryGetData(Database.VFXSettingsData, identifier, out var VFXData))
                {
                    vfxField = Instantiate(VFXData.vfxSource, CachedTransform);
                }
            }
        }

        public void SetSelectedCar(int index)
        {
            if (m_SelectedCarIndex != -1 &&
                m_SelectedCarIndex < cars.Count &&
                cars[m_SelectedCarIndex] != null)
            {
                cars[m_SelectedCarIndex].SetSelectedStatus(false);
            }
            
            m_SelectedCarIndex = index;

            if (index == -1)
            {
                cameraController.FollowTarget = CachedTransform;
                return;
            }

            var target = cars[index];
            target.SetSelectedStatus(true);
            cameraController.FollowTarget = cars[index].CachedTransform;
        }

        public void AddAugment(Augments.Augment augment)
        {
            if (augments.Exists(e => string.CompareOrdinal(e.Guid, augment.Guid) == 0))
            {
                return;
            }
            
            augments.Add(augment);
            statComponent.AddModifier(augment, new List<int>(16), augment.ApplyToChainedTargets);
        }

        public void RandomCarCircuitFailure()
        {
            List<Car> notCircuitFailureCars = cars
                        .Where(car => !(car is EngineCompartment) && !car.CircuitFailure)
                        .ToList();

            if (notCircuitFailureCars.Count > 0)
            {
                Car randomCar = notCircuitFailureCars[UnityEngine.Random.Range(0, notCircuitFailureCars.Count)];
                randomCar.CircuitFailure = true;
            }
        }

        public void PowerStoneConsume()
        {
            m_TotalConsumePowerStone = 0f;

            foreach (var car in cars)
            {
                m_TotalConsumePowerStone += car.StatComponent.FuelEfficiency;
            }

            if (!TryConsumeResourcesFromInventory(ResourceType.PowerStone, m_TotalConsumePowerStone))
            {
                inventory[ResourceType.PowerStone] = 0f;
                OnInventoryChanged?.Invoke(inventory);
                statComponent.StatusEffectManager.AddStatusEffect(Instantiate(notEnoughPowerstoneDebuff), false);
            }
            else
            {
                statComponent.StatusEffectManager.RemoveStatusEffect(notEnoughPowerstoneDebuff);
            }
        }
        
        private void ChangedCarSafety()
        {
            if (Manager.GameModeManager.Get != null)
            {
                Manager.GameModeManager.Get.SetCircuitFailureTime();
            }

            foreach (var car in cars)
            {
                car.CheckCircuitFailure();
            }
        }

        private void OnMiningTargetSelected(List<Environments.ResourceVein> veins)
        {
            Debug.Log($"{nameof(Train.OnMiningTargetSelected)}: Mining targets selected(count: {veins?.Count})");
            if (veins == null || veins.Count == 0)
            {
                return;
            }

            canMove = false;
            foreach (var car in cars)
            {
                if (car is not ScrapperCompartment compartment)
                {
                    continue;
                }
                
                compartment.OnMiningTargetSelected(veins);
            }
        }

        public void RepairAllCars(float recoveryRatio)
        {
            foreach (var car in cars)
            {
                car.RepairDurabilityByRatio(recoveryRatio);
            }
        }

        #endregion // Train Management
        
        #region Inventory Utility

        private bool CheckIfPlayerCanAffordCarCost(CarCostDataTuple cost)
        {
            return inventory[ResourceType.Iron] >= cost.ResourceSteel &&
                   inventory[ResourceType.Titanium] >= cost.ResourceTitanium &&
                   inventory[ResourceType.Mythrill] >= cost.ResourceMithryl &&
                   inventory[ResourceType.Adamantine] >= cost.ResourceAdamantium &&
                   inventory[ResourceType.Circuit] >= cost.CircuitNormal;
        }
        
        private void ConsumeResourcesFromInventory(CarCostDataTuple costData)
        {
            inventory.ConsumeResourceOfType(ResourceType.Iron, costData.ResourceSteel);
            inventory.ConsumeResourceOfType(ResourceType.Titanium, costData.ResourceTitanium);
            inventory.ConsumeResourceOfType(ResourceType.Mythrill, costData.ResourceMithryl);
            inventory.ConsumeResourceOfType(ResourceType.Adamantine, costData.ResourceAdamantium);
            inventory.ConsumeResourceOfType(ResourceType.Circuit, costData.CircuitNormal);
            
            OnInventoryChanged?.Invoke(inventory);
        }
        
        public bool TryConsumeResourcesFromInventory(CarCostDataTuple costData)
        {
            if (!CheckIfPlayerCanAffordCarCost(costData))
            {
                return false;
            }

            ConsumeResourcesFromInventory(costData);
            return true;
        }
        
        public bool TryConsumeResourcesFromInventory(ResourceType resourceType, float amount)
        {
            if (inventory[resourceType] < Mathf.Abs(amount))
            {
                return false;
            }

            inventory.ConsumeResourceOfType(resourceType, amount);
            OnInventoryChanged?.Invoke(inventory);
            return true;
        }
        
        public void ReturnResourcesToInventory(CarCostDataTuple costData)
        {
            inventory.AddResourceOfType(ResourceType.Iron, costData.ResourceSteel);
            inventory.AddResourceOfType(ResourceType.Titanium, costData.ResourceTitanium);
            inventory.AddResourceOfType(ResourceType.Mythrill, costData.ResourceMithryl);
            inventory.AddResourceOfType(ResourceType.Adamantine, costData.ResourceAdamantium);
            inventory.AddResourceOfType(ResourceType.Circuit, costData.CircuitNormal);
            
            OnInventoryChanged?.Invoke(inventory);
        }
        
        public void AddResourceToInventory(ResourceType resourceType, int amount)
        {
            inventory.AddResourceOfType(resourceType, amount);
            OnInventoryChanged?.Invoke(inventory);
        }

        public void ConsumeResourceToInventory(ResourceType resourceType, float amount)
        {
            inventory.ConsumeResourceOfType(resourceType, amount);
            OnInventoryChanged?.Invoke(inventory);
        }

        public bool CheckAvailabilityResource(ResourceType resourceType, float amount)
        {
            return inventory[resourceType] >= Mathf.Abs(amount);
        }
        
        #endregion // Inventory Utility
        
        #region ContextMenu Actions

        public void ToggleMovement()
        {
            canMove = !canMove;

            foreach (var car in cars)
            {
                if (car is ScrapperCompartment compartment)
                {
                    compartment.DisableScrapMode();
                }
            }
        }

        public void EnableMiningMode()
        {
            var miningTargetSelector = FindObjectOfType<MiningTargetSelector>(true);
            if (miningTargetSelector == null)
            {
                return;
            }
            
            miningTargetSelector.OnMiningTargetSelectedCallback = OnMiningTargetSelected;
            miningTargetSelector.Activate();
        }

        public void ConstructCar()
        {
            if (!TryGetComponent(out CompartmentConstructionHandler constructionHandler))
            {
                Debug.LogWarning($"{nameof(CompartmentConstructionHandler)} is not attached to the train.");
                return;
            }
            
            constructionHandler.Activate();
        }
        
        #endregion
        
        #region ISerializationCallbackReceiver
        
        public void OnBeforeSerialize()
        {
            if (Application.isPlaying)
            {
                return;
            }
            
            augments?.Clear();
        }

        public void OnAfterDeserialize()
        {
        }
        
        #endregion // ISerializationCallbackReceiver
        
        #region IContextMenuDataProvider
        
        public List<ContextMenuData> GetContextMenuData()
        {
            // TODO: List<T> instantiation is not good for memory.
            if (m_SelectedCarIndex >= 0)
            {
                var combinedMenuData = new List<ContextMenuData>(contextMenuData);
                combinedMenuData.AddRange(cars[m_SelectedCarIndex].GetContextMenuData());
                return combinedMenuData;
            }
            
            return contextMenuData;
        }

        public ContextMenuCondition GetContextMenuConditionState()
        {
            var state = ContextMenuCondition.None;
            
            if (m_SelectedCarIndex >= 0)
            {
                state |= cars[m_SelectedCarIndex].GetContextMenuConditionState();
            }
            
            return state;
        }

        public void OnContextMenuClosed()
        {
        }

        public Dictionary<ResourceType, float> GetResourceData(ContextMenuCondition condition)
        {
            if (m_SelectedCarIndex >= 0)
            {
                return cars[m_SelectedCarIndex].GetResourceData(condition);
            }
            var dic = new Dictionary<ResourceType, float>();
            
            return dic;
        }

        #endregion // IContextMenuDataProvider

        #region Delegates

        public delegate void OnTrainStateChangedCallback(Train train);
        
        public delegate void OnInventoryChangedCallback(Inventory inventory);
        
        #endregion // Delegates
    }
}