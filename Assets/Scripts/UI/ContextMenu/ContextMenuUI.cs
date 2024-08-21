using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utility;
using Data;
using InputComponents;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

namespace UI
{
    [System.Serializable]
    public class ContextMenuUI : MonoBehaviour, IInputHandler
    {
        [Header("Component References")]
        [SerializeField] private Canvas canvas;
        
        [Header("ContextMenu Settings"), Space(10)]
        [SerializeField] private ContextMenuItem contextMenuItemPrefab;
        [SerializeField] private float innerRadius;
        [SerializeField] private float outerRadius;
        [SerializeField] private float selectableSearchInterval = 0.1f;
        [SerializeField] private RectTransform visualizerTransform;
        [SerializeField] private float visualizerOperationRadius = 10;

        [Header("ContextMenu Detail Item Settings"), Space(10)]
        [SerializeField] private ContextMenuDetailItem contextMenuDetailItem;
        [SerializeField] private GameObject contextMenuSelectDescripition;
        
        private float m_MenuItemAngularGap;
        private Vector2 m_LastAxisInputValue;
        private float m_LastSelectableUpdateTime;
        private ContextMenuItem m_LastSelectedMenuItem;
        private EventSystem m_EventSystem;

        private IContextMenuDataProvider m_CurrentMenuDataProvider;
        private readonly List<ContextMenuItem> m_ActiveContextMenuItems = new(8);
        private ObjectPool<ContextMenuItem> m_ContextMenuItemPool;
        private List<ContextMenuData> m_CurrentMenuData;
        
        public bool IsEnabled { get; private set; }
        
        public float InputAngularDirection { get; private set; }
        
        public IReadOnlyList<ContextMenuItem> ActiveContextMenuItems => m_ActiveContextMenuItems;

        #region MonoBehaviour Events
        
        private void Awake()
        {
            canvas.enabled = false;
            m_ContextMenuItemPool = ObjectPool<ContextMenuItem>.CreateObjectPool(contextMenuItemPrefab, transform);
            m_EventSystem = EventSystem.current;
        }

        private void OnGUI()
        {
            if (!IsEnabled)
            {
                return;
            }

            foreach (var menuData in m_CurrentMenuData)
            {
                if (GUILayout.Button(menuData.itemName))
                {
                    CloseMenu();
                    menuData.onClickEvent.Invoke();
                }
            }
        }
        
        #endregion // MonoBehaviour Events

        #region ContextMenuUI Managing
        
        public void OpenMenu(Vector2 screenPos, IContextMenuDataProvider provider)
        {
            IsEnabled = true;
            canvas.enabled = true;
            m_EventSystem.enabled = !IsEnabled;

            InputComponent.Get.RegisterHandler(this);
            TimeScaleManager.Get.ChangeGlobalTimeScale(0f);

            m_CurrentMenuDataProvider = provider;
            m_LastSelectedMenuItem = null;
                
            CreateContextMenuItems(screenPos, provider);
        }

        public void CloseMenu()
        {
            IsEnabled = false;
            canvas.enabled = false;
            m_EventSystem.enabled = !IsEnabled;
            InputComponent.Get.UnregisterHandler(this);
            TimeScaleManager.Get.ChangeGlobalTimeScale(1f);
            
            m_LastAxisInputValue = Vector2.zero;
            
            m_CurrentMenuDataProvider?.OnContextMenuClosed();
            m_CurrentMenuDataProvider = null;

            visualizerTransform.localPosition = Vector2.zero;
            SetContextMenuDetailItem(null);

            m_ContextMenuItemPool.ReturnObjects(m_ActiveContextMenuItems);
            m_ActiveContextMenuItems.Clear();
        }

        public void CreateContextMenuItems(Vector2 screenPosition, IContextMenuDataProvider provider)
        {
            m_CurrentMenuData = provider.GetContextMenuData();
            var providerState = provider.GetContextMenuConditionState();
            m_ContextMenuItemPool.ReturnObjects(m_ActiveContextMenuItems);
            m_ActiveContextMenuItems.Clear();
            
            var validMenuCount = m_CurrentMenuData.Count(menuData =>
                menuData.exposeCondition == ContextMenuCondition.None ||
                (menuData.exposeCondition != ContextMenuCondition.None && providerState.HasFlag(menuData.exposeCondition)));
            
            var angleOffset = validMenuCount == 2 ? 180f : 90f;
            m_MenuItemAngularGap = 360f / validMenuCount;
            var distanceDelta = Mathf.Lerp(innerRadius, outerRadius, 0.5f);
            
            foreach (var menuData in m_CurrentMenuData)
            {
                if (!providerState.HasFlag(menuData.exposeCondition))
                {
                    continue;
                }
                var resourceData = provider.GetResourceData(menuData.exposeCondition);
                var itemPosition = new Vector2(Mathf.Cos(angleOffset * Mathf.Deg2Rad), Mathf.Sin(angleOffset * Mathf.Deg2Rad)) * distanceDelta;
                
                var menuItem = m_ContextMenuItemPool.GetOrCreate();
                menuItem.CachedTransform.SetParent(transform);
                menuItem.CachedTransform.localPosition = itemPosition;
                menuItem.SetData(menuData, angleOffset, CloseMenu, resourceData);
                
                m_ActiveContextMenuItems.Add(menuItem);
                
                angleOffset -= m_MenuItemAngularGap;
                if (angleOffset < 0)
                {
                    angleOffset += 360;
                }
            }
        }

        public void SetContextMenuDetailItem(ContextMenuItem contextMenuItem, bool active = false)
        {
            if (!IsEnabled || contextMenuItem == null)
            {
                contextMenuDetailItem.gameObject.SetActive(false);
                contextMenuSelectDescripition.SetActive(true);
                return;
            }
            
            contextMenuDetailItem.gameObject.SetActive(active);
            contextMenuSelectDescripition.SetActive(!active);

            if (!active)
            {
                return;
            }

            if (DatabaseUtility.TryGetData(Database.TextData, contextMenuItem.Identifier, "", "Desc", out var titleTextData, out var descTextData) && descTextData != null)
            {
                contextMenuDetailItem.DetailName = titleTextData.korean;
                contextMenuDetailItem.DetailDescription = descTextData.korean;
                contextMenuDetailItem.SetResource(contextMenuItem.ResourceData);
            }
            else
            {
                contextMenuDetailItem.gameObject.SetActive(false);
                contextMenuSelectDescripition.SetActive(true);
            }
        }

        public bool TriggerLastSelectedMenuItem()
        {
            if (m_LastSelectedMenuItem == null)
            {
                return false;
            }
            
            m_LastSelectedMenuItem.onClick.Invoke();
            return true;
        }
        
        #endregion // ContextMenuUI Managing
        
        #region UI Events Handling

        private void FindSelectable(Vector2 direction)
        {
            if (direction.sqrMagnitude < 0.2f)
            {
                return;
            }

            InputAngularDirection = Vector2.SignedAngle(Vector2.right, direction);
            if (InputAngularDirection < 0)
            {
                InputAngularDirection += 360;
            }

            var angularGapHalf = m_MenuItemAngularGap * 0.5f;
            foreach (var item in m_ActiveContextMenuItems)
            {
                if (!(InputAngularDirection >= item.AngularPosition - angularGapHalf) ||
                    !(InputAngularDirection <= item.AngularPosition + angularGapHalf))
                {
                    item.Select(false);
                    continue;
                }
                
                m_LastSelectedMenuItem = item;
                SetContextMenuDetailItem(m_LastSelectedMenuItem, visualizerTransform.localPosition != Vector3.zero || m_LastSelectedMenuItem != null);
                item.Select(true);
            }
        }
        
        #endregion // UI Events Handling

        #region IInputHandler

        public void ResetInputHandlingState()
        {
        }

        public bool HandlePlayerSelectNextCar(InputAction.CallbackContext context) => false;

        public bool HandlePlayerZoom(InputAction.CallbackContext context) => false;

        public bool HandlePlayerRotateCamera(InputAction.CallbackContext context) => false;

        public bool HandlePlayerMoveCamera(InputAction.CallbackContext context) => false;

        public bool HandlePlayerShowWheelMenu(InputAction.CallbackContext context)
        {
            if (!IsEnabled ||
                context.phase != InputActionPhase.Canceled)
            {
                return false;
            }
            
            CloseMenu();
            //TriggerLastSelectedMenuItem();
            return true;
        }

        public bool HandlePlayerChangeHUDLayer(InputAction.CallbackContext context) => false;

        public bool HandlePlayerScaleMiningArea(InputAction.CallbackContext context) => false;

        public bool HandleUINavigate(InputAction.CallbackContext context)
        {
            if (!IsEnabled)
            {
                return false;
            }
            
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    break;
                case InputActionPhase.Performed:
                    var value = context.ReadValue<Vector2>();
                    m_LastAxisInputValue = value;
                    visualizerTransform.localPosition = value * visualizerOperationRadius;
                    if (Time.unscaledTime > m_LastSelectableUpdateTime + selectableSearchInterval)
                    {
                        m_LastSelectableUpdateTime = Time.unscaledTime;
                        FindSelectable(value);
                    }
                    break;
                case InputActionPhase.Canceled:
                    visualizerTransform.localPosition = Vector2.zero;

                    if (m_LastSelectedMenuItem != null)
                    {
                        CloseMenu();
                        m_LastSelectedMenuItem.onClick.Invoke();
                        m_LastSelectedMenuItem = null;
                    }
                    break;
            }

            return true;
        }

        public bool HandleUISubmit(InputAction.CallbackContext context)
        {
            if (IsEnabled && context.phase == InputActionPhase.Performed)
            {
                CloseMenu();
                TriggerLastSelectedMenuItem();
            }

            return true;
        }

        public bool HandleUIApply(InputAction.CallbackContext context)
        {
            return true;
        }

        public bool HandleUICancel(InputAction.CallbackContext context)
        {
            if (IsEnabled && context.phase == InputActionPhase.Performed)
            {
                CloseMenu();
            }

            return true;
        }

        public bool HandleUIPoint(InputAction.CallbackContext context)
        {
            if (!IsEnabled)
            {
                return false;
            }
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    break;
                case InputActionPhase.Performed:
                    var mousePos = context.ReadValue<Vector2>();
                    var screenCenterValue = new Vector2(Screen.width / 2, Screen.height / 2);
                    var value = (mousePos - screenCenterValue).normalized;
                    visualizerTransform.localPosition = value * visualizerOperationRadius;
                    m_LastAxisInputValue = value;
                    if (Time.unscaledTime > m_LastSelectableUpdateTime + selectableSearchInterval)
                    {
                        m_LastSelectableUpdateTime = Time.unscaledTime;
                        FindSelectable(value);
                    }
                    break;
                case InputActionPhase.Canceled:
                    break;
            }

            return true;
        }

        public bool HandleUIClick(InputAction.CallbackContext context)
        {
            if (!IsEnabled)
            {
                return false;
            }

            FindSelectable(m_LastAxisInputValue);

            if (m_LastSelectedMenuItem != null)
            {
                CloseMenu();
                m_LastSelectedMenuItem.onClick.Invoke();
                m_LastSelectedMenuItem = null;
            }
            return true;
        }

        public bool HandleUIRightClick(InputAction.CallbackContext context) => false;


        #endregion // IInputHandler
    }
}