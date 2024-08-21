using Attributes;
using UnityEngine;
namespace Environments
{
    public class StageSectorEventTrigger : MonoBehaviour
    {
        [SerializeField] private BoxCollider triggerCollider;
        [SerializeField] private string triggerObjectTag = "Train";
        [SerializeField, Disabled] private bool didActivated;
        [SerializeField] private Collider triggeringCollider;

        public Collider TriggeringCollider => triggeringCollider;
        
        public event OnTriggerEnterDelegate onEnterTrigger;

        private void Reset()
        {
            if (!TryGetComponent(out triggerCollider))
            {
                triggerCollider = gameObject.AddComponent<BoxCollider>();
            }
            triggerCollider.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(triggerObjectTag) || didActivated)
            {
                return;
            }

            didActivated = true;
            triggeringCollider = other;
            onEnterTrigger?.Invoke();
        }

        public delegate void OnTriggerEnterDelegate();
    }
}