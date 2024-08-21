using System.Collections.Generic;
using UnityEngine;

namespace StatusEffects
{
    [DisallowMultipleComponent]
    public class StatusEffectManager : MonoBehaviour
    {
        public List<StatusEffect> statusEffects = new();

        private void Update()
        {
            var deltaTime = TimeScaleManager.Get.GetDeltaTimeOfTag(tag);
            
            for (var i = 0; i < statusEffects.Count;)
            {
                var effect = statusEffects[i];
                
                if (effect is IUpdateableStatusEffect updateableEffect)
                {
                    if (!updateableEffect.OnUpdate(deltaTime))
                    {
                        updateableEffect.OnEnd();
                        statusEffects.RemoveAt(i);
                        Destroy(effect);
                        continue;
                    }
                }

                i += 1;
            }
        }

        public void AddStatusEffect(StatusEffect effect, bool nesting = true)
        {
            if (!nesting && statusEffects.Find(e => e.Guid == effect.Guid) || effect == null)
            {
                return;
            }
            statusEffects.Add(effect);
            
            if (effect is IUpdateableStatusEffect updateableEffect)
            {
                updateableEffect.OnBegin(gameObject);
            }
        }

        public void RemoveStatusEffect(StatusEffect effect)
        {
            if (effect == null)
            {
                return;
            }
            var statusEffect = statusEffects.Find(e => e.Guid == effect.Guid);
            if (statusEffect != null)
            {
                if (statusEffect is IUpdateableStatusEffect updateableEffect)
                {
                    updateableEffect.OnEnd();
                }

                statusEffects.Remove(statusEffect);
            }
        }
    }
}