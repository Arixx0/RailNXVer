using System;
using UnityEngine;
using TMPro;
using Data;
using TrainScripts;

namespace UI
{
    public class CarDetailElectricIcon : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI electricAmount;
        [SerializeField] private ChangingImageLoader changeElectricLevelImageLoader;
        [SerializeField] private ChangingTextLoader changeElectricLevelTextLoader;
        [SerializeField] private ChangingTextLoader changeElectricStatusTextLoader;
        [SerializeField] private Transform levelIconPanel;
        [SerializeField] private Transform levelTextPanel;
        [SerializeField] private Color generatorColor;
        [SerializeField] private Color normalColor;

        public void ChangeCarDetailElectricIcon(Car car)
        {
            var isGenertator = car is GeneratorCompartment;

            levelIconPanel.gameObject.SetActive(!isGenertator);
            levelTextPanel.gameObject.SetActive(!isGenertator);
            
            electricAmount.text = isGenertator ? $"+{car.CurrentElectricPowerGeneration}" : $"-{car.CurrentElectricPowerUsage}";
            electricAmount.color = isGenertator ? generatorColor : normalColor;
            changeElectricLevelImageLoader.ChangeImage(car.CurrentElectricPowerUsageType.ToString());
            changeElectricLevelTextLoader.ChangeText(car.CurrentElectricPowerUsageType.ToString());
            changeElectricStatusTextLoader.gameObject.SetActive(!isGenertator);
            if (isGenertator)
            {
                return;
            }
            var statusTextIdentifier = car is UtilityCompartment ? $"_{car.IdentifierData.SubType}_{car.StatComponent.UpgradeLevel}" : "";
            changeElectricStatusTextLoader.ChangeText($"{car.IdentifierData.ObjectType}_{Convert.ToInt32(car.CurrentElectricPowerUsageType)}{statusTextIdentifier}");
        }
    }
}
