using UnityEngine;

using Data;
using StatusEffects;
using TrainScripts;
using Utility;
using UI;
using Units.Stats;

namespace Units.Enemies
{
    public class Barricade : MonoBehaviour, ICombatEventReceiver, IDestroyedEventExecutor
    {
        [SerializeField] private UnitStatComponent statComponent;
        [SerializeField] private UnitDestructionCompositor destructionCompositor;
        [SerializeField] private AnimationCurve damageMultiplierCurve;
        [SerializeField] private StatusEffect trainDerailmentStatusEffect;
        [SerializeField] private UnitHealthBar healthBar;
        [SerializeField] private TextIdentifier enemyUnitIdentifier;
        [SerializeField] private Collider boxCollider;
        
        private readonly Collider[] m_HitColliders = new Collider[8];

        public TextIdentifier Identifier => enemyUnitIdentifier;

        private UnitCombatStatCaptureData m_CombatStatData;

        private event IDestroyedEventExecutor.OnDestroyedEventDelegate OnDestroyedEvent;

        #region MonoBehaviour Events

        private void Start()
        {
            Debug.Assert(!string.IsNullOrEmpty(enemyUnitIdentifier.Identifier));
            enemyUnitIdentifier.Set(enemyUnitIdentifier);

            if (DatabaseUtility.TryGetData(Database.UnitStatData, enemyUnitIdentifier.Identifier, out var statData))
            {
                statComponent.Set(statData);
                m_CombatStatData = statComponent.CaptureCombatStatData();
            }

            if (healthBar != null)
            {
                healthBar.SetHealthProperties(statComponent.MaxHealth, statComponent.CurrentHealth);
                statComponent.OnHealthPointChanged += healthBar.UpdateHealthPoint;
            }
            else
            {
                healthBar = null;
            }
        }

        private void Update()
        {
            if (destructionCompositor.IsDestroyed)
            {
                return;
            }

            var hitCount =
                Physics.OverlapBoxNonAlloc(boxCollider.bounds.center, boxCollider.bounds.size, m_HitColliders, transform.rotation);

            for (var i = 0; i < hitCount; i++)
            {
                var hitCollider = m_HitColliders[i];
                
                if (hitCollider.TryGetComponent(out Car car))
                {
                    car.ParentTrain.StatComponent.StatusEffectManager?.AddStatusEffect(Instantiate(trainDerailmentStatusEffect));
                    OnCollidTrain(car.ParentTrain);
                    destructionCompositor.DoDestroy();
                    OnDestroyedEvent?.Invoke();
                    break;
                }
                
                if (hitCollider.TryGetComponent(out Train train))
                {
                    train.StatComponent.StatusEffectManager?.AddStatusEffect(Instantiate(trainDerailmentStatusEffect));
                    OnCollidTrain(train);
                    destructionCompositor.DoDestroy();
                    OnDestroyedEvent?.Invoke();
                    break;
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(boxCollider.bounds.center, boxCollider.bounds.size);
        }

        #endregion

        private void OnCollidTrain(Train train)
        {
            foreach (var car in train.cars)
            {
                if (car is EngineCompartment)
                {
                    car.TakeDamage(m_CombatStatData);
                    break;
                }
                
            }
        }

        #region ICombatEventReceiver
        
        public void TakeDamage(UnitCombatStatCaptureData data)
        {
            var finalDamage = Mathf.Max(1, (data.AttackDamage - Mathf.Max(0, statComponent.Armor * (1 - data.ArmorPierce))));
            statComponent.CurrentHealth -= finalDamage;
            
            if (statComponent.CurrentHealth <= 0)
            {
                destructionCompositor.DoDestroy();
                OnDestroyedEvent?.Invoke();
            }
        }
        
        #endregion

        #region IDestroyedEventExecutor
        
        public void RegisterDestroyedEvent(IDestroyedEventExecutor.OnDestroyedEventDelegate onDestroyedEvent)
        {
            OnDestroyedEvent += onDestroyedEvent;
        }

        public void UnregisterDestroyedEvent(IDestroyedEventExecutor.OnDestroyedEventDelegate onDestroyedEvent)
        {
            OnDestroyedEvent -= onDestroyedEvent;
        }
        
        #endregion // IDestroyedEventExecutor
    }
}