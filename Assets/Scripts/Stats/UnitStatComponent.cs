using Attributes;
using Data;
using StatusEffects;

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Units.Stats
{
    public class UnitStatComponent : MonoBehaviour, ISerializationCallbackReceiver
    {
        #region Unit Stat Components

        private const float UPGRADE_DELAY_MULTIPLIER = 2f;
        private const float DEMOLISH_DELAY_MULTIPLIER = 2f;
        private const float RESTORE_DELAY_MULTIPLIER = 2f;

        public UnitTag unitTag;
        
        [Space]
        public StatVariable healthPoint = new();
        public float healthRegenInterval;
        public Shield.ShieldVariable shield = new();
        public StatVariable armorPoint = new();
        public StatVariable moveSpeed = new();
        public StatVariable rotateSpeed = new();
        public float moveSpeedDamp;

        [Space]
        public StatVariable attackDamage = new();
        public StatVariable armorPierce = new();
        public StatVariable attackSpeed = new();
        public StatVariable attackRange = new();

        [Space]
        public StatVariable fuelEfficiency = new();
        public StatVariable energyCost = new();
        public StatVariable taskHandlingSpeed = new();
        public StatVariable carSafety = new();

        [Space]
        [SerializeField] protected float unitSize;
        [SerializeField] protected float buildDelay;
        [SerializeField] protected int level;
        [SerializeField] protected float turnThresholdAngle;
        [SerializeField, Range(0, 1)] protected float attackRangeThreshold;
        [SerializeField] protected float attackableSectorAngle;
        
        [Space]
        [SerializeField, Disabled] protected float turnThresholdAngleAlpha;
        [SerializeField, Disabled] protected float sqrAttackRange;
        [SerializeField, Disabled] protected float sqrThresholdAttackRange;
        [SerializeField, Disabled] protected float attackableSectorAngleAlpha;

        [Space]
        [SerializeField] protected StatusEffectManager statusEffectManager;
        [SerializeField] protected List<UnitStatComponent> chainedStatComponents = new(16);

        public float MaxHealth => healthPoint.Value;

        public float CurrentHealth 
        { 
            get => healthPoint.CurrentValue;
            set
            {
                healthPoint.CurrentValue = Mathf.Clamp(value, 0f, healthPoint.Value);
                OnHealthPointChanged?.Invoke(healthPoint.CurrentValue);
            }
        }

        public float LostHealth => healthPoint.Value - healthPoint.CurrentValue;

        public float HealthRegenAmount
        {
            get
            {
                var regenAmount = healthPoint.Value / (BuildDelay * Database.GlobalBalanceSetting.healthRegenMultiplier);
                if (regenAmount < 1)
                {
                    regenAmount = 1;
                }
                
                return regenAmount;
            }
        }

        public float UnitSize => unitSize;

        public float UnitSizeHalf => unitSize * 0.5f;

        public float Armor => armorPoint.Value;

        public float ArmorPierce => armorPierce.Value;

        public float MoveSpeed => moveSpeed.Value;

        public float MoveSpeedDamp => moveSpeedDamp;

        public float RotateSpeed => rotateSpeed.Value;
        
        public float TurnThresholdAngle => turnThresholdAngle;
        
        public float TurnThresholdAngleAlpha => turnThresholdAngleAlpha;

        public float AttackSpeed => attackSpeed.Value;

        public float AttackRange => attackRange.Value;

        public float AttackRangeThreshold => attackRangeThreshold;
        
        public virtual float SqrAttackRange => sqrAttackRange;
        
        public virtual float SqrThresholdAttackRange => sqrThresholdAttackRange;
        
        public float AttackableSectorAngleAlpha => attackableSectorAngleAlpha;
        
        public float FuelEfficiency => fuelEfficiency.Value;

        public float CarSafety => carSafety.Value;

        public float BuildDelay => buildDelay;

        public int UpgradeLevel { get => level; set => level = value; }
        
        public float UpgradeDelay => buildDelay * UPGRADE_DELAY_MULTIPLIER;
        
        public float DemolishDelay => buildDelay * DEMOLISH_DELAY_MULTIPLIER;

        public float RestoreDelay => buildDelay * RESTORE_DELAY_MULTIPLIER;

        public StatusEffectManager StatusEffectManager => statusEffectManager;

        public Action OnChangedStat;

        public event OnHealthPointChangedDelegate OnHealthPointChanged;
        
        public delegate void OnHealthPointChangedDelegate(float value);

        #endregion

        protected virtual void Awake()
        {
            if (statusEffectManager == null)
            {
                statusEffectManager = GetComponent<StatusEffectManager>();
            }
        }

        protected virtual void Reset()
        {
        }

        private void OnValidate()
        {
            Setup();
        }

        public virtual void Set(UnitStatDataTuple data)
        {
            healthPoint.BaseValue = data.MaxHealth;
            armorPoint.BaseValue = data.Armor;
            moveSpeed.BaseValue = data.MoveSpeed;
            // rotateSpeed.BaseValue = data.RotateSpeed;
            attackDamage.BaseValue = data.AttackDamage;
            armorPierce.BaseValue = data.ArmorPierece;
            attackSpeed.BaseValue = data.AttackSpeed;
            attackRange.BaseValue = data.AttackRange;
            fuelEfficiency.BaseValue = data.FuelEfficiency;
            energyCost.BaseValue = data.EnergyCost;
            // taskHandlingSpeed.BaseValue = data.TaskHandlingSpeed;
            unitSize = data.UnitSize;

            turnThresholdAngleAlpha = Vector3.Dot(Vector3.forward, Quaternion.Euler(0, -turnThresholdAngle * -0.5f, 0) * Vector3.forward);
            sqrAttackRange = attackRange.Value * attackRange.Value;
            sqrThresholdAttackRange = (attackRange.Value * attackRangeThreshold) * (attackRange.Value * attackRangeThreshold);
            attackableSectorAngleAlpha = Vector3.Dot(Vector3.forward, Quaternion.Euler(0, attackableSectorAngle * 0.5f, 0) * Vector3.forward);
        }

        public virtual void Setup(bool reset = false)
        {
            armorPoint.Setup();
            moveSpeed.Setup();
            rotateSpeed.Setup();
            
            attackDamage.Setup();
            armorPierce.Setup();
            attackSpeed.Setup();
            attackRange.Setup();
            
            fuelEfficiency.Setup();
            taskHandlingSpeed.Setup();
            
            turnThresholdAngleAlpha = Vector3.Dot(Vector3.forward, Quaternion.Euler(0, -turnThresholdAngle * -0.5f, 0) * Vector3.forward);
            sqrAttackRange = attackRange.Value * attackRange.Value;
            sqrThresholdAttackRange = (attackRange.Value * attackRangeThreshold) * (attackRange.Value * attackRangeThreshold);
            attackableSectorAngleAlpha = Vector3.Dot(Vector3.forward, Quaternion.Euler(0, attackableSectorAngle * 0.5f, 0) * Vector3.forward);

            if (reset)
            {
                return;
            }
            
            healthPoint.Setup();
        }

        public UnitCombatStatCaptureData CaptureCombatStatData()
        {
            return new UnitCombatStatCaptureData
            {
                AttackDamage = attackDamage.Value,
                ArmorPierce = armorPierce.Value
            };
        }

        public UnitUpgradeStatCaptureData CaptureUpgradeStatData()
        {
            return new UnitUpgradeStatCaptureData
            {
                HealthPoint = healthPoint.CurrentValue,
                ArmorPoint = armorPoint.Value,
                UnitSize = unitSize,
                FuelEfficiency = 0f,
            };
        }

        public void AddModifier(StatModifierCollection modifierCollection, List<int> traverseHistory, bool isChained)
        {
            if (traverseHistory.Contains(GetInstanceID()))
            {
                return;
            }
            
            modifierCollection.ApplyTo(this);
            traverseHistory.Add(GetInstanceID());
            
            foreach (var statComponent in chainedStatComponents)
            {
                statComponent.AddModifier(modifierCollection, traverseHistory, isChained);
            }
        }

        public void RemoveModifier(StatModifierCollection modifierCollection, List<int> traverseHistory, bool isChained = true)
        {
            if (traverseHistory.Contains(GetInstanceID()))
            {
                return;
            }

            modifierCollection.ReleaseFrom(this);
            traverseHistory.Add(GetInstanceID());

            if (isChained)
            {
                foreach (var statComponent in chainedStatComponents)
                {
                    statComponent.RemoveModifier(modifierCollection, traverseHistory);
                }
            }
        }

        public void RemoveAllModifiers()
        {
            healthPoint.RemoveAllModifiers();
            armorPoint.RemoveAllModifiers();
            moveSpeed.RemoveAllModifiers();
            rotateSpeed.RemoveAllModifiers();
            
            attackDamage.RemoveAllModifiers();
            armorPierce.RemoveAllModifiers();
            attackSpeed.RemoveAllModifiers();
            attackRange.RemoveAllModifiers();
            
            fuelEfficiency.RemoveAllModifiers();
            taskHandlingSpeed.RemoveAllModifiers();
        }
        
        public void AddChainedStatComponent(UnitStatComponent statComponent)
        {
            chainedStatComponents.Add(statComponent);
        }

        public void RemoveChainedStatComponent(UnitStatComponent statComponent)
        {
            chainedStatComponents.Remove(statComponent);
        }

        public virtual void DoShallowCopyToTarget(UnitStatComponent target)
        {
            StatVariable.CopyModifiers(healthPoint, target.healthPoint);
            StatVariable.CopyModifiers(armorPoint, target.armorPoint);
            StatVariable.CopyModifiers(moveSpeed, target.moveSpeed);
            StatVariable.CopyModifiers(rotateSpeed, target.rotateSpeed);
            
            StatVariable.CopyModifiers(attackDamage, target.attackDamage);
            StatVariable.CopyModifiers(armorPierce, target.armorPierce);
            StatVariable.CopyModifiers(attackSpeed, target.attackSpeed);
            StatVariable.CopyModifiers(attackRange, target.attackRange);

            StatVariable.CopyModifiers(fuelEfficiency, target.fuelEfficiency);
            StatVariable.CopyModifiers(energyCost, target.energyCost);
            StatVariable.CopyModifiers(taskHandlingSpeed, target.taskHandlingSpeed);
            StatVariable.CopyModifiers(carSafety, target.carSafety);
            
            // update health point's current value
            target.healthPoint.CurrentValue = healthPoint.CurrentValue + (target.healthPoint.BaseValue - healthPoint.BaseValue);
        }

        public void OnBeforeSerialize()
        {
            if (Application.isPlaying)
            {
                return;
            }
            
            RemoveAllModifiers();
        }

        public void OnAfterDeserialize()
        {
        }
    }
}