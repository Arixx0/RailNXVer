using System.Collections.Generic;

using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "NewSFXPreset", menuName = "TrainCraft/SFX Preset")]
    public class SFXPreset : ScriptableObject
    {
        [HideInInspector] public List<AudioClip> sfx = new();
        [HideInInspector] public bool isRandomPitch;
        [HideInInspector] public float minPitch;
        [HideInInspector] public float maxPitch;

        public AudioClip GetRandomSFX()
        {
            return sfx[Random.Range(0, sfx.Count)];
        }

        public float GetRandomPitch()
        {
            return Random.Range(minPitch, maxPitch);
        }
    }
}