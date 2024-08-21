using Attributes;
using Data;
using Projectiles;
using UnityEngine;
using Utility;

namespace Units.Enemies
{
    public class SpiderTank : CombatUnit
    {
        #region Movement Option Field
        
        [Header("Movement Options")]
        [SerializeField] protected float moveSpeed;
        [SerializeField] protected float moveAcceleration;
        [SerializeField, Range(0, 360f)] protected float turnThresholdAngle;
        [SerializeField, Disabled] protected float turnThresholdAngleDot;

        private Vector3 m_MoveAmount;

        public Vector3 MoveAmount
        {
            get => m_MoveAmount;
            private set => m_MoveAmount = value;
        }
        
        public float TurnThresholdAngle => turnThresholdAngle;
        
        #endregion
        
        #region Combat Option Fields

        [Header("Statistics")]
        [SerializeField] protected int health;
        [SerializeField] protected int armor;
        [SerializeField] protected int unitSize;
        [SerializeField, Disabled] protected int currentHealth;
        [SerializeField, Disabled] protected int currentArmor;
        [SerializeField, Disabled] protected int currentUnitSize;
        
        [Header("Combat Options")]
        [SerializeField] protected int attackDamage;
        [SerializeField] protected float attackRange;
        [SerializeField, Range(0, 1)] protected float attackRangeThreshold = 0.75f;
        [SerializeField, Disabled] protected float sqrAttackRange;
        [SerializeField, Range(0.01f, 10f)] protected float fireRate;
        [SerializeField] protected Projectile projectilePrefab;
        [SerializeField] protected float projectileSpeed;
        [SerializeField] protected float aimRotationAcceleration;
        [SerializeField] protected Transform muzzlePoint;
        [SerializeField] protected float attackableFanRange;
        [SerializeField, Disabled] protected float attackableFanRangeDot;

        private float m_LastFireTime = -1f;
        
        public float AcquisitionRange => enemySensingComponent.AcquisitionRange;
        
        public float AttackRange => attackRange;
        
        public float AttackableFanRange => attackableFanRange;
        
        public EnemySensingComponent EnemySensingComponent => enemySensingComponent;
        
        #endregion
        
        #region VFX/SFX Effect Option Fields

        [Header("Sound Options")]
        [SerializeField] protected AudioSource fireSoundSource;
        
        [Header("VFX Options")]
        [SerializeField] protected ParticleSystem muzzleFlashVFX;
        
        #endregion

        protected override void Awake()
        {
            base.Awake();
            
            var forward = CachedTransform.forward;

            turnThresholdAngleDot = Vector3.Dot(
                Vector3.forward,
                Quaternion.Euler(0, turnThresholdAngle * 0.5f, 0) * forward);

            attackableFanRangeDot = Vector3.Dot(
                Vector3.forward,
                Quaternion.Euler(0, attackableFanRange * 0.5f, 0) * forward);

            sqrAttackRange = attackRange * attackRange;
            
            currentHealth = health;
            currentArmor = armor;
            currentUnitSize = unitSize;
        }
        
        private void Update()
        {
            if (!isActiveAndEnabled)
            {
                return;
            }
            
            ActByState();
            
            // MoveTowardsEnemy();
            // FireProjectile();
        }

        private void ActByState()
        {
            // if any enemy is not detected, patrol or just hold the position
            if (!enemySensingComponent.CurrentTargetedEnemy)
            {
                return;
            }
            
            var position = CachedTransform.position;
            var targetPosition = enemySensingComponent.CurrentTargetedEnemy.position;
            var dirToTarget = new Vector3(targetPosition.x - position.x, 0, targetPosition.z - position.z);
            
            var sqrDistanceToTarget = VectorUtility.DistanceInXZCoord(position, targetPosition, true);
            var dotForwardToTarget = Vector3.Dot(CachedTransform.forward, dirToTarget.normalized);
            
            // check if the target is in the attack range and target is in the attackable fan area
            if (sqrDistanceToTarget < sqrAttackRange &&
                dotForwardToTarget >= attackableFanRangeDot &&
                m_LastFireTime + fireRate <= Time.unscaledTime)
            {
                m_LastFireTime = Time.unscaledTime;

                fireSoundSource.PlayOneShot(fireSoundSource.clip);
            }

            // calculate new rotation and position towards the target
            
            var newRotation = CachedTransform.rotation;
            var newPosition = position;
            
            var targetRotation = Quaternion.LookRotation(dirToTarget, Vector3.up);
            newRotation = Quaternion.Lerp(newRotation, targetRotation, aimRotationAcceleration);
            
            if (sqrDistanceToTarget > sqrAttackRange &&
                dotForwardToTarget >= turnThresholdAngleDot)
            {
                // need to move towards the target

                var targetMoveAmount = CachedTransform.forward * moveSpeed;
                MoveAmount = Vector3.Lerp(MoveAmount, targetMoveAmount, moveAcceleration);
                newPosition = position + MoveAmount * Time.deltaTime * TimeScaleManager.Get.GetTimeScaleOfTag(tag);
            }

            // set calculated position and rotation
            CachedTransform.SetPositionAndRotation(newPosition, newRotation);
        }

        public override void TakeDamage(UnitCombatStatCaptureData data)
        {
            currentHealth -= (int)data.AttackDamage;
            healthBar?.UpdateHealthPoint(currentHealth);
            
            if (currentHealth <= 0)
            {
                destructionCompositor.DoDestroy();
                InvokeDestroyedEvent();
            }
        }
    }
}