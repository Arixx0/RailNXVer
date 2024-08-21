// ReSharper disable CheckNamespace

using UnityEngine;
using UnityEngine.InputSystem;

namespace TrainScripts
{
    public partial class Train
    {
        public class TrainDefaultInputHandler : InputComponents.IInputHandler
        {
            private readonly Train m_Train;
            
            public TrainDefaultInputHandler(Train train)
            {
                m_Train = train;
            }
            
            public void ResetInputHandlingState()
            {
            }

            public bool HandlePlayerSelectNextCar(InputAction.CallbackContext context)
            {
                if (!m_Train.enableInputHandling || context.phase != InputActionPhase.Performed)
                {
                    return false;
                }
                
                if (m_Train.cars.Count <= 0)
                {
                    m_Train.SetSelectedCar(-1);
                    return true;
                }

                var offset = Mathf.RoundToInt(context.ReadValue<float>());
                var targetIndex = (m_Train.m_SelectedCarIndex + offset + m_Train.cars.Count) % m_Train.cars.Count;
                
                m_Train.SetSelectedCar(targetIndex);
                return true;
            }

            public bool HandlePlayerZoom(InputAction.CallbackContext context)
            {
                if (!m_Train.enableInputHandling)
                {
                    return false;
                }

                switch (context.phase)
                {
                    case InputActionPhase.Performed:
                        var value = context.ReadValue<float>();
                        m_Train.cameraController.SetZoomVelocity(value);
                        break;
                    case InputActionPhase.Canceled:
                        m_Train.cameraController.SetZoomVelocity(0);
                        break;
                }

                return true;
            }

            public bool HandlePlayerRotateCamera(InputAction.CallbackContext context)
            {
                if (!m_Train.enableInputHandling)
                {
                    return false;
                }

                switch (context.phase)
                {
                    case InputActionPhase.Performed:
                        var value = context.ReadValue<float>();
                        m_Train.cameraController.SetRotateVelocity(value);
                        break;
                    case InputActionPhase.Canceled:
                        m_Train.cameraController.SetRotateVelocity(0);
                        break;
                }

                return true;
            }

            public bool HandlePlayerMoveCamera(InputAction.CallbackContext context)
            {
                return true;
            }

            public bool HandlePlayerShowWheelMenu(InputAction.CallbackContext context)
            {
                if (!m_Train.enableInputHandling || context.phase != InputActionPhase.Performed)
                {
                    return false;
                }

                if (!m_Train.contextMenu.IsEnabled)
                {
                    m_Train.contextMenu.OpenMenu(Vector2.zero, m_Train);
                }
                return true;
            }

            public bool HandlePlayerScaleMiningArea(InputAction.CallbackContext context) => false;

            public bool HandlePlayerChangeHUDLayer(InputAction.CallbackContext context)
            {
                if (!m_Train.enableInputHandling || context.phase != InputActionPhase.Performed)
                {
                    return false;
                }
                m_Train.HUD.DetailLayerIndex = (m_Train.HUD.DetailLayerIndex + 1) % m_Train.HUD.DetailLayerCount;
                m_Train.HUD.OnChangeDetailLayer(m_Train.HUD.DetailLayerIndex);
                return true;
            }

            public bool HandleUINavigate(InputAction.CallbackContext context) => false;

            public bool HandleUISubmit(InputAction.CallbackContext context)
            {
                if (!m_Train.enableInputHandling || context.phase != InputActionPhase.Performed)
                {
                    return false;
                }

                var targetCar = m_Train.cars[m_Train.m_SelectedCarIndex];

                if (targetCar.CircuitFailure)
                {
                    targetCar.RepairCircuit();
                }
                else if (m_Train.hud.gameObject.activeSelf && m_Train.hud.DetailLayerIndex == 2)
                {
                    targetCar.ShowElectricPowerChangeItem();
                }

                return true;
            }

            public bool HandleUIApply(InputAction.CallbackContext context) => false;

            public bool HandleUICancel(InputAction.CallbackContext context) => false;

            public bool HandleUIPoint(InputAction.CallbackContext context) => false;

            public bool HandleUIClick(InputAction.CallbackContext context) => false;

            public bool HandleUIRightClick(InputAction.CallbackContext context) => false;
        }
    }
}