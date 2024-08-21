// #define DEBUG_UNIT_COMBATS

using UnityEngine;

using Data;
using UI;
using Units.Stats;

namespace Units
{
    public abstract class CombatUnit : MonoBehaviour, ICombatEventReceiver, IDestroyedEventExecutor
    {
        [SerializeField] protected LayerMask enemyLayerMask;
        [SerializeField] protected EnemySensingComponent enemySensingComponent;
        [SerializeField] protected UnitStatComponent statComponent;
        [SerializeField] protected UnitHealthBar healthBar;
        [SerializeField] protected UnitDestructionCompositor destructionCompositor;
        [SerializeField] protected AudioSource attackSoundSource;
        [SerializeField] protected SFXPreset unitAttackSFXPreset;

        [Space]
        [SerializeField] protected float navigationPathRequestInterval = 0.5f;
        
        private Transform m_CachedTransform;
        protected UnitCombatStatCaptureData m_CombatStatData;

        public Transform CachedTransform => m_CachedTransform ? m_CachedTransform : (m_CachedTransform = transform);

        public EnemySensingComponent SensingComponent => enemySensingComponent;

        protected event IDestroyedEventExecutor.OnDestroyedEventDelegate OnDestroyedEvent;

        protected virtual void Awake()
        {
            statComponent.Setup();
            statComponent.OnChangedStat += ReSetupStat;

            healthBar = healthBar == null ? null : healthBar;
            if (healthBar != null)
            {
                healthBar.SetHealthProperties(statComponent.healthPoint.Value, statComponent.healthPoint.CurrentValue);
                statComponent.OnHealthPointChanged += healthBar.UpdateHealthPoint;
            }
        }

        public virtual void ReSetupStat()
        {
            statComponent.Setup(true);
            m_CombatStatData = statComponent.CaptureCombatStatData();
        }

        protected virtual void OnDestroy()
        {
#if DEBUG_UNIT_COMBATS
            InvokeDestroyedEvent();
#endif
            statComponent.OnChangedStat -= ReSetupStat;
        }

        public virtual void TakeDamage(UnitCombatStatCaptureData data)
        {
            float finalDamage = Mathf.Max(1, (data.AttackDamage - Mathf.Max(0, statComponent.Armor * (1 - data.ArmorPierce))));
            statComponent.CurrentHealth -= finalDamage;

            if (statComponent.CurrentHealth <= 0)
            {
                destructionCompositor.DoDestroy();
                InvokeDestroyedEvent();
            }
        }

        protected virtual void PlayAttackEffect()
        {
            if (attackSoundSource != null && unitAttackSFXPreset != null)
            {
                attackSoundSource.clip = unitAttackSFXPreset.GetRandomSFX();
                attackSoundSource.pitch = unitAttackSFXPreset.isRandomPitch ? unitAttackSFXPreset.GetRandomPitch() : 1f;
            }
            attackSoundSource?.Play();
        }

        protected void InvokeDestroyedEvent()
        {
            OnDestroyedEvent?.Invoke();
        }

        public virtual void RegisterDestroyedEvent(IDestroyedEventExecutor.OnDestroyedEventDelegate onDestroyedEvent)
        {
            this.OnDestroyedEvent += onDestroyedEvent;
        }

        public void UnregisterDestroyedEvent(IDestroyedEventExecutor.OnDestroyedEventDelegate onDestroyedEvent)
        {
            this.OnDestroyedEvent -= onDestroyedEvent;
        }
    }
}