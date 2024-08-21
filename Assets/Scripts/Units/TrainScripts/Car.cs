// ReSharper disable CheckNamespace

using UI;
using Attributes;
using Data;
using Units.Stats;
using Utility;

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// NOTE:
//  `healthBar` field can be null. to prevent fake null state error, assign null manually from `Start()`.
//  There is null checking code for healthBar with `?.` operator.

// TODO: Extract Repairment logics to separated component class.

// TODO: Extract Demolish logics to separated component class.

namespace TrainScripts
{
    [DisallowMultipleComponent]
    public partial class Car : MonoBehaviour, IUnitUpgradeSelectCallbackReceiver, Units.ICombatEventReceiver,
        Units.IDestroyedEventExecutor, IContextMenuDataProvider
    {
        #region Fields & Properties

        [Header("Component References")]
        [SerializeField, Disabled] protected Train parentTrain;
        [SerializeField] protected UnitStatComponent statComponent;
        [SerializeField] protected Units.UnitDestructionCompositor destructionCompositor;

        [Header("Car UI References")]
        [SerializeField] protected UnitHealthBar healthBar;
        [SerializeField] protected Canvas selectedIndicatorCanvas;
        [SerializeField] protected ChangingImageLoader taskProgressBarIconLoader;
        [SerializeField] protected Slider taskProgressBar;
        [SerializeField] protected CarDetailStatEffectIcon carDetailStatEffectIcon;
        [SerializeField] protected CarDetailElectricIcon carDetailElectricIcon;
        [SerializeField] protected CarDetailElectricInformation carDetailElectricInformation;
        [SerializeField] protected CarDetailPowerStone carDetailPowerStone;
        [SerializeField] protected CarCircuitRepairItem carCircuitRepairItem;
        [SerializeField] protected CarElectricPowerChangeItem carElectricPowerChangeItem;
        [SerializeField] protected NoticeCompositor noticeCompositor;

        [Header("Context Menu Definitions")]
        [SerializeField] protected List<ContextMenuData> contextMenuData = new(8);

        [Header("Unit Identifier and Name")]
        [SerializeField] protected UnitIdentifier unitIdentifier;
        [SerializeField] protected float unitLength = 8f;

        private Transform m_CachedTransform;
        private UnitUpgradeHistory m_UnitUpgradeHistory = new(6);
        private CarDelayedOperation m_DelayedOperation;

        protected bool isCarDestroyed;

        public Train ParentTrain { get => parentTrain; private set => parentTrain = value; }

        public UnitStatComponent StatComponent => statComponent;

        public Transform CachedTransform => m_CachedTransform ??= GetComponent<Transform>();

        public UnitIdentifier IdentifierData => unitIdentifier;

        public float UnitLength => unitLength;

        public float UnitLengthHalf => unitLength * 0.5f;

        private Action OnElectricIconMouseEnter;

        private Action OnElectricIconMouseExit;

        private event Units.IDestroyedEventExecutor.OnDestroyedEventDelegate OnDestroyedEvent;

        #endregion

        #region MonoBehaviour Events

        protected virtual void Start()
        {
            Debug.Assert(!string.IsNullOrEmpty(unitIdentifier.Identifier));
            unitIdentifier.Set(unitIdentifier);

            // Setup HealthBar
            healthBar = healthBar == null ? null : healthBar;
            if (healthBar != null)
            {
                healthBar.SetHealthProperties(statComponent.CurrentHealth, statComponent.CurrentHealth);
                statComponent.OnHealthPointChanged += healthBar.UpdateHealthPoint;
            }

            // Setup TaskProgressBar
            taskProgressBar = taskProgressBar == null ? null : taskProgressBar;
#if UNITY_EDITOR
            if (taskProgressBar != null)
            {
                taskProgressBar.minValue = 0;
                taskProgressBar.maxValue = 1;
            }
#else // !UNITY_EDITOR
            taskProgressBar.minValue = 0;
            taskProgressBar.maxValue = 1;
#endif

            statComponent.OnChangedStat += UpdateStatComponent;
            statComponent.OnHealthPointChanged += CheckCircuitFailure;
            OnCircuitFailure += CarCircuitFailure;
            parentTrain.OnInventoryChanged += OnChangeCircuitRepairItem;
        }

        protected virtual void Update()
        {
            var deltaTime = TimeScaleManager.Get.GetDeltaTimeOfTag("Global");

            UpdateCircuitFailureTime(deltaTime);
        }

        #endregion

        public virtual void Setup(Train parent)
        {
            ParentTrain = parent;

            m_UnitUpgradeHistory.Init(unitIdentifier.Identifier);

            OnChangeElectricPower -= parentTrain.CheckTrainElectricPower; // prevent multiple subscription
            OnChangeElectricPower += parentTrain.CheckTrainElectricPower;
            if (this is EngineCompartment || this is GeneratorCompartment)
            {
                if (DatabaseUtility.TryGetData(Database.GeneratorData, IdentifierData.GetIdentifier(statComponent.UpgradeLevel), out var generatorData))
                {
                    CurrentElectricPowerGeneration = generatorData.energyGenerate;
                }
            }
            CurrentElectricPowerUsage = statComponent.energyCost.Value;
            m_CurrentElectricPowerUsageType = ElectricPowerUsageType.Standard;

            noticeCompositor.RegisterNoticeEvent();

            statComponent.Setup();
            statComponent.AddChainedStatComponent(ParentTrain.StatComponent);
            VFXLoad();
        }

        public void StartOperationAfterDelay(float delay, OperationType operationType, Action onTaskComplete = null)
        {
            if (m_DelayedOperation != null)
            {
                return;
            }

            CarStopWorking();

            m_DelayedOperation = new CarDelayedOperation(this, delay, operationType);

            taskProgressBar.gameObject.SetActive(true);
            taskProgressBar.value = 0f;
            taskProgressBarIconLoader.ChangeImage(operationType.ToString());

            m_DelayedOperation.OnTaskUpdate += progress =>
            {
                taskProgressBar.value = progress;
            };

            m_DelayedOperation.OnTaskComplete += () =>
            {
                CarStartWorking();
                taskProgressBar.gameObject.SetActive(false);
                onTaskComplete?.Invoke();
                noticeCompositor.InvokeNoticeEvent(NoticeType.Notice, operationType.ToString(), "Venus");
                m_DelayedOperation = null;
            };
            StartCoroutine(m_DelayedOperation);
        }

        public void CancelOperation()
        {
            if (m_DelayedOperation == null || m_DelayedOperation.OperationType == OperationType.Demolish && m_DelayedOperation.RemainingTime < 0.5f)
            {
                return;
            }
            StopCoroutine(m_DelayedOperation);
            CarStartWorking();
            taskProgressBar.gameObject.SetActive(false);
            m_DelayedOperation = null;
        }

        protected virtual void VFXLoad()
        {
            LoadVFX(ref m_OverLoadVFX, "OverLoad");
            LoadVFX(ref m_CircuitFailureVFX, "CircuitFailure");
        }

        private void LoadVFX(ref VFX vfxField, string identifierSuffix)
        {
            if (vfxField == null)
            {
                var identifier = $"{Database.VFXCommonCategory}_{IdentifierData.UnitCategory}_{StatComponent.UpgradeLevel}_{identifierSuffix}";
                if (DatabaseUtility.TryGetData(Database.VFXSettingsData, identifier, out var VFXData))
                {
                    vfxField = Instantiate(VFXData.vfxSource, CachedTransform);
                }
            }
        }

        protected virtual void UpdateStatComponent()
        {
            statComponent.Setup(true);
        }

        // Switch the car state to destroyed.
        // Note that the destroyed state doesn't mean that this car object(unity object) is destroyed.
        protected virtual void SwitchToDestroyedState()
        {
            if (isCarDestroyed)
            {
                return;
            }

            isCarDestroyed = true;

            OnDestroyedEvent?.Invoke();

            healthBar?.gameObject.SetActive(false);
            taskProgressBar?.gameObject.SetActive(false);
            destructionCompositor.DoPlayDestructionEffects();

            CarStopWorking();
            StopCircuitFailure(true);

            parentTrain.SwitchToJunkState(this);
        }

        protected virtual void DoDemolish()
        {
            if (isCarDestroyed)
            {
                var consumedResource = new CarCostDataTuple();
                foreach (var identifier in m_UnitUpgradeHistory)
                {
                    if (!Database.CarCostData.TryGetValue(identifier, out var upgradeCostData))
                    {
                        Debug.LogWarning($"Upgrade cost data for identifier {identifier} is not found.");
                        continue;
                    }

                    consumedResource += upgradeCostData;
                }

                consumedResource /= 3;
                parentTrain.ReturnResourcesToInventory(consumedResource);
            }

            SetSelectedStatus(false);

            parentTrain.RemoveCarFromManagedList(this);
            destructionCompositor.DoDestroy();

            OnDestroyedEvent?.Invoke();
        }

        /// <param name="layerIndex"> 0 : Default, 1 : PowerStone, 2 : ElectricPower, 3 : PowerStoneDetail</param>
        public void OnChangeCarDetailLayer(int layerIndex)
        {
            carDetailStatEffectIcon.gameObject.SetActive(layerIndex == 1);
            carDetailElectricIcon.gameObject.SetActive(layerIndex == 2);
            carDetailPowerStone.gameObject.SetActive(layerIndex == 3);

            if (layerIndex == 2)
            {
                OnElectricIconMouseEnter = () => carDetailElectricInformation.gameObject.SetActive(true);
                OnElectricIconMouseExit = () => carDetailElectricInformation.gameObject.SetActive(false);
            }
            else
            {
                OnElectricIconMouseEnter = null;
                OnElectricIconMouseExit = null;
                carDetailElectricInformation.gameObject.SetActive(false);
            }

            UpdateCarDetailValue();
        }

        public void UpdateCarDetailValue()
        {
            carDetailElectricIcon.ChangeCarDetailElectricIcon(this);
            carDetailElectricInformation.ChangeCarDetailElectricInformation(this);
            carDetailPowerStone.PowerStoneAmount = $"{statComponent.FuelEfficiency}/s";
        }

        public virtual void CarStopWorking()
        {
        }

        public virtual void CarStartWorking()
        {
        }

        private void OnMouseEnter()
        {
            OnElectricIconMouseEnter?.Invoke();
        }

        private void OnMouseExit()
        {
            OnElectricIconMouseExit?.Invoke();
        }

        public void SetSelectedStatus(bool status)
        {
            selectedIndicatorCanvas.gameObject.SetActive(status);
        }

        protected virtual void DoShallowCopyTo(Car replacement)
        {
            replacement.parentTrain = parentTrain;

            replacement.OnDestroyedEvent = OnDestroyedEvent;
            replacement.m_UnitUpgradeHistory = m_UnitUpgradeHistory;

            statComponent.DoShallowCopyToTarget(replacement.statComponent);
        }

        public void OnRestored()
        {
            CarStartWorking();
        }

        public void RepairDurabilityByRatio(float ratio)
        {
            var repairAmount = statComponent.MaxHealth * ratio;
            statComponent.CurrentHealth += repairAmount;
        }

        #region ContextMenu Actions

#if UNITY_EDITOR

        [ContextMenu("Set Default Context Menu Data")]
        protected virtual void SetDefaultContextMenuData()
        {
            contextMenuData.Clear();
            contextMenuData.Add(new ContextMenuData("Show Upgrades", ShowUpgrades));
            contextMenuData.Add(new ContextMenuData("Repair Car", RepairDurability, ContextMenuCondition.OnCompartmentDamaged | ContextMenuCondition.OnCompartmentCircuitFailed));
            contextMenuData.Add(new ContextMenuData("Cancel Operation", CancelOperation, ContextMenuCondition.OnCompartmentOperating));
            contextMenuData.Add(new ContextMenuData("Tear Down Car", Demolish));
            contextMenuData.Add(new ContextMenuData("Reorder Car", ReorderCar));

            UnityEditor.EditorUtility.SetDirty(this);
        }

#endif

        public void ShowUpgrades()
        {
            var unitUpgradeSelectUI = FindObjectOfType<UnitUpgradeSelectUI>();
            if (unitUpgradeSelectUI == null)
            {
                Debug.LogError($"{nameof(unitUpgradeSelectUI)} is not assigned.");
                return;
            }

            if (!Database.UnitUpgradeTree.TryGetValue(unitIdentifier.UnitCategory, out var unitUpgradeTree))
            {
                Debug.LogError($"Upgrade tree for {unitIdentifier.UnitCategory} is not found.");
                return;
            }

            var identifier = unitIdentifier.GetIdentifier(statComponent.UpgradeLevel);
            if (!unitUpgradeTree.TryGetValue(identifier, out _))
            {
                Debug.LogError($"Upgrade node for {identifier} is not found.");
                return;
            }

            TimeScaleManager.Get.ChangeGlobalTimeScale(0f);
            unitUpgradeSelectUI.ShowWithOptions(this);
        }

        public void RepairDurability()
        {
            if (m_DelayedOperation != null ||
                statComponent.CurrentHealth >= statComponent.MaxHealth)
            {
                Debug.Log(
                    "Cannot repair car. Car is already started Operation or car is already fully repaired.");
                return;
            }

            taskProgressBar.gameObject.SetActive(true);
            taskProgressBar.value = 0f;
            taskProgressBarIconLoader.ChangeImage(OperationType.RepairDurability.ToString());
            bool isCarRepairing = false;

            m_DelayedOperation = new CarDelayedOperation(this, statComponent.healthRegenInterval);

            m_DelayedOperation.OnTaskComplete += () =>
            {
                if (statComponent.LostHealth <= 0f)
                {
                    m_DelayedOperation = null;
                    taskProgressBar.gameObject.SetActive(false);
                    return;
                }

                var regenAmount = Mathf.Round(Mathf.Max(statComponent.LostHealth, statComponent.HealthRegenAmount) * 10f) / 10f;
                var regenRatio = Mathf.Round((regenAmount / statComponent.HealthRegenAmount) * 10f) / 10f;

                if (!isCarRepairing)
                {
                    isCarRepairing = true;
                    Database.CarCostData.TryGetValue(unitIdentifier.GetIdentifier(statComponent.UpgradeLevel), out var costData);
                    costData *= (Database.GlobalBalanceSetting.DurabilityRepairCostMultiplier * regenRatio);
                    if (!parentTrain.TryConsumeResourcesFromInventory(costData))
                    {
                        Debug.LogWarning($"Not enough resources to repair car({gameObject.name}).", this);

                        m_DelayedOperation = null;
                        taskProgressBar.gameObject.SetActive(false);
                        return;
                    }
                }

                statComponent.CurrentHealth += statComponent.HealthRegenAmount;
                m_DelayedOperation.Reset(this, statComponent.healthRegenInterval, OperationType.RepairDurability);
            };

            StartCoroutine(m_DelayedOperation);
        }

        public void Demolish()
        {
            StartOperationAfterDelay(statComponent.DemolishDelay, OperationType.Demolish, DoDemolish);
        }

        public void ReorderCar()
        {
            parentTrain.BeginReorderCar(this);
        }

        #endregion // ContextMenu Actions

        #region IUnitUpgradeSelectCallbackReceiver

        public void OnUnitUpgradeSelected(string targetUpgradeIdentifier)
        {
            if (m_DelayedOperation == null
                && DatabaseUtility.TryGetData(Database.CarCostData, targetUpgradeIdentifier, out var upgradeCostData)
                && parentTrain.TryConsumeResourcesFromInventory(upgradeCostData))
            {
                StartOperationAfterDelay(statComponent.UpgradeDelay, OperationType.Upgrade,
                () =>
                {
                    Debug.Log("Replace Car Instance");
                    var prefab = Database.UnitUpgradeTree[unitIdentifier.UnitCategory][
                        targetUpgradeIdentifier].model;
                    var replacement = Instantiate(prefab);
                    var carComponent = replacement.GetComponent<Car>();

                    m_UnitUpgradeHistory.Add(targetUpgradeIdentifier);

                    DoShallowCopyTo(carComponent);
                    carComponent.Setup(carComponent.parentTrain);

                    parentTrain.ReplaceCarFromManagedList(this, carComponent);
                });
            }
        }

        public UnitUpgradeStatCaptureData GetCapturedUnitStatData()
        {
            return new UnitUpgradeStatCaptureData
            {
                FuelEfficiency = statComponent.FuelEfficiency,
                HealthPoint = statComponent.CurrentHealth,
                ArmorPoint = statComponent.armorPoint.Value,
                UnitSize = statComponent.UnitSize
            };
        }

        public string[] GetUnitUpgradeOptionIdentifiers()
        {
            return Database.UnitUpgradeTree
                [unitIdentifier.UnitCategory]
                [unitIdentifier.GetIdentifier(statComponent.UpgradeLevel)]
                .nextUpgradeNodeIdentifier;
        }

        public bool CheckAvailabilityResource(ResourceType resourceType, float amount)
        {
            return parentTrain.CheckAvailabilityResource(resourceType, amount);
        }

        #endregion

        #region ICombatEventReceiver

        public void TakeDamage(UnitCombatStatCaptureData data)
        {
            var finalDamage
                = Mathf.Max(1, (data.AttackDamage - Mathf.Max(0, statComponent.Armor * (1 - data.ArmorPierce))));
            statComponent.CurrentHealth -= finalDamage;

            parentTrain.HUD?.UpdateCarControlPanel(this);

            if (statComponent.CurrentHealth > 0)
            {
                return;
            }

            SwitchToDestroyedState();
        }

#if UNITY_EDITOR

        [ContextMenu("Do Take Damage")]
        private void TakeDamage()
        {
            TakeDamage(
                new UnitCombatStatCaptureData
                {
                    AttackDamage = UnityEngine.Random.Range(20, 100),
                    ArmorPierce = 10
                });
        }

#endif

        #endregion

        #region IDestroyedEventExecutor

        public void RegisterDestroyedEvent(Units.IDestroyedEventExecutor.OnDestroyedEventDelegate onDestroyedEvent)
        {
            OnDestroyedEvent += onDestroyedEvent;
        }

        public void UnregisterDestroyedEvent(Units.IDestroyedEventExecutor.OnDestroyedEventDelegate onDestroyedEvent)
        {
            OnDestroyedEvent -= onDestroyedEvent;
        }

        #endregion

        #region IContextMenuDataProvider

        public List<ContextMenuData> GetContextMenuData()
        {
            return contextMenuData;
        }

        public virtual ContextMenuCondition GetContextMenuConditionState()
        {
            var state = ContextMenuCondition.None;

            if (statComponent.CurrentHealth < statComponent.MaxHealth)
            {
                state |= ContextMenuCondition.OnCompartmentDamaged;
            }

            if (isCarDestroyed)
            {
                state |= ContextMenuCondition.OnCompartmentDestroyed;
            }

            if (m_CircuitFailure)
            {
                state |= ContextMenuCondition.OnCompartmentCircuitFailed;
            }

            if (m_DelayedOperation?.OperationType is OperationType.Demolish or OperationType.Upgrade or OperationType.RepairDurability or OperationType.Restore)
            {
                state |= ContextMenuCondition.OnCompartmentOperating;
            }

            return state;
        }

        public void OnContextMenuClosed()
        {
        }

        public Dictionary<ResourceType, float> GetResourceData(ContextMenuCondition condition)
        {
            var resourceData = new Dictionary<ResourceType, float>();
            switch (condition)
            {
                case ContextMenuCondition.OnCompartmentCircuitFailed:
                    resourceData.Add(ResourceType.Circuit, m_CircuitRepairAmount);
                    break;
                case ContextMenuCondition.OnCompartmentDamaged:
                    var regenAmount = Mathf.Round(Mathf.Max(statComponent.LostHealth, statComponent.HealthRegenAmount) * 10f) / 10f;
                    var regenRatio = Mathf.Round((regenAmount / statComponent.HealthRegenAmount) * 10f) / 10f;
                    Database.CarCostData.TryGetValue(unitIdentifier.GetIdentifier(statComponent.UpgradeLevel), out var costData);
                    costData *= (Database.GlobalBalanceSetting.DurabilityRepairCostMultiplier * regenRatio);

                    var conditions = new[]
                        {
                        (costData.ResourceSteel, ResourceType.Iron),
                        (costData.ResourceMithryl, ResourceType.Mythrill),
                        (costData.ResourceAdamantium, ResourceType.Adamantine),
                        (costData.ResourceTitanium, ResourceType.Titanium),
                        (costData.CircuitNormal, ResourceType.Circuit)
                        };
                    foreach (var (amount, type) in conditions)
                    {
                        if (amount > 0) resourceData.Add(type, amount);
                    }
                    break;
                default:
                    break;
            }
            return resourceData;
        }

        #endregion
    }
}