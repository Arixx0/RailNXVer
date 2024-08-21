using UnityEngine;
using UnityEngine.InputSystem;

namespace TrainScripts
{
    public partial class Train
    {
        [Header("Train Reorder Properties")]
        [SerializeField] private GameObject reorderOverlayUI;
        
        private readonly TrainReorderInputHandler m_ReorderInputHandler = new();
        
        public class TrainReorderInputHandler : InputComponents.IInputHandler
        {
            private Train m_Train;
            private int m_OriginalCarIndex;
            private int m_TargetOrderIndex;

            public void Activate(Train train)
            {
                m_Train = train;
                m_OriginalCarIndex = m_Train.m_SelectedCarIndex;
                m_TargetOrderIndex = m_Train.m_SelectedCarIndex;
                
                m_Train.reorderOverlayUI.SetActive(true);
                
                InputComponents.InputComponent.Get.RegisterHandler(this);
            }

            public void Deactivate()
            {
                m_Train.reorderOverlayUI.SetActive(false);
                
                InputComponents.InputComponent.Get.UnregisterHandler(this);
            }
            
            #region IInputHandler
            
            public void ResetInputHandlingState() { }

            public bool HandlePlayerSelectNextCar(InputAction.CallbackContext context)
            {
                if (context.phase != InputActionPhase.Performed)
                {
                    return true;
                }

                var targetIndex = m_TargetOrderIndex + (int)context.ReadValue<float>();
                
                // NOTE: the index 0 is always occupied by the engine compartment.

                if (targetIndex <= 0)
                {
                    targetIndex = m_Train.cars.Count;
                }

                if (targetIndex >= m_Train.cars.Count)
                {
                    targetIndex = 1;
                }
                
                m_Train.ReorderCar(m_TargetOrderIndex, targetIndex);
                m_TargetOrderIndex = targetIndex;
                
                return true;
            }

            public bool HandlePlayerZoom(InputAction.CallbackContext context) => false;

            public bool HandlePlayerRotateCamera(InputAction.CallbackContext context) => false;

            public bool HandlePlayerMoveCamera(InputAction.CallbackContext context) => false;

            public bool HandlePlayerShowWheelMenu(InputAction.CallbackContext context) => true;

            public bool HandlePlayerScaleMiningArea(InputAction.CallbackContext context) => false;

            public bool HandlePlayerChangeHUDLayer(InputAction.CallbackContext context) => false;

            public bool HandleUINavigate(InputAction.CallbackContext context) => false;

            public bool HandleUISubmit(InputAction.CallbackContext context)
            {
                if (context.phase != InputActionPhase.Performed)
                {
                    return true;
                }
                
                Deactivate();
                return true;
            }

            public bool HandleUIApply(InputAction.CallbackContext context) => false;

            public bool HandleUICancel(InputAction.CallbackContext context)
            {
                if (context.phase != InputActionPhase.Performed)
                {
                    return true;
                }
                
                m_Train.ReorderCar(m_TargetOrderIndex, m_OriginalCarIndex);
                
                Deactivate();
                return true;
            }

            public bool HandleUIPoint(InputAction.CallbackContext context) => false;

            public bool HandleUIClick(InputAction.CallbackContext context) => false;

            public bool HandleUIRightClick(InputAction.CallbackContext context) => false;

            #endregion // IInputHandler
        }
    }
}