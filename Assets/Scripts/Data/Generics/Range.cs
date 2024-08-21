using UnityEngine;

namespace Data
{
    [System.Serializable]
    public struct Range
    {
        public float min;
        public float max;
        
        public Range(float min, float max)
        {
            this.min = min;
            this.max = max;
        }

        public float FromRandom(int seed)
        {
            return (new System.Random(seed)).Next((int)min, (int)max + 1);
        }

        public float FromRandom(System.Random random)
        {
            return random.Next((int)min, (int)max + 1);
        }

        public bool IsInRange(float value)
        {
            return min <= value && max >= value;
        }

        public float Clamp(float value)
        {
            return Mathf.Clamp(value, min, max);
        }

        public float GetNormalizedValue(float value)
        {
            return Mathf.InverseLerp(min, max, value);
        }

        public float FromNormalizedValue(float delta)
        {
            return Mathf.Lerp(min, max, delta);
        }
    }
}