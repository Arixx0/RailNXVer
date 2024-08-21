using UnityEngine;

namespace TrainScripts
{
    public partial class Train
    {
        private const float TravelSpeedMinimumThreshold = 0.005f;
        
        [Header("Train Movement Options"), SerializeField]
        private bool canMove;
        
        [SerializeField] private bool syncTrainTransformToPath;
        [SerializeField, Tooltip("체크 시, 기차를 정렬 대상으로 취급해 첫번째 차량에도 오프셋이 적용됩니다.")] private bool regardTrainAsAlignTarget;
        [SerializeField] private StatusEffects.StatusEffect notEnoughPowerstoneDebuff;

        private float m_TravelDistanceOnPath;
        
        private float m_TotalTravelDistance;

        private float m_TravelSpeed;
        
        private float m_AdditiveTravelSpeed;
        
        private float m_AdditiveRateTravelSpeed;

        private bool m_EngineInActive;

        private VFX m_MoveVFX;

        public bool CanMove => canMove;

        public bool IsMoving => (m_TravelSpeed < TravelSpeedMinimumThreshold);

        public float CurrentTravelSpeed => m_TravelSpeed;
        
        public void SetMovementState(bool newState)
        {
            canMove = newState;
        }

        public void SetEngineInActive(bool newState)
        {
            m_EngineInActive = newState;
        }

        private void Move()
        {
            if (!canMove && IsMoving || m_ElectricPowerOverload || m_EngineInActive)
            {
                if (m_MoveVFX != null)
                {
                    m_MoveVFX.StopVFX();
                }
                return;
            }

            AlignTrainFromDistanceOnPath(m_TravelDistanceOnPath);
            UpdateMovementProgressState();
        }

        private void UpdateMovementProgressState()
        {
            var targetTravelSpeed = canMove ? statComponent.MoveSpeed : 0;
            m_TravelSpeed = Mathf.Lerp(m_TravelSpeed, targetTravelSpeed, statComponent.MoveSpeedDamp);

            var targetVelocity = m_TravelSpeed * TimeScaleManager.Get.GetDeltaTimeOfTag(tag);
            if (m_MoveVFX != null)
            {
                m_MoveVFX.PlayVFX(m_TravelSpeed);
            }

            m_TotalTravelDistance += targetVelocity;
            m_TravelDistanceOnPath += targetVelocity;
        }
    }
}