using Units.Stats;
using UnityEngine;
using UnityEngine.AI;
using Utility;

namespace Units
{
    public class UnitMovement : MonoBehaviour
    {
        [SerializeField] protected new Rigidbody rigidbody;
        [SerializeField] protected NavMeshAgent navigationAgent;
        [SerializeField] protected LayerMask collisionMask;
        
        [Space, SerializeField] protected float moveSpeed = 1f;
        [SerializeField] protected float moveAcceleration = 0.1f;
        [SerializeField] protected float rotateSpeed = 0.1f;
        
        [Space, SerializeField] protected float pathingRadius = 0.5f;
        [SerializeField] protected float sightAngle = 90f;
        [SerializeField] protected float sightAngleInterval = 15f;
        [SerializeField] protected float sightDistance = 5f;
        [SerializeField] protected float turnThresholdAngle = 45f;
        
        protected Transform cachedTransform;

        private Vector3 m_TargetPoint;
        private NavMeshPath m_NavMeshPath;
        private int m_NavMeshPathIndex = -1;
        private Vector3 m_MoveAmount;
        private float m_SqrPathingRadius;
        private float m_TurnThresholdAngleDot;

        public float Velocity => m_MoveAmount.sqrMagnitude;

        private void Awake()
        {
            cachedTransform = GetComponent<Transform>();

            if (rigidbody == null)
            {
                rigidbody = null;
            }
        }

        private void OnEnable()
        {
            m_TargetPoint = cachedTransform.position;
            m_SqrPathingRadius = pathingRadius * pathingRadius;
            m_TurnThresholdAngleDot = Quaternion.Dot(Quaternion.identity, Quaternion.Euler(0, turnThresholdAngle / 2f, 0));
        }

        protected virtual void Update()
        {
            var destination = GetNextNavigationPoint();
            var moveAmount = Vector3.zero;
            var position = cachedTransform.position;
            var rotation = cachedTransform.rotation;
            var rotator = rotation;
            var displacement = (destination - position).GetXZ();
            
            CalculateRotator(displacement, ref rotator);
            CalculateMoveAmount(displacement, ref moveAmount, ref rotator);
            
            // rotator = Quaternion.Lerp(rotation, rotator, rotateSpeed);
            // m_MoveAmount = Vector3.Lerp(m_MoveAmount, moveAmount * moveSpeed, moveAcceleration);
            // cachedTransform.SetPositionAndRotation(position + m_MoveAmount * Time.deltaTime, rotator);
            
            rigidbody.Move(position + m_MoveAmount * Time.deltaTime, rotator);
        }

        public void UpdateProperties(UnitStatComponent statComponent)
        {
            moveSpeed = statComponent.MoveSpeed;
            moveAcceleration = statComponent.MoveSpeedDamp;
            rotateSpeed = statComponent.RotateSpeed;
            m_TurnThresholdAngleDot = statComponent.TurnThresholdAngleAlpha;
        }

        public virtual void Move(Vector3 targetPoint)
        {
            var displacement = (m_TargetPoint - targetPoint);
            if (displacement.sqrMagnitude < m_SqrPathingRadius)
            {
                return;
            }
            
            m_TargetPoint = targetPoint;

            var didPathFind = navigationAgent.CalculatePath(targetPoint, m_NavMeshPath);
            m_NavMeshPathIndex = didPathFind ? 0 : -1;
        }

        protected virtual Vector3 GetNextNavigationPoint()
        {
            if (m_NavMeshPathIndex < 0 || m_NavMeshPath == null)
            {
                return m_TargetPoint;
            }

            var destination = m_NavMeshPath.corners[m_NavMeshPathIndex];
            var displacement = (cachedTransform.position - destination);
            
            // if the displacement is less than the pathing radius, move to the next corner
            if (displacement.sqrMagnitude < m_SqrPathingRadius)
            {
                m_NavMeshPathIndex += 1;
                if (m_NavMeshPathIndex >= m_NavMeshPath.corners.Length)
                {
                    m_NavMeshPathIndex = -1;
                }
            }

            return destination;
        }

        protected virtual void CalculateRotator(Vector3 displacement, ref Quaternion rotator)
        {
            if (displacement.sqrMagnitude < m_SqrPathingRadius)
            {
                return;
            }

            rotator = Quaternion.LookRotation(displacement, Vector3.up);
        }

        protected virtual void CalculateMoveAmount(Vector3 displacement, ref Vector3 moveAmount, ref Quaternion rotator)
        {
            if (displacement.sqrMagnitude < m_SqrPathingRadius)
            {
                moveAmount = Vector3.zero;
                return;
            }

            var lookDirDot = Quaternion.Dot(cachedTransform.rotation, rotator);
            if (Mathf.Abs(lookDirDot) <= m_TurnThresholdAngleDot)
            {
                moveAmount = Vector3.zero;
                return;
            }
            
            var forward = rotator * Vector3.forward;
            var ray = new Ray { origin = cachedTransform.position };
            for (var angle = 0f; angle <= sightAngle; angle += sightAngleInterval)
            {
                var yaw = Quaternion.AngleAxis(angle, Vector3.up);
                ray.direction = yaw * forward;
                if (!Physics.Raycast(ray, out _, sightDistance, collisionMask, QueryTriggerInteraction.Ignore))
                {
                    rotator *= yaw;
                    break;
                }
                
                yaw = Quaternion.AngleAxis(-angle, Vector3.up);
                ray.direction = yaw * forward;
                if (!Physics.Raycast(ray, out _, sightDistance, collisionMask, QueryTriggerInteraction.Ignore))
                {
                    rotator *= yaw;
                    break;
                }
            }

            moveAmount = cachedTransform.forward;
        }
    }
}