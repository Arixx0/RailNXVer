using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class LabeledSlider : Slider
    {
        [SerializeField] private TextMeshProUGUI valueLabel;

        public override float value
        {
            get => base.value;
            set
            {
                base.value = value;
                valueLabel.text = value.ToString("0");
            }
        }
    }
}