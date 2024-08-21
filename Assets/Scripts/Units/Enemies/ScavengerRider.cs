using UnityEngine;

namespace Units.Enemies
{
    public class ScavengerRider : RangedEnemyUnit
    {
        #region Scavenger Rider Components

        [Header("Scavenger Rider Components")]
        [SerializeField] private EnemyAreaComponent enemyAreaComponent;
        [SerializeField] private float fireInterval;

        #endregion

        #region MonoBehaviour Events

        protected override void Start()
        {
            base.Start();
            navigationAgent.updateRotation = false;
            projectileParticleHandler.SetProjectileType(statComponent, fireInterval);
        }

        #endregion

    }
}