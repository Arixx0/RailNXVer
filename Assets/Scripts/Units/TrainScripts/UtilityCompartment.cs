using UI;
using UnityEngine;

namespace TrainScripts
{
    public class UtilityCompartment : Car
    {
        #region ContextMenu Actions
        
#if UNITY_EDITOR
        
        [ContextMenu("Set Default Context Menu Data(Utility Compartment)")]
        protected override void SetDefaultContextMenuData()
        {
            base.SetDefaultContextMenuData();
            
            contextMenuData.Add(new ContextMenuData("Change Effect Offset", ShowEffectAreaSelector));
            
            UnityEditor.EditorUtility.SetDirty(this);
        }

#endif
        
        public virtual void ShowEffectAreaSelector()
        {
        }
        
        #endregion
    }
}