using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using Data;
using System.Reflection;
using Utility;

namespace UI
{
    // Dummy Game Result Data
    public struct GameResultData
    {
        public bool explorationSuccess;
        public int explorationTime;
        public int enemiesKilled;
        public int augmentOwned;
        public int carOwned;
        public string reasonForDestruction;
    }

    [System.Serializable]
    public struct GameResultItem
    {
        public TextIdentifier identifier;
        public TextMeshProUGUI name;
        public TextMeshProUGUI value;
    }

    [System.Serializable]
    public struct GameResultOption
    {
        public TextIdentifier identifier;
        public TextMeshProUGUI name;
        public Button button;
    }

    public class GameResultUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI gameResultName;

        [Header("Game Result Name Field")]
        [SerializeField] private TextIdentifier successIdentifier;
        [SerializeField] private TextIdentifier failIdentifier;

        [Header("Game Result Item Panel")]
        [SerializeField] private GameResultItem explorationTime;
        [SerializeField] private GameResultItem enemiesKilled;
        [SerializeField] private GameResultItem augmentOwned;
        [SerializeField] private GameResultItem carOwned;
        [SerializeField] private GameResultItem reasonForDestruction;
        [SerializeField] private GameResultItem totalScore;
        [SerializeField] private GameResultItem acquiredResearchPoint;

        [Header("Game Result Options Field")]
        [SerializeField] private GameResultOption okOption;
        [SerializeField] private GameResultOption retryOption;

        public Action CloseCallBack;

        private List<GameResultItem> m_GameResultItems = new();

        private void Awake()
        {
            FieldInfo[] fields = GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (FieldInfo field in fields)
            {
                if (field.FieldType == typeof(GameResultItem))
                {
                    GameResultItem item = (GameResultItem)field.GetValue(this);
                    m_GameResultItems.Add(item);
                }
            }
            okOption.button.onClick.AddListener(() => Close());
            retryOption.button.onClick.AddListener(() => Close());
        }

        private void Start()
        {
            ShowGameResult(new GameResultData
            {
                explorationSuccess = false,
                explorationTime = 24,
                enemiesKilled = 49,
                augmentOwned = 2,
                carOwned = 5,
                reasonForDestruction = "ÆøÇ³",
            });
        }

        private string ConvertMinutesToHourMinuteString(int totalMinutes)
        {
            int hours = totalMinutes / 60;
            int minutes = totalMinutes % 60;
            return hours > 0 ? $"{hours}h {minutes}m" : $"{minutes}m";
        }

        public void ShowGameResult(GameResultData gameResultData)
        {
            if (!DatabaseUtility.TryGetData(Database.TextData, successIdentifier.Identifier, out var successTextData) ||
                !DatabaseUtility.TryGetData(Database.TextData, failIdentifier.Identifier, out var failTextData))
            {
                return;
            }
            
            foreach (var gameResultItem in m_GameResultItems)
            {
                if (!DatabaseUtility.TryGetData(Database.TextData, gameResultItem.identifier.Identifier, out var resultTextData))
                {
                    return;
                }
                gameResultItem.name.text = resultTextData.korean;
            }

            gameResultName.text = gameResultData.explorationSuccess ? successTextData.korean : failTextData.korean;
            // TODO : Get GameResult Data
            explorationTime.value.text = ConvertMinutesToHourMinuteString(gameResultData.explorationTime);
            enemiesKilled.value.text = gameResultData.enemiesKilled.ToString();
            augmentOwned.value.text = gameResultData.augmentOwned.ToString();
            carOwned.value.text = gameResultData.carOwned.ToString();
            reasonForDestruction.value.text = gameResultData.reasonForDestruction;

            int score = (gameResultData.explorationTime * 10) + (gameResultData.enemiesKilled * 10) + (gameResultData.augmentOwned * 50) + (gameResultData.carOwned * 100);
            int point = (int)(score * 0.01f);
            totalScore.value.text = score.ToString();
            acquiredResearchPoint.value.text = point.ToString();
            // TODO : Add acquriredResearchPoint
            retryOption.button.gameObject.SetActive(!gameResultData.explorationSuccess);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Close()
        {
            gameObject.SetActive(false);
            CloseCallBack?.Invoke();
        }

    }
}