using UnityEngine;

using Projectiles;

namespace Units.Enemies
{
    public class ScrapTank : RangedEnemyUnit
    {
        [Header("Scrap Tank Components")]
        [SerializeField] private EnemyAreaComponent enemyAreaComponent;
        [SerializeField] private Transform muzzleTransform;
        [SerializeField] private float projectileRange;

        private float m_LastAttackTime;

        #region MonoBehaviour Events

        private void OnDrawGizmosSelected()
        {
            if (muzzleTransform != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(muzzleTransform.position + muzzleTransform.forward * (projectileRange + 0.05f), projectileRange);
            }
        }

        protected override void Start()
        {
            base.Start();
            navigationAgent.updateRotation = false;
        }

        #endregion

        protected override void OnProjectileParticleCollided(GameObject other)
        {
            var hitColliders = new Collider[8];
            var hitCount = Physics.OverlapSphereNonAlloc(other.transform.position, projectileRange,
                hitColliders, enemyLayerMask.value, QueryTriggerInteraction.Ignore);

            for (var i = 0; i < hitCount; ++i)
            {
                if (hitColliders[i].TryGetComponent(out ICombatEventReceiver eventReceiver))
                {
                    eventReceiver.TakeDamage(m_CombatStatData);
                }
            }
        }

    }
}