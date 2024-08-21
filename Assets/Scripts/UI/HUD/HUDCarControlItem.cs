using System;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using TrainScripts;
using Data;
using Utility;
namespace UI
{
    public class HUDCarControlItem : Button, IPointerClickHandler
    {
        [SerializeField] private Transform commonPanel;
        [SerializeField] private Transform generatorPanel;
        [SerializeField] private ChangingImageLoader electricPowerIconImageLoader;
        [SerializeField] private ChangingImageLoader carIconImageLoader;
        [SerializeField] private TextMeshProUGUI carUpgradeLevelText;
        [SerializeField] private TextMeshProUGUI electricPowerUsageText;
        [SerializeField] private TextMeshProUGUI generatorPowerText;
        [SerializeField] private Gradient carIconColorGradient;

        private Car m_Car;
        private bool isGenerator;
        private Transform m_CachedTransform;
        private RectTransform m_RectTransform;

        public Transform CachedTransform =>
            m_CachedTransform is null ? (m_CachedTransform = transform) : m_CachedTransform;

        public RectTransform RectTransform =>
            m_RectTransform is null ? (m_RectTransform = (RectTransform)CachedTransform) : m_RectTransform;

        public void UpdateCarControlItem(Car car)
        {
            m_Car = car;

            isGenerator = car is GeneratorCompartment;
            commonPanel.gameObject.SetActive(!isGenerator);
            generatorPanel.gameObject.SetActive(isGenerator);

            if (isGenerator)
            {
                generatorPowerText.text = $"+{car.CurrentElectricPowerGeneration}";
            }
            else
            {
                electricPowerUsageText.text = car.CurrentElectricPowerUsage.ToString();
                electricPowerIconImageLoader.ChangeImage(car.CurrentElectricPowerUsageType.ToString());
            }

            carUpgradeLevelText.text = car.StatComponent.UpgradeLevel > 0 ? $"+{car.StatComponent.UpgradeLevel.ToString()}" : "";
            carIconImageLoader.ChangeImage(car.IdentifierData.GetImageIdentifier(car.StatComponent.UpgradeLevel));
            var healthDelta = Mathf.InverseLerp(0, car.StatComponent.MaxHealth, car.StatComponent.CurrentHealth);
            carIconImageLoader.TargetImage.color = carIconColorGradient.Evaluate(healthDelta);
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            if (m_Car == null || isGenerator)
            {
                return;
            }
            
            var delta = 0;
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                delta = 1;
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                delta = -1;
            }
            var value = Convert.ToInt32(m_Car.CurrentElectricPowerUsageType) + delta;
            var newEnumValue = (Car.ElectricPowerUsageType)Enum.ToObject(typeof(Car.ElectricPowerUsageType), value);
            if ((Enum.IsDefined(typeof(Car.ElectricPowerUsageType), newEnumValue)))
            {
                m_Car.ElectricPowerChange(newEnumValue);
            }
        }
    }

    [CustomEditor(typeof(HUDCarControlItem))]
    public class HUDCarControlItemEditor : ButtonEditor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("HUD Car Control Item Properties", EditorStyles.boldLabel);

            serializedObject.Update();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("commonPanel"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("generatorPanel"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("electricPowerIconImageLoader"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("carIconImageLoader"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("carIconColorGradient"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("carUpgradeLevelText"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("electricPowerUsageText"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("generatorPowerText"));
            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("UI.Button Base Properties", EditorStyles.boldLabel);

            base.OnInspectorGUI();
        }
    }
}