using UnityEngine;

using Units;
using Data;

namespace StatusEffects.Environments
{
    [CreateAssetMenu(fileName = "GreatStorm", menuName = "TrainCraft/Environment/Great Storm")]
    public class GreatStorm : EnvironmentStatusEffect
    {
        [SerializeField] private float coolDown;
        [SerializeField] private float healthReductionRate;

        private UnitCombatStatCaptureData m_CombatStatData;

        private UnitCombatStatCaptureData GreatStormCaptureData(float health)
        {
            return new UnitCombatStatCaptureData
            {
                AttackDamage = health * healthReductionRate,
                ArmorPierce = float.MaxValue
            };
        }

        #region IUpdateableStatusEffect

        public override bool OnUpdate(float deltaTime)
        {
            if (targetStatComponent == null)
            {
                return false;
            }
            m_ElapsedTime += deltaTime;
            if (m_ElapsedTime >= coolDown)
            {
                if (targetStatComponent.TryGetComponent(out ICombatEventReceiver eventReceiver))
                {
                    m_CombatStatData = GreatStormCaptureData(targetStatComponent.CurrentHealth);
                    eventReceiver.TakeDamage(m_CombatStatData);
                }
                m_ElapsedTime = 0f;
            }
            return true;
        }

        #endregion
    }
}