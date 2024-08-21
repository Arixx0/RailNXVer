using UnityEngine;

namespace Data
{
    public static partial class Database
    {
        public static GlobalBalanceSetting GlobalBalanceSetting;

        public static void AssignGlobalBalanceSetting(GlobalBalanceSetting data)
        {
            GlobalBalanceSetting = data;
        }
    }

    [CreateAssetMenu(menuName = "TrainCraft/Global Balance Setting")]
    public class GlobalBalanceSetting : ScriptableObject
    {
        [Header("Interval Setting")]
        public float patrolSquadSpawnInterval;
        public float circuitFailureInterval;
        public float powerStoneConsumeInterval;

        [Space, Header("Car Circuit Failure Value Setting")]
        [Tooltip("해당 퍼센트만큼의 체력이면 회로 파손 타이머 시작 : \n ((CircuitFailureMultiplier * 차량 현재 체력 퍼센트) + CircuitFailureAddition) * 차량 안전성 수치")]
        public float circuitFailureHealthPercentage;
        [Tooltip("회로 수리 시간 : 해당 값 + 차량 업그레이드 레벨")]
        public float circuitDefaultRepairTime;
        [Tooltip("차량 파손 타이머 경과후 해당 차량이 작업중이면 회로 파손 대기 시간")]
        public float circuitFailureWorkingDelay;
        [Tooltip("전체 파손 타이머 경과후 모든 차량이 작업중이면 해당 차량 회로 파손 대기 시간")]
        public float circuitFailureAllWorkingDelay;
        [Tooltip("회로 파손 시간 곱셈 값")]
        public float circuitFailureMultiplier;
        [Tooltip("회로 파손 시간 덧셈 값")]
        public float circuitFailureAddition;

        [Space, Header("Car Durability Repair Value Setting")]
        [Tooltip("체력 리젠 값 : 전체 체력 / (빌드 딜레이 * 해당 값) \n체력 리젠 비율 : Min(잃은 체력, 체력 리젠 값) / 체력 리젠 값\n 체력 리젠 값의 최솟값은 1")]
        public float healthRegenMultiplier;
        [Tooltip("수리 비용 : 수리 차량 CostData * (해당 값 * 체력 리젠 비율)")]
        public float DurabilityRepairCostMultiplier;
    }
}
