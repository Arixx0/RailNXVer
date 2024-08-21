using UnityEngine;

using Data;
using TrainScripts;

namespace UI
{
    public class CarDetailElectricInformation : MonoBehaviour
    {
        [SerializeField] private TextLoader electricPowerTextLoader;
        [SerializeField] private Transform electricIncomePanel;
        [SerializeField] private Transform electricConsumePanel;


        public void ChangeCarDetailElectricInformation(Car car)
        {
            var isGenerator = car is GeneratorCompartment;
            electricIncomePanel.gameObject.SetActive(isGenerator || car is EngineCompartment);
            electricConsumePanel.gameObject.SetActive(!isGenerator);
            electricPowerTextLoader.ChangeValue(electricPowerTextLoader.TextIdentifierData(0), car.CurrentElectricPowerGeneration);
            electricPowerTextLoader.ChangeValue(electricPowerTextLoader.TextIdentifierData(1), car.CurrentElectricPowerUsage);
        }
    }
}