using System.Collections.Generic;
using Data;
using UnityEngine;

namespace Environments
{
    public class ResourceVeinSpawner : MonoBehaviour
    {
        [SerializeField] private List<ResourceVein> spawnedVeins = new();
        
        private readonly Collider[] m_CollisionResolveBuffer = new Collider[16];

        public void Spawn(MapSector target, List<ResourceVeinData> context)
        {
            // use the camera's orthographic size to determine the spawnable bounds.
            // because of the UX problem, need to limit the spawnable bounds to the orthographic size.
            const float maxOrthoSize = 30f * 2;
            
            var rand = new System.Random();

            // calculate spawn bounds
            var sectorBounds = target.SpawnMap.Bounds;
            var spawnBounds = target.Path.GetRailBounds();
            for (var i = 0; i < spawnBounds.Count; i++)
            {
                var bounds = spawnBounds[i];
                bounds.Expand(Vector3.right * maxOrthoSize);

                bounds.min = new Vector3(
                    Mathf.Min(bounds.min.x, sectorBounds.min.x),
                    bounds.min.y,
                    Mathf.Min(bounds.min.z, sectorBounds.min.z));
                bounds.max = new Vector3(
                    Mathf.Max(bounds.max.x, sectorBounds.max.x),
                    bounds.max.y,
                    Mathf.Max(bounds.max.z, sectorBounds.max.z));

                spawnBounds[i] = bounds;
            }

            foreach (var data in context)
            {
                var prefab = Database.ResourcesSettingsData.GetVeinPrefabOfType(data.resourceType);
                var maxAmount = Database.ResourcesSettingsData.GetDepositPerVeinOfType(data.resourceType);
                
                for (var amount = data.amount; amount > 0; amount -= maxAmount)
                {
                    var targetBound = spawnBounds[rand.Next(0, spawnBounds.Count)];
                    var position = GetRandomPositionFromBounds(rand, targetBound);
                    var rotation = Quaternion.Euler(0, rand.Next(0, 360), 0);
                    
                    // check the collision with rails
                    var worldPoint = transform.TransformPoint(position);
                    if (target.Path.IsCollidedFromPoint(worldPoint, out var translation))
                    {
                        worldPoint += translation;
                    }
                    
                    // check the collision with other veins
                    // TODO: Collision resolve with other veins required.
                    
                    position = transform.InverseTransformPoint(worldPoint);
                    
                    var vein = Instantiate(prefab, transform);
                    vein.SetResourceTypeAndAmount(data.resourceType, Mathf.Min(amount, maxAmount));
                    vein.transform.SetLocalPositionAndRotation(position, rotation);
                    spawnedVeins.Add(vein);
                }
            }
        }

        public void DestroySpawnedVeins()
        {
            foreach (var vein in spawnedVeins)
            {
                if (vein != null)
                {
                    Destroy(vein.gameObject);
                }
            }
            
            spawnedVeins.Clear();
        }

        private static Vector3 GetRandomPositionFromBounds(System.Random rand, Bounds bounds)
        {
            return new Vector3(
                Mathf.Lerp(bounds.min.x, bounds.max.x, (float)rand.NextDouble()),
                bounds.center.y,
                Mathf.Lerp(bounds.min.z, bounds.max.z, (float)rand.NextDouble()));
        }
    }
}

