using Data;

using UnityEngine;

namespace Units.Enemies
{
    public abstract class MeleeEnemyUnit : EnemyUnit
    {
        [Space, Header("Melee Enemy Components")]
        [SerializeField] protected MeleeWeaponColliderHandler meleeWeaponColliderHandler;

        protected float m_MeleeAttackCoolTime;

        protected override void Start()
        {
            base.Start();
            m_MeleeAttackCoolTime = statComponent.AttackSpeed;
            meleeWeaponColliderHandler.OnWeaponCollided += OnWeaponCollied;
        }

        public override void ReSetupStat()
        {
            StatComponent.armorPoint.Setup();
            StatComponent.moveSpeed.Setup();
            StatComponent.rotateSpeed.Setup();

            StatComponent.attackDamage.Setup();
            StatComponent.armorPierce.Setup();
            StatComponent.attackSpeed.Setup();

            StatComponent.fuelEfficiency.Setup();
            StatComponent.taskHandlingSpeed.Setup();
            m_CombatStatData = statComponent.CaptureCombatStatData();
            if (navigationAgent != null)
            {
                navigationAgent.speed = statComponent.MoveSpeed;
                navigationAgent.angularSpeed = statComponent.RotateSpeed;
            }
        }

        protected override void PlayAttackEffect()
        {
            base.PlayAttackEffect();
            m_MeleeAttackCoolTime = 0f;
            enemyUnitAnimator?.SetBool("IsMoving", false);
            enemyUnitAnimator?.SetTrigger("Attack");
        }

        protected void OnWeaponCollied(GameObject other)
        {
            if (other.TryGetComponent(out ICombatEventReceiver eventReceiver))
            {
                eventReceiver.TakeDamage(m_CombatStatData);
            }
        }
    }
}