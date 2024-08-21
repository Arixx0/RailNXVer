using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using StatusEffects;

namespace TrainScripts
{
    [DisallowMultipleComponent]
    public class EngineCompartment : Car
    {
        public int powerCapacity = 100;
        [SerializeField] private StatusEffect engineCircuitFailureDebuff;

        protected override void CarCircuitFailure()
        {
            base.CarCircuitFailure();
            parentTrain.StatComponent.StatusEffectManager?.AddStatusEffect(engineCircuitFailureDebuff, false);
        }

        protected override void CarCircuitRepair()
        {
            base.CarCircuitRepair();
            parentTrain.StatComponent.StatusEffectManager?.RemoveStatusEffect(engineCircuitFailureDebuff);
        }
        
        #region ContextMenu Actions

#if UNITY_EDITOR
        
        [ContextMenu("Set Default Context Menu Data(Engine Compartment)")]
        protected override void SetDefaultContextMenuData()
        {
            base.SetDefaultContextMenuData();

            var tearDownMenu = contextMenuData.FirstOrDefault(e => string.Equals("Tear Down Car", e.itemName));
            if (tearDownMenu != null)
            {
                contextMenuData.Remove(tearDownMenu);
            }

            var cancelTearDownMenu = contextMenuData.FirstOrDefault(e => string.Equals("Cancel Tearing Down", e.itemName));
            if (cancelTearDownMenu != null)
            {
                contextMenuData.Remove(cancelTearDownMenu);
            }

            var reorderMenu = contextMenuData.FirstOrDefault(e => string.Equals("Reorder Car", e.itemName));
            if (reorderMenu != null)
            {
                contextMenuData.Remove(reorderMenu);
            }
            
            UnityEditor.EditorUtility.SetDirty(this);
        }
        
#endif

        #endregion
    }
}