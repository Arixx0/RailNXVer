using Data;
using UnityEngine;
using Utility;

namespace Units.Enemies
{
    public class ScavengerTank : CombatUnit
    {
        private float m_CurrentMoveSpeed;
        
        private void Update()
        {
            if (!isActiveAndEnabled)
            {
                return;
            }
            
            // if any enemy is not detected, patrol or just hold the position
            if (!enemySensingComponent.CurrentTargetedEnemy)
            {
                return;
            }

            var position = CachedTransform.position;
            var forward = CachedTransform.forward;
            var enemyPosition = enemySensingComponent.CurrentTargetedEnemy.position;

            var distanceDelta = (enemyPosition - position).GetXZ();
            var sqrDistance = distanceDelta.sqrMagnitude;
            var turnFactor = Vector3.Dot(forward, distanceDelta.normalized);

            var targetForward = distanceDelta.normalized;
            var targetMoveSpeed = statComponent.MoveSpeed;
            if (sqrDistance < statComponent.SqrAttackRange &&
                turnFactor < statComponent.TurnThresholdAngleAlpha)
            {
                targetMoveSpeed = statComponent.MoveSpeed * 0.25f;
            }
            else if (sqrDistance < statComponent.SqrThresholdAttackRange)
            {
                targetForward = enemySensingComponent.CurrentTargetedEnemy.forward;
                
                if (turnFactor > statComponent.AttackableSectorAngleAlpha)
                {
                    targetForward = Quaternion.Euler(0, -90, 0) * targetForward;
                    targetMoveSpeed = statComponent.MoveSpeed * 0.25f;
                }
            }
            
            m_CurrentMoveSpeed = Mathf.Lerp(m_CurrentMoveSpeed, targetMoveSpeed, statComponent.MoveSpeedDamp);
            forward = Vector3.Lerp(forward, targetForward, statComponent.RotateSpeed);
            CachedTransform.forward = forward;
            CachedTransform.position += forward * (m_CurrentMoveSpeed * Time.deltaTime);
        }

        public override void TakeDamage(UnitCombatStatCaptureData data)
        {
            statComponent.CurrentHealth -= (int)data.AttackDamage;
            
            if (statComponent.CurrentHealth <= 0)
            {
                destructionCompositor.DoDestroy();
                InvokeDestroyedEvent();
            }
        }
    }
}