using UnityEngine;

using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

using Projectiles;
using Utility;

namespace Units.Enemies
{
    public class HarpoonTurret : RangedEnemyUnit
    {
        #region Harpoon Turret Components

        [Header("Harpoon Turret Components")]
        [SerializeField] private Transform muzzleTransform;
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private ProjectileHarpoon projectileHarpoon;

        #endregion

        #region MonoBehaviour Events

        protected override void Awake()
        {
            base.Awake();
            lineRenderer.SetPosition(1, muzzleTransform.position);
        }

        protected override void Start()
        {
            base.Start();
            projectileHarpoon.OnHarpoonCollided += OnHarpoonCollided;
            projectileHarpoon.OnHarpoonCollidedExit += OnHarpponCollidedExit;
        }

        #endregion
        protected override void PlayAttackEffect()
        {
            base.PlayAttackEffect();
            enemyUnitAnimator?.SetTrigger("Attack");
        }

        private void OnHarpoonCollided(GameObject other)
        {
            if (other.TryGetComponent(out ICombatEventReceiver eventReceiver))
            {
                eventReceiver.TakeDamage(m_CombatStatData);
                Debug.Log("Slow Status Effect On");
            }
        }

        private void OnHarpponCollidedExit(GameObject other)
        {
            Debug.Log("Slow Satus Effect Off");
        }

        #region Harpoon Turret Behavior Action Task

        [TaskCategory("Traincraft/EnemyUnit/InitializeAction")]
        public class HarpoonTurretInitialize : EnemyUnitCommonInitialize
        {
            public Shared<HarpoonTurret> harpoonTurret;
            public SharedBool isEnemyAttack;
            public SharedBool isEnemyAttackEnd;
            public SharedBool isConnectedHarpoon;
            
            public override TaskStatus OnUpdate()
            {
                var status = base.OnUpdate();
                harpoonTurret.Value.lineRenderer.SetPosition(0, harpoonTurret.Value.muzzleTransform.position);

                if (status == TaskStatus.Failure || isEnemyAttackEnd.Value)
                {
                    float positionSpeedPerFrame = isConnectedHarpoon.Value ? harpoonTurret.Value.statComponent.AttackSpeed : harpoonTurret.Value.statComponent.AttackSpeed * Time.deltaTime;
                    Vector3 currentHarpoonPosition = harpoonTurret.Value.projectileHarpoon.transform.position;
                    Vector3 newEndPosition = Vector3.MoveTowards(currentHarpoonPosition, harpoonTurret.Value.muzzleTransform.position, positionSpeedPerFrame);
                    harpoonTurret.Value.lineRenderer.SetPosition(1, newEndPosition);
                    harpoonTurret.Value.projectileHarpoon.transform.position = newEndPosition;
                    isEnemyAttack.Value = false;
                }

                var distance = VectorUtility.DistanceInXZCoord(harpoonTurret.Value.muzzleTransform.position, harpoonTurret.Value.projectileHarpoon.transform.position);

                if (distance <= 0.01f)
                {
                    isEnemyAttackEnd.Value = false;
                    isConnectedHarpoon.Value = false;
                }

                return status;
            }
        }

        [TaskCategory("Traincraft/EnemyUnit/AttackAction")]
        public class HarpoonTurretAttackAction : EnemyUnitCommonAttackAction
        {
            public Shared<HarpoonTurret> harpoonTurret;
            public SharedBool isEnemyAttack;
            public SharedBool isEnemyAttackEnd;
            public SharedBool isConnectedHarpoon;
            public SharedVector3 hitPosition;

            public override TaskStatus OnUpdate()
            {
                if (!isEnemyDetected.Value || !isEnemyWithinAttackRange.Value || !isPossibleAttack.Value)
                {
                    return TaskStatus.Failure;
                }

                Physics.Raycast(harpoonTurret.Value.muzzleTransform.position, harpoonTurret.Value.muzzleTransform.up, out var hit, harpoonTurret.Value.statComponent.AttackRange, harpoonTurret.Value.enemyLayerMask);

                if (isConnectedHarpoon.Value)
                {
                    hitPosition.Value = hit.point;
                    harpoonTurret.Value.lineRenderer.SetPosition(1, hitPosition.Value);
                    harpoonTurret.Value.projectileHarpoon.transform.position = hitPosition.Value;
                    isEnemyAttack.Value = false;
                }

                else if (!isEnemyAttack.Value)
                {
                    isEnemyAttack.Value = true;
                    hitPosition.Value = hit.point;
                    harpoonTurret.Value.PlayAttackEffect();
                    return TaskStatus.Success;
                }

                if (isEnemyAttack.Value && !isEnemyAttackEnd.Value && !isConnectedHarpoon.Value)
                {
                    Vector3 currentHarpoonPosition = harpoonTurret.Value.projectileHarpoon.transform.position;
                    Vector3 newEndPosition = Vector3.MoveTowards(currentHarpoonPosition, hitPosition.Value, harpoonTurret.Value.statComponent.AttackSpeed * Time.deltaTime);
                    harpoonTurret.Value.lineRenderer.SetPosition(1, newEndPosition);
                    harpoonTurret.Value.projectileHarpoon.transform.position = newEndPosition;
                }

                var distance = VectorUtility.DistanceInXZCoord(hitPosition.Value, harpoonTurret.Value.projectileHarpoon.transform.position);

                if (distance <= 0.01f)
                {
                    isConnectedHarpoon.Value = harpoonTurret.Value.projectileHarpoon.isConnected;
                    isEnemyAttackEnd.Value = !isConnectedHarpoon.Value;
                }

                return TaskStatus.Failure;
            }
        }

        #endregion
    }
}