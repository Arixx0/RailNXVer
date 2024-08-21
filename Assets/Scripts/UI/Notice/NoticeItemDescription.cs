using System.Collections;
using TMPro;

namespace UI
{
    public class NoticeItemDescription : TextMeshProUGUI, IEnumerator
    {
        public float delay;
        private float elapsedTime;

        public event OnTaskUpdateDelegate OnTaskUpdate;
        public event OnTaskCompleteDelegate OnTaskComplete;

        bool IEnumerator.MoveNext()
        {
            var deltaTime = TimeScaleManager.Get.GetDeltaTimeOfTag("Global");
            elapsedTime += deltaTime;

            OnTaskUpdate?.Invoke(elapsedTime);

            if (elapsedTime >= delay)
            {
                OnTaskComplete?.Invoke();
            }

            return elapsedTime <= delay;
        }

        void IEnumerator.Reset()
        {
            text = null;
            delay = 0;
            elapsedTime = 0;
        }

        object IEnumerator.Current => null;

        public delegate void OnTaskCompleteDelegate();

        public delegate void OnTaskUpdateDelegate(float progress);

    }
}