using PathCreation;
using UnityEngine;

namespace Environments
{
    public class PathUpdateNotifier : MonoBehaviour
    {
        [SerializeField]
        private StageSectorEventTrigger sectorEventTrigger;

        [SerializeField]
        private PathCreator nextPath;
        
        public PathCreator NextPath
        {
            get => nextPath;
            set => nextPath = value;
        }

        private void Reset()
        {
            if (!TryGetComponent(out sectorEventTrigger))
            {
                Debug.Assert(sectorEventTrigger != null, "Sector Event Trigger is missing", gameObject);
            }
        }

        private void Start()
        {
            Debug.Assert(sectorEventTrigger != null, "Sector Event Trigger is missing", gameObject);
            sectorEventTrigger.onEnterTrigger += HandleOnEnterTrigger;
        }

        private void HandleOnEnterTrigger()
        {
            Debug.Log($"Next path is {nextPath.name}");
        }
    }
}