using Attributes;

using UnityEngine;
using UnityEngine.InputSystem;

namespace UI
{
    public class AttackTargetSelector : MonoBehaviour, InputComponents.IInputHandler
    {
        private const float MAX_RAY_DISTANCE = 10000f;
        
        [Header("UI Elements References")]
        [SerializeField] private Canvas canvas;
        
        [Header("RayCast Settings")]
        [SerializeField] private LayerMask enemyLayerMask;
        [SerializeField] private LayerMask groundLayerMask;
        [SerializeField] private float cursorRadius = 1f;
        [SerializeField] private Vector2 screenSpaceRayPoint;
        
        [Header("Internal References")]
        [SerializeField, Disabled] private new Camera camera;
        [SerializeField, Disabled] private CameraUtility.CameraController cameraController;
        
        public System.Action<Units.Enemies.EnemyUnit> OnAttackTargetSelected; 

        private void Awake()
        {
            enabled = false;
            canvas.enabled = false;
        }

        public void Activate()
        {
            enabled = true;
            canvas.enabled = true;
            InputComponents.InputComponent.Get.RegisterHandler(this);
            TimeScaleManager.Get.ChangeGlobalTimeScale(0.1f);

            camera = FindObjectOfType<Camera>();
            cameraController = FindObjectOfType<CameraUtility.CameraController>();
        }

        private void Deactivate()
        {
            enabled = false;
            canvas.enabled = false;
            InputComponents.InputComponent.Get.UnregisterHandler(this);
            TimeScaleManager.Get.ChangeGlobalTimeScale(1f);
            
            cameraController.ResetMoveOffset();
        }

        private bool TryQueryEnemy(out Units.Enemies.EnemyUnit enemyUnit)
        {
            enemyUnit = null;

            screenSpaceRayPoint.x = camera.pixelWidth * 0.5f;
            screenSpaceRayPoint.y = camera.pixelHeight * 0.5f;
            
            var ray = camera.ScreenPointToRay(screenSpaceRayPoint);
            if (Physics.Raycast(ray, out var hit, MAX_RAY_DISTANCE, enemyLayerMask))
            {
                enemyUnit = hit.collider.GetComponent<Units.Enemies.EnemyUnit>();
                return true;
            }

            return false;
        }
        
        #region IInputHandler
        
        public void ResetInputHandlingState()
        {
        }

        public bool HandlePlayerSelectNextCar(InputAction.CallbackContext context) => true;

        public bool HandlePlayerZoom(InputAction.CallbackContext context) => false;

        public bool HandlePlayerRotateCamera(InputAction.CallbackContext context) => false;

        public bool HandlePlayerMoveCamera(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                case InputActionPhase.Performed:
                    cameraController.SetMoveVelocity(context.ReadValue<Vector2>());
                    break;
                case InputActionPhase.Canceled:
                    cameraController.SetMoveVelocity(Vector2.zero);
                    break;
            }

            return true;
        }

        public bool HandlePlayerShowWheelMenu(InputAction.CallbackContext context) => true;

        public bool HandlePlayerScaleMiningArea(InputAction.CallbackContext context) => true;

        public bool HandlePlayerChangeHUDLayer(InputAction.CallbackContext context) => true;

        public bool HandleUINavigate(InputAction.CallbackContext context) => true;

        public bool HandleUISubmit(InputAction.CallbackContext context)
        {
            if (TryQueryEnemy(out var enemyUnit))
            {
                Debug.Log($"Enemy Unit: {enemyUnit.name}");
                OnAttackTargetSelected?.Invoke(enemyUnit);
                OnAttackTargetSelected = null;
                
                Deactivate();
            }
            
            return true;
        }

        public bool HandleUIApply(InputAction.CallbackContext context) => true;

        public bool HandleUICancel(InputAction.CallbackContext context)
        {
            OnAttackTargetSelected?.Invoke(null);
            OnAttackTargetSelected = null;
            
            Deactivate();

            return true;
        }


        public bool HandleUIPoint(InputAction.CallbackContext context) => false;

        public bool HandleUIClick(InputAction.CallbackContext context) => false;

        public bool HandleUIRightClick(InputAction.CallbackContext context) => false;

        #endregion // IInputHandler
    }
}