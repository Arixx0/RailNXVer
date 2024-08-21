using UnityEngine;

namespace Utility
{
    public static partial class MathUtility
    {
        public static float Squared(this float value)
        {
            return value * value;
        }

        public static float ClampOverflow(float value, float min, float max)
        {
            if (value < min)
                return max - Mathf.Abs(Mathf.Abs(min) - Mathf.Abs(value));

            if (value > max)
                return min + Mathf.Abs(Mathf.Abs(max) - Mathf.Abs(value));

            return value;
        }
    }
}