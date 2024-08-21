using UnityEngine;
using Utility;

namespace Environments
{
    public class GreatStormSector : EnvironmentSector
    {
        [SerializeField] private float sectorMovementSpeed;
        [SerializeField] private string targetTag = "Train";

        private GameObject m_Target;

        private void Awake()
        {
            m_Target = GameObject.FindGameObjectWithTag(targetTag);
        }

        private void Update()
        {
            if (VectorUtility.DistanceInXZCoord(m_Target.transform.position, transform.position) <= 1f)
            {
                return;
            }
            var forwardRotation = Quaternion.LookRotation(transform.forward, Vector3.up);
            var targetDirection = (m_Target.transform.position - transform.position).normalized;
            var newPosition = transform.position + targetDirection.GetXZ() * sectorMovementSpeed * TimeScaleManager.Get.GetDeltaTimeOfTag(targetTag);
            transform.SetPositionAndRotation(newPosition,forwardRotation);
        }
    }
}