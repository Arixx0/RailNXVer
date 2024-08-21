using System;
using Attributes;
using Units.Stats;
using UnityEngine;
using Utility;

namespace Units
{
    public class MortarTurretStatComponent : UnitStatComponent
    {
        [Header("Mortar Turret Statistics")]
        [SerializeField] protected float launchAngle;
        [SerializeField] protected MortarProjectileLaunchOption launchOptionAtMinRange;
        [SerializeField] protected MortarProjectileLaunchOption launchOptionAtMaxRange;

        public override float SqrAttackRange => launchOptionAtMaxRange.sqrAttackRange;
        
        public float LaunchAngle => launchAngle;
        
        public MortarProjectileLaunchOption LaunchOptionAtMinRange => launchOptionAtMinRange;
        
        public MortarProjectileLaunchOption LaunchOptionAtMaxRange => launchOptionAtMaxRange;

        protected override void Reset()
        {
            base.Reset();
            
            launchOptionAtMinRange = new MortarProjectileLaunchOption
            {
                attackRange = 10,
                travelTime = 1.5f,
                launchForce = 3,
                totalTimeOfMotion = 1.5f
            };

            launchOptionAtMaxRange = new MortarProjectileLaunchOption
            {
                attackRange = 20,
                travelTime = 2.5f,
                launchForce = 5,
                totalTimeOfMotion = 2.5f
            };
        }

        public override void Setup(bool reset = false)
        {
            base.Setup();
            
            launchOptionAtMinRange.sqrAttackRange =
                launchOptionAtMinRange.attackRange * launchOptionAtMinRange.attackRange;
            
            launchOptionAtMaxRange.sqrAttackRange =
                launchOptionAtMaxRange.attackRange * launchOptionAtMaxRange.attackRange;
        }

        [Serializable]
        public struct MortarProjectileLaunchOption
        {
            public float attackRange;
            public float travelTime;
            public float launchForce;
            public float totalTimeOfMotion;
            
            [Space]
            [Disabled] public float sqrAttackRange;
        }
    }
}