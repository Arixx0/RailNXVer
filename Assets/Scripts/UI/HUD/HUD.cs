using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utility;
using Data;
using TrainScripts;
using System;
using UnityEngine.InputSystem;

namespace UI
{
    public partial class HUD : MonoBehaviour
    {
        #region HUD Fields & Properties

        [SerializeField] private Train targetTrain;

        [Header("Resources Status Panel")]
        [SerializeField] private Transform resourceStatusPanel;
        [SerializeField] private HUDResourceItem hudResourceItemPrefab;

        [Header("Augment Icon Panel")]
        [SerializeField] private Transform AugmentIconPanel;
        [SerializeField] private HUDAugmentItem hudAugmentItemPrefab;

        [Header("Train Control Panel")]
        [SerializeField] private TexturedToggle trainMovementControlToggle;

        [Header("Electric Power Indicator Panel")]
        [SerializeField] private Slider electricPowerSlider;
        [SerializeField] private TextMeshProUGUI electricPowerText;
        [SerializeField] private Image electricPowerValueImage;
        [SerializeField] private Image previewElectricPowerUpImage;
        [SerializeField] private Image previewElectricPowerDownImage;
        [SerializeField] private Image previewElectricPowerOverloadImage;

        [Header("Car Control Panel")]
        [SerializeField] private Transform carControlPanel;
        [SerializeField] private HUDCarControlItem hudCarControlItemPrefab;

        [Header("Detail Item Panel")]
        [SerializeField] private Transform detailItemPanel;
        [SerializeField] private HUDDetailItem hudDetailItemPrefab;
        [SerializeField] private List<DetailLayer> detailLayers = new();

        private readonly List<HUDResourceItem> m_ActiveHUDResourceItems = new List<HUDResourceItem>();
        private ObjectPool<HUDResourceItem> m_HUDResourceItemPool;

        private readonly List<HUDAugmentItem> m_ActiveHUDAugmentItmes = new List<HUDAugmentItem>();
        private ObjectPool<HUDAugmentItem> m_HUDAugmentItemPool;

        private readonly List<HUDDetailItem> m_ActiveHUDDetailItems = new (4);
        private ObjectPool<HUDDetailItem> m_HUDDetailItemPool;

        private readonly List<HUDCarControlItem> m_ActiveHUDCarControlItems = new List<HUDCarControlItem>();
        private ObjectPool<HUDCarControlItem> m_HUDCarControlItemPool;

        private int m_DetailLayerIndex;

        public int DetailLayerIndex
        {
            get => m_DetailLayerIndex;
            set => m_DetailLayerIndex = value;
        }

        public int DetailLayerCount => detailLayers.Count;

        public Train TargetTrain
        {
            get => targetTrain;
            set
            {
                targetTrain = value;
                targetTrain.OnInventoryChanged += UpdateResourcesStatusPanel;
                trainMovementControlToggle.ResetOnValueChangedListeners();
                trainMovementControlToggle.onValueChanged.AddListener(targetTrain.SetMovementState);

                SetupHUD();

                UpdateResourcesStatusPanel(targetTrain.Inventory);
            }
        }

        public Transform CachedTransform { get; private set; }

        #endregion

        #region MonoBehaviour Events

        private void Awake()
        {
            CachedTransform = transform;
            m_HUDResourceItemPool = ObjectPool<HUDResourceItem>.CreateObjectPool(hudResourceItemPrefab, CachedTransform);
            m_HUDAugmentItemPool = ObjectPool<HUDAugmentItem>.CreateObjectPool(hudAugmentItemPrefab, CachedTransform);
            m_HUDDetailItemPool = ObjectPool<HUDDetailItem>.CreateObjectPool(hudDetailItemPrefab, CachedTransform);
            m_HUDCarControlItemPool = ObjectPool<HUDCarControlItem>.CreateObjectPool(hudCarControlItemPrefab, CachedTransform);
            noticeWarningItemPool = ObjectPool<NoticeWarningItem>.CreateObjectPool(noticeWarningItemPrefab, CachedTransform);
        }

        #endregion

        #region HUD Implementations

        public void SetupHUD()
        {
            if (targetTrain is not null)
            {
                CreateResourceStatusPanel(targetTrain.Inventory);
                OnChangeDetailLayer(m_DetailLayerIndex);
                CreateCarControlPanel();
            }
        }

        private void CreateResourceStatusPanel(Inventory inventory)
        {
            foreach (HUDResourceItem activeResourceItem in m_ActiveHUDResourceItems)
            {
                m_HUDResourceItemPool.ReturnObject(activeResourceItem);
            }

            m_ActiveHUDResourceItems.Clear();

            foreach (var item in inventory)
            {
                HUDResourceItem hudResourceItemInstance = m_HUDResourceItemPool.GetOrCreate();
                hudResourceItemInstance.CachedTransform.SetParent(resourceStatusPanel);
                hudResourceItemInstance.ResourceType = item.Key;
                hudResourceItemInstance.ResourceAmount = item.Value.ToString("0");
                hudResourceItemInstance.ResourceImage.ChangeImage(item.Key.ToString());
                m_ActiveHUDResourceItems.Add(hudResourceItemInstance);
            }
            UpdateResourcesStatusPanel(targetTrain.Inventory);
        }

        private void UpdateResourcesStatusPanel(Inventory inventory)
        {
            foreach (var HUDResourceItem in m_ActiveHUDResourceItems)
            {
                HUDResourceItem.ResourceAmount = inventory[HUDResourceItem.ResourceType].ToString("0");
            }
        }

        private void UpdateAugmentIconPanel()
        {
            foreach (HUDAugmentItem hudAugmentItem in m_ActiveHUDAugmentItmes)
            {
                m_HUDAugmentItemPool.ReturnObject(hudAugmentItem);
            }

            m_ActiveHUDAugmentItmes.Clear();


            for (int i = 0; i < 6; i++) // TODO : Get AugmentData (identifier), 6 is Dummy
            {
                HUDAugmentItem hudAugmentItem = m_HUDAugmentItemPool.GetOrCreate();
                hudAugmentItem.CachedTransform.SetParent(AugmentIconPanel);
                hudAugmentItem.AugmentName = null;
                hudAugmentItem.AugmentDescription = null;
                hudAugmentItem.AugmentIconImage = null; // TODO : Change AugmentIcon Image

                m_ActiveHUDAugmentItmes.Add(hudAugmentItem);
            }
            
        }

        public void CreateCarControlPanel()
        {
            foreach (var activeCarControlItem in m_ActiveHUDCarControlItems)
            {
                m_HUDCarControlItemPool.ReturnObject(activeCarControlItem);
            }

            m_ActiveHUDCarControlItems.Clear();

            for (int i = 0; i < targetTrain.cars.Count; i++)
            {
                var carControlItem = m_HUDCarControlItemPool.GetOrCreate();
                carControlItem.CachedTransform.SetParent(carControlPanel);
                var car = targetTrain.cars[i];
                carControlItem.UpdateCarControlItem(car);
                m_ActiveHUDCarControlItems.Add(carControlItem);
            }
        }

        public void UpdateCarControlPanel(Car car)
        {
            var index = targetTrain.cars.IndexOf(car);
            m_ActiveHUDCarControlItems[index].UpdateCarControlItem(car);
        }


        /// <param name="layerIndex"> 0 : Default, 1 : PowerStone, 2 : ElectricPower, 3 : PowerStoneDetail</param>
        public void OnChangeDetailLayer(int layerIndex)
        {
            foreach (HUDDetailItem activehudDetailItem in m_ActiveHUDDetailItems)
            {
                m_HUDDetailItemPool.ReturnObject(activehudDetailItem);
            }

            m_ActiveHUDDetailItems.Clear();

            DetailLayer currentLayer = detailLayers[layerIndex];

            for (int i = 0; i < currentLayer.detailItems.Count; i++)
            {
                if (!DatabaseUtility.TryGetData(Database.TextData, currentLayer.detailItems[i].identifier.Identifier, out var DetailTextData))
                {
                    return;
                }

                HUDDetailItem hudDetailItemInstance = m_HUDDetailItemPool.GetOrCreate();
                hudDetailItemInstance.CachedTransform.SetParent(detailItemPanel);
                hudDetailItemInstance.DetailName = DetailTextData.korean;
                m_ActiveHUDDetailItems.Add(hudDetailItemInstance);
            }

            foreach (var car in targetTrain.cars)
            {
                car.OnChangeCarDetailLayer(layerIndex);
            }
            UpdateHUDItemValue();
        }

        public void UpdateHUDItemValue()
        {
            DetailLayer currentLayer = detailLayers[m_DetailLayerIndex];

            for(int i = 0; i < currentLayer.detailItems.Count; i++)
            {
                switch (m_DetailLayerIndex)
                {
                    case 1:
                        m_ActiveHUDDetailItems[i].DetailValue = targetTrain.TotalConsumePowerStone.ToString();
                        break;
                    case 2:
                        if (i == 0)
                        {
                            m_ActiveHUDDetailItems[i].DetailValue = targetTrain.TrainCurrentElectricPowerUsage.ToString();
                        }
                        else if (i == 1)
                        {
                            m_ActiveHUDDetailItems[i].DetailValue = targetTrain.TrainCurrentElectricPowerGeneration.ToString();
                        }
                        break;
                    case 3:
                        if (i == 0)
                        {
                            m_ActiveHUDDetailItems[i].DetailValue = targetTrain.TotalConsumePowerStone.ToString();
                        }
                        else if (i == 1)
                        {
                            m_ActiveHUDDetailItems[i].DetailValue = targetTrain.cars.Count.ToString();
                        }
                        else if (i == 2)
                        {
                            m_ActiveHUDDetailItems[i].DetailValue = Mathf.CeilToInt((
                                targetTrain.Inventory.GetResourceOfType(ResourceType.PowerStone))
                                / targetTrain.TotalConsumePowerStone).ToString();
                        }
                        break;
                }
            }

            electricPowerSlider.minValue = 0;
            electricPowerSlider.maxValue = targetTrain.TrainCurrentElectricPowerGeneration;
            electricPowerSlider.value = targetTrain.TrainCurrentElectricPowerUsage;
            electricPowerText.text = $"{targetTrain.TrainCurrentElectricPowerUsage} / {targetTrain.TrainCurrentElectricPowerGeneration}";
            electricPowerValueImage.color = targetTrain.ElectricPowerOverload ? Color.red : Color.yellow;
            
            previewElectricPowerOverloadImage.gameObject.SetActive(false);
            previewElectricPowerUpImage.gameObject.SetActive(false);
            previewElectricPowerDownImage.gameObject.SetActive(false);
        }

        public void PreViewElectricPowerValue(float previewElectricPower, float currentCarElectricPower)
        {
            previewElectricPowerOverloadImage.gameObject.SetActive(false);
            previewElectricPowerUpImage.gameObject.SetActive(false);
            previewElectricPowerDownImage.gameObject.SetActive(false);

            if (targetTrain.ElectricPowerOverload)
            {
                return;
            }

            float previewTotalElectricPower = targetTrain.TrainCurrentElectricPowerUsage - currentCarElectricPower + previewElectricPower;

            if (previewTotalElectricPower > targetTrain.TrainCurrentElectricPowerGeneration) // overload
            {
                electricPowerSlider.value = previewTotalElectricPower;
                previewElectricPowerOverloadImage.gameObject.SetActive(true);
                previewElectricPowerOverloadImage.fillAmount = (previewTotalElectricPower - targetTrain.TrainCurrentElectricPowerGeneration) / targetTrain.TrainCurrentElectricPowerGeneration;
            }
            else if (previewTotalElectricPower > targetTrain.TrainCurrentElectricPowerUsage) // up
            {
                electricPowerSlider.value = previewTotalElectricPower;
                previewElectricPowerUpImage.gameObject.SetActive(true);
                previewElectricPowerUpImage.fillAmount = (previewTotalElectricPower - targetTrain.TrainCurrentElectricPowerUsage) / targetTrain.TrainCurrentElectricPowerGeneration;
            }
            else if (previewTotalElectricPower < targetTrain.TrainCurrentElectricPowerUsage) // down
            {
                electricPowerSlider.value = targetTrain.TrainCurrentElectricPowerUsage;
                previewElectricPowerDownImage.gameObject.SetActive(true);
                previewElectricPowerDownImage.fillAmount = (targetTrain.TrainCurrentElectricPowerUsage - previewTotalElectricPower) / targetTrain.TrainCurrentElectricPowerGeneration;
            }
            else
            {
                electricPowerSlider.value = targetTrain.TrainCurrentElectricPowerUsage;
            }
        }

        #endregion
    }

    [System.Serializable]
    public struct DetailItem
    {
        public string itemName;
        public TextIdentifier identifier;
    }

    [System.Serializable]
    public class DetailLayer
    {
        public List<DetailItem> detailItems = new();
    }
}