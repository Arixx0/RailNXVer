using Attributes;
using CameraUtility;
using Data;
using Environments;
using InputComponents;

using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// NOTE:
//  - `MiningTargetSelector.resourceLayerMask` is a layer mask to filter out the resource layer and the ground layer.
//  - ground layer is used to detect the ground when the player is not pointing at a resource.
//  - when the player try to select a resource without magnifying any resource vein, the resource vein in radius from the raycasted point will be selected.

namespace UI
{
    public class MiningTargetSelector : MonoBehaviour, IInputHandler
    {
        private const float MAX_RAY_DISTANCE = 10000f;
        private const int MAX_VEIN_QUERY_COUNT = 16;
        
        [Header("UI Elements References")]
        [SerializeField] private Canvas canvas;
        [SerializeField] private GameObject resourceMagnifierPanel;
        [SerializeField] private Image resourceTypeIcon;
        [SerializeField] private TextMeshProUGUI resourceTypeText;

        [Space]
        [SerializeField] private GameObject informationOverlayPanel;
        [SerializeField] private TextMeshProUGUI informationText;
        
        [Header("Resource Icons")]
        [SerializeField] private Sprite powerStoneIcon;
        [SerializeField] private Sprite ironOreIcon;
        [SerializeField] private Sprite titaniumOreIcon;
        [SerializeField] private Sprite mithrilOreIcon;
        [SerializeField] private Sprite adamantiumOreIcon;
        
        [Header("RayCast Settings")]
        [SerializeField] private LayerMask resourceLayerMask;
        [SerializeField, Disabled] private ResourceVein lastQueriedVein;
        [SerializeField] private float cursorRadius = 1f;
        [SerializeField] private LayerMask groundLayerMask;
        [SerializeField] private Vector2 screenSpaceRayPoint;
        
        [Header("Internal References")]
        [SerializeField, Disabled] private new Camera camera;
        [SerializeField, Disabled] private CameraController cameraController;

        [Space]
        [SerializeField, Disabled] private List<ResourceVein> queriedVeinsBuffer = new(MAX_VEIN_QUERY_COUNT);

        public Action<List<ResourceVein>> OnMiningTargetSelectedCallback;

        private readonly StringBuilder m_QueryInformationLogBuilder = new(2);
        private readonly Dictionary<ResourceType, int> m_QueriedResourceContext = new(5);
        
        private void Awake()
        {
            enabled = false;
            canvas.enabled = false;
            
            resourceMagnifierPanel.gameObject.SetActive(false);
            informationOverlayPanel.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (!enabled)
            {
                return;
            }

            screenSpaceRayPoint.x = camera.pixelWidth * 0.5f;
            screenSpaceRayPoint.y = camera.pixelHeight * 0.5f;
            var ray = camera.ScreenPointToRay(screenSpaceRayPoint);
            Debug.DrawRay(ray.origin, ray.direction * MAX_RAY_DISTANCE, Color.red);

            if (!Physics.Raycast(ray, out var hit, MAX_RAY_DISTANCE, resourceLayerMask))
            {
                resourceMagnifierPanel.SetActive(false);
                return;
            }
            
            if (hit.collider.TryGetComponent(out ResourceVein resourceVein))
            {
                if (lastQueriedVein == resourceVein)
                {
                    return;
                }
                
                lastQueriedVein = resourceVein;
                
                resourceMagnifierPanel.SetActive(true);
                resourceTypeText.text = resourceVein.ResourceType.ToString();
                resourceTypeIcon.sprite = resourceVein.ResourceType switch
                {
                    ResourceType.PowerStone => powerStoneIcon,
                    ResourceType.Adamantine => adamantiumOreIcon,
                    ResourceType.Mythrill => mithrilOreIcon,
                    ResourceType.Titanium => titaniumOreIcon,
                    _ => ironOreIcon
                };
            }
            else
            {
                resourceMagnifierPanel.SetActive(false);
                lastQueriedVein = null;
            }
        }

        public void Activate()
        {
            enabled = true;
            canvas.enabled = true;
            InputComponent.Get.RegisterHandler(this);
            TimeScaleManager.Get.ChangeGlobalTimeScale(0f);

            camera = FindObjectOfType<Camera>();
            cameraController = FindObjectOfType<CameraController>();
            
            // clear the queried colliders buffer.
            queriedVeinsBuffer.Clear();
        }

        private void Deactivate()
        {
            enabled = false;
            canvas.enabled = false;
            InputComponent.Get.UnregisterHandler(this);
            TimeScaleManager.Get.ChangeGlobalTimeScale(1f);
            
            cameraController.ResetMoveOffset();
            
            // clear the queried colliders buffer.
            queriedVeinsBuffer.Clear();
            
            resourceMagnifierPanel.gameObject.SetActive(false);
            informationOverlayPanel.gameObject.SetActive(false);
        }

        private void SelectVeins()
        {
            screenSpaceRayPoint.x = camera.pixelWidth * 0.5f;
            screenSpaceRayPoint.y = camera.pixelHeight * 0.5f;
            var ray = camera.ScreenPointToRay(screenSpaceRayPoint);
            
            if (!Physics.Raycast(ray, out var hit, MAX_RAY_DISTANCE, resourceLayerMask) ||
                hit.collider.gameObject.layer == groundLayerMask)
            {
                return;
            }
            
            var wasResourceVeinSelected = hit.collider.TryGetComponent<ResourceVein>(out var vein);
            Debug.Assert(wasResourceVeinSelected, $"ResourceVein is not selected from current context.");
            
            if (wasResourceVeinSelected && !queriedVeinsBuffer.Remove(vein))
            {
                // if the count of queried veins is over maximum capacity, remove the oldest one.
                if (queriedVeinsBuffer.Count >= MAX_VEIN_QUERY_COUNT)
                {
                    queriedVeinsBuffer.RemoveAt(0);
                }
                
                queriedVeinsBuffer.Add(vein);
            }

            for (var i = 0; i < queriedVeinsBuffer.Count; i++)
            {
                var e = queriedVeinsBuffer[i];
                if (m_QueriedResourceContext.TryAdd(e.ResourceType, e.Amount))
                {
                    continue;
                }
                m_QueriedResourceContext[e.ResourceType] += e.Amount;
            }

            m_QueryInformationLogBuilder.AppendLine($"{queriedVeinsBuffer.Count} out of {MAX_VEIN_QUERY_COUNT} veins are selected.");
            foreach (var (resourceType, amount) in m_QueriedResourceContext)
            {
                m_QueryInformationLogBuilder.Append($"{resourceType}: {amount}, ");
            }
            
            informationOverlayPanel.gameObject.SetActive(true);
            informationText.text = m_QueryInformationLogBuilder.ToString();
            
            m_QueryInformationLogBuilder.Clear();
            m_QueriedResourceContext.Clear();
        }
        
        #region IInputHandler

        public void ResetInputHandlingState()
        {
        }

        public bool HandlePlayerSelectNextCar(InputAction.CallbackContext context)
        {
            return true;
        }

        public bool HandlePlayerZoom(InputAction.CallbackContext context)
        {
            return false;
        }

        public bool HandlePlayerRotateCamera(InputAction.CallbackContext context)
        {
            return false;
        }

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

        public bool HandlePlayerShowWheelMenu(InputAction.CallbackContext context)
        {
            return true;
        }

        public bool HandlePlayerScaleMiningArea(InputAction.CallbackContext context)
        {
            return true;
        }
        public bool HandlePlayerChangeHUDLayer(InputAction.CallbackContext context)
        {
            return false;
        }

        public bool HandleUINavigate(InputAction.CallbackContext context)
        {
            return true;
        }

        public bool HandleUISubmit(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                SelectVeins();
            }
            
            return true;
        }

        public bool HandleUIApply(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                OnMiningTargetSelectedCallback?.Invoke(queriedVeinsBuffer);
                
                Deactivate();
            }

            return true;
        }

        public bool HandleUICancel(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                OnMiningTargetSelectedCallback?.Invoke(null);
                
                Deactivate();
            }
            
            return true;
        }

        public bool HandleUIPoint(InputAction.CallbackContext context) => false;

        public bool HandleUIClick(InputAction.CallbackContext context) => false;

        public bool HandleUIRightClick(InputAction.CallbackContext context) => false;

        #endregion
    }
}