using UnityEngine;

using BehaviorDesigner.Runtime.Tasks;

using Projectiles;

using System.Collections;

namespace Units.Enemies
{
    public class EnemyTurret : RangedEnemyUnit
    {

        #region Enemy Turret Components

        [Header("Enemy Turret Components")]

        [SerializeField] protected ProjectileParticleHandler rightProjectileParticleHandler;
        [SerializeField] protected AudioSource rightFireSoundSource;
        [SerializeField] protected ParticleSystem rightMuzzleFlashVFX;
        [SerializeField] private Transform turretBody;
        [SerializeField] private float firedealy = 0.1f;

        #endregion

        #region MonoBehaviour Events

        protected override void Start()
        {
            base.Start();
            if (rightProjectileParticleHandler != null)
            {
                rightProjectileParticleHandler.SetParticleCollisionLayer(enemyLayerMask);
                rightProjectileParticleHandler.SetParticleEmissionRate(statComponent.AttackSpeed);
                rightProjectileParticleHandler.SetProjectileType(statComponent);
                rightProjectileParticleHandler.OnParticleEmitted += PlayRightMuzzleEffect;
                rightProjectileParticleHandler.OnParticleCollided += OnProjectileParticleCollided;
            }
        }

        #endregion

        private Coroutine m_AttackCoroutine;

        public override void ReSetupStat()
        {
            base.ReSetupStat();
            rightProjectileParticleHandler.SetParticleEmissionRate(statComponent.AttackSpeed);
        }

        private void PlayRightMuzzleEffect()
        {
            rightMuzzleFlashVFX?.Play();
            if (rightFireSoundSource != null && unitAttackSFXPreset != null)
            {
                rightFireSoundSource.clip = unitAttackSFXPreset.GetRandomSFX();
                rightFireSoundSource.pitch = unitAttackSFXPreset.isRandomPitch ? unitAttackSFXPreset.GetRandomPitch() : 1f;
            }
            rightFireSoundSource?.Play();
        }


        private IEnumerator AttackStart()
        {
            projectileParticleHandler.StartEmitting();
            yield return new WaitForSeconds(firedealy);
            rightProjectileParticleHandler.StartEmitting();
        }

        #region Enemy Turret Behavior Action Task

        [TaskCategory("Traincraft/EnemyUnit/InitializeAction")]
        public class TurretInitializeAction : RangedEnemyUnitCommonInitialize
        {
            public Shared<EnemyTurret> enemyTurret;

            public override TaskStatus OnUpdate()
            {
                var status = base.OnUpdate();

                if (status == TaskStatus.Failure)
                {
                    enemyTurret.Value.rightProjectileParticleHandler.PauseEmitting();
                    if (enemyTurret.Value.m_AttackCoroutine != null)
                    {
                        enemyTurret.Value.StopCoroutine(enemyTurret.Value.m_AttackCoroutine);
                        enemyTurret.Value.m_AttackCoroutine = null;
                    }
                }
                
                enemyTurret.Value.enemyUnitAnimator.enabled = !isEnemyDetected.Value || !isEnemyWithinAttackRange.Value;

                return status;
            }
        }

        [TaskCategory("Traincraft/EnemyUnit/AttackAction")]
        public class TurretAttackAction : RangedEnemyUnitCommonAttackAction
        {
            public Shared<EnemyTurret> enemyTurret;

            public override TaskStatus OnUpdate()
            {
                if (!isEnemyDetected.Value || !isEnemyWithinAttackRange.Value || !isPossibleAttack.Value || enemyTurret.Value.m_AttackCoroutine != null)
                {
                    return TaskStatus.Failure;
                }

                if (enemyTurret.Value.m_AttackCoroutine == null)
                {
                    enemyTurret.Value.m_AttackCoroutine = StartCoroutine(enemyTurret.Value.AttackStart());
                }

                return TaskStatus.Success;
            }
        }

        #endregion
    }
}