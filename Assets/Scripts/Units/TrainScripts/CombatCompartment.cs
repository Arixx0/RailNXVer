using UI;
using Units.Enemies;
using Units.Turrets;
using UnityEngine;

namespace TrainScripts
{
    [DisallowMultipleComponent]
    public class CombatCompartment : Car
    {
        [Header("Combat Compartment")]
        [Space, SerializeField] private Turret turret;

        protected override void Start()
        {
            base.Start();
            turret.StatComponent = statComponent;
            turret.SetStatData(statComponent);
        }

        protected override void UpdateStatComponent()
        {
            base.UpdateStatComponent();
            
            if (turret != null)
            {
                turret.StatComponent = statComponent;
                turret.ReSetupStat();
            }
        }

        public override void CarStopWorking()
        {
            if (turret != null)
            {
                turret.StopTurret();
            }
        }

        public override void CarStartWorking()
        {
            if (turret != null && !CircuitFailure && !parentTrain.ElectricPowerOverload)
            {
                turret.ReStartTurret();
            }
        }

        private void UpdateTargetedEnemy(EnemyUnit enemyUnit)
        {
            if (enemyUnit == null)
            {
                Debug.Log("No enemy unit is selected.");
                return;
            }
            turret.SensingComponent.UpdateForceAttack(enemyUnit.transform);
            Debug.Log($"Enemy {enemyUnit.gameObject.name} is selected!");
        }
        
        #region ContextMenu Actions
        
#if UNITY_EDITOR
        [ContextMenu("Set Default Context Menu Data(Combat Compartment)")]
        protected override void SetDefaultContextMenuData()
        {
            base.SetDefaultContextMenuData();
            contextMenuData.Add(new ContextMenuData("Select Attack Target", SelectAttackTarget));
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif

        public void SelectAttackTarget()
        {
            var attackTargetSelector = FindObjectOfType<AttackTargetSelector>();
            if (attackTargetSelector == null)
            {
                Debug.LogError("AttackTargetSelector is not found.");
                return;
            }

            attackTargetSelector.OnAttackTargetSelected = UpdateTargetedEnemy;
            attackTargetSelector.Activate();
        }
        
        #endregion // ContextMenu Actions
    }
}