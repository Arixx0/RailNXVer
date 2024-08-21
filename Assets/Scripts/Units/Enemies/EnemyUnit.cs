using UnityEngine;
using UnityEngine.AI;

using Data;
using Utility;

using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Units.Stats;

namespace Units.Enemies
{
    public abstract class EnemyUnit : CombatUnit
    {
        [Space, Header("Enemy Unit Common Components")]
        [SerializeField] protected TextIdentifier enemyUnitIdentifier;
        [SerializeField] protected NavMeshAgent navigationAgent;
        [SerializeField] protected Animator enemyUnitAnimator;

        public UnitStatComponent StatComponent => statComponent;

        public TextIdentifier EnemyUnitIdentifier => enemyUnitIdentifier;

        protected virtual void Start()
        {
            Debug.Assert(!string.IsNullOrEmpty(enemyUnitIdentifier.Identifier));
            enemyUnitIdentifier.Set(enemyUnitIdentifier);

            if (DatabaseUtility.TryGetData(Database.UnitStatData, enemyUnitIdentifier.Identifier, out var statData))
            {
                statComponent.Set(statData);
            }

            if (healthBar != null)
            {
                healthBar.SetHealthProperties(statComponent.MaxHealth, statComponent.CurrentHealth);
            }
            else
            {
                healthBar = null;
            }

            if (navigationAgent != null)
            {
                navigationAgent.speed = statComponent.MoveSpeed;
                navigationAgent.angularSpeed = statComponent.RotateSpeed;
            }

            m_CombatStatData = statComponent.CaptureCombatStatData();
        }

        public override void ReSetupStat()
        {
            base.ReSetupStat();
            if (navigationAgent != null)
            {
                navigationAgent.speed = statComponent.MoveSpeed;
                navigationAgent.angularSpeed = statComponent.RotateSpeed;
            }
        }

        public class Shared<T> : SharedVariable<T>
        {
            public static implicit operator Shared<T>(T value)
            {
                return new Shared<T> { mValue = value };
            }
        }

        #region Enemy Unit Common Behavior Action Task

        #region Initialize Action Task

        [TaskCategory("Traincraft/EnemyUnit/InitializeAction")]
        public class EnemyUnitCommonInitialize : Action
        {
            public Shared<EnemyUnit> enemyUnit;
            public SharedBool isEnemyDetected;
            public SharedBool isEnemyWithinAttackRange;
            public SharedVector3 displacement;
            public SharedVector3 targetPoint;

            public override TaskStatus OnUpdate()
            {
                isEnemyDetected.Value = enemyUnit.Value.SensingComponent.CurrentTargetedEnemy is not null;

                displacement.Value = isEnemyDetected.Value
                    ? (enemyUnit.Value.enemySensingComponent.CurrentTargetedEnemyColliderCenter - enemyUnit.Value.CachedTransform.position).GetXZ()
                    : Vector3.zero;

                isEnemyWithinAttackRange.Value = isEnemyDetected.Value &&
                                                 (displacement.Value.sqrMagnitude <= enemyUnit.Value.statComponent.SqrAttackRange);

                targetPoint.Value = SetTargetPoint();

                if (!isEnemyDetected.Value || !isEnemyWithinAttackRange.Value)
                {
                    return TaskStatus.Failure;
                }

                return TaskStatus.Success;
            }

            protected virtual Vector3 SetTargetPoint()
            {
                return isEnemyDetected.Value ? enemyUnit.Value.SensingComponent.CurrentTargetedEnemyColliderCenter : Vector3.zero;
            }
        }

        [TaskCategory("Traincraft/EnemyUnit/InitializeAction")]
        public class AreaUnitCommonInitialize : EnemyUnitCommonInitialize
        {
            public Shared<EnemyAreaComponent> enemyAreaComponent;
            public Shared<RangedEnemyUnit> rangedEnemyUnit;

            public override TaskStatus OnUpdate()
            {
                var status = base.OnUpdate();
                
                enemyAreaComponent.Value.CurrentTarget = isEnemyDetected.Value
                        ? enemyUnit.Value.SensingComponent.CurrentTargetedEnemy.gameObject
                        : null;

                if (status == TaskStatus.Failure && rangedEnemyUnit.Value != null)
                {
                    rangedEnemyUnit.Value.ProjectileParticleHandler.PauseEmitting();
                }

                return status;
            }

            protected override Vector3 SetTargetPoint()
            {
                return enemyAreaComponent.Value.TargetPoint;
            }
        }

        #endregion

        #region Rotate Action Task

        [TaskCategory("Traincraft/EnemyUnit/RotateAction")]
        public class EnemyUnitCommonRotateAction : Action
        {
            public Shared<EnemyUnit> enemyUnit;
            public SharedBool isEnemyDetected;
            public SharedBool isPossibleAttack;
            public SharedTransform rotateObject;
            public SharedVector3 displacement;

            public override TaskStatus OnUpdate()
            {
                if (!isEnemyDetected.Value)
                {
                    return TaskStatus.Failure;
                }

                var currentRotation = rotateObject.Value.rotation;
                var targetRotation = Quaternion.LookRotation(displacement.Value);
                float rotateSpeedPerFrame = enemyUnit.Value.statComponent.RotateSpeed * Time.deltaTime;
                var newRotation = Quaternion.RotateTowards(currentRotation, targetRotation, rotateSpeedPerFrame);
                rotateObject.Value.rotation = newRotation;
                isPossibleAttack.Value = false;

                if (Quaternion.Angle(currentRotation, targetRotation) < 0.5f)
                {
                    isPossibleAttack.Value = true;
                    return TaskStatus.Failure;
                }

                return TaskStatus.Success;
            }
        }

        #endregion

        #region Move Action Task

        [TaskCategory("Traincraft/EnemyUnit/MoveAction")]
        public class EnemyUnitCommonMoveAction : Action
        {
            public Shared<EnemyUnit> enemyUnit;
            public SharedBool isEnemyDetected;
            public SharedBool isEnemyWithinAttackRange;
            public SharedVector3 targetPoint;

            private float m_NavMeshPathRequestTimer;

            public override void OnStart()
            {
                m_NavMeshPathRequestTimer = enemyUnit.Value.navigationPathRequestInterval;
            }

            public override TaskStatus OnUpdate()
            {
                if (!isEnemyDetected.Value || isEnemyWithinAttackRange.Value)
                {
                    enemyUnit.Value.navigationAgent.isStopped = true;
                    return TaskStatus.Failure;
                }

                m_NavMeshPathRequestTimer += Time.deltaTime;
                if (m_NavMeshPathRequestTimer < enemyUnit.Value.navigationPathRequestInterval)
                {
                    return TaskStatus.Failure;
                }

                m_NavMeshPathRequestTimer -= enemyUnit.Value.navigationPathRequestInterval;
                enemyUnit.Value.navigationAgent.SetDestination(targetPoint.Value);
                enemyUnit.Value.navigationAgent.isStopped = false;

                return TaskStatus.Success;
            }
        }

        [TaskCategory("Traincraft/EnemyUnit/MoveAction")]
        public class AreaUnitCommonMoveAction : EnemyUnitCommonMoveAction
        {
            public Shared<EnemyAreaComponent> enemyAreaComponent;

            public override TaskStatus OnUpdate()
            {
                if (!isEnemyDetected.Value)
                {
                    enemyUnit.Value.navigationAgent.isStopped = true;
                    return TaskStatus.Failure;
                }

                enemyUnit.Value.navigationAgent.speed = enemyAreaComponent.Value.MoveSpeed;
                enemyUnit.Value.navigationAgent.SetDestination(targetPoint.Value);
                enemyUnit.Value.navigationAgent.isStopped = false;

                return TaskStatus.Success;
            }
        }

        #endregion

        #region Attack Action Task

        [TaskCategory("Traincraft/EnemyUnit/AttackAction")]
        public class EnemyUnitCommonAttackAction : Action
        {
            public Shared<EnemyUnit> enemyUnit;
            public SharedBool isEnemyDetected;
            public SharedBool isEnemyWithinAttackRange;
            public SharedBool isPossibleAttack;

            private float m_AttackCoolTime;

            public override TaskStatus OnUpdate()
            {
                if (!isEnemyDetected.Value || !isEnemyWithinAttackRange.Value || !isPossibleAttack.Value)
                {
                    return TaskStatus.Failure;
                }

                return TaskStatus.Success;
            }
        }

        #endregion

        #endregion
    }

    public enum EnemyType
    {
        Normal,
        Elite,
        Boss
    }
}