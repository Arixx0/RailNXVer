using Units.Stats;

using System.Collections.Generic;
using UnityEngine;

namespace StatusEffects
{
    public interface IUpdateableStatusEffect
    {
        public void OnBegin(GameObject target);
        
        /// <returns>If return false, this StatusEffect should be destroyed.</returns>
        public bool OnUpdate(float deltaTime);

        public void OnEnd();
    }

    [CreateAssetMenu(fileName = "StatusEffect", menuName = "TrainCraft/Status Effects/Status Effect Definition")]
    public class StatusEffect : StatModifierCollection, IUpdateableStatusEffect
    {
        [SerializeField] protected float duration;
        
        protected float m_ElapsedTime;

        #region IUpdateableStatusEffect

        public virtual void OnBegin(GameObject target)
        {
            target.TryGetComponent(out targetStatComponent);

            m_ElapsedTime = 0f;
            targetStatComponent.AddModifier(this, new List<int>(16), applyToChainedTargets);
        }

        public virtual bool OnUpdate(float deltaTime)
        {
            if (targetStatComponent == null)
            {
                return false;
            }
            
            m_ElapsedTime += deltaTime;
            if (m_ElapsedTime >= duration)
            {
                return false;
            }

            return true;
        }

        public virtual void OnEnd()
        {
            targetStatComponent.RemoveModifier(this, new List<int>(16), applyToChainedTargets);
        }

        #endregion
    }
}