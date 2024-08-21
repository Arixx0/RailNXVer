using UnityEngine;
using Projectiles;
using Utility;

namespace Units.Turrets
{
    public class MortarTurret : CombatUnit
    {
        [SerializeField] protected float gravity = -9.82f;
        [SerializeField] protected Transform muzzleTransform;
        [SerializeField] protected AudioSource projectileFireSound;
        [SerializeField] protected ParticleSystem muzzleFlashEffect;
        [SerializeField, Range(45f, 90f)] protected float projectileLaunchAngle;
        [SerializeField] protected MortarShell projectilePrefab;

        private float m_ProjectileLaunchAngleTan;
        private float m_LastAttackTime;

        //public MortarTurretStatComponent MortarTurretStatComponent => (MortarTurretStatComponent)statComponent;

        public Vector3 MuzzlePosition => muzzleTransform.position + muzzleTransform.forward * (projectilePrefab.ColliderRadius + 0.05f);

        protected override void Awake()
        {
            base.Awake();

            muzzleTransform.localRotation = muzzleTransform.rotation * Quaternion.AngleAxis(projectileLaunchAngle, Vector3.left);

            projectileFireSound = projectileFireSound == null ? null : projectileFireSound;

            muzzleFlashEffect = muzzleFlashEffect == null ? null : muzzleFlashEffect;
            
            m_ProjectileLaunchAngleTan = Mathf.Tan(projectileLaunchAngle * Mathf.Deg2Rad);

            projectilePrefab = Instantiate(projectilePrefab, CachedTransform);
            projectilePrefab.gameObject.SetActive(false);
            projectilePrefab.EnemyLayerMask = enemyLayerMask;
            projectilePrefab.DamageContextData = statComponent.CaptureCombatStatData();
        }
        
        private void Update()
        {
            if (!isActiveAndEnabled)
            {
                return;
            }
            
            AimAtEnemy();
            LaunchProjectile();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(MuzzlePosition, projectilePrefab.ColliderRadius);
        }

        private void AimAtEnemy()
        {
            if (!enemySensingComponent.CurrentTargetedEnemy)
            {
                return;
            }

            var position = CachedTransform.position;
            var enemyPosition = enemySensingComponent.CurrentTargetedEnemyColliderCenter;
            var dirTowardEnemy = (enemyPosition - position).GetXZ();

            var targetRotation = Quaternion.LookRotation(dirTowardEnemy, Vector3.up);
            CachedTransform.rotation = Quaternion.Lerp(CachedTransform.rotation, targetRotation, statComponent.RotateSpeed);

            /* muzzle should look at towards at the launch angle: realtime rotation is not necessary to muzzle
             *
            var targetMuzzleRotation = Quaternion.LookRotation(enemyPosition - muzzleTransform.position, Vector3.up) *
                                       Quaternion.AngleAxis(projectileLaunchAngle, Vector3.left);
            muzzleTransform.rotation =
                Quaternion.Lerp(muzzleTransform.rotation, targetMuzzleRotation, statComponent.RotateSpeed);
            */
        }

        private void LaunchProjectile()
        {
            if (enemySensingComponent.CurrentTargetedEnemy is null)
            {
                return;
            }
                
            var nextAvailableAttackTime = m_LastAttackTime + statComponent.AttackSpeed;
            if (nextAvailableAttackTime > Time.unscaledTime)
            {
                return;
            }
            
            var attackableSectorAlpha = Vector3.Dot(
                CachedTransform.forward,
                enemySensingComponent.CurrentTargetedEnemyColliderCenter - CachedTransform.position);
            if (attackableSectorAlpha < statComponent.AttackableSectorAngleAlpha)
            {
                return;
            }

            var distanceToEnemy = VectorUtility.DistanceInXZCoord(
                CachedTransform.position, enemySensingComponent.CurrentTargetedEnemyColliderCenter);
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
            
            var projectile = Instantiate(projectilePrefab, projectileSpawnPosition, Quaternion.identity, null);
            projectile.DamageContextData = statComponent.CaptureCombatStatData();
            projectile.gameObject.SetActive(true);
            projectile.InitialForce = globalVelocity;
            
            muzzleFlashEffect?.Play();
            projectileFireSound?.Play();
        }
    }
}