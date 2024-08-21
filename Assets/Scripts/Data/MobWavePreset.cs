using System;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "NewMobWavePreset", menuName = "TrainCraft/Mob Wave Preset")]
    public class MobWavePreset : ScriptableObject
    {
        [HideInInspector]
        public List<MobSpawnProfile> spawnProfiles = new();
    }

    [Serializable]
    public struct MobSpawnProfile
    {
        public GameObject prefab;
        
        public Vector3 localPosition;
    }
}