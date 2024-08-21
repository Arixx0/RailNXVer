using Attributes;
using UnityEngine;
using UnityEngine.AI;

using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

using Projectiles;
using Utility;

namespace Units
{
    public class Dummy : MonoBehaviour
    {
        [SerializeField] private EnemySensingComponent enemySensor;
        [SerializeField] private NavMeshAgent navigationAgent;
        [SerializeField] private new Rigidbody rigidbody;
        [SerializeField] private Transform cachedTransform;
        [SerializeField] private ProjectileParticleHandler projectileParticleHandler;
        [SerializeField] private float attackRange;
        [SerializeField] private float navMeshPathRequestCooldown;
        [SerializeField] private float rotateSpeed;
        [SerializeField, Range(1f, 360f)] private float visionRange;

        [Disabled, SerializeField] private bool isEnemyDetected;
        [Disabled, SerializeField] private bool isEnemyInAttackRange;
        [Disabled, SerializeField] private bool isPathAgentPending;
        [Disabled, SerializeField] private float navMeshPathRequestTimer;

        private float m_AttackRangeSqr;
        
        private void OnEnable()
        {
            cachedTransform = GetComponent<Transform>();
            
            m_AttackRangeSqr = attackRange * attackRange;
        }

        [System.Serializable]
        public class SharedDummy : SharedVariable<Dummy>
        {
            public static implicit operator SharedDummy(Dummy value)
            {
                return new SharedDummy { mValue = value };
            }
        }
        
        [TaskCategory("Traincraft/Dummy")]
        public class ManipulateBehaviourProperties : Action
        {
            public SharedDummy dummy;
            public SharedBool isEnemyDetected;
            public SharedBool isEnemyWithinAttackRange;
            public SharedVector3 displacement;
            
            public override TaskStatus OnUpdate()
            {
                Debug.Log(nameof(ManipulateBehaviourProperties));
                
                isEnemyDetected.Value = dummy.Value.enemySensor.CurrentTargetedEnemy is not null;
                if (isEnemyDetected.Value == false)
                {
                    return TaskStatus.Success;
                }

                displacement.Value = (dummy.Value.enemySensor.CurrentTargetedEnemyColliderCenter -
                                      dummy.Value.cachedTransform.position).GetXZ();
                isEnemyWithinAttackRange.Value = displacement.Value.sqrMagnitude <= dummy.Value.m_AttackRangeSqr;

                return TaskStatus.Success;
            }
        }

        [TaskCategory("Traincraft/Dummy")]
        public class RotateTowardsEnemy : Action
        {
            public SharedDummy dummy;
            public SharedBool isEnemyDetected;
            public SharedVector3 displacement;
            
            public override TaskStatus OnUpdate()
            {
                Debug.Log(nameof(RotateTowardsEnemy));
                
                if (!isEnemyDetected.Value)
                {
                    return TaskStatus.Success;
                }
                
                // dummy.Value.rigidbody.MoveRotation(Quaternion.LookRotation(displacement.Value));

                var rotation = dummy.Value.cachedTransform.rotation;
                rotation = Quaternion.Lerp(rotation, Quaternion.LookRotation(displacement.Value),
                    dummy.Value.rotateSpeed);
                dummy.Value.cachedTransform.rotation = rotation;
                
                return TaskStatus.Success;
            }
        }
        
        [TaskCategory("Traincraft/Dummy")]
        public class MoveTowardsEnemy : Action
        {
            public SharedDummy dummy;
            public SharedBool isEnemyDetected;
            public SharedBool isEnemyWithinAttackRange;
            
            private float m_NavMeshPathRequestTimer;

            public override void OnStart()
            {
                base.OnStart();

                m_NavMeshPathRequestTimer = dummy.Value.navMeshPathRequestCooldown;
            }

            public override TaskStatus OnUpdate()
            {
                Debug.Log(nameof(MoveTowardsEnemy));

                if (!isEnemyDetected.Value || isEnemyWithinAttackRange.Value)
                {
                    dummy.Value.navigationAgent.isStopped = true;
                    return TaskStatus.Success;
                }
                
                m_NavMeshPathRequestTimer += Time.deltaTime;
                if (m_NavMeshPathRequestTimer < dummy.Value.navMeshPathRequestCooldown)
                {
                    return TaskStatus.Success;
                }
                
                m_NavMeshPathRequestTimer -= dummy.Value.navMeshPathRequestCooldown;
                dummy.Value.navigationAgent.SetDestination(dummy.Value.enemySensor.CurrentTargetedEnemyColliderCenter);
                dummy.Value.navigationAgent.isStopped = false;
                return TaskStatus.Success;
            }
        }

        [TaskCategory("Traincraft/Dummy")]
        public class AttackEnemy : Action
        {
            public SharedDummy dummy;
            public SharedBool isEnemyDetected;
            public SharedBool isEnemyWithinAttackRange;

            public override TaskStatus OnUpdate()
            {
                Debug.Log(nameof(AttackEnemy));
                
                if (!isEnemyDetected.Value)
                {
                    dummy.Value.projectileParticleHandler.PauseEmitting();
                    return TaskStatus.Success;
                }

                if (!isEnemyWithinAttackRange.Value)
                {
                    dummy.Value.projectileParticleHandler.PauseEmitting();
                    return TaskStatus.Success;
                }
                
                dummy.Value.projectileParticleHandler.StartEmitting();
                return TaskStatus.Success;
            }
        }
    }
}