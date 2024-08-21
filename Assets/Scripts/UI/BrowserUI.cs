using Data;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace UI
{
    public class BrowserUI : MonoBehaviour
    {
        #region Browser References

        [SerializeField] private List<Button> browserOptions;
        [SerializeField] private List<GameObject> browserContents;

        [Header("Augment Content")]
        [SerializeField] private Transform augmentIconPanel;
        [SerializeField] private AugmentIcon augmentIconPrefab;
        [SerializeField] private Image augmentInformationIcon;
        [SerializeField] private TextMeshProUGUI augmentInformationName;
        [SerializeField] private TextMeshProUGUI augmentInformationDescription;
        [SerializeField] private TextMeshProUGUI augmentInformationStat;

        [Header("Train Content")]
        [SerializeField] private Transform trainInformationPanel;
        [SerializeField] private Transform carListPanel;
        [SerializeField] private HUDDetailItem detailItemPrefab;
        [SerializeField] private Button browserCarButtonPrefab;
        [SerializeField] private List<TextIdentifier> detailItemIdentifiers;
        [SerializeField] private Transform carModelPanel;
        [SerializeField] private TextMeshProUGUI carInformationName;
        [SerializeField] private TextMeshProUGUI carInformationStat;
        [SerializeField] private TextMeshProUGUI carInformationDescription;

        #endregion

        #region Pooling

        private ObjectPool<AugmentIcon> m_AugmentIconPool;
        readonly private List<AugmentIcon> m_ActiveAugmentIcons = new();
        private ObjectPool<HUDDetailItem> m_DetailItemPool;
        readonly private List<HUDDetailItem> m_ActiveDetailItems = new();
        private ObjectPool<Button> m_CarButtonPool;
        readonly private List<Button> m_ActiveCarButtons = new();

        #endregion

        #region MonoBehaviour Events

        private void Awake()
        {
            for (int i = 0; i < browserOptions.Count; i++)
            {
                int currentIndex = i;
                browserOptions[i].onClick.AddListener(() => SelectBrowserOption(currentIndex));
            }
            m_AugmentIconPool = ObjectPool<AugmentIcon>.CreateObjectPool(augmentIconPrefab, browserContents[0].transform);
            m_DetailItemPool = ObjectPool<HUDDetailItem>.CreateObjectPool(detailItemPrefab, browserContents[1].transform);
            m_CarButtonPool = ObjectPool<Button>.CreateObjectPool(browserCarButtonPrefab, browserContents[1].transform);
        }

        #endregion

        public Image AugmentInformationIcon
        {
            get => augmentInformationIcon;
            set => augmentInformationIcon = value;
        }

        private void SelectBrowserOption(int index)
        {
            for (int i = 0; i < browserOptions.Count; i++)
            {
                browserOptions[i].image.color = Color.gray;
            }
            for (int i = 0; i < browserContents.Count; i++)
            {
                browserContents[i].SetActive(false);
            }
            browserOptions[index].image.color = Color.white;
            browserContents[index].SetActive(true);

            switch(index)
            {
                case 0:
                    CreateAugmentIcons();
                    break;
                case 1:
                    CreateCarList();
                    break;
                default:
                    break;
            }
        }

        #region Augment

        private void CreateAugmentIcons()
        {
            foreach (AugmentIcon augmentIcon in m_ActiveAugmentIcons)
            {
                m_AugmentIconPool.ReturnObject(augmentIcon);
            }
            m_ActiveAugmentIcons.Clear();

            for (int i = 0; i < 6; i++) // TODO : Get Number of Augment Data (identifier), 6 is Dummy
            {
                int currentIndex = i;
                AugmentIcon augmentIcon = m_AugmentIconPool.GetOrCreate();
                augmentIcon.CachedTransform.SetParent(augmentIconPanel);
                augmentIcon.Identifier = $"Augment_0{i + 1}";
                augmentIcon.onClick.RemoveAllListeners();
                augmentIcon.onClick.AddListener(() => SelectAugmentIcon(currentIndex));

                m_ActiveAugmentIcons.Add(augmentIcon);
            }
            SelectAugmentIcon();
        }

        private void SelectAugmentIcon(int index = -1)
        {
            // TODO : Select Effect Add, Current Select Effect is Dummy
            for (int i = 0; i < m_ActiveAugmentIcons.Count; i++)
            {
                m_ActiveAugmentIcons[i].AugmentIconImage.color = Color.black;
            }
            AugmentInformationIcon = null;
            augmentInformationName.text = null;
            augmentInformationDescription.text = null;
            augmentInformationStat.text = null;
            if (index == -1) return;

            m_ActiveAugmentIcons[index].AugmentIconImage.color = Color.green;
            
            if (!DatabaseUtility.TryGetData(Database.TextData, m_ActiveAugmentIcons[index].Identifier,
                "Name", "Desc", out var augmentNameTextData, out var augmentDescTextData))
            {
                return;
            }

            // TODO : Get AugmentData

            AugmentInformationIcon = m_ActiveAugmentIcons[index].AugmentIconImage; // TODO : Change Augment Icon Image
            augmentInformationName.text = augmentNameTextData.korean;
            augmentInformationDescription.text = augmentDescTextData.korean;
            augmentInformationStat.text = null; // TODO : Augment Stat Setting
        }

        #endregion

        #region Train

        private void CreateCarList()
        {
            foreach (HUDDetailItem hudDetailItem in m_ActiveDetailItems)
            {
                m_DetailItemPool.ReturnObject(hudDetailItem);
            }
            foreach (Button carButton in m_ActiveCarButtons)
            {
                m_CarButtonPool.ReturnObject(carButton);
            }
            m_ActiveDetailItems.Clear();
            m_ActiveCarButtons.Clear();

            for (int i = 0; i < detailItemIdentifiers.Count; i++)
            {
                if (!DatabaseUtility.TryGetData(Database.TextData, detailItemIdentifiers[i].Identifier, out var detailNameTextData))
                {
                    return;
                }

                HUDDetailItem detailItem = m_DetailItemPool.GetOrCreate();
                detailItem.CachedTransform.SetParent(trainInformationPanel);
                detailItem.DetailName = detailNameTextData.korean;
                detailItem.DetailValue = "5"; // TODO : Get Train Detail Data
                
                m_ActiveDetailItems.Add(detailItem);
            }

            for (int i = 0; i < 6; i++)
            {
                // TODO : Get Number of Car Data (identifier, Model, Stat), 6 is Dummy
                carInformationName.text = null;
                carInformationStat.text = null;
                carInformationDescription.text = null;
                int currentIndex = i;

                Button button = m_CarButtonPool.GetOrCreate();
                button.transform.SetParent(carListPanel);
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => SelectCar(currentIndex));

                m_ActiveCarButtons.Add(button);
            }
            SelectCar();
        }

        private void SelectCar(int index = -1)
        {
            for (int i = 0; i < m_ActiveCarButtons.Count; i++)
            {
                m_ActiveCarButtons[i].image.color = Color.gray;
            }
            if (index == -1) return;
            m_ActiveCarButtons[index].image.color = Color.white;

            //TODO : Get Train.cars[i] Data (identifier, stat)
        }

        #endregion

        #region Map
        // TODO : Create Map, Get Map Data, Get Map Information
        private void CreateMap()
        {

        }

        #endregion
    }
}