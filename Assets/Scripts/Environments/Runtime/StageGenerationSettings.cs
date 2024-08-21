using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using UnityEngine;

namespace Environments
{
    [CreateAssetMenu(menuName = "TrainCraft/Stage Generation Settings")]
    public class StageGenerationSettings : ScriptableObject
    {
        [Header("Primitive Options")]
        public int mapWidth = 6;
        public int mapHeight = 14;
        public string seed;
        [Attributes.Disabled] public int seedHash;
        public Data.Range seedPathCountRange = new(4, 7);
        public int uniqueSeedPathCount = 3;
        
        [Header("Event Placement Options")]
        public FixedRoomEventArgs[] fixedRoomEvents;
        public DynamicRoomEventArgs[] dynamicRoomEvents;

        [Header("Polishing Options")]
        public int earlyPlacementCheckDepth = 5;
        public RoomEventType[] earlyPlacementBlackList;
        public RoomEventType[] consecutivePlacementBlackList;

        [Header("Play Experience Options")]
        public MapSectorPrefabCollection mapSectorPrefabCollection;
        public DisplacementArgs[] displacementArgs;
        [Tooltip("단일 자원 이벤트 당 소환될 수 있는 최대 자원 개수")]
        public int spawnableResourceOfTypeCount = 3;
        public InGameResourcesSpawnArgs[] inGameResourcesSpawnArgs;
        public MobWavePreset[] mobSpawnProfiles;
        public ShopKeeperItemsDefinition shopKeeperItemsDefinition;

        private void Reset()
        {
            seed = DateTime.Now.ToString("yyyy-mm-dd hh:mm:ss");
            seedHash = seed.GetHashCode(StringComparison.InvariantCulture);
            
            fixedRoomEvents = new[]
            {
                new FixedRoomEventArgs { roomEventType = RoomEventType.ChopShop, isAbsolutePosition = false, position = 0.5f },
                new FixedRoomEventArgs { roomEventType = RoomEventType.ChopShop, isAbsolutePosition = true, position = mapHeight - 2 },
                new FixedRoomEventArgs { roomEventType = RoomEventType.ResourcePiles, isAbsolutePosition = true, position = 1 },
                new FixedRoomEventArgs { roomEventType = RoomEventType.NormalEnemyWave, isAbsolutePosition = true, position = 2 },
                new FixedRoomEventArgs { roomEventType = RoomEventType.BossWave, isAbsolutePosition = false, position = 1 },
                new FixedRoomEventArgs { roomEventType = RoomEventType.InitialRoom, isAbsolutePosition = true, position = 0 },
            };

            dynamicRoomEvents = new[]
            {
                new DynamicRoomEventArgs { roomEventType = RoomEventType.EnemyOutpost, possibility = 0.15f },
                new DynamicRoomEventArgs { roomEventType = RoomEventType.EliteEnemyWave, possibility = 0.0f },
                new DynamicRoomEventArgs { roomEventType = RoomEventType.NormalEnemyWave, possibility = 0.30f },
                new DynamicRoomEventArgs { roomEventType = RoomEventType.ScrapPiles, possibility = 0.15f },
                new DynamicRoomEventArgs { roomEventType = RoomEventType.SpaceCenter, possibility = 0.10f },
                new DynamicRoomEventArgs { roomEventType = RoomEventType.ShopKeeper, possibility = 0.10f },
                new DynamicRoomEventArgs { roomEventType = RoomEventType.ResourcePiles, possibility = 0.10f },
                new DynamicRoomEventArgs { roomEventType = RoomEventType.ChopShop, possibility = 0.10f },
            };

            earlyPlacementBlackList = new[]
            {
                RoomEventType.EnemyOutpost,
                RoomEventType.ChopShop,
                RoomEventType.ShopKeeper,
            };
            
            consecutivePlacementBlackList = new[]
            {
                RoomEventType.EnemyOutpost,
                RoomEventType.ChopShop,
                RoomEventType.ShopKeeper,
                RoomEventType.EliteEnemyWave,
            };

            displacementArgs = Enum.GetValues(typeof(RoomEventType))
                .Cast<RoomEventType>()
                .Where(e => e != RoomEventType.None)
                .Select(e => new DisplacementArgs { roomEventType = e, displacement = 1 })
                .ToArray();

            inGameResourcesSpawnArgs = new[]
            {
                new InGameResourcesSpawnArgs { resourceType = ResourceType.PowerStone, spawnPossibility = 1f, spawnAmountRange = new Data.Range(200, 400) },
                new InGameResourcesSpawnArgs { resourceType = ResourceType.Iron, spawnPossibility = 0.3f, spawnAmountRange = new Data.Range(200, 400) },
                new InGameResourcesSpawnArgs { resourceType = ResourceType.Titanium, spawnPossibility = 0.2f, spawnAmountRange = new Data.Range(100, 200) },
                new InGameResourcesSpawnArgs { resourceType = ResourceType.Mythrill, spawnPossibility = 0.1f, spawnAmountRange = new Data.Range(0, 0) },
            };
        }
    }

    [System.Flags]
    public enum Directions
    {
        None = 0,
        Left = 1,
        Forward = 2,
        Right = 4,
        ExclusiveForward = 8,
    }

    public enum RoomEventType
    {
        None,
        ChopShop,
        ResourcePiles,
        NormalEnemyWave,
        EliteEnemyWave,
        EnemyOutpost,
        ScrapPiles,
        SpaceCenter,
        ShopKeeper,
        BossWave,
        InitialRoom,
    }
    
    public class Room
    {
        public int x, y;
        public int id;
        public Directions directions;
        public int nextRoomCount;
        public Room[] nextRooms = Array.Empty<Room>();
        public RoomEventType roomEventType;
        public bool isFixedEvent;
        public int displacementRequired;
        public List<ResourceVeinData> resourceVeinData = null;
        public MobWavePreset mobWavePreset = null;
    }

    [System.Serializable]
    public class FixedRoomEventArgs
    {
        public RoomEventType roomEventType;
        public bool isAbsolutePosition;
        public float position;
    }

    [System.Serializable]
    public class DynamicRoomEventArgs
    {
        public RoomEventType roomEventType;
        [Range(0, 1)] public float possibility;
        public int maxCount = -1;
    }

    [System.Serializable]
    public class DisplacementArgs
    {
        public RoomEventType roomEventType;
        public int displacement;
    }

    [System.Serializable]
    public class InGameResourcesSpawnArgs
    {
        public ResourceType resourceType;
        public float spawnPossibility;
        public Data.Range spawnAmountRange;
    }
}