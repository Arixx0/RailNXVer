using UnityEngine;

namespace Utility
{
    public static partial class VectorUtility
    {
        public static float DistanceInXZCoord(Vector3 a, Vector3 b, bool allowSquaredDistance = false)
        {
            var diff = a - b;
            diff.y = 0f;
            var distance = allowSquaredDistance ? diff.sqrMagnitude : diff.magnitude;
            return distance;
        }

        public static Vector3 GetXZ(this Vector3 value, bool normalize = false)
        {
            var vec = new Vector3(value.x, 0f, value.z);

            if (normalize)
            {
                vec.Normalize();
            }
            
            return vec;
        }

        public static Vector3 Abs(this Vector3 value)
        {
            return new Vector3(Mathf.Abs(value.x), Mathf.Abs(value.y), Mathf.Abs(value.z));
        }
    }
}