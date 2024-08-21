using System.Collections.Generic;
using Attributes;
using CameraUtility;
using Data;
using UI;
using UnityEngine;

namespace TrainScripts
{
    [RequireComponent(typeof(Train))]
    public class CompartmentConstructionHandler : MonoBehaviour, IContextMenuDataProvider
    {
        [SerializeField, Disabled] private Train train;
        [SerializeField] private List<ContextMenuData> contextMenuData = new();
        [SerializeField] private List<string> compartmentIdentifiers = new();
        [SerializeField] private Transform selectedIndicator;

        private void Reset()
        {
            train = GetComponent<Train>();
            
            compartmentIdentifiers.Clear();
            compartmentIdentifiers.Capacity = 4;
            compartmentIdentifiers.Add("Car_ScrapperComp_0_Default");
            compartmentIdentifiers.Add("Car_CombatComp_0_Default");
            compartmentIdentifiers.Add("Car_UtilityComp_0_Default");
            compartmentIdentifiers.Add("Car_GeneratorComp_0_Default");
            
            contextMenuData.Clear();
            contextMenuData.Capacity = 4;
            contextMenuData.Add(new ContextMenuData("Scrapper", () => SpawnCompartmentOfIdentifier(compartmentIdentifiers[0])));
            contextMenuData.Add(new ContextMenuData("Combat", () => SpawnCompartmentOfIdentifier(compartmentIdentifiers[1])));
            contextMenuData.Add(new ContextMenuData("Utility", () => SpawnCompartmentOfIdentifier(compartmentIdentifiers[2])));
            contextMenuData.Add(new ContextMenuData("Generator", () => SpawnCompartmentOfIdentifier(compartmentIdentifiers[3])));
        }

        private void OnValidate()
        {
            Debug.Assert(
                contextMenuData.Count == compartmentIdentifiers.Count,
                "ContextMenuData and CompartmentIdentifiers count must be equal.");
        }

        public void Activate()
        {
            var contextMenu = FindObjectOfType<ContextMenuUI>();
            if (contextMenu == null)
            {
                Debug.LogWarning($"{nameof(ContextMenuUI)} is not found from current scene.");
                return;
            }

            selectedIndicator.gameObject.SetActive(true);
            selectedIndicator.transform.position = train.TailSpawnPosition;

            var cameraController = FindObjectOfType<CameraController>();
            cameraController.FollowTarget = selectedIndicator.transform;
            
            contextMenu.OpenMenu(Vector2.zero, this);
        }

        public void SpawnCompartmentOfIdentifier(string identifier)
        {
            var constructionCost = Database.CarCostData[identifier];
            if (!train.TryConsumeResourcesFromInventory(constructionCost))
            {
                Debug.LogWarning("The Train does not have enough resources to construct the compartment.");
                RestoreIndicatingState();
                return;
            }
            
            var identifierElement = identifier.Split('_');
            var categoryIdentifier = $"{identifierElement[0]}_{identifierElement[1]}";
            var modulePrefab = Database.UnitUpgradeTree[categoryIdentifier][identifier].model.GetComponent<Car>();

            train.SpawnCar(modulePrefab, modulePrefab.StatComponent.BuildDelay);
            
            // post process for the construction
            RestoreIndicatingState();
        }
        
        private void RestoreIndicatingState()
        {
            selectedIndicator.gameObject.SetActive(false);
            train.SetSelectedCar(train.SelectedCarIndex);
        }
        
        #region IContextMenuDataProvider

        public List<ContextMenuData> GetContextMenuData()
        {
            return contextMenuData;
        }

        public ContextMenuCondition GetContextMenuConditionState()
        {
            var state = ContextMenuCondition.None;
            return state;
        }

        public void OnContextMenuClosed()
        {
            RestoreIndicatingState();
        }

        public Dictionary<ResourceType, float> GetResourceData(ContextMenuCondition condition) => null;

        #endregion
    }
}