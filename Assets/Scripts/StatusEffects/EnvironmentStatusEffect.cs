using UnityEngine;

namespace StatusEffects.Environments
{
    [CreateAssetMenu(fileName = "Environment StatusEffect", menuName = "TrainCraft/Environment/Environment Status Effect Definition")]
    public class EnvironmentStatusEffect : StatusEffect
    {
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            StatusEffect other = (StatusEffect)obj;
            return string.CompareOrdinal(guid, other.Guid) == 0;
        }

        public override int GetHashCode()
        {
            return Guid.GetHashCode();
        }

        public override bool OnUpdate(float deltaTime)
        {
            return targetStatComponent != null;
        }
    }
}