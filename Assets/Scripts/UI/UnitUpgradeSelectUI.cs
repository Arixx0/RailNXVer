using Data;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace UI
{
    public class UnitUpgradeSelectUI : MonoBehaviour, InputComponents.IInputHandler
    {
        [SerializeField] private UnitUpgradeOptionCard optionCardPrefab;
        [SerializeField] private Canvas canvas;
        [SerializeField] private Transform contentLayoutGroup;
        [SerializeField] private LabeledButton cancelButton;
        [SerializeField] private InputComponents.AxisInputIntervalModule axisInputIntervalModule;

        private Utility.ObjectPool<UnitUpgradeOptionCard> m_OptionCardPool;
        private readonly List<UnitUpgradeOptionCard> m_ActiveOptionCards = new();
        
        private IUnitUpgradeSelectCallbackReceiver CallbackReceiver { get; set; }
        
        private void Awake()
        {
            m_OptionCardPool = Utility.ObjectPool<UnitUpgradeOptionCard>.CreateObjectPool(optionCardPrefab, transform);
            
            cancelButton.onClick.RemoveAllListeners();
            cancelButton.onClick.AddListener(CancelUpgrade);

            canvas.enabled = false;
        }

        private void Update()
        {
            ProcessNavigateInput();
        }

        public void ShowWithOptions(IUnitUpgradeSelectCallbackReceiver receiver)
        {
            canvas.enabled = true;
            InputComponents.InputComponent.Get.RegisterHandler(this);
            TimeScaleManager.Get.ChangeGlobalTimeScale(0f);
            
            m_OptionCardPool.ReturnObjects(m_ActiveOptionCards);
            m_ActiveOptionCards.Clear();
            
            CallbackReceiver = receiver;

            var optionIdentifiers = receiver.GetUnitUpgradeOptionIdentifiers();
            var capturedUnitStatData = receiver.GetCapturedUnitStatData();
            
            foreach (var targetOptionIdentifier in optionIdentifiers)
            {
                CreateUpgradeCard(targetOptionIdentifier, capturedUnitStatData);
            }
            
            m_ActiveOptionCards[0].Select();
        }

        private void CreateUpgradeCard(string targetOptionIdentifier, UnitUpgradeStatCaptureData capturedUnitStatData)
        {
            if (Utility.DatabaseUtility.TryGetData(Database.UnitStatData, targetOptionIdentifier, out var upgradedStatData)
                && Utility.DatabaseUtility.TryGetData(Database.CarCostData, targetOptionIdentifier, out var upgradeCostData))
            {
                var resourceData = new Dictionary<ResourceType, float>();

                var resources = new[]
                {
                    (upgradeCostData.ResourceSteel, ResourceType.Iron),
                    (upgradeCostData.ResourceMithryl, ResourceType.Mythrill),
                    (upgradeCostData.ResourceAdamantium, ResourceType.Adamantine),
                    (upgradeCostData.ResourceTitanium, ResourceType.Titanium),
                    (upgradeCostData.CircuitNormal, ResourceType.Circuit)
                };

                foreach (var (amount, type) in resources)
                {
                    if (amount > 0) resourceData.Add(type, amount);
                }

                var optionDescription = string.Empty;
                optionDescription = $"{optionDescription}{nameof(upgradedStatData.MaxHealth)}:{capturedUnitStatData.HealthPoint} > {upgradedStatData.MaxHealth}\n";
                optionDescription = $"{optionDescription}{nameof(upgradedStatData.Armor)}:{capturedUnitStatData.ArmorPoint} > {upgradedStatData.Armor}\n";
                optionDescription = $"{optionDescription}{nameof(upgradedStatData.FuelEfficiency)}:{capturedUnitStatData.FuelEfficiency} > {upgradedStatData.FuelEfficiency}\n";
                optionDescription = $"{optionDescription}{nameof(upgradedStatData.UnitSize)}:{capturedUnitStatData.UnitSize} > {upgradedStatData.UnitSize}\n";

                var optionCard = m_OptionCardPool.GetOrCreate();
                optionCard.CachedTransform.SetParent(contentLayoutGroup);
                optionCard.UpgradeName = targetOptionIdentifier;
                optionCard.UpgradeDescription = optionDescription;
                optionCard.SetData(resourceData, CallbackReceiver);
                optionCard.onClick.RemoveAllListeners();

                optionCard.onClick.AddListener(() =>
                {
                    canvas.enabled = false;
                    InputComponents.InputComponent.Get.UnregisterHandler(this);
                    TimeScaleManager.Get.ChangeGlobalTimeScale(1f);

                    CallbackReceiver.OnUnitUpgradeSelected(targetOptionIdentifier);
                });

                m_ActiveOptionCards.Add(optionCard);
            }
        }

        private void CancelUpgrade()
        {
            canvas.enabled = false;
            InputComponents.InputComponent.Get.UnregisterHandler(this);
            TimeScaleManager.Get.ChangeGlobalTimeScale(1f);
            
            CallbackReceiver?.OnUnitUpgradeSelected(string.Empty);
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
                value.y = Mathf.Sign(value.x);
            }
            
            var currentSelected = EventSystem.current.currentSelectedGameObject;
            if (currentSelected == null)
            {
                m_ActiveOptionCards[0].Select();
                return;
            }

            if (!currentSelected.gameObject.transform.IsChildOf(canvas.transform))
            {
                return;
            }

            if (!currentSelected.TryGetComponent(out Selectable currentSelectable))
            {
                return;
            }

            currentSelectable = currentSelectable.FindSelectable(value);
            currentSelectable?.Select();
        }
        
        #region IInputHandler

        public void ResetInputHandlingState()
        {
        }

        public bool HandlePlayerSelectNextCar(InputAction.CallbackContext context) => true;

        public bool HandlePlayerZoom(InputAction.CallbackContext context) => false;

        public bool HandlePlayerRotateCamera(InputAction.CallbackContext context) => true;

        public bool HandlePlayerMoveCamera(InputAction.CallbackContext context) => true;

        public bool HandlePlayerShowWheelMenu(InputAction.CallbackContext context) => true;

        public bool HandlePlayerScaleMiningArea(InputAction.CallbackContext context) => true;

        public bool HandlePlayerChangeHUDLayer(InputAction.CallbackContext context) => false;

        public bool HandleUINavigate(InputAction.CallbackContext context)
        {
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
            var selected = EventSystem.current.currentSelectedGameObject;
            if (selected != null)
            {
                selected.GetComponent<Button>().onClick.Invoke();
            }
            
            return true;
        }

        public bool HandleUIApply(InputAction.CallbackContext context) => true;

        public bool HandleUICancel(InputAction.CallbackContext context)
        {
            CancelUpgrade();
            return true;
        }

        public bool HandleUIPoint(InputAction.CallbackContext context) => false;

        public bool HandleUIClick(InputAction.CallbackContext context) => false;

        public bool HandleUIRightClick(InputAction.CallbackContext context) => false;


        #endregion // IInputHandler
    }

    public interface IUnitUpgradeSelectCallbackReceiver
    {
        // Triggered when an upgrade option is selected
        // optionIdentifier: The identifier of the selected upgrade option. Can be empty or null if the select is canceled.
        public void OnUnitUpgradeSelected(string optionIdentifier);

        public UnitUpgradeStatCaptureData GetCapturedUnitStatData();

        public string[] GetUnitUpgradeOptionIdentifiers();

        public bool CheckAvailabilityResource(ResourceType resourceType, float amount);
    }
}