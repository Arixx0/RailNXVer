using Attributes;
using UnityEngine;

using Utility;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Units
{
    public class EnemySensingComponent : MonoBehaviour
    {
        #region Enemy Detect Option Fields
        
        [Header("Enemy Detect Options")]
        [SerializeField] private LayerMask enemyLayerMask;
        [Tooltip("적 감지 범위")]
        [SerializeField, Range(0, 100f)] private float acquisitionRange;
        [Tooltip("최대로 감지할 수")]
        [SerializeField] private int maximumSensingCount;
        [Tooltip("주위에 적을 해당 시간 후에 다시 감지")]
        [SerializeField] private float reSensingInterval = 3f;
        [Tooltip("배열 내에 가장 근접한 적을 해당 시간 후에 다시 감지")]
        [SerializeField] private float acquisitionCheckInterval;
        [Tooltip("체크시 가장 근접한 적을 탐색하지 않습니다")]
        [SerializeField] private bool notClosestEnemySense;

        [SerializeField, Disabled] private Collider[] enemyInAcquisitionRange;
        [SerializeField, Disabled] private int lastDetectedEnemyCount;
        [SerializeField, Disabled] private Transform currentTargetedEnemy;
        [SerializeField, Disabled] private Collider currentTargetedEnemyCollider;
        [SerializeField, Disabled] private bool m_ForcedAttack;

        private float sqrAcquisitionRange;
        private float m_ClosestEnemySensingElapsedTime;
        private float m_ReSensingElapsedTime;
        private float m_CurrentShortestDistance;
        private IDestroyedEventExecutor currentTargetedDestroyedEventExecutor;
        
        public float AcquisitionRange => acquisitionRange;

        public Transform CurrentTargetedEnemy => currentTargetedEnemy;

        public Vector3 CurrentTargetedEnemyColliderCenter => currentTargetedEnemyCollider.bounds.center;
        
        #endregion
        
        #region MonoBehaviour Wrappers

        private Transform m_CachedTransform;
        
        public Transform CachedTransform => m_CachedTransform ? m_CachedTransform : (m_CachedTransform = transform);
        
        #endregion
        
        #region MonoBehaviour Events
        
        private void Awake()
        {
            sqrAcquisitionRange = acquisitionRange * acquisitionRange;
            enemyInAcquisitionRange = new Collider[maximumSensingCount];
            lastDetectedEnemyCount = -1;
            currentTargetedEnemy = null;
            currentTargetedEnemyCollider = null;
            m_ReSensingElapsedTime = reSensingInterval;
            m_ClosestEnemySensingElapsedTime = acquisitionCheckInterval;
            m_CurrentShortestDistance = sqrAcquisitionRange * 2;
        }

        private void Update()
        {
            var localDeltaTime = Time.fixedDeltaTime * TimeScaleManager.Get.GetTimeScaleOfTag(tag);

            // consider that world is paused
            if (localDeltaTime <= 0)
            {
                return;
            }

#if UNITY_EDITOR
            currentTargetedEnemy = (currentTargetedEnemy == null) ? null : currentTargetedEnemy;
#endif

            m_ClosestEnemySensingElapsedTime += localDeltaTime;
            m_ReSensingElapsedTime += localDeltaTime;
            SenseEnemy();
        }
        
        #endregion

        #region Enemy Sensing Component Implementations
        
        private void SenseEnemy()
        {
            if (m_ReSensingElapsedTime >= reSensingInterval)
            {
                m_ReSensingElapsedTime = 0f;
                UpdateSenseEnemy();
            }

            if (m_ClosestEnemySensingElapsedTime >= acquisitionCheckInterval)
            {
                m_ClosestEnemySensingElapsedTime = 0f;
                SenseCloseEnemy(m_CurrentShortestDistance);
            }

            // check if the current targeted enemy is still in the acquisition range
            if (currentTargetedEnemy is not null && TargetOutOfRange(currentTargetedEnemy))
            {
                currentTargetedDestroyedEventExecutor?.UnregisterDestroyedEvent(ReleaseCurrentTargetedEnemy);

                currentTargetedEnemy = null;
                currentTargetedEnemyCollider = null;
                currentTargetedDestroyedEventExecutor = null;
                m_ForcedAttack = false;
            }
        }
        
        private void ReleaseCurrentTargetedEnemy()
        {
            Debug.Log("Targeted enemy is destroyed.");

            m_ReSensingElapsedTime = 0f;
            m_ClosestEnemySensingElapsedTime = 0f;
            currentTargetedEnemy = null;
            currentTargetedEnemyCollider = null;
            m_ForcedAttack = false;
            UpdateSenseEnemy();
            SenseCloseEnemy(m_CurrentShortestDistance);
        }

        private void UpdateSenseEnemy()
        {
            lastDetectedEnemyCount = Physics.OverlapSphereNonAlloc(
                CachedTransform.position, acquisitionRange, enemyInAcquisitionRange, enemyLayerMask.value, QueryTriggerInteraction.Ignore);
        }

        private void SenseCloseEnemy(float currentShortestDistance)
        {
            if (m_ForcedAttack || notClosestEnemySense && currentTargetedEnemy is not null)
            {
                return;
            }
            for (var i = 0; i < lastDetectedEnemyCount; ++i)
            {
                var sqrDistance = VectorUtility.DistanceInXZCoord(
                    CachedTransform.position, enemyInAcquisitionRange[i].transform.position, true);
                if (sqrDistance < currentShortestDistance)
                {
                    currentShortestDistance = sqrDistance;
                    currentTargetedEnemy = enemyInAcquisitionRange[i].transform;
                }
            }
            SetCurrentTargetedEnemyColider();
        }

        private void SetCurrentTargetedEnemyColider()
        {
            if (currentTargetedEnemy is null)
            {
                return;
            }

            if (currentTargetedEnemy.TryGetComponent(out currentTargetedEnemyCollider) &&
                currentTargetedEnemy.TryGetComponent(out currentTargetedDestroyedEventExecutor))
            {
                currentTargetedDestroyedEventExecutor.UnregisterDestroyedEvent(ReleaseCurrentTargetedEnemy);
                currentTargetedDestroyedEventExecutor.RegisterDestroyedEvent(ReleaseCurrentTargetedEnemy);
            }
            else
            {
                Debug.LogError($"{currentTargetedEnemy.gameObject.name} is not attached to the IDestroyEventExecutor or Collider");
            }

        }

        private bool TargetOutOfRange(Transform target)
        {
            var distance = VectorUtility.DistanceInXZCoord(
                CachedTransform.position, target.position, true);
            return (distance > sqrAcquisitionRange);
        }

        public void UpdateForceAttack(Transform target)
        {
            if(TargetOutOfRange(target))
            {
                return;
            }
            m_ForcedAttack = true;
            currentTargetedEnemy = target;
            SetCurrentTargetedEnemyColider();
        }

        #endregion
    }
    
#if UNITY_EDITOR
    [CustomEditor(typeof(EnemySensingComponent))]
    public class EnemySensingComponentEditor : Editor
    {
        private EnemySensingComponent m_Target;
        
        private void OnEnable()
        {
            m_Target = (EnemySensingComponent)target;
        }
        
        private void OnSceneGUI()
        {
            var transform = m_Target.CachedTransform;
            var position = transform.position;
            var acquisitionRange = m_Target.AcquisitionRange;
            var currentTargetedEnemy = m_Target.CurrentTargetedEnemy;
            var targetPosition = currentTargetedEnemy ? currentTargetedEnemy.position : position;
            
            Handles.color = Color.red;
            Handles.DrawWireArc(position, transform.up, transform.right, 360, acquisitionRange, 3f);
            Handles.Label(position + transform.forward * acquisitionRange, "visible range");

            Handles.color = Color.yellow;
            Handles.DrawLine(position, targetPosition, 2f);
        }
    }
#endif
}