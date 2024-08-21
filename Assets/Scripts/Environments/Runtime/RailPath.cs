using System.Collections.Generic;
using Attributes;
using PathCreation;
using UnityEngine;

namespace Environments
{
    public class RailPath : MonoBehaviour
    {
        [SerializeField, Disabled]
        private List<PathCreator> currentPaths = new(32);

        [SerializeField] 
        private float totalLength;
        
        public float TotalLength => totalLength;

        public void ClearPaths()
        {
            currentPaths.Clear();
            totalLength = 0f;
        }

        public void AddPath(PathCreator path)
        {
            currentPaths.Add(path);

            totalLength += path.path.length;
        }

        public void GetTransformAtDistance(float distance, out Vector3 position, out Quaternion rotation)
        {
            var accumulatedDistance = 0f;
            
            foreach (var path in currentPaths)
            {
                if (accumulatedDistance + path.path.length < distance)
                {
                    accumulatedDistance += path.path.length;
                    continue;
                }

                var localDistance = distance - accumulatedDistance;
                path.path.GetPointAndRotationAtDistance(
                    out position,
                    out rotation,
                    localDistance,
                    EndOfPathInstruction.Stop);
                return;
            }
            
            currentPaths[^1].path.GetPointAndRotationAtDistance(
                out position,
                out rotation,
                distance,
                EndOfPathInstruction.Stop);
        }
    }
}