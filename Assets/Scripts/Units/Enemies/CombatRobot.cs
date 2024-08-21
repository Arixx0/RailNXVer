using UnityEngine;

using BehaviorDesigner.Runtime.Tasks;

namespace Units.Enemies
{
    public class CombatRobot : MeleeEnemyUnit
    {
        #region CombatRobot Components

        [Header("CombatRobot Components")]
        [SerializeField] private EnemyAreaComponent enemyAreaComponent;
        [SerializeField] private ParticleSystem combatRobotLeftAttackFX;
        [SerializeField, UnityEngine.Tooltip("해당 클립에 Left FX 재생")] private string combatRobotLeftAttackClipName;
        [SerializeField] private string combatRobotLeftAttack_2ClipName;
        [SerializeField] private ParticleSystem combatRobotRightAttackFX;
        [SerializeField, UnityEngine.Tooltip("해당 클립에 Right FX 재생")] private string combatRobotRightAttackClipName;
        [SerializeField] private string combatRobotRightAttack_2ClipName;

        #endregion

        protected override void Start()
        {
            base.Start();
            navigationAgent.updateRotation = false;
        }

        protected override void PlayAttackEffect()
        {
            base.PlayAttackEffect();
            enemyUnitAnimator?.SetInteger("AttackDirection", Random.Range(0, 2) == 0 ? 1 : -1);

            if (enemyUnitAnimator?.GetInteger("AttackDirection") == 1)
            {
                combatRobotRightAttackFX?.Play();
            }
            else if (enemyUnitAnimator?.GetInteger("AttackDirection") == -1)
            {
                combatRobotLeftAttackFX?.Play();
            }

        }

        private bool AttackAniPlayCheck()
        {
            if (enemyUnitAnimator.GetCurrentAnimatorStateInfo(0).IsName(combatRobotRightAttackClipName)
                || enemyUnitAnimator.GetCurrentAnimatorStateInfo(0).IsName(combatRobotLeftAttackClipName)
                || enemyUnitAnimator.GetCurrentAnimatorStateInfo(0).IsName(combatRobotRightAttack_2ClipName)
                || enemyUnitAnimator.GetCurrentAnimatorStateInfo(0).IsName(combatRobotLeftAttack_2ClipName))
            {
                return false;
            }
            return true;
        }

        #region CombatRobot Behavior Action Task

        [TaskCategory("Traincraft/EnemyUnit/InitializeAction")]
        public class CombatRobotInitialize : AreaUnitCommonInitialize
        {
            public Shared<CombatRobot> combatRobot;

            public override TaskStatus OnUpdate()
            {
                var status = base.OnUpdate();

                if (status == TaskStatus.Failure)
                {
                    combatRobot.Value.enemyUnitAnimator.SetFloat("Speed", 0);
                }

                return status;
            }
        }

        [TaskCategory("Traincraft/EnemyUnit/MoveAction")]
        public class CombatRobotMoveAction : AreaUnitCommonMoveAction
        {
            public Shared<CombatRobot> combatRobot;

            public override TaskStatus OnUpdate()
            {
                var status = base.OnUpdate();

                if (status == TaskStatus.Success)
                {
                    combatRobot.Value.enemyUnitAnimator.SetBool("IsMoving", true);
                    combatRobot.Value.enemyUnitAnimator.SetFloat("Speed", combatRobot.Value.navigationAgent.speed);
                }

                return status;
            }
        }

        [TaskCategory("Traincraft/EnemyUnit/AttackAction")]
        public class CombatRobtAttackAction : EnemyUnitCommonAttackAction
        {
            public Shared<CombatRobot> combatRobot;
            
            public override TaskStatus OnUpdate()
            {
                if (!isEnemyDetected.Value || !isEnemyWithinAttackRange.Value || !combatRobot.Value.AttackAniPlayCheck())
                {
                    return TaskStatus.Failure;
                }

                combatRobot.Value.m_MeleeAttackCoolTime += Time.deltaTime;
                if (combatRobot.Value.m_MeleeAttackCoolTime >= combatRobot.Value.statComponent.AttackSpeed)
                {
                    combatRobot.Value.PlayAttackEffect();
                }
                return TaskStatus.Success;

            }
        }

        #endregion
    }
}


