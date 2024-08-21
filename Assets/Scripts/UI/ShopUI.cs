using Augments;
using Data;
using InputComponents;
using TrainScripts;
using Utility;

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using TMPro;

namespace UI
{
    public class ShopUI : MonoBehaviour, IInputHandler
    {
        #region Fields & Properties

        [SerializeField] private Canvas canvas;
        [SerializeField] private ShopItem shopItemPrefab;
        [SerializeField] private Transform layoutGroup;
        [SerializeField] private GameObject itemDetailsPanel;
        [SerializeField] private TextMeshProUGUI itemNameText;
        [SerializeField] private TextMeshProUGUI itemDescriptionText;
        [SerializeField] private Button buyButton;
        [SerializeField] private Button cancelButton;
        [SerializeField] private GameObject purchaseResultPanel;
        [SerializeField] private TextMeshProUGUI purchaseResultText;
        [SerializeField] private AxisInputIntervalModule axisInputIntervalModule;

        private List<ShopKeeperItemContext> m_ShopItems;
        private readonly List<ShopItem> m_ActiveShopItemInstances = new(8);
        private ObjectPool<ShopItem> m_ShopItemInstancePool;
        private Action m_OnUIClosedCallback;
        
        private ShopKeeperItemsDefinition CurrentDefinition { get; set; }
        
        private Train CurrentTrain { get; set; }

        private string CurrentSelectedItemIdentifier { get; set; } = string.Empty;

        private int CurrentSelectedItemIndex { get; set; } = -1;
        
        #endregion // Fields & Properties
        
        #region MonoBehaviour Events

        private void Awake()
        {
            m_ShopItemInstancePool = ObjectPool<ShopItem>.CreateObjectPool(shopItemPrefab, transform);

            canvas.enabled = false;
            itemDetailsPanel.SetActive(false);
        }

        private void Update()
        {
            ProcessNavigateInput();
        }

        #endregion // MonoBehaviour Events
        
        #region ShopUI Implementations

        public void AssignProperties(ShopKeeperItemsDefinition definition, Action onUIClosed = null)
        {
            m_OnUIClosedCallback = onUIClosed;
            
            CurrentTrain = FindObjectOfType<Train>();
            CurrentDefinition = definition;
        }

        public void Show(ShopKeeperItemsDefinition definition, Action onUIClosed = null)
        {
            canvas.enabled = true;
            itemDetailsPanel.SetActive(false);
            purchaseResultPanel.SetActive(false);
            InputComponent.Get.RegisterHandler(this);
            
            m_ShopItemInstancePool.ReturnObjects(m_ActiveShopItemInstances);
            m_ActiveShopItemInstances.Clear();
            m_OnUIClosedCallback = onUIClosed;
            
            CurrentTrain = FindObjectOfType<Train>();
            CurrentDefinition = definition;

            m_ShopItems = definition.GetItems();
            for (var i = 0; i < m_ShopItems.Count; i++)
            {
                var contextData = m_ShopItems[i];
                var itemInstance = m_ShopItemInstancePool.GetOrCreate();
                itemInstance.transform.SetParent(layoutGroup);

                var isItemValid = Database.ShopItemsData.TryGetValue(contextData.identifier, out var itemData);
                Debug.Assert(isItemValid, $"Item {contextData.identifier} is not found in ShopItemsData", this);

                itemInstance.ItemIdentifier = contextData.identifier;
                itemInstance.ItemName = contextData.identifier;
                itemInstance.ItemCost = itemData.Cost.ToString();
                itemInstance.IndexFromSlot = i;

                itemInstance.onClick.RemoveAllListeners();
                itemInstance.onClick.AddListener(() => SelectItem(itemInstance));
                itemInstance.navigation = new Navigation
                {
                    mode = Navigation.Mode.Explicit | Navigation.Mode.Automatic,
                    selectOnUp = null,
                    selectOnDown = null,
                    selectOnLeft = null,
                    selectOnRight = buyButton
                };

                m_ActiveShopItemInstances.Add(itemInstance);
            }

            m_ActiveShopItemInstances[0].Select();
            SelectItem(m_ActiveShopItemInstances[0]);
        }

        public void Show()
        {
            // canvas.enabled = true;
            //
            // m_ShopItemInstancePool.ReturnObjects(m_ActiveShopItemInstances);
            // m_ActiveShopItemInstances.Clear();
            //
            // for (int i = 0; i < 3; i++)
            // {
            //     var currentIndex = i;
            //
            //     var shopItemInstance = m_ShopItemInstancePool.GetOrCreate();
            //     m_ActiveShopItemInstances.Add(shopItemInstance);
            //     
            //     shopItemInstance.transform.SetParent(layoutGroup);
            //     shopItemInstance.ItemName = "item_" + i;
            //     shopItemInstance.ItemCost = (50 * (i + 1)).ToString();
            //     shopItemInstance.ItemImage.color = new Color(UnityEngine.Random.Range(0, 256), UnityEngine.Random.Range(0, 256), UnityEngine.Random.Range(0, 256));
            //     shopItemInstance.onClick.RemoveAllListeners();
            //     shopItemInstance.onClick.AddListener(() => SelectItem(currentIndex));
            // }
        }

        public void Close()
        {
            canvas.enabled = false;
            itemDetailsPanel.SetActive(false);
            purchaseResultPanel.SetActive(false);
            InputComponent.Get.UnregisterHandler(this);
            
            m_OnUIClosedCallback?.Invoke();
        }

        public void BuyItem()
        {
            Debug.Assert(CurrentTrain != null, "CurrentTrain is not assigned", this);
            Debug.Assert(CurrentDefinition != null, "CurrentDefinition is not assigned", this);
            Debug.Assert(!string.IsNullOrEmpty(CurrentSelectedItemIdentifier), $"CurrentSelectedItemIdentifier is not assigned", this);

            var isItemValid = Database.ShopItemsData.TryGetValue(CurrentSelectedItemIdentifier, out var itemData);
            Debug.Assert(isItemValid, $"Item {CurrentSelectedItemIdentifier} is not found in ShopItemsData", this);

            if (m_ShopItems[CurrentSelectedItemIndex].stockedAmount <= 0)
            {
                Debug.LogWarning($"item {CurrentSelectedItemIdentifier} is out of stock", this);
                purchaseResultPanel.SetActive(true);
                purchaseResultText.text = "PurchaseFailed: Out of stock!";
                return;
            }

            if (!CurrentTrain.TryConsumeResourcesFromInventory(ResourceType.Circuit, itemData.Cost))
            {
                Debug.LogWarning($"Failed to purchase {CurrentSelectedItemIdentifier} due to insufficient resources", this);
                purchaseResultPanel.SetActive(true);
                purchaseResultText.text = "PurchaseFailed: Not enough money!";
                return;
            }

            if (itemData.ItemType == ItemType.Module && CurrentTrain.LeftOverCarSlots <= 0)
            {
                Debug.LogWarning($"Failed to purchase {CurrentSelectedItemIdentifier} due to insufficient car slots", this);
                purchaseResultPanel.SetActive(true);
                purchaseResultText.text = "PurchaseFailed: Not enough car slots!";
                return;
            }
            
            purchaseResultPanel.SetActive(true);
            purchaseResultText.text = $"PurchaseSucceeded: {itemData.Identifier}({itemData.DropAmount}, {itemData.ItemType})";
            
            switch (itemData.ItemType)
            {
                case ItemType.Augment:
                    var augmentAsset = Resources.Load<Augment>($"AgumentAsset/{itemData.Identifier}");
                    Debug.Assert(augmentAsset != null, $"Augment asset {itemData.Identifier} is not found", this);

                    CurrentTrain.AddAugment(augmentAsset);
                    break;
                case ItemType.Resource:
                    var isValidResourceType = EnumUtility.IdentifierToItemType(itemData.Identifier, out var resourceType);
                    Debug.Assert(isValidResourceType, $"Resource type {itemData.Identifier} is not found", this);

                    CurrentTrain.AddResourceToInventory(resourceType, itemData.DropAmount);
                    break;
                case ItemType.Module:
                    var rawIdentifier = itemData.Identifier.Split('_');
                    var categoryIdentifier = $"{rawIdentifier[0]}_{rawIdentifier[1]}";
                    var modulePrefab = Database.UnitUpgradeTree[categoryIdentifier][itemData.Identifier].model.GetComponent<Car>();
                    
                    CurrentTrain.SpawnCar(modulePrefab);
                    break;
                case ItemType.CarBasePart:
                    CurrentTrain.AdjustMaxSpawnableCars(1);
                    break;
            }

            m_ShopItems[CurrentSelectedItemIndex].stockedAmount--;
        }

        private void SelectItem(int index)
        {
        }

        private void SelectItem(ShopItem shopItem)
        {
            var isItemValid = Database.ShopItemsData.TryGetValue(shopItem.ItemIdentifier, out var itemData);
            Debug.Assert(isItemValid, $"Item {shopItem.ItemIdentifier} is not found in ShopItemsData", this);
            
            CurrentSelectedItemIdentifier = shopItem.ItemIdentifier;
            CurrentSelectedItemIndex = shopItem.IndexFromSlot;
            
            itemNameText.text = itemData.Identifier;
            itemDescriptionText.text = itemData.ToString();
            itemDetailsPanel.SetActive(true);
            
            purchaseResultPanel.SetActive(false);
        }

        private void ProcessNavigateInput()
        {
            if (!axisInputIntervalModule.CanHandleInput(Time.unscaledTime))
            {
                return;
            }

            var value = axisInputIntervalModule.Value;
            if (value is { x: 0f, y: 0f })
            {
                return;
            }

            if (Mathf.Abs(value.x) > Mathf.Abs(value.y))
            {
                value.x = Mathf.Sign(value.x);
                value.y = 0f;
            }
            else
            {
                value.x = 0f;
                value.y = Mathf.Sign(value.y);
            }

            var currentSelected = EventSystem.current.currentSelectedGameObject;
            if (currentSelected == null)
            {
                m_ActiveShopItemInstances[0].Select();
                return;
            }

            if (!currentSelected.TryGetComponent(out Selectable currentSelectable))
            {
                return;
            }

            currentSelectable = currentSelectable.FindSelectable(value);
            currentSelectable?.Select();

            if (currentSelectable is ShopItem shopItem)
            {
                shopItem.onClick.Invoke();
            }
        }
        
        #endregion // ShopUI Implementations

        #region IInputHandler

        public void ResetInputHandlingState()
        {
        }

        public bool HandlePlayerSelectNextCar(InputAction.CallbackContext context) => enabled;

        public bool HandlePlayerZoom(InputAction.CallbackContext context) => enabled;

        public bool HandlePlayerRotateCamera(InputAction.CallbackContext context) => enabled;

        public bool HandlePlayerMoveCamera(InputAction.CallbackContext context) => enabled;

        public bool HandlePlayerShowWheelMenu(InputAction.CallbackContext context) => enabled;

        public bool HandlePlayerScaleMiningArea(InputAction.CallbackContext context) => enabled;

        public bool HandlePlayerChangeHUDLayer(InputAction.CallbackContext context) => enabled;

        public bool HandleUINavigate(InputAction.CallbackContext context)
        {
            if (!enabled)
            {
                return false;
            }
            
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    axisInputIntervalModule.IsEnabled = true;
                    axisInputIntervalModule.Value = context.ReadValue<Vector2>();
                    break;
                case InputActionPhase.Performed:
                    axisInputIntervalModule.IsEnabled = true;
                    axisInputIntervalModule.Value = context.ReadValue<Vector2>();
                    break;
                case InputActionPhase.Canceled:
                    axisInputIntervalModule.IsEnabled = false;
                    axisInputIntervalModule.Reset();
                    break;
            }

            return true;
        }

        public bool HandleUISubmit(InputAction.CallbackContext context)
        {
            if (!enabled)
            {
                return false;
            }

            if (context.phase == InputActionPhase.Performed)
            {
                var selected = EventSystem.current.currentSelectedGameObject;
                if (selected != null)
                {
                    selected.GetComponent<Button>().onClick.Invoke();
                }
            }
            
            return true;
        }

        public bool HandleUIApply(InputAction.CallbackContext context)
        {
            return true;
        }

        public bool HandleUICancel(InputAction.CallbackContext context)
        {
            if (!enabled)
            {
                return false;
            }

            if (context.phase != InputActionPhase.Performed)
            {
                return true;
            }
            
            Close();
            return true;
        }

        public bool HandleUIPoint(InputAction.CallbackContext context) => false;

        public bool HandleUIClick(InputAction.CallbackContext context) => false;

        public bool HandleUIRightClick(InputAction.CallbackContext context) => false;
        #endregion
    }
}

