using UnityEngine;

namespace Utility
{
    public static partial class QuaternionUtility
    {
        public static Quaternion ResetZAxis(this ref Quaternion rotation)
        {
            var eulerRotation = rotation.eulerAngles;
            eulerRotation.z = 0;
                
            var newRotation = Quaternion.Euler(eulerRotation);
            rotation = newRotation;
            return newRotation;
        }

        public static Quaternion GetRotationTowards(Vector3 position, Vector3 lookAtPosition,
            bool lockXRotation = false)
        {
            var direction = lookAtPosition - position;
            direction.y = lockXRotation ? 0 : direction.y;
            return Quaternion.LookRotation(direction, Vector3.up);
        }
    }
}