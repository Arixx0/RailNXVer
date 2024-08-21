using Attributes;
using UI;
using UnityEngine;

namespace TrainScripts
{
    public class JunkStateCompartment : Car
    {
        [Header("Junk State Compartment Properties")]
        [SerializeField, Disabled] private Car actualInstance;

        private bool m_IsRestoring;

        public Car ActualInstance
        {
            get => actualInstance;
            set
            {
                actualInstance = value;

                if (actualInstance != null)
                {
                    actualInstance.CachedTransform.SetParent(CachedTransform);
                    actualInstance.gameObject.SetActive(false);
                }
            }
        }

        public override void Setup(Train parent)
        {
            base.Setup(parent);
            
            // because the junk state compartment is not a real car,
            // we don't need to check circuit failure

            isCarDestroyed = true;
            
            CarStopWorking();
            StopCircuitFailure(true);
        }

        protected override void DoDemolish()
        {
            if (ActualInstance != null)
            {
                parentTrain.RemoveCarFromManagedList(actualInstance);
            }
            
            base.DoDemolish();
        }

        #region ContextMenu Actions
        
#if UNITY_EDITOR

        [ContextMenu("Set Default Context Menu Data(Junk Compartment)")]
        protected override void SetDefaultContextMenuData()
        {
            contextMenuData.Add(new ContextMenuData("Restore", RestoreCar, ContextMenuCondition.OnCompartmentDestroyed));
            contextMenuData.Add(new ContextMenuData("Demolish", Demolish, ContextMenuCondition.OnCompartmentDestroyed));
        }

#endif

        public void RestoreCar()
        {
            if (m_IsRestoring || actualInstance == null)
            {
                return;
            }

            m_IsRestoring = true;

            var restoreOperation = new CarDelayedOperation(this, statComponent.RestoreDelay);
            
            restoreOperation.OnTaskUpdate += progress =>
                taskProgressBar.value = progress;
            
            restoreOperation.OnTaskComplete += () =>
            {
                m_IsRestoring = false;
            
                ParentTrain.ReplaceCarInstance(this, actualInstance);
                actualInstance.gameObject.SetActive(true);
                actualInstance.OnRestored();
            
                destructionCompositor.DoDestroy();
            };

            StartCoroutine(restoreOperation);
        }
        
        #endregion
    }
}