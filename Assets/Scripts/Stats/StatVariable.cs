// ReSharper disable CheckNamespace

using UnityEngine;

namespace Units.Stats
{
    public enum StatModifierType
    {
        Additive,
        Multiplicative,
        Error
    }
    
    [System.Serializable]
    public class StatModifier
    {
        public string guid;
        public StatModifierType modifierType;
        public float value;

        public StatModifier() : this(System.Guid.NewGuid().ToString())
        {
        }

        public StatModifier(string guid)
        {
            this.guid = guid;
        }

        public StatModifier(StatModifier other)
        {
            guid = other.guid;
            modifierType = other.modifierType;
            value = other.value;
        }
    }

    

    [System.Serializable]
    public class Shield
    {
        [SerializeField] protected float amount;
        
        [System.Serializable]
        public class ShieldVariable
        {
            protected System.Collections.Generic.List<Shield> shields = new(16);

            public float Amount { get; private set; } = 0;

            public void Add(Shield shield)
            {
                shields.Add(shield);

                Amount += shield.amount;
            }
            
            public void Remove(Shield shield)
            {
                shields.Remove(shield);

                Amount -= shield.amount;
            }

            public void TakeDamage(float amount)
            {
                if (amount > 0)
                {
                    Debug.LogWarning("Receiving positive damage amount is forbidden.");
                    return;
                }

                Amount += amount;
                
                var remainingAmount = Mathf.Abs(amount);
                for (var i = shields.Count - 1; i >= 0; i--)
                {
                    var shield = shields[i];
                    if (shield.amount >= remainingAmount)
                    {
                        shield.amount -= remainingAmount;
                        break;
                    }
                    
                    remainingAmount -= shield.amount;
                    shields.RemoveAt(i);
                }
            }
        }
    }
    
    [System.Serializable]
    public class StatVariable
    {
        [SerializeField] protected bool isRangeDeltaValue;
        [SerializeField] protected float baseValue;
        [SerializeField] protected float additiveValue;
        [SerializeField] protected float multiplicativeValue;
        [SerializeField] protected float errorFactor;
        [SerializeField] protected float finalValue;
        [SerializeField] protected float currentValue;
        
        protected System.Collections.Generic.Dictionary<string, StatModifier> modifiers = new(16);
        
        public float BaseValue
        {
            get => baseValue;
            set
            {
                baseValue = value;
                OnValidate();
            }
        }

        public float AdditiveValue
        {
            get => additiveValue;
            set
            {
                additiveValue = value;
                OnValidate();
            }
        }

        public float MultiplicativeValue
        {
            get => multiplicativeValue;
            set
            {
                multiplicativeValue = value;
                OnValidate();
            }
        }

        public float ErrorFactor
        {
            get => errorFactor;
            set
            {
                errorFactor = value;
                OnValidate();
            }
        }
        
        public float Value { get => finalValue; private set => finalValue = value; }
        
        public float CurrentValue { get => currentValue; set => currentValue = value; }
        
        public virtual void Setup()
        {
            OnValidate();

            if (isRangeDeltaValue)
            {
                currentValue = finalValue;
            }
        }

        protected virtual void OnValidate()
        {
            additiveValue = 0;
            multiplicativeValue = 0;
            errorFactor = 0;
            
            foreach (var modifier in modifiers.Values)
            {
                switch (modifier.modifierType)
                {
                    case StatModifierType.Additive:
                        additiveValue += modifier.value;
                        break;
                    case StatModifierType.Multiplicative:
                        multiplicativeValue += modifier.value;
                        break;
                    case StatModifierType.Error:
                        errorFactor += modifier.value;
                        break;
                    default:
                        throw new System.ArgumentOutOfRangeException();
                }
            }
            
            
            var value = (BaseValue * (1 + MultiplicativeValue) + AdditiveValue) * (1 + ErrorFactor);

            if (isRangeDeltaValue)
            {
                var deltaDiff = value - Value;
                currentValue += deltaDiff;
            }
            
            Value = value;
        }
        
        public virtual void AddModifier(StatModifier modifier, bool doRefresh = false)
        {
            if (!modifiers.TryAdd(modifier.guid, modifier) && doRefresh)
            {
                modifiers.Remove(modifier.guid);
                modifiers.Add(modifier.guid, modifier);
            }
            
            OnValidate();
        }
        
        public virtual void RemoveModifier(StatModifier modifier)
        {
            modifiers.Remove(modifier.guid);
            
            OnValidate();
        }

        public void RemoveAllModifiers()
        {
            modifiers.Clear();
            
            OnValidate();
        }

        public static void CopyModifiers(StatVariable from, StatVariable to)
        {
            to.modifiers.Clear();
            foreach (var modifier in from.modifiers)
            {
                to.modifiers.Add(modifier.Key, new StatModifier(modifier.Value));
            }
        }
    }
}