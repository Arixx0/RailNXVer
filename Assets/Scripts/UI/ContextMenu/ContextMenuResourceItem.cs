using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ContextMenuResourceItem : MonoBehaviour
    {
        [SerializeField] private Data.ChangingImageLoader resourceIconLoader;
        [SerializeField] private TextMeshProUGUI resourceAmount;

        private Transform m_CachedTransform;

        public Transform CachedTransform => m_CachedTransform ??= GetComponent<Transform>();

        public void ChangeResourceItem(string identifier,float value, bool availabilityResource = true)
        {
            resourceIconLoader.ChangeImage(identifier);
            resourceAmount.text = value.ToString();
            resourceAmount.color = availabilityResource ? Color.white : Color.red;
        }
    }
}