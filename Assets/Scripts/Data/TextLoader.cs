using UnityEngine;
using System.Collections.Generic;
using TMPro;
using Utility;

namespace Data
{
    public class TextLoader : MonoBehaviour
    {

        [SerializeField] private List<textIdentifierData> textIdentifierDatas = new();

        public textIdentifierData TextIdentifierData(int index) => textIdentifierDatas[index];

        private void Start()
        {
            foreach (var textIdentifierData in textIdentifierDatas)
            {
                if (DatabaseUtility.TryGetData(Database.TextData, textIdentifierData.identifier.Identifier, out var textData) && !textIdentifierData.hasValue)
                {
                    textIdentifierData.targetText.text = textData.korean;
                }
            }
        }

        public void ChangeValue(textIdentifierData textIdentifierData , float value)
        {
            if (textIdentifierData.hasValue && string.IsNullOrEmpty(textIdentifierData.identifier.Identifier))
            {
                textIdentifierData.targetText.text = value.ToString();
            }
            else if (DatabaseUtility.TryGetData(Database.TextData, textIdentifierData.identifier.Identifier, out var textData))
            {
                textIdentifierData.targetText.text = $"{textData.korean} {value}";
            }
        }

        [System.Serializable]
        public struct textIdentifierData
        {
            public string name;
            public bool hasValue;
            public TextIdentifier identifier;
            public TextMeshProUGUI targetText;
        }
    }

}