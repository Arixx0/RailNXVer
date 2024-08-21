using Data;
using Projectiles;
using System;
using Units.Stats;
using UnityEngine;
using Utility;

// TODO: `Projectile` class should be refactored to be handled as particle entity.
//      - It would be more optimized if use compute shader to calculate projectile's velocity.

namespace Units.Turrets
{
    public class Turret : CombatUnit
    {
        #region Turret Components

        [Header("Turret Components")]
        [SerializeField] private TurretType turretType;
        [SerializeField] private Transform muzzleTransform;
        [SerializeField] private Transform turretBodyTransform;
        [SerializeField] private Transform turretNeckTransform;
        [SerializeField] private Animator turretAnimator;
        [SerializeField] private ParticleSystem muzzleFlashVFX;

        public TurretType TurretType => turretType;

        public UnitStatComponent StatComponent
        {
            get => statComponent;
            set => statComponent = value;
        }
        
        #endregion

        #region Projectile

        [Header("Projectile")]
        [SerializeField] private ParticleSystem projectileParticle;
        [SerializeField] private ProjectileParticleHandler projectileParticleHandler;

        [SerializeField] private float projectileRange;
        [SerializeField] private short minProjectileCount;
        [SerializeField] private short maxProjectileCount;

        [SerializeField] private float gravity = -9.82f;
        [SerializeField, Range(45f, 90f)] private float projectileLaunchAngle;
        [SerializeField] private MortarShell projectilePrefab;

        private float m_ProjectileLaunchAngleTan;
        private float m_LastAttackTime;
        private bool m_isPossibleAttack;
        private MortarShell m_Projectile;

        #endregion

        #region MonoBehaviour Events

        private void Update()
        {
            if (!isActiveAndEnabled)
            {
                return;
            }

            AimAtEnemy();
            if (m_isPossibleAttack)
            {
                FireProjectile();
            }
            else
            {
                projectileParticleHandler?.PauseEmitting();
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (turretType == TurretType.Mortar && projectilePrefab != null && muzzleTransform != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(muzzleTransform.position + muzzleTransform.up * (projectileRange + 0.05f), projectileRange);
            }
        }

        #endregion

        #region Turret Implementations

        public void SetStatData(UnitStatComponent carStatComponent)
        {
            statComponent = carStatComponent;
            m_CombatStatData = statComponent.CaptureCombatStatData();

            switch (turretType)
            {
                case TurretType.BasicRifle:
                    projectileParticleHandler.ProjectileType = ProjectileType.BasicRifle;
                    break;
                case TurretType.ShotGun:
                    projectileParticleHandler.ProjectileType = ProjectileType.ShotGun;
                    projectileParticleHandler.ProjectileRange = projectileRange;
                    projectileParticleHandler.MinProjectileCount = minProjectileCount;
                    projectileParticleHandler.MaxProjectileCount = maxProjectileCount;
                    break;
                case TurretType.FlameThrower:
                    projectileParticleHandler.ProjectileType = ProjectileType.FlameThrower;
                    projectileParticleHandler.ProjectileRange = projectileRange;
                    break;
                case TurretType.Mortar:
                    turretNeckTransform.rotation = Quaternion.Euler(projectileLaunchAngle, 0, 0);
                    m_ProjectileLaunchAngleTan = Mathf.Tan(projectileLaunchAngle * Mathf.Deg2Rad);

                    m_Projectile = Instantiate(projectilePrefab, CachedTransform);
                    m_Projectile.gameObject.SetActive(false);
                    m_Projectile.EnemyLayerMask = enemyLayerMask | (1 << LayerMask.NameToLayer("Ground"));
                    m_Projectile.DamageContextData = m_CombatStatData;
                    m_Projectile.ExplosionImpactRadius = projectileRange;
                    break;
                default:
                    break;
            }
            if (projectileParticleHandler != null)
            {
                projectileParticleHandler.SetParticleCollisionLayer(enemyLayerMask);
                projectileParticleHandler.SetParticleEmissionRate(statComponent.AttackSpeed);
                projectileParticleHandler.SetProjectileType(statComponent);
                projectileParticleHandler.OnParticleEmitted += PlayAttackEffect;
                projectileParticleHandler.OnParticleCollided += OnProjectileParticleCollided;
            }
        }

        public override void ReSetupStat()
        {
            base.ReSetupStat();
            projectileParticleHandler.SetParticleEmissionRate(statComponent.AttackSpeed);
        }

        [ContextMenu("Force Update Stat Data")]
        private void ForceUpdateStatData()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return;
            }
#endif
            
            if (statComponent == null)
            {
                return;
            }

            SetStatData(statComponent);
        }

        private void AimAtEnemy()
        {
            if (!enemySensingComponent?.CurrentTargetedEnemy)
            {
                return;
            }
            turretAnimator.enabled = false;
            var position = turretBodyTransform.position;
            var targetPosition = enemySensingComponent.CurrentTargetedEnemyColliderCenter;
            var targetForward = new Vector3(targetPosition.x - position.x, 0, targetPosition.z - position.z);
            float rotateSpeedPerFrame = statComponent.RotateSpeed * Time.deltaTime;

            //Update yaw rotation: set forward direction to the enemy
            var targetRotation = Quaternion.LookRotation(targetForward, Vector3.up);
            turretBodyTransform.rotation = Quaternion.RotateTowards(turretBodyTransform.rotation, targetRotation, rotateSpeedPerFrame);
            m_isPossibleAttack = false;
            if (Quaternion.Angle(turretBodyTransform.rotation, targetRotation) < 0.1f)
            {
                m_isPossibleAttack = true;
                switch (turretType)
                {
                    case TurretType.Mortar:
                        break;
                    default:
                        // Update pitch rotation: set muzzle's forward to the enemy
                        var targetMuzzleForward = targetPosition - muzzleTransform.position;
                        var targetMuzzleRotation = Quaternion.LookRotation(targetMuzzleForward, Vector3.up) * Quaternion.Euler(90, 0, 0);
                        turretNeckTransform.rotation = Quaternion.RotateTowards(turretNeckTransform.rotation, targetMuzzleRotation, rotateSpeedPerFrame);
                        break;
                }
            }
        }

        private void FireProjectile()
        {
            if (!enemySensingComponent?.CurrentTargetedEnemy)
            {
                projectileParticleHandler?.PauseEmitting();
                return;
            }
            var targetDir = enemySensingComponent.CurrentTargetedEnemy.position - CachedTransform.position;
            if (targetDir.sqrMagnitude >= statComponent.SqrAttackRange)
            {
                projectileParticleHandler?.PauseEmitting();
                return;
            }

            switch(turretType)
            {
                case TurretType.Mortar:
                    MortarTurretFire();
                    break;
                default:
                    var nextAvailableAttackTime = m_LastAttackTime + statComponent.AttackSpeed;
                    if (nextAvailableAttackTime > Time.unscaledTime)
                    {
                        return;
                    }
                    m_LastAttackTime = Time.unscaledTime;
                    projectileParticleHandler?.StartEmitting();
                    break;
            }



            // check if the target is in the attackable fan area
            // TODO: optimization required; take away normalizing and dot product calculation

            /* check if turret can fire a projectile
            //if (lastAttackTime + statComponent.AttackSpeed > Time.unscaledTime)
            //{
            //    return;
            //}

            //lastAttackTime = Time.unscaledTime;

            //// NOTE: currently imported sound clips for automatic fire does not have only shooting sound(those includes reloading sound, empty shell dropping sound, etc.)
            //fireSoundSource.PlayOneShot(fireSoundSource.clip);
            muzzleFlashVFX.Play(); */

        }
        private void MortarTurretFire()
        {
            var nextAvailableAttackTime = m_LastAttackTime + statComponent.AttackSpeed;
            if (nextAvailableAttackTime > Time.unscaledTime)
            {
                return;
            }

            //var attackableSectorAlpha = Vector3.Dot(
            //    turretBodyTransform.forward,
            //    enemySensingComponent.CurrentTargetedEnemyColliderCenter - turretBodyTransform.position);
            //if (attackableSectorAlpha < statComponent.AttackableSectorAngleAlpha)
            //{
            //    return;
            //}

            var distanceToEnemy = VectorUtility.DistanceInXZCoord(
                turretBodyTransform.position, enemySensingComponent.CurrentTargetedEnemyColliderCenter);
            //if (distanceToEnemy < MortarTurretStatComponent.LaunchOptionAtMinRange.attackRange ||
            //    distanceToEnemy > MortarTurretStatComponent.LaunchOptionAtMaxRange.attackRange)
            //{
            //    return;
            //}
            // Currently disables adjusting shell speed proportional to distance.
            m_LastAttackTime = Time.unscaledTime;

            var projectileSpawnPosition = muzzleTransform.position + muzzleTransform.forward * (projectilePrefab.ColliderRadius * 2 + 0.05f);
            var enemyPosition = enemySensingComponent.CurrentTargetedEnemyColliderCenter;

            var hDiff = enemyPosition.y - projectileSpawnPosition.y;

            var horizontalVelocityAlpha = gravity * distanceToEnemy * distanceToEnemy / (2.0f * (hDiff - distanceToEnemy * m_ProjectileLaunchAngleTan));
            var zVelocity = Mathf.Sqrt(horizontalVelocityAlpha);
            var yVelocity = m_ProjectileLaunchAngleTan * zVelocity;

            var localVelocity = new Vector3(0f, yVelocity, zVelocity);
            var globalVelocity = transform.TransformDirection(localVelocity);

            var projectile = Instantiate(m_Projectile, projectileSpawnPosition, Quaternion.identity, null);
            projectile.gameObject.SetActive(true);
            projectile.DamageContextData = m_CombatStatData;
            projectile.InitialForce = globalVelocity;
            PlayAttackEffect();
        }

        protected override void PlayAttackEffect()
        {
            base.PlayAttackEffect();
            muzzleFlashVFX?.Play();
        }

        private void OnProjectileParticleCollided(GameObject other)
        {
            if (other.TryGetComponent(out ICombatEventReceiver eventReceiver))
            {
                eventReceiver.TakeDamage(m_CombatStatData);
            }
        }

        public void StopTurret()
        {
            projectileParticleHandler.PauseEmitting();
            enabled = false;
        }

        public void ReStartTurret()
        {
            enabled = true;
        }

        #endregion
    }

    public enum TurretType
    {
        BasicRifle,
        ShotGun,
        FlameThrower,
        Mortar
    }
}