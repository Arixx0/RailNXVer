using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Environments
{
    [CreateAssetMenu(fileName = "MapSectorPrefabCollection", menuName = "TrainCraft/Map Sector Prefab Collection")]
    public class MapSectorPrefabCollection : ScriptableObject
    {
        public List<MapSectorPrefabReference> eventSectorPrefabs;
        public MapSectorPrefabReference variantEventSectorPrefabRef;
        public List<MapSector> normalRoadSectorPrefabs;
        public List<MapSector> crossRoadSectorPrefabs;
        
        public MapSector GetPrefab(RoomEventType roomEventType, MapSectorType mapSectorType)
        {
            MapSector prefab = null;
            switch (mapSectorType)
            {
                case MapSectorType.NormalRoad:
                    prefab = normalRoadSectorPrefabs[Random.Range(0, normalRoadSectorPrefabs.Count)];
                    break;
                case MapSectorType.EventSector:
                    var refer = eventSectorPrefabs.FirstOrDefault(refer => refer.roomEventType == roomEventType);
                    prefab = refer != null
                        ? refer.GetRandomPrefab()
                        : variantEventSectorPrefabRef.GetRandomPrefab();
                    break;
                case MapSectorType.CrossRoad:
                    prefab = crossRoadSectorPrefabs[Random.Range(0, crossRoadSectorPrefabs.Count)];
                    break;
                default:
                    Debug.LogError($"Sector type {mapSectorType} is not supported.");
                    break;
            }
            
            Debug.Assert(prefab != null, $"Prefab for {roomEventType} is not found.");
            return prefab;
        }
    }

    [System.Serializable]
    public class MapSectorPrefabReference
    {
        public RoomEventType roomEventType;
        public MapSector[] prefabs;
        
        public MapSector GetRandomPrefab()
        {
            return prefabs[Random.Range(0, prefabs.Length)];
        }
    }
}