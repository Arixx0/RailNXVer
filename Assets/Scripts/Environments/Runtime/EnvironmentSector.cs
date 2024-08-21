using UnityEngine;

using StatusEffects;
using Units;
using Units.Stats;

namespace Environments
{
    public class EnvironmentSector : MonoBehaviour
    {
        [SerializeField] private StatusEffect statusEffect;

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out UnitStatComponent statComponent))
            {
                statComponent.StatusEffectManager?.AddStatusEffect(Instantiate(statusEffect), false);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out UnitStatComponent statComponent))
            {
                statComponent.StatusEffectManager?.RemoveStatusEffect(statusEffect);
            }
        }
    }

}