using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Utility;

namespace InputComponents
{
    public interface IInputHandler
    {
        // Reset this object's input handling state.
        public void ResetInputHandlingState();

        public bool HandlePlayerSelectNextCar(InputAction.CallbackContext context);

        public bool HandlePlayerZoom(InputAction.CallbackContext context);
        
        public bool HandlePlayerRotateCamera(InputAction.CallbackContext context);
        
        public bool HandlePlayerMoveCamera(InputAction.CallbackContext context);
        
        public bool HandlePlayerShowWheelMenu(InputAction.CallbackContext context);
        
        public bool HandlePlayerScaleMiningArea(InputAction.CallbackContext context);

        public bool HandlePlayerChangeHUDLayer(InputAction.CallbackContext context);
        
        public bool HandleUINavigate(InputAction.CallbackContext context);
        
        public bool HandleUISubmit(InputAction.CallbackContext context);

        public bool HandleUIApply(InputAction.CallbackContext context);
        
        public bool HandleUICancel(InputAction.CallbackContext context);

        public bool HandleUIPoint(InputAction.CallbackContext context);

        public bool HandleUIClick(InputAction.CallbackContext context);

        public bool HandleUIRightClick(InputAction.CallbackContext context);
    }

    [Serializable]
    public class InputActionRefOverride
    {
        private string m_ActionName;
        
        private InputActionReference m_InputActionReference;
        
        public InputActionRefOverride(InputActionReference inputActionReference)
        {
            m_InputActionReference = inputActionReference;
            m_ActionName = inputActionReference.action.name;
        }
    }
    
    public class InputComponent : SingletonObject<InputComponent>
    {
        [SerializeField]
        private InputActionAsset definitionAsset;

        [SerializeField]
        private PlayerInput playerInput;

        [SerializeField]
        private bool enableDebugOverlay;
        
        private InputActions m_InputActionsInstance;

        private List<InputActionRefOverride> m_InputActionReferences = new();

        private readonly List<IInputHandler> m_ActiveHandlers = new(8);
        
        private readonly System.Text.StringBuilder m_StringBuilder = new(16);

        protected override void Awake()
        {
            base.Awake();

            // NOTE: Handling specialized input action processing can result in additional work.
            //  To prevent this, use the `InputActionReference` and iterate actions integrated with it.
            var iterator = definitionAsset.GetEnumerator();
            m_InputActionReferences.Clear();
            while (iterator.MoveNext())
            {
                var inputActionRef = InputActionReference.Create(iterator.Current);
                m_InputActionReferences.Add(new InputActionRefOverride(inputActionRef));
            }
            iterator.Dispose();

            m_InputActionsInstance = new InputActions();

            m_InputActionsInstance.Player.SelectNextCar.started += OnPlayerSelectNextCar;
            m_InputActionsInstance.Player.SelectNextCar.performed += OnPlayerSelectNextCar;
            m_InputActionsInstance.Player.SelectNextCar.canceled += OnPlayerSelectNextCar;

            m_InputActionsInstance.Player.Zoom.started += OnPlayerZoom;
            m_InputActionsInstance.Player.Zoom.performed += OnPlayerZoom;
            m_InputActionsInstance.Player.Zoom.canceled += OnPlayerZoom;

            m_InputActionsInstance.Player.RotateCamera.started += OnPlayerRotateCamera;
            m_InputActionsInstance.Player.RotateCamera.performed += OnPlayerRotateCamera;
            m_InputActionsInstance.Player.RotateCamera.canceled += OnPlayerRotateCamera;

            m_InputActionsInstance.Player.MoveCamera.started += OnPlayerMoveCamera;
            m_InputActionsInstance.Player.MoveCamera.performed += OnPlayerMoveCamera;
            m_InputActionsInstance.Player.MoveCamera.canceled += OnPlayerMoveCamera;

            m_InputActionsInstance.Player.ShowWheelMenu.started += OnPlayerShowWheelMenu;
            m_InputActionsInstance.Player.ShowWheelMenu.performed += OnPlayerShowWheelMenu;
            m_InputActionsInstance.Player.ShowWheelMenu.canceled += OnPlayerShowWheelMenu;
            
            m_InputActionsInstance.Player.ScaleMiningArea.started += OnPlayerScaleMiningArea;
            m_InputActionsInstance.Player.ScaleMiningArea.performed += OnPlayerScaleMiningArea;
            m_InputActionsInstance.Player.ScaleMiningArea.canceled += OnPlayerScaleMiningArea;

            m_InputActionsInstance.Player.ChangeHUDLayer.started += OnPlayerChangeHUDLayer;
            m_InputActionsInstance.Player.ChangeHUDLayer.performed += OnPlayerChangeHUDLayer;
            m_InputActionsInstance.Player.ChangeHUDLayer.canceled += OnPlayerChangeHUDLayer;

            m_InputActionsInstance.UI.Navigate.started += OnUINavigate;
            m_InputActionsInstance.UI.Navigate.performed += OnUINavigate;
            m_InputActionsInstance.UI.Navigate.canceled += OnUINavigate;

            m_InputActionsInstance.UI.Submit.started += OnUISubmit;
            m_InputActionsInstance.UI.Submit.performed += OnUISubmit;
            m_InputActionsInstance.UI.Submit.canceled += OnUISubmit;

            m_InputActionsInstance.UI.Apply.started += OnUISubmitHold;
            m_InputActionsInstance.UI.Apply.performed += OnUISubmitHold;
            m_InputActionsInstance.UI.Apply.canceled += OnUISubmitHold;
            
            m_InputActionsInstance.UI.Cancel.started += OnUICancel;
            m_InputActionsInstance.UI.Cancel.performed += OnUICancel;
            m_InputActionsInstance.UI.Cancel.canceled += OnUICancel;

            m_InputActionsInstance.UI.Point.started += OnUIPoint;
            m_InputActionsInstance.UI.Point.performed += OnUIPoint;
            m_InputActionsInstance.UI.Point.canceled += OnUIPoint;

            m_InputActionsInstance.UI.Click.started += OnUIClick;
            m_InputActionsInstance.UI.Click.performed += OnUIClick;
            m_InputActionsInstance.UI.Click.canceled += OnUIClick;

            m_InputActionsInstance.UI.RightClick.started += OnUIRightClick;
            m_InputActionsInstance.UI.RightClick.performed += OnUIRightClick;
            m_InputActionsInstance.UI.RightClick.canceled += OnUIRightClick;

            m_InputActionsInstance.Enable();
        }

        private void OnGUI()
        {
            if (!enabled || !enableDebugOverlay)
            {
                return;
            }
            
            m_StringBuilder.Clear();
            m_StringBuilder.AppendLine("UI:");
            m_StringBuilder.AppendLine($"- Navigate: {m_InputActionsInstance.UI.Navigate.phase}, {m_InputActionsInstance.UI.Navigate.ReadValue<Vector2>()}");
            m_StringBuilder.AppendLine($"- Submit: {m_InputActionsInstance.UI.Submit.phase}");
            m_StringBuilder.AppendLine($"- Cancel: {m_InputActionsInstance.UI.Cancel.phase}");
            m_StringBuilder.AppendLine($"- Point: {m_InputActionsInstance.UI.Point.phase}, {m_InputActionsInstance.UI.Navigate.ReadValue<Vector2>()}");
            m_StringBuilder.AppendLine($"- Click: {m_InputActionsInstance.UI.Click.phase}");
            m_StringBuilder.AppendLine($"- Scroll: {m_InputActionsInstance.UI.ScrollWheel.phase}, {m_InputActionsInstance.UI.ScrollWheel.ReadValue<Vector2>()}");
            m_StringBuilder.AppendLine($"- MiddleClick: {m_InputActionsInstance.UI.MiddleClick.phase}");
            m_StringBuilder.AppendLine($"- RightClick: {m_InputActionsInstance.UI.RightClick.phase}");
            m_StringBuilder.AppendLine("Player:");
            m_StringBuilder.AppendLine($"- SelectNextCar: {m_InputActionsInstance.Player.SelectNextCar.phase}");
            m_StringBuilder.AppendLine($"- Zoom: {m_InputActionsInstance.Player.Zoom.phase}, {m_InputActionsInstance.Player.Zoom.ReadValue<float>()}");
            m_StringBuilder.AppendLine($"- RotateCamera: {m_InputActionsInstance.Player.RotateCamera.phase}, {m_InputActionsInstance.Player.RotateCamera.ReadValue<Vector2>()}");
            m_StringBuilder.AppendLine($"- MoveCamera: {m_InputActionsInstance.Player.MoveCamera.phase}, {m_InputActionsInstance.Player.MoveCamera.ReadValue<Vector2>()}");
            m_StringBuilder.AppendLine($"- ShowWheelMenu: {m_InputActionsInstance.Player.ShowWheelMenu.phase}");
            m_StringBuilder.AppendLine($"- ScaleMiningArea: {m_InputActionsInstance.Player.ScaleMiningArea.phase}");
            m_StringBuilder.AppendLine($"- ChangeHUDLayer: {m_InputActionsInstance.Player.ChangeHUDLayer.phase}");
            
            GUILayout.Label(m_StringBuilder.ToString());
        }

        public void RegisterHandler(IInputHandler handler)
        {
            if (m_ActiveHandlers.Contains(handler))
            {
                m_ActiveHandlers.Remove(handler);
            }
            
            m_ActiveHandlers.Add(handler);
        }
        
        public void UnregisterHandler(IInputHandler handler)
        {
            m_ActiveHandlers.Remove(handler);
        }

        private void OnPlayerSelectNextCar(InputAction.CallbackContext context)
        {
            for (var i = 1; i <= m_ActiveHandlers.Count; i += 1)
            {
                if (m_ActiveHandlers[^i].HandlePlayerSelectNextCar(context))
                {
                    break;
                }
            }
        }
        
        private void OnPlayerZoom(InputAction.CallbackContext context)
        {
            for (var i = 1; i <= m_ActiveHandlers.Count; i += 1)
            {
                if (m_ActiveHandlers[^i].HandlePlayerZoom(context))
                {
                    break;
                }
            }
        }
        
        private void OnPlayerRotateCamera(InputAction.CallbackContext context)
        {
            for (var i = 1; i <= m_ActiveHandlers.Count; i += 1)
            {
                if (m_ActiveHandlers[^i].HandlePlayerRotateCamera(context))
                {
                    break;
                }
            }
        }
        
        private void OnPlayerMoveCamera(InputAction.CallbackContext context)
        {
            for (var i = 1; i <= m_ActiveHandlers.Count; i += 1)
            {
                if (m_ActiveHandlers[^i].HandlePlayerMoveCamera(context))
                {
                    break;
                }
            }
        }

        private void OnPlayerShowWheelMenu(InputAction.CallbackContext context)
        {
            for (var i = 1; i <= m_ActiveHandlers.Count; i += 1)
            {
                if (m_ActiveHandlers[^i].HandlePlayerShowWheelMenu(context))
                {
                    break;
                }
            }
        }

        private void OnPlayerScaleMiningArea(InputAction.CallbackContext context)
        {
            for (var i = 1; i <= m_ActiveHandlers.Count; i += 1)
            {
                if (m_ActiveHandlers[^i].HandlePlayerScaleMiningArea(context))
                {
                    break;
                }
            }
        }

        private void OnPlayerChangeHUDLayer(InputAction.CallbackContext context)
        {
            for (var i = 1; i <= m_ActiveHandlers.Count; i += 1)
            {
                if (m_ActiveHandlers[^i].HandlePlayerChangeHUDLayer(context))
                {
                    break;
                }
            }
        }

        private void OnUINavigate(InputAction.CallbackContext context)
        {
            for (var i = 1; i < m_ActiveHandlers.Count; i += 1)
            {
                if (m_ActiveHandlers[^i].HandleUINavigate(context))
                {
                    break;
                }
            }
        }
        
        private void OnUISubmit(InputAction.CallbackContext context)
        {
            for (var i = 1; i <= m_ActiveHandlers.Count; i += 1)
            {
                if (m_ActiveHandlers[^i].HandleUISubmit(context))
                {
                    break;
                }
            }
        }

        private void OnUISubmitHold(InputAction.CallbackContext obj)
        {
            for (var i = 1; i <= m_ActiveHandlers.Count; i += 1)
            {
                if (m_ActiveHandlers[^i].HandleUIApply(obj))
                {
                    break;
                }
            }
        }
        
        private void OnUICancel(InputAction.CallbackContext context)
        {
            for (var i = 1; i < m_ActiveHandlers.Count; i += 1)
            {
                if (m_ActiveHandlers[^i].HandleUICancel(context))
                {
                    break;
                }
            }
        }

        private void OnUIPoint(InputAction.CallbackContext context)
        {
            for (var i = 1; i < m_ActiveHandlers.Count; i += 1)
            {
                if (m_ActiveHandlers[^i].HandleUIPoint(context))
                {
                    break;
                }
            }
        }

        private void OnUIClick(InputAction.CallbackContext context)
        {
            for (var i = 1; i < m_ActiveHandlers.Count; i += 1)
            {
                if (m_ActiveHandlers[^i].HandleUIClick(context))
                {
                    break;
                }
            }
        }

        private void OnUIRightClick(InputAction.CallbackContext context)
        {
            for (var i = 1; i < m_ActiveHandlers.Count; i += 1)
            {
                if (m_ActiveHandlers[^i].HandleUIRightClick(context))
                {
                    break;
                }
            }
        }
    }

    [Serializable]
    public class AxisInputIntervalModule
    {
        [SerializeField]
        private float moveRepeatDelay = 0.5f;

        [SerializeField]
        private float moveRepeatRate = 0.1f;
        
        private float m_LastHandledTime;

        private byte m_ConsecutiveHandleCount;
        
        public bool IsEnabled { get; set; }
        
        public Vector2 Value { get; set; }
        
        public void Reset()
        {
            m_ConsecutiveHandleCount = 0;
            
            Value = Vector2.zero;
        }

        public bool CanHandleInput(float time)
        {
            if (!IsEnabled)
            {
                return false;
            }

            if (m_ConsecutiveHandleCount == 0)
            {
                m_LastHandledTime = time;
                ++m_ConsecutiveHandleCount;
                return true;
            }
            
            var targetInterval = m_ConsecutiveHandleCount <= 1 ? moveRepeatDelay : moveRepeatRate;
            if (time < (m_LastHandledTime + targetInterval))
            {
                return false;
            }

            m_LastHandledTime = time;
            ++m_ConsecutiveHandleCount;
            if (m_ConsecutiveHandleCount > 3)
            {
                m_ConsecutiveHandleCount = 3;
            }
            return true;
        }
    }
}