
using TMPro;
using UnityEngine;

namespace Data
{
    public class ChangingTextLoader : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI targetText;
        [SerializeField,Tooltip("Identifier의 Sub Type을 제외한 전부")] private string textType;

        public void ChangeText(string Identifier)
        {
            if (Utility.DatabaseUtility.TryGetData(Database.TextData, $"{textType}_{Identifier}", out var textData))
            {
                targetText.text = textData.korean;
            }
        }
    }
}