// #define DEBUG_UNIT_COMBATS

using Attributes;
using Data;

using System.Collections.Generic;
using UnityEngine;

// NOTE:
//  `MobSpawner.SpawnableAreaBoundary` is used for constraint spawn position within the boundary.
//  That property will shrink down the boundary by 10% to prevent the mob from spawning outside the NavMesh area.

// NOTE:
//  `MobSpawner.Spawn()` will spawn the mob at the local position of the MobSpawner.
//  But, the spawn position will be constraint within the `MobSpawner.SpawnableAreaBoundary`.
//  This is temporarily added logic, and it should be removed when the advanced units movement implemented.

namespace Environments
{
    public class MobSpawner : MonoBehaviour
    {
        [SerializeField] private bool initializeOnStart;
        [SerializeField] private MobWavePreset mobWavePreset;
        [SerializeField, Disabled] private Vector3 spawnableAreaBoundary;

        private readonly List<GameObject> m_SpawnedMobs = new();

        public Vector3 SpawnableAreaBoundary
        {
            get => spawnableAreaBoundary;
            set => spawnableAreaBoundary = value * 0.9f;
        }

        private void Start()
        {
            if (initializeOnStart)
            {
                if (!TryGetComponent<StageSectorEventTrigger>(out var trigger))
                {
                    return;
                }

                trigger.onEnterTrigger += ActivateMobWave;

                if (mobWavePreset == null)
                {
                    return;
                }

                Spawn(mobWavePreset);
            }
        }

        public void Spawn(MobWavePreset preset)
        {
            m_SpawnedMobs.Clear();
            m_SpawnedMobs.Capacity = preset.spawnProfiles.Count;

            var spawnBoundary = SpawnableAreaBoundary * 0.5f;

            foreach (var spawnProfile in preset.spawnProfiles)
            {
                Debug.Assert(spawnProfile.prefab != null,
                    $"Prefab which is referencing from `MobSpawnProfile` is null");
#if DEBUG_UNIT_COMBATS
                if (spawnProfile.prefab == null)
                {
                    continue;
                }
#endif

                var spawnPosition = transform.TransformPoint(spawnProfile.localPosition);

                // constraint spawn position within the boundary
                spawnPosition.x = Mathf.Clamp(spawnPosition.x, spawnBoundary.x * -1f, spawnBoundary.x);
                spawnPosition.z = Mathf.Clamp(spawnPosition.z, spawnBoundary.z * -1f, spawnBoundary.z);

                var mob = Instantiate(
                    spawnProfile.prefab,
                    transform.TransformPoint(spawnProfile.localPosition),
                    transform.rotation);

                m_SpawnedMobs.Add(mob.gameObject);

                mob.gameObject.SetActive(false);
            }
        }

        [ContextMenu("Try Spawn")]
        public void ActivateMobWave()
        {
            Debug.Log("Activate Wave", gameObject);

            foreach (var mob in m_SpawnedMobs)
            {
                mob.gameObject.SetActive(true);
            }
        }

        public void DestroySpawnedMobs()
        {
            foreach (var mob in m_SpawnedMobs)
            {
                if (mob == null)
                {
                    continue;
                }

                Destroy(mob.gameObject);
            }

            m_SpawnedMobs.Clear();
        }
    }
}