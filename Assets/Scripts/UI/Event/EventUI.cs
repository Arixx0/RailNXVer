using System.Collections.Generic;
using UnityEngine;
using Data;
using TMPro;
using UnityEngine.UI;
using System;
using Utility;

namespace UI
{
    public class EventUI : MonoBehaviour
    {
        #region Fields & Properties
        
        [SerializeField] private TextMeshProUGUI eventNameText;
        [SerializeField] private TextMeshProUGUI eventDescriptionText;

        [SerializeField] private EventOptionItem eventOptionItemPrefab;
        [SerializeField] private EventRewardItem eventRewardItemPrefab;

        [SerializeField] private Transform eventOptionItemContainer;
        [SerializeField] private Transform eventRewardItemContainer;

        [SerializeField] private Image eventImage;

        [SerializeField] private ShopUI shopUI;
        [SerializeField] private HUD hud;

        private IEventDataProvider m_EventDataProvider;
        
        private ObjectPool<EventOptionItem> m_OptionPool;
        private readonly List<EventOptionItem> m_ActiveOptionItems = new();

        private ObjectPool<EventRewardItem> m_RewardPool;
        private List<EventRewardItem> m_ActiveRewardItems = new();

        public Image EventImage
        {
            get => eventImage;
            set => eventImage = value;
        }

        public Transform CachedTransform { get; private set; }
        
        #endregion // Fields & Properties

        #region MonoBehaviour Events
        
        private void Awake()
        {
            CachedTransform = transform;
            m_OptionPool = ObjectPool<EventOptionItem>.CreateObjectPool(eventOptionItemPrefab, CachedTransform);
            m_RewardPool = ObjectPool<EventRewardItem>.CreateObjectPool(eventRewardItemPrefab, CachedTransform);
        }
        
        #endregion // MonoBehaviour Events

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }

        public void InitEventUI(IEventDataProvider provider)
        {
            m_EventDataProvider = provider; // TODO : Change provider Identifier
            if (!DatabaseUtility.TryGetData(Database.EventData, provider.GetCurrentEventIdentifier(), out var eventData))
            {
                return;
            }
            if (!DatabaseUtility.TryGetData(Database.TextData, eventData.Identifier, "Title", "Desc", out var eventTitleData, out var eventDescData))
            {
                return;
            }

            eventNameText.text = eventTitleData.korean;
            eventDescriptionText.text = eventDescData.korean;
            EventImage = null; // TODO : Change Event Image

            ClearActiveItems();

            SetOptions(eventData);

            SetRewards(eventData);
        }

        private void ClearActiveItems()
        {
            foreach (EventOptionItem eventOptionItem in m_ActiveOptionItems)
            {
                m_OptionPool.ReturnObject(eventOptionItem);
            }
            foreach (EventRewardItem eventRewardItem in m_ActiveRewardItems)
            {
                m_RewardPool.ReturnObject(eventRewardItem);
            }

            m_ActiveOptionItems.Clear();
            m_ActiveRewardItems.Clear();
        }

        private void SetOptions(EventDataTuple eventData)
        {
            for (int i = 0; i < eventData.EventOptions.Count; i++)
            {
                if (!DatabaseUtility.TryGetData(Database.TextData, eventData.EventOptions[i], out var eventOptionTextData))
                {
                    return;
                }
                
                int currentIndex = i;
                EventOptionItem eventOptionItemInstance = m_OptionPool.GetOrCreate();
                eventOptionItemInstance.CachedTransform.SetParent(eventOptionItemContainer);
                eventOptionItemInstance.interactable = true;
                eventOptionItemInstance.Option = eventOptionTextData.korean;
                eventOptionItemInstance.onClick.RemoveAllListeners();

                if (currentIndex == 1) // TODO : Ignore Options Change
                {
                    eventOptionItemInstance.onClick.AddListener(() => Close());
                }
                else
                {
                    eventOptionItemInstance.onClick.AddListener(() => GetType().GetMethod(
                    eventData.Identifier, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.Invoke(this, new object[] { eventData, currentIndex }));
                }

                m_ActiveOptionItems.Add(eventOptionItemInstance);
            }
        }

        private void SetRewards(EventDataTuple eventData)
        {
            if (eventData.EventRewardIconIndex == null) return;

            for (int i = 0; i < eventData.EventRewardIconIndex.Count; i++)
            {
                if(!DatabaseUtility.TryGetData(Database.TextData, eventData.EventRewardIconIndex[i],
                    "", "Desc", out var rewardNameTextData, out var rewardDescTextData))
                {
                    return;
                }

                EventRewardItem eventRewardItemInstance = m_RewardPool.GetOrCreate();
                eventRewardItemInstance.CachedTransform.SetParent(eventRewardItemContainer);
                eventRewardItemInstance.rewardName = rewardNameTextData.korean;
                eventRewardItemInstance.rewardDescription = rewardDescTextData.korean;
                eventRewardItemInstance.rewardQuantity = eventData.EventRewardQuantity[i].ToString();
                m_ActiveRewardItems.Add(eventRewardItemInstance);

                if (eventData.EventRewardQuantity[i] < 0)
                {
                    if (!DatabaseUtility.TryGetData(Database.TextData, eventData.EventOptions[i], out var eventOptionTextData))
                    {
                        return;
                    }

                    string resourceName = eventData.EventRewardIconIndex[i];
                    int Index = resourceName.IndexOf("_");

                    ResourceType resourceType = (ResourceType)Enum.Parse(typeof(ResourceType), resourceName.Substring(Index + 1));
                    bool isAvailable = hud.TargetTrain.CheckAvailabilityResource(resourceType, eventData.EventRewardQuantity[i]);
                    string color = isAvailable ? "#0000FF" : "#FF0000";
                    string rewardText = $"{rewardNameTextData.korean} {eventData.EventRewardQuantity[i]}";
                    m_ActiveOptionItems[i].Option = $"{eventOptionTextData.korean}\n<color={color}>{rewardText}</color>";
                    m_ActiveOptionItems[i].interactable = isAvailable;
                    eventRewardItemInstance.gameObject.SetActive(false);
                }
            }
        }
        private void InitShopUI()
        {
            // TODO : shop list update
            Close();
            shopUI.Show();
        }
        // TODO : Implement each event method
        #region EventMethodList

        #region VENUS_Upgrade
        private void Event_VENUS_Upgrade_TemporaryShield(EventDataTuple eventData, int currentIndex)
        {
            Debug.Log("임시 방어막");
            Close();
        }

        private void Event_VENUS_Upgrade_AdequateShield(EventDataTuple eventData, int currentIndex)
        {
            Debug.Log("준수한 방어막");
            Close();
        }

        private void Event_VENUS_Upgrade_RegenerativeShield(EventDataTuple eventData, int currentIndex)
        {
            Debug.Log("재생 방어막");
            Close();
        }
        #endregion

        #region Wreck
        private void Event_Wreck(EventDataTuple eventData, int currentIndex)
        {
            //TODO : Get probability data
            m_EventDataProvider.SetEventIdentifier(m_EventDataProvider.GetNextEventIdentifier()[UnityEngine.Random.Range(0, 2)]);
            InitEventUI(m_EventDataProvider);
        }

        private void Event_WreckEnemy_AttackWarning(EventDataTuple eventData, int currentIndex)
        {
            string str = eventData.Identifier.Split("_")[1];
            AttackEvent(str);
        }

        private void Event_WreckDamagedAugment(EventDataTuple eventData, int currentIndex)
        {
            Debug.Log($"손상된 증강체 수리 완료 소모한 강철 {eventData.EventRewardQuantity[currentIndex]}");
            GetResource(eventData.EventRewardIconIndex, eventData.EventRewardQuantity);
            Close();
        }
        #endregion

        private void Event_WastModule(EventDataTuple eventData, int currentIndex)
        {
            Debug.Log("랜덤 모듈 획득");
        }

        private void Event_Manipulator(EventDataTuple eventData, int currentIndex)
        {
            Debug.Log("기후 위성 조작");
            Close();
        }

        private void Event_Small_Battle(EventDataTuple eventData, int currentIndex)
        {
            Debug.Log("소규모 적 스폰");
            Close();
        }

        private void Event_BrokenTrack(EventDataTuple eventData, int currentIndex)
        {
            Debug.Log("선로 수리 완료");
            Close();
        }

        private void Event_SolarWind(EventDataTuple eventData, int currentIndex)
        {
            Debug.Log("발전량 5분간 -2 감소");
            Close();
        }

        private void Event_EmergencyEscape(EventDataTuple eventData, int currentIndex)
        {
            Debug.Log("열차 속도 상승");
            Close();
        }

        private void Event_DestroyedCar(EventDataTuple eventData, int currentIndex)
        {
            Debug.Log("랜덤 모듈 획득");
            Close();
        }

        private void Event_UunidentifiedFactory(EventDataTuple eventData, int currentIndex)
        {
            Debug.Log("랜덤한 Booster 획득");
            Close();
        }

        private void Event_RadiationDepot(EventDataTuple eventData, int currentIndex)
        {
            Debug.Log("보상 획득, 방사선 폭발 발생");
            Close();
        }

        #region RepairShop
        private void Event_Repairshop(EventDataTuple eventData, int currentIndex)
        {
            Debug.Log("모든 차량 체력 30% 회복");

            //TODO : Get probability data
            int randomInt = UnityEngine.Random.Range(0, 2);
            if (randomInt == 0)
            {
                m_EventDataProvider.SetEventIdentifier(m_EventDataProvider.GetNextEventIdentifier()[0]);
                InitEventUI(m_EventDataProvider);
            }
            else
            {
                Close();
            }
        }

        private void Event_Repairshop_Findresource(EventDataTuple eventData, int currentIndex)
        {
            GetResource(eventData.EventRewardIconIndex, eventData.EventRewardQuantity);
            Close();
        }
        #endregion

        #region Merchant
        private void Event_WanderingMerchant(EventDataTuple eventData, int currentIndex)
        {
            if (currentIndex == 0)
            {
                //TODO : Get Shop List
                InitShopUI();
            }
            else
            {
                m_EventDataProvider.SetEventIdentifier(m_EventDataProvider.GetNextEventIdentifier()[0]);
                InitEventUI(m_EventDataProvider);
            }
        }

        private void Event_WanderingMerchant_AttackWarning(EventDataTuple eventData, int currentIndex)
        {
            string str = eventData.Identifier.Split("_")[1];
            AttackEvent(str);
        }
        #endregion

        #region Outpost
        private void Event_OutpostStorage(EventDataTuple eventData, int currentIndex)
        {
            GetResource(eventData.EventRewardIconIndex, eventData.EventRewardQuantity);
            Close();
        }

        private void Event_Outpost(EventDataTuple eventData, int currentIndex)
        {
            string str = eventData.Identifier.Split("_")[1];
            AttackEvent(str);
        }
        #endregion

        #region Evacuee

        private void Event_Evacuee(EventDataTuple eventData, int currentIndex)
        {
            if (currentIndex == 0)
            {
                Debug.Log("난민 수용 - 동력석 소모량 +30%, 전력 생산량 +5");
                Close();
            }
            else
            {
                m_EventDataProvider.SetEventIdentifier(m_EventDataProvider.GetNextEventIdentifier()[0]);
                InitEventUI(m_EventDataProvider);
            }
        }

        private void Event_Evacuee_AttackWarning(EventDataTuple eventData, int currentIndex)
        {
            string str = eventData.Identifier.Split("_")[1];
            AttackEvent(str);
        }

        #endregion

        #region GroundbreakingDiscovery

        private void Event_GroundbreakingDiscovery_Development_Start(EventDataTuple eventData, int currentIndex)
        {
            Debug.Log("열차 발전 효율 개선 시작");
            Invoke(nameof(GroundbreakingDiscovery_Development), 3f);
            Close();
        }

        private void GroundbreakingDiscovery_Development()
        {
            Show();
            m_EventDataProvider.SetEventIdentifier(m_EventDataProvider.GetNextEventIdentifier()[0]);
            InitEventUI(m_EventDataProvider);
        }

        private void Event_GroundbreakingDiscovery_Development_Complite(EventDataTuple eventData, int currentIndex)
        {
            Debug.Log("열차 발전 효율 개선 완료");
            Close();
        }

        private void Event_GroundbreakingDiscovery_Power_Efficiency_Start(EventDataTuple eventData, int currentIndex)
        {
            Debug.Log("동력 효율 개선 시작");
            Invoke(nameof(GroundbreakingDiscovery_Power_Efficiency), 3f);
            Close();
        }

        private void GroundbreakingDiscovery_Power_Efficiency()
        {
            Show();
            m_EventDataProvider.SetEventIdentifier(m_EventDataProvider.GetNextEventIdentifier()[0]);
            InitEventUI(m_EventDataProvider);
        }

        private void Event_GroundbreakingDiscovery_Power_Efficiency_Complite(EventDataTuple eventData, int currentIndex)
        {
            Debug.Log("동력 효율 개선 완료");
            Close();
        }

        #endregion

        #region GeneratorTrouble

        private void Event_GeneratorTrouble_Repair(EventDataTuple eventData, int currentIndex)
        {
            if (currentIndex == 0)
            {
                Debug.Log("수리 완료");
                Close();
            }
            else
            {
                m_EventDataProvider.SetEventIdentifier(m_EventDataProvider.GetNextEventIdentifier()[0]);
                InitEventUI(m_EventDataProvider);
            }
        }

        private void Event_GeneratorTrouble_Ignore(EventDataTuple eventData, int currentIndex)
        {
            Debug.Log("전력 생산량 2 감소");
            Close();
        }

        #endregion

        #region Caravan

        private void Event_Caravan(EventDataTuple eventData, int currentIndex)
        {
            if (currentIndex == 0)
            {
                Debug.Log($"횡단자 집단한테 강철{eventData.EventRewardQuantity[currentIndex]}소모");
                GetResource(eventData.EventRewardIconIndex, eventData.EventRewardQuantity);
                Close();
            }
            else
            {
                m_EventDataProvider.SetEventIdentifier(m_EventDataProvider.GetNextEventIdentifier()[0]);
                InitEventUI(m_EventDataProvider);
            }
        }

        private void Event_Caravan_AttackWarning(EventDataTuple eventData, int currentIndex)
        {
            string str = eventData.Identifier.Split("_")[1];
            AttackEvent(str);
        }

        #endregion

        #region Resource
        private void Event_Resources(EventDataTuple eventData, int currentIndex)
        {
            Debug.Log("자원 스폰");
            Close();
        }

        private void Event_MassOfResources(EventDataTuple eventData, int currentIndex)
        {
            Debug.Log("자원 대량 스폰");
            Close();
        }
        #endregion

        #region Augment
        private void Event_AugmentSacrifice(EventDataTuple eventData, int currentIndex)
        {
            Debug.Log("증강체 1개 희생, 전체 차량 체력 20퍼 회복");
            Close();
        }

        private void Event_AugmentMerchant(EventDataTuple eventData, int currentIndex)
        {
            if (currentIndex == 0)
            {
                // TODO : Get Shop Data
                InitShopUI();
            }
            else
            {
                m_EventDataProvider.SetEventIdentifier(m_EventDataProvider.GetNextEventIdentifier()[0]);
                InitEventUI(m_EventDataProvider);
            }
        }

        private void Event_AugmentMerchant_AttackWarning(EventDataTuple eventData, int currentIndex)
        {
            string str = eventData.Identifier.Split("_")[1];
            AttackEvent(str);
        }
        #endregion

        #region Reinforcement

        private void Event_TemporaryReinforcement_AtkDamage(EventDataTuple eventData, int currentIndex)
        {
            Debug.Log("강철 50개 소모, 5분간 공격력 +50%");
            Close();
        }

        private void Event_TemporaryReinforcement_Armor(EventDataTuple eventData, int currentIndex)
        {
            Debug.Log("강철 50개 소모, 5분간 방어력 +5");
            Close();
        }

        #endregion

        #endregion
        // TODO : Implement attack event method
        private void AttackEvent(string eventName)
        {
            Debug.Log(eventName + " Attack_Enemy");
            Close();
        }

        private void GetResource(List<string> resourceType, List<int> resourceQuantity)
        {
            for (int i = 0; i < resourceType.Count; i++)
            {
                int Index = resourceType[i].IndexOf("_");
                string str = resourceType[i].Substring(Index + 1);
                ResourceType returnRsourceType = (ResourceType)Enum.Parse(typeof(ResourceType), str);
                hud.TargetTrain.AddResourceToInventory(returnRsourceType, resourceQuantity[i]);
            }
        }
    }

    public interface IEventDataProvider
    {
        public string[] GetNextEventIdentifier();
        public string GetCurrentEventIdentifier();
        public void SetEventIdentifier(string identifier);
    }
}

