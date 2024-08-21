using System;
using UnityEngine;
using Data;

namespace Environments
{
    [Serializable]
    public struct ResourceVeinData
    {
        public ResourceType resourceType;

        public int amount;
    }
    
    public class ResourceVein : MonoBehaviour
    {
        [SerializeField] private new MeshRenderer renderer;
        [SerializeField] private GameObject beingScrappedNoteIcon;
        
        [Space]
        [SerializeField] private ResourceType resourceType;
        [SerializeField] private int amount;

        private Transform m_CachedTransform;
        private bool m_IsBeingScrapped;
        
        public Transform CachedTransform => (m_CachedTransform != CachedTransform ? m_CachedTransform : m_CachedTransform = transform);

        public bool IsValidToScrap => amount > 0;

        public bool IsBeingScrapped
        {
            get => m_IsBeingScrapped;
            set
            {
                m_IsBeingScrapped = value;
                beingScrappedNoteIcon?.SetActive(value);
            }
        }
        
        public ResourceType ResourceType => resourceType;
        
        public int Amount => amount;

        public event OnDestroyEventDelegate onDestroyEvent;

        private void Awake()
        {
            if (beingScrappedNoteIcon == null) { beingScrappedNoteIcon = null; }
            else { beingScrappedNoteIcon.SetActive(false); }
        }

        public void SetResourceTypeAndAmount(ResourceType resourceType, int amount)
        {
            this.resourceType = resourceType;
            this.amount = amount;
        }

        /// <returns>Returns TRUE if the stored amount run out.</returns>
        public bool TryTakeAwayResource(int desiredAmount, out int outputAmount)
        {
            outputAmount = Mathf.Min(desiredAmount, amount);

            amount -= desiredAmount;
            if (amount <= 0)
            {
                onDestroyEvent?.Invoke();
                gameObject.SetActive(false);
                // Destroy(gameObject, 3f);

                return true;
            }

            return false;
        }

        public static bool IsVeinValidToScrap(ResourceVein vein)
        {
            if (vein is null || vein == null || !vein.gameObject.activeSelf)
            {
                return false;
            }
            
            return vein.IsValidToScrap;
        }

        public delegate void OnDestroyEventDelegate();
    }
}