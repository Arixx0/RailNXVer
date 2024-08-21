using UnityEngine;
using Attributes;
using System;
using StatusEffects;

namespace TrainScripts
{
    public partial class Car
    {
        #region Electric Power References

        [Space, Header("Electric Power References")]
        [SerializeField] private float changeDelay = 3f;
        [SerializeField] private StatusEffect lowElectricPowerDebuff;
        [SerializeField] private StatusEffect highElectricPowerBuff;
        [SerializeField] private StatusEffect maxElectricPowerBuff;
        [SerializeField, Disabled] private float m_CurrentElectricPowerGeneration;
        [SerializeField, Disabled] private float m_CurrentElectricPowerUsage;
        [SerializeField, Disabled] private ElectricPowerUsageType m_CurrentElectricPowerUsageType = ElectricPowerUsageType.Standard;

        private VFX m_OverLoadVFX;
        private Action OnChangeElectricPower;

        public VFX OverLoadVFX => m_OverLoadVFX;

        public ElectricPowerUsageType CurrentElectricPowerUsageType => m_CurrentElectricPowerUsageType;

        public float CurrentElectricPowerGeneration
        {
            get => m_CurrentElectricPowerGeneration;
            set
            {
                m_CurrentElectricPowerGeneration = value;
                OnChangeElectricPower?.Invoke();
            }
        }

        public float CurrentElectricPowerUsage
        {
            get => m_CurrentElectricPowerUsage;
            set
            {
                m_CurrentElectricPowerUsage = value;
                OnChangeElectricPower?.Invoke();
            }
        }

        #endregion

        #region Electric Power Change Implementations

        public void ShowElectricPowerChangeItem()
        {
            if (!carElectricPowerChangeItem.gameObject.activeSelf && !taskProgressBar.gameObject.activeSelf && this is not GeneratorCompartment)
            {
                carElectricPowerChangeItem.OpenElectricPowerChangeItem(this);
            }
        }

        public void ElectricPowerBuffAdd(StatusEffectManager target)
        {
            switch (m_CurrentElectricPowerUsageType)
            {
                case ElectricPowerUsageType.Inactive:
                    CarStopWorking();
                    break;
                case ElectricPowerUsageType.Low:
                    target.AddStatusEffect(parentTrain.ElectricPowerOverload ? null : lowElectricPowerDebuff, false);
                    break;
                case ElectricPowerUsageType.Standard:
                    break;
                case ElectricPowerUsageType.High:
                    target.AddStatusEffect(parentTrain.ElectricPowerOverload ? null : highElectricPowerBuff, false);
                    break;
                case ElectricPowerUsageType.Max:
                    target.AddStatusEffect(parentTrain.ElectricPowerOverload ? null : maxElectricPowerBuff, false);
                    break;
                default:
                    break;
            }
        }

        public void ElectricPowerBuffRemove(StatusEffectManager target)
        {
            target.RemoveStatusEffect(lowElectricPowerDebuff != null ? lowElectricPowerDebuff : null);
            target.RemoveStatusEffect(highElectricPowerBuff != null ? highElectricPowerBuff : null);
            target.RemoveStatusEffect(maxElectricPowerBuff != null ? maxElectricPowerBuff : null);
        }

        public void ElectricPowerChange(ElectricPowerUsageType electricPowerUsageType)
        {
            if (taskProgressBar.gameObject.activeSelf || m_CurrentElectricPowerUsageType == electricPowerUsageType)
            {
                return;
            }

            StartOperationAfterDelay(changeDelay, OperationType.ElectricPowerChange,
                () =>
                {
                    m_CurrentElectricPowerUsageType = electricPowerUsageType;
                    var isEngineComp = this is EngineCompartment;
                    var target = statComponent.StatusEffectManager;
                    CurrentElectricPowerUsage = Convert.ToInt32(m_CurrentElectricPowerUsageType) * 0.5f * statComponent.energyCost.Value;
                    if (isEngineComp)
                    {
                        parentTrain.SetEngineInActive(electricPowerUsageType == ElectricPowerUsageType.Inactive);
                        target = parentTrain.StatComponent.StatusEffectManager;
                    }
                    ElectricPowerBuffRemove(target);
                    ElectricPowerBuffAdd(target);
                    parentTrain.HUD?.UpdateCarControlPanel(this);
                }
                );
        }

        #endregion

        public enum ElectricPowerUsageType
        {
            Inactive,
            Low,
            Standard,
            High,
            Max
        }
    }
}