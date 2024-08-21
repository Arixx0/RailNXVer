// ReSharper disable CheckNamespace

using Attributes;

using UnityEngine;

namespace Units.Stats
{
    public abstract class StatModifierCollection : ScriptableObject
    {
        [SerializeField] protected string displayName;
        [SerializeField, TextArea] protected string description;
        [SerializeField, Disabled] protected string guid = System.Guid.NewGuid().ToString();
        [SerializeField] protected UnitTag applicableUnitTag;
        [SerializeField] protected bool refreshOnApply;
        [SerializeField] protected bool applyToChainedTargets;

        [Space]
        [SerializeField] protected bool enableHealthPointModifier;
        [SerializeField] protected StatModifier healthPointModifier;

        [SerializeField] protected bool enableArmorPointModifier;
        [SerializeField] protected StatModifier armorPointModifier;

        [SerializeField] protected bool enableMoveSpeedModifier;
        [SerializeField] protected StatModifier moveSpeedModifier;

        [SerializeField] protected bool enableRotateSpeedModifier;
        [SerializeField] protected StatModifier rotateSpeedModifier;

        [Space]
        [SerializeField] protected bool enableAttackDamageModifier;
        [SerializeField] protected StatModifier attackDamageModifier;

        [SerializeField] protected bool enableArmorPierceModifier;
        [SerializeField] protected StatModifier armorPierceModifier;

        [SerializeField] protected bool enableAttackSpeedModifier;
        [SerializeField] protected StatModifier attackSpeedModifier;

        [SerializeField] protected bool enableAttackRangeModifier;
        [SerializeField] protected StatModifier attackRangeModifier;

        [Space]
        [SerializeField] protected bool enableFuelEfficiencyModifier;
        [SerializeField] protected StatModifier fuelEfficiencyModifier;
        
        [SerializeField] protected bool enableCarSafetyModifier;
        [SerializeField] protected StatModifier carSafetyModifier;

        protected UnitStatComponent targetStatComponent;

        public string Guid => guid;
        
        public bool ApplyToChainedTargets => applyToChainedTargets;

        protected virtual void Reset()
        {
            AssignGuid();
        }

        private void AssignGuid()
        {
            healthPointModifier = new StatModifier(guid);
            armorPointModifier = new StatModifier(guid);
            moveSpeedModifier = new StatModifier(guid);
            rotateSpeedModifier = new StatModifier(guid);
            attackDamageModifier = new StatModifier(guid);
            armorPierceModifier = new StatModifier(guid);
            attackSpeedModifier = new StatModifier(guid);
            attackRangeModifier = new StatModifier(guid);
            fuelEfficiencyModifier = new StatModifier(guid);
            carSafetyModifier = new StatModifier(guid);
        }
    
        public virtual void ApplyTo(UnitStatComponent statComponent)
        {
            if (!applicableUnitTag.HasFlag(statComponent.unitTag))
            {
                return;
            }
        
            if (enableHealthPointModifier) { statComponent.healthPoint.AddModifier(healthPointModifier, refreshOnApply); }
            if (enableArmorPointModifier) { statComponent.armorPoint.AddModifier(armorPointModifier, refreshOnApply); }
            if (enableMoveSpeedModifier) { statComponent.moveSpeed.AddModifier(moveSpeedModifier, refreshOnApply); }
            if (enableRotateSpeedModifier) { statComponent.rotateSpeed.AddModifier(rotateSpeedModifier, refreshOnApply); }

            if (enableAttackDamageModifier) { statComponent.attackDamage.AddModifier(attackDamageModifier, refreshOnApply); }
            if (enableArmorPierceModifier) { statComponent.armorPierce.AddModifier(armorPierceModifier, refreshOnApply); }
            if (enableAttackSpeedModifier) { statComponent.attackSpeed.AddModifier(attackSpeedModifier, refreshOnApply); }
            if (enableAttackRangeModifier) { statComponent.attackRange.AddModifier(attackRangeModifier, refreshOnApply); }
            
            if (enableFuelEfficiencyModifier) { statComponent.fuelEfficiency.AddModifier(fuelEfficiencyModifier, refreshOnApply); }
            if (enableCarSafetyModifier) { statComponent.carSafety.AddModifier(carSafetyModifier, refreshOnApply); }
        
            statComponent.OnChangedStat?.Invoke();
        }
    
        public virtual void ReleaseFrom(UnitStatComponent statComponent)
        {
            if (!applicableUnitTag.HasFlag(statComponent.unitTag))
            {
                return;
            }
        
            if (enableHealthPointModifier) { statComponent.healthPoint.RemoveModifier(healthPointModifier); }
            if (enableArmorPointModifier) { statComponent.armorPoint.RemoveModifier(armorPointModifier); }
            if (enableMoveSpeedModifier) { statComponent.moveSpeed.RemoveModifier(moveSpeedModifier); }
            if (enableRotateSpeedModifier) { statComponent.rotateSpeed.RemoveModifier(rotateSpeedModifier); }

            if (enableAttackDamageModifier) { statComponent.attackDamage.RemoveModifier(attackDamageModifier); }
            if (enableArmorPierceModifier) { statComponent.armorPierce.RemoveModifier(armorPierceModifier); }
            if (enableAttackSpeedModifier) { statComponent.attackSpeed.RemoveModifier(attackSpeedModifier); }
            if (enableAttackRangeModifier) { statComponent.attackRange.RemoveModifier(attackRangeModifier); }

            if (enableFuelEfficiencyModifier) { statComponent.fuelEfficiency.RemoveModifier(fuelEfficiencyModifier); }
            if (enableCarSafetyModifier) { statComponent.carSafety.RemoveModifier(carSafetyModifier); }
        
            statComponent.OnChangedStat?.Invoke();
        }
    }
}
