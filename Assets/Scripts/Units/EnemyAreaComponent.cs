using UnityEngine;
using TrainScripts;
using Units.Stats;
using Utility;

namespace Units.Enemies
{
    public class EnemyAreaComponent : MonoBehaviour
    {
        #region Enemy Area Fields

        [Header("Common Enemy Area Fields")]
        [Tooltip("선호지역 이동 대기 시간")]
        [SerializeField] private float waitingTime;

        [Header("Moving Enemy Area Fields")]
        [Tooltip("열차 이동시 앞서는 값")]
        [SerializeField] private float targetZPos = 3f;
        [Tooltip("열차 이동시 가속도 정도(목표 지역 도착시 감속)")]
        [SerializeField] private float decelerationRatio = 1f;
        [Tooltip("열차 이동시 목표 지점에서 감속시작할 거리")]
        [SerializeField] private float decelerationDistance = 3f;

        [Header("Stop Enemy Area Fields")]
        [Tooltip("열차 정지시 현재 위치에서 목표 위치까지 이 값보다 작으면 곡선 이동을 하지 않음")]
        [SerializeField] private float minCurvedDistance = 6f;
        [Tooltip("휘어지는 곡선의 정도")]
        [SerializeField] private float controlCurvedOffset = 5f;
        [Tooltip("휘어지는 곡선의 정확도")]
        [SerializeField] private int curvedPathCount = 10;

        private float m_MoveCoolTime;
        private float m_MoveSpeed;
        private int m_DirectionMultiplier;
        private int m_CurrentCurvedIndex;
        private int m_EnemyAttackType;
        private bool m_CanMove;
        private bool m_Deceleration;
        private Vector3 m_TargetPointDirection;
        private Vector3 m_TargetPoint;
        private Vector3[] m_CurvedTargetPoint;
        private GameObject m_CurrentTarget;
        private Transform m_CachedTransform;
        private UnitStatComponent m_StatComponent;

        #endregion

        #region MonoBehaviour Wrappers

        public Transform CachedTransform => m_CachedTransform ? m_CachedTransform : (m_CachedTransform = transform);

        public GameObject CurrentTarget
        {
            get => m_CurrentTarget;
            set => m_CurrentTarget = value;
        }

        public Vector3 TargetPoint => m_TargetPoint;

        public int DirectionMultiplier => m_DirectionMultiplier;

        public int EnemyAttackType => m_EnemyAttackType;

        public float MoveSpeed => m_MoveSpeed;

        #endregion

        #region MonoBehaviour Events

        private void Awake()
        {
            m_CurvedTargetPoint = new Vector3[curvedPathCount + 1];
            m_EnemyAttackType = TryGetComponent(out MeleeEnemyUnit _) ? 1 : 0;
        }

        private void Update()
        {
            EnemyAreaSetting();
        }

        #endregion

        #region Enemy Area Component Implementations

        private void EnemyAreaSetting()
        {
            if (m_CurrentTarget == null)
            {
                m_DirectionMultiplier = 0;
                m_TargetPoint = Vector3.zero;
                m_CanMove = false;
                for (int i = 0; i <= curvedPathCount; i++)
                {
                    m_CurvedTargetPoint[i] = Vector3.zero;
                }
                m_CurrentCurvedIndex = 0;
                return;
            }

            if (m_CurrentTarget.TryGetComponent(out Car car))
            {
                m_CanMove = car.ParentTrain.CanMove;
            }

            if (m_DirectionMultiplier == 0 || m_TargetPoint == Vector3.zero)
            {
                if (TryGetComponent(out UnitStatComponent statcomponent))
                {
                    m_StatComponent = statcomponent;
                    m_MoveSpeed = statcomponent.MoveSpeed;
                }
                var cross = Vector3.Cross((CurrentTarget.transform.position - CachedTransform.position).GetXZ(true),
                        CachedTransform.forward.GetXZ(true)).y;
                m_DirectionMultiplier = cross > 0 ? 1 : -1;
                SetTargetPoint();
            }

            if (m_CanMove)
            {
                m_TargetPoint = new Vector3(m_TargetPoint.x, m_TargetPoint.y, m_CurrentTarget.transform.position.z + targetZPos);
                if (VectorUtility.DistanceInXZCoord(CachedTransform.position, m_TargetPoint) <= decelerationDistance)
                {
                    m_MoveCoolTime += Time.fixedDeltaTime * TimeScaleManager.Get.GetTimeScaleOfTag(tag);
                    m_Deceleration = true;
                }
            }
            else if (CachedTransform.position.GetXZ() == m_TargetPoint.GetXZ())
            {
                m_MoveCoolTime += Time.fixedDeltaTime * TimeScaleManager.Get.GetTimeScaleOfTag(tag);
            }

            if (m_MoveCoolTime >= waitingTime)
            {
                m_MoveCoolTime = 0;
                SetTargetPoint();
                if (VectorUtility.DistanceInXZCoord(CachedTransform.position, m_TargetPoint) <= minCurvedDistance)
                {
                    m_CurvedTargetPoint[0] = Vector3.zero;
                }
                else
                {
                    SetCurvedTargetPoint();
                }
            }
            SetTargetPointInCurvedPoint();
            SetSpeed();
        }

        private Vector3 GetEnemyAreaRandomPosition()
        {
            if (m_CurrentTarget.TryGetComponent(out Car car))
            {
                return car.ParentTrain.EnemyAreaPosition(this);
            }
            return Vector3.zero;
        }

        private void SetTargetPoint()
        {
            const int maxAttemps = 100;
            int attemps = 0;
            do
            {
                m_TargetPoint = GetEnemyAreaRandomPosition();
                m_TargetPointDirection = (m_CurrentTarget.transform.position - m_TargetPoint).GetXZ();
                attemps++;
                if (attemps >= maxAttemps)
                {
                    m_CurrentTarget = null;
                    return;
                }
            } while (m_TargetPointDirection.sqrMagnitude > m_StatComponent.SqrThresholdAttackRange);
        }

        private void SetCurvedTargetPoint()
        {
            if (m_EnemyAttackType == 1)
            {
                return;
            }
            if (!m_CanMove)
            {
                for (int i = 0; i <= curvedPathCount; i++)
                {
                    float t = (float)i / curvedPathCount;

                    m_CurvedTargetPoint[i] = Mathf.Pow(1 - t, 2) * m_CachedTransform.position.GetXZ() +
                                             2 * (1 - t) * t * GetControlPoint().GetXZ() +
                                             Mathf.Pow(t, 2) * m_TargetPoint.GetXZ();
                }
            }
        }

        private void SetTargetPointInCurvedPoint()
        {
            if (m_CanMove || m_CurvedTargetPoint[0] == Vector3.zero || m_EnemyAttackType == 1)
            {
                return;
            }
            if (m_CurrentCurvedIndex >= m_CurvedTargetPoint.Length - 1)
            {
                m_CurrentCurvedIndex = 0;
            }
            if (VectorUtility.DistanceInXZCoord(CachedTransform.position, m_CurvedTargetPoint[m_CurrentCurvedIndex]) <= 1f)
            {
                if (m_CurrentCurvedIndex < m_CurvedTargetPoint.Length - 1)
                {
                    m_CurrentCurvedIndex++;
                }
                m_TargetPoint = m_CurvedTargetPoint[m_CurrentCurvedIndex];
                m_MoveSpeed = m_StatComponent.MoveSpeed * (float)(m_CurrentCurvedIndex / (float)(m_CurvedTargetPoint.Length - 1));
            }
        }

        private void SetSpeed()
        {
            if (!m_CanMove)
            { 
                return;
            }
            if (m_Deceleration)
            {
                m_MoveSpeed -= decelerationRatio;
            }
            else
            {
                m_MoveSpeed += decelerationRatio;
                if (m_MoveSpeed >= m_StatComponent.MoveSpeed)
                {
                    m_MoveSpeed = m_StatComponent.MoveSpeed;
                }
            }

            if (m_MoveSpeed < 0.1f)
            {
                m_Deceleration = false;
            }
        }

        private Vector3 GetControlPoint()
        {
            Vector3 midPoint = (CachedTransform.position + m_TargetPoint) / 2;
            Vector3 direction = (m_TargetPoint - CachedTransform.position).normalized;
            Vector3 perpendicular = new Vector3(-direction.z, 0, direction.x);
            return midPoint + perpendicular * controlCurvedOffset;
        }
        #endregion
    }
}