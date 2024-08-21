using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class TexturedToggle : Toggle
    {
        public Image stateImage;
        public Sprite onStateSprite;
        public Sprite offStateSprite;
        
        protected override void Awake()
        {
            base.Awake();
            
            ResetOnValueChangedListeners();
        }

        private void SwapStateTexture(bool state)
        {
            stateImage.sprite = state
                ? onStateSprite
                : offStateSprite;
        }
        
        public void ResetOnValueChangedListeners()
        {
            onValueChanged.RemoveAllListeners();
            onValueChanged.AddListener(SwapStateTexture);
        }

        public void SetStateWithoutNotify(bool state)
        {
            SetIsOnWithoutNotify(state);
            SwapStateTexture(state);
        }
    }
}