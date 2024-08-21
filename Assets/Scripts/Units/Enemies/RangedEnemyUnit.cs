using UnityEngine;

using Projectiles;

using BehaviorDesigner.Runtime.Tasks;

namespace Units.Enemies
{
    public abstract class RangedEnemyUnit : EnemyUnit
    {
        #region Ranged Enemy Components

        [Space, Header("Ranged Enemy Components")]
        [SerializeField] protected ParticleSystem muzzleFlashVFX;
        [SerializeField] protected ProjectileParticleHandler projectileParticleHandler;

        private float m_FireCoolTime;

        public ProjectileParticleHandler ProjectileParticleHandler => projectileParticleHandler;

        #endregion

        protected override void Start()
        {
            base.Start();
            if (projectileParticleHandler != null)
            {
                projectileParticleHandler.SetParticleCollisionLayer(enemyLayerMask);
                projectileParticleHandler.SetParticleEmissionRate(statComponent.AttackSpeed);
                projectileParticleHandler.SetProjectileType(statComponent);
                projectileParticleHandler.OnParticleEmitted += PlayAttackEffect;
                projectileParticleHandler.OnParticleCollided += OnProjectileParticleCollided;
            }
            m_FireCoolTime = statComponent.AttackSpeed;
        }

        public override void ReSetupStat()
        {
            base.ReSetupStat();
            if (projectileParticleHandler != null)
            {
                projectileParticleHandler.SetParticleEmissionRate(statComponent.AttackSpeed);
            }
        }

        protected override void PlayAttackEffect()
        {
            base.PlayAttackEffect();
            muzzleFlashVFX?.Play();
        }

        protected virtual void OnProjectileParticleCollided(GameObject other)
        {
            if (other.TryGetComponent(out ICombatEventReceiver eventReceiver))
            {
                eventReceiver.TakeDamage(m_CombatStatData);
            }
        }

        #region Ranged Enemy Unit Behavior Action Task

        [TaskCategory("Traincraft/EnemyUnit/InitializeAction")]
        public class RangedEnemyUnitCommonInitialize : EnemyUnitCommonInitialize
        {
            public Shared<RangedEnemyUnit> rangedEnemyUnit;

            public override TaskStatus OnUpdate()
            {
                var status = base.OnUpdate();

                if (status == TaskStatus.Failure)
                {
                    rangedEnemyUnit.Value.projectileParticleHandler.PauseEmitting();
                }

                return status;
            }
        }

        [TaskCategory("Traincraft/EnemyUnit/AttackAction")]
        public class RangedEnemyUnitCommonAttackAction : EnemyUnitCommonAttackAction
        {
            public Shared<RangedEnemyUnit> rangedEnemyUnit;

            public override TaskStatus OnUpdate()
            {
                var status = base.OnUpdate();

                if (status == TaskStatus.Success)
                {
                    rangedEnemyUnit.Value.m_FireCoolTime += Time.deltaTime;
                    if (rangedEnemyUnit.Value.m_FireCoolTime >= rangedEnemyUnit.Value.statComponent.AttackSpeed)
                    {
                        rangedEnemyUnit.Value.m_FireCoolTime = 0;
                        rangedEnemyUnit.Value.projectileParticleHandler.StartEmitting();
                    }
                }

                return status;
            }
        }

        #endregion
    }
}