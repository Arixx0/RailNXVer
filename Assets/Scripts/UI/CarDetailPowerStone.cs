using TMPro;
using UnityEngine;

namespace UI
{
    public class CarDetailPowerStone : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI powerstoneAmount;

        public string PowerStoneAmount
        {
            get => powerstoneAmount.text;
            set => powerstoneAmount.text = value;
        }
    }
}