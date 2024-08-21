using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

using TrainScripts;
using Data;

namespace UI
{
    public class CarElectricPowerChangeItem : Slider, InputComponents.IInputHandler
    {
        [SerializeField] private Image[] powerIndicators;
        [SerializeField] private ChangingTextLoader electricPowerUsageTypeTextLoader;
        [SerializeField] private ChangingTextLoader electricPowerStatusTextLoader;
        [SerializeField] private TextMeshProUGUI changeElectricPowerUsageDisplayText;
        [SerializeField] private GameObject overloadWarningPanel;

        private Car m_Car;

        protected override void Start()
        {
            base.Start();
            UpdateIndicators();
            onValueChanged.AddListener(OnSliderValueChanged);
        }

        public void OpenElectricPowerChangeItem(Car car)
        {
            if (car == null)
            {
                return;
            }

            gameObject.SetActive(true);
            m_Car = car;
            value = System.Convert.ToInt32(car.CurrentElectricPowerUsageType);
            InputComponents.InputComponent.Get?.RegisterHandler(this);
        }

        //private void UpdateSliderValue(PointerEventData eventData)
        //{
        //    RectTransform sliderRectTransform = GetComponent<RectTransform>();
        //    RectTransformUtility.ScreenPointToLocalPointInRectangle(sliderRectTransform, eventData.position, eventData.pressEventCamera, out var localPoint);
        //    float newValue = Mathf.Clamp01((localPoint.x - sliderRectTransform.rect.x) / sliderRectTransform.rect.width);
        //    value = newValue * (maxValue - minValue) + minValue;
        //    value = Mathf.RoundToInt(value);

        //    UpdateIndicators();
        //}

        private void OnSliderValueChanged(float value)
        {
            UpdateIndicators();
        }

        private void UpdateIndicators()
        {
            int value = Mathf.RoundToInt(this.value);

            for (int i = 0; i < powerIndicators.Length; i++)
            {
                if (i < value)
                {
                    powerIndicators[i].color = Color.white;
                }
                else
                {
                    powerIndicators[i].color = Color.clear;
                }
            }
            if (Application.isPlaying)
            {
                electricPowerUsageTypeTextLoader.ChangeText(((Car.ElectricPowerUsageType)value).ToString());
                var previewCarElectricPower = value * 0.5f * m_Car.StatComponent.energyCost.Value;
                var overloading = m_Car.ParentTrain.TrainCurrentElectricPowerGeneration < m_Car.ParentTrain.TrainCurrentElectricPowerUsage - m_Car.CurrentElectricPowerUsage + previewCarElectricPower;
                var statusTextIdentifier = m_Car is UtilityCompartment ? $"_{m_Car.IdentifierData.SubType}_{m_Car.StatComponent.UpgradeLevel}" : "";
                
                m_Car.ParentTrain.HUD.PreViewElectricPowerValue(previewCarElectricPower, m_Car.CurrentElectricPowerUsage);
                changeElectricPowerUsageDisplayText.text = $"{m_Car.CurrentElectricPowerUsage} ¢º {previewCarElectricPower}";
                electricPowerStatusTextLoader.ChangeText($"{m_Car.IdentifierData.ObjectType}_{value}{statusTextIdentifier}");
                overloadWarningPanel.SetActive(overloading);
            }
        }

        #region IInputHandler

        public void ResetInputHandlingState()
        {
        }

        public bool HandlePlayerSelectNextCar(InputAction.CallbackContext context)
        {
            if (!gameObject.activeSelf || context.phase != InputActionPhase.Performed)
            {
                return false;
            }

            var offset = Mathf.RoundToInt(context.ReadValue<float>());
            value += offset;

            UpdateIndicators();
            return true;
        }

        public bool HandlePlayerChangeHUDLayer(InputAction.CallbackContext context) => false;

        public bool HandlePlayerZoom(InputAction.CallbackContext context) => false;

        public bool HandlePlayerRotateCamera(InputAction.CallbackContext context) => false;

        public bool HandlePlayerMoveCamera(InputAction.CallbackContext context) => false;

        public bool HandlePlayerShowWheelMenu(InputAction.CallbackContext context) => false;

        public bool HandlePlayerScaleMiningArea(InputAction.CallbackContext context) => false;

        public bool HandleUINavigate(InputAction.CallbackContext context) => false;

        public bool HandleUISubmit(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Performed)
            {
                return false;
            }

            m_Car?.ElectricPowerChange((Car.ElectricPowerUsageType)value);
            gameObject.SetActive(false);
            InputComponents.InputComponent.Get.UnregisterHandler(this);
            return true;
        }

        public bool HandleUIApply(InputAction.CallbackContext context) => false;

        public bool HandleUICancel(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Performed)
            {
                return false;
            }
            m_Car.ParentTrain.HUD.PreViewElectricPowerValue(m_Car.CurrentElectricPowerUsage, m_Car.CurrentElectricPowerUsage);
            gameObject.SetActive(false);
            InputComponents.InputComponent.Get.UnregisterHandler(this);
            return true;
        }


        public bool HandleUIPoint(InputAction.CallbackContext context) => false;

        public bool HandleUIClick(InputAction.CallbackContext context) => false;

        public bool HandleUIRightClick(InputAction.CallbackContext context) => false;

        #endregion
    }

    [CustomEditor(typeof(CarElectricPowerChangeItem))]
    public class CarElectricPowerChangeItemEditor : SliderEditor
    {
        public override void OnInspectorGUI()
        {
            CarElectricPowerChangeItem carElectricPowerChangeItem = (CarElectricPowerChangeItem)target;

            serializedObject.Update();

            SerializedProperty powerIndicators = serializedObject.FindProperty("powerIndicators");
            EditorGUILayout.PropertyField(powerIndicators, true);
            SerializedProperty electricPowerUsageTypeTextLoader = serializedObject.FindProperty("electricPowerUsageTypeTextLoader");
            EditorGUILayout.PropertyField(electricPowerUsageTypeTextLoader);
            SerializedProperty electricPowerStatusTextLoader = serializedObject.FindProperty("electricPowerStatusTextLoader");
            EditorGUILayout.PropertyField(electricPowerStatusTextLoader);
            SerializedProperty changeElectricPowerUsageDisplayText = serializedObject.FindProperty("changeElectricPowerUsageDisplayText");
            EditorGUILayout.PropertyField(changeElectricPowerUsageDisplayText);
            SerializedProperty overloadWarningPanel = serializedObject.FindProperty("overloadWarningPanel");
            EditorGUILayout.PropertyField(overloadWarningPanel);

            serializedObject.ApplyModifiedProperties();

            base.OnInspectorGUI();
        }
    }
}