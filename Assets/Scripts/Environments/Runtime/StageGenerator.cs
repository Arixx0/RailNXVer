using Data;

using System;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

namespace Environments
{
    public class StageGenerator : MonoBehaviour
    {
        [SerializeField] private StageGenerationSettings settings;
        [SerializeField] private int seedPathCount;
        [SerializeField] private MobSpawner mobSpawnerPrefab;
        [SerializeField] private ResourceVeinSpawner resourceVeinSpawnerPrefab;
        [SerializeField] private NavMeshSurface navMeshSurface;
        [SerializeField] private NavMeshData navMeshData;

        private List<Room> m_Rooms = new();
        private System.Random m_Random;
        private List<MapSector> m_SpawnedMapSectors = new();
        
        public List<Room> Rooms => m_Rooms;
        
        public StageGenerationSettings Settings => settings;

        public List<MapSector> MapSectors => m_SpawnedMapSectors;

        public void Initialize()
        {
            GeneratePaths();
            SpawnStageSectors(m_Rooms.First(t => t.roomEventType == RoomEventType.InitialRoom));
        }
        
        public void GeneratePaths()
        {
            m_Random = new System.Random(settings.seedHash);
            
            GeneratePrimitivePaths();
            CacheNextRooms();
            AssignRoomEvents();
            ApplyEarlyPlacementRule();
            ApplyConsecutivePreventionRule();
            InitializeRoomEventData();
        }
        
        // Generate rooms and assign the heading direction(in perspective of map data).
        private void GeneratePrimitivePaths()
        {
            // Calculate seed path count; The number fo DFS runs to create the path.
            seedPathCount = m_Random.Next((int)settings.seedPathCountRange.min, (int)settings.seedPathCountRange.max);
            
            // Process passage generation
            var dirMap = new Directions[settings.mapWidth, settings.mapHeight];
            var uniqueSeedPointCount = settings.uniqueSeedPathCount;
            for (var seedPointCount = 0; seedPointCount < seedPathCount;)
            {
                var x = m_Random.Next(0, settings.mapWidth);
                if (dirMap[x, 1] != Directions.None && uniqueSeedPointCount > 0)
                {
                    continue;
                }

                uniqueSeedPointCount--;
                seedPointCount++;
                
                GeneratePassage(ref dirMap, x, 1);
            }
            
            m_Rooms.Clear();
            
            // get valid rooms from generated map
            for (var y = 1; y < settings.mapHeight; y++)
            {
                for (var x = 0; x < settings.mapWidth; x++)
                {
                    if (dirMap[x, y] == Directions.None)
                    {
                        continue;
                    }
                    
                    m_Rooms.Add(new Room { x = x, y = y, directions = dirMap[x, y], id = m_Rooms.Count });
                }
            }
            
            // add first room and last room
            m_Rooms.Add(new Room { x = 0, y = 0, directions = Directions.ExclusiveForward, id = m_Rooms.Count });
            m_Rooms.Add(new Room { x = 0, y = settings.mapHeight - 1, directions = Directions.None, id = m_Rooms.Count });
        }
        
        // Generate passage recursively. The process continues until reaching the last room(before the boss event room).
        private void GeneratePassage(ref Directions[,] dirMap, int x, int y)
        {
            if (y >= settings.mapHeight - 2)
            {
                dirMap[x, y] |= Directions.ExclusiveForward;
                return;
            }
            
            var dir = m_Random.Next(-1, 2);

            if (x + dir < 0) dir = 0;
            else if (x + dir >= settings.mapWidth) dir = 0;

            if (dir == -1 && dirMap[x + dir, y].HasFlag(Directions.Right)) dir = 0;
            else if (dir == 1 && dirMap[x + dir, y].HasFlag(Directions.Left)) dir = 0;
                
            if (dir == -1) dirMap[x, y] |= Directions.Left;
            else if (dir == 0) dirMap[x, y] |= Directions.Forward;
            else if (dir == 1) dirMap[x, y] |= Directions.Right;
            
            GeneratePassage(ref dirMap, x + dir, y + 1);
        }
        
        // Cache the next rooms for each room based on the designated directions.
        private void CacheNextRooms()
        {
            foreach (var e in m_Rooms)
            {
                // if room is the first room...; if room is the initial room
                if (e.y == 0)
                {
                    e.nextRooms = m_Rooms.Where(t => t.y == 1).ToArray();
                    e.nextRoomCount = e.nextRooms.Length;
                    continue;
                }
                
                // if room is the last room...
                if (e.y == settings.mapHeight - 1)
                {
                    e.nextRoomCount = 0;
                    e.nextRooms = null;
                    continue;
                }
                
                // if room is right before the last room and heading to there
                // `Directions.ExclusiveForward` means that the room is heading to the last room
                if (e.directions == Directions.ExclusiveForward && e.y == settings.mapHeight - 2)
                {
                    e.nextRoomCount = 1;
                    e.nextRooms = m_Rooms.Where(t => t.y == settings.mapHeight - 1).ToArray();
                    continue;
                }
                
                e.nextRoomCount = 0;
                e.nextRooms = new Room[3];
                
                if (e.directions.HasFlag(Directions.Left))
                {
                    e.nextRooms[e.nextRoomCount++] = m_Rooms.First(r => r.x == e.x - 1 && r.y == e.y + 1);
                }
                
                if (e.directions.HasFlag(Directions.Forward))
                {
                    e.nextRooms[e.nextRoomCount++] = m_Rooms.First(r => r.x == e.x && r.y == e.y + 1);
                }
                
                if (e.directions.HasFlag(Directions.Right))
                {
                    e.nextRooms[e.nextRoomCount++] = m_Rooms.First(r => r.x == e.x + 1 && r.y == e.y + 1);
                }
            }
        }

        // Assign fixed & dynamic room events to the rooms.
        private void AssignRoomEvents()
        {
            // event distribution table
            var eventDistributionLUT = Enum.GetValues(typeof(RoomEventType))
                .Cast<RoomEventType>()
                .ToDictionary(t => t, _ => 0);
            
            // assign fixed room events
            foreach (var e in settings.fixedRoomEvents)
            {
                var targetY = (int)(e.isAbsolutePosition ? e.position : Mathf.Lerp(0, settings.mapHeight - 1, e.position));
                foreach (var targetRoom in m_Rooms.Where(t => t.y == targetY))
                {
                    targetRoom.roomEventType = e.roomEventType;
                    targetRoom.isFixedEvent = true;
                }
                
                eventDistributionLUT[e.roomEventType] += 1;
            }

            // validate dynamic event total possibility
            var totalPossibility = settings.dynamicRoomEvents.Sum(t => t.possibility);
            Debug.Assert(Mathf.Abs(1 - totalPossibility) < 0.01f, "Total possibility must be 1");
            
            // assign dynamic room events
            
            var eventArgs = settings.dynamicRoomEvents
                .Select(e => new DynamicRoomEventArgs { roomEventType = e.roomEventType, possibility = e.possibility })
                .ToList();
            var logBuilder = new System.Text.StringBuilder();
            
            foreach (var room in m_Rooms.Where(t => t.roomEventType == RoomEventType.None))
            {
                var possibility = m_Random.NextDouble();
                var accumulatedPossibility = 0.0f;
                var validEventTypeCount = eventArgs.Count;
                
                foreach (var e in eventArgs)
                {
                    if (e.possibility == 0)
                    {
                        continue;
                    }
                    
                    accumulatedPossibility += e.possibility;
                    if (possibility > accumulatedPossibility)
                    {
                        continue;
                    }
                    
                    room.roomEventType = e.roomEventType;
                    eventDistributionLUT[e.roomEventType] += 1;

                    if (e.maxCount != -1)
                    {
                        e.maxCount -= 1;
                        if (e.maxCount == 0)
                        {
                            validEventTypeCount -= 1;
                            
                            Debug.Log($"{e.roomEventType} has reached max count, distribute {e.possibility} to {validEventTypeCount} other events");
                            
                            var extraPossibility = e.possibility / validEventTypeCount;
                            e.possibility = 0;

                            foreach (var other in eventArgs.Where(other => other.roomEventType != e.roomEventType))
                            {
                                other.possibility += extraPossibility;
                            }
                        }
                    }
                    
                    break;
                }
                
                logBuilder.AppendLine($"Room({room.x}, {room.y}) - possibility({possibility}), {room.roomEventType}  ");
            }
            
            logBuilder.AppendLine($"\nEvent Distribution: {string.Join(", ", eventDistributionLUT.Select(t => $"{t.Key}({t.Value})"))}");
            Debug.Log(logBuilder.ToString());
        }

        // Apply early placement rule to distinct the room events that should be placed early.
        private void ApplyEarlyPlacementRule()
        {
            var extraProbability = settings.dynamicRoomEvents
                .Where(e => settings.earlyPlacementBlackList.Contains(e.roomEventType))
                .Sum(e => e.possibility);
            extraProbability /= settings.dynamicRoomEvents.Length - settings.earlyPlacementBlackList.Length;
            
            var alteredProbability = settings.dynamicRoomEvents
                .Where(e => !settings.earlyPlacementBlackList.Contains(e.roomEventType))
                .Select(e => new DynamicRoomEventArgs() { roomEventType = e.roomEventType, possibility = e.possibility + extraProbability })
                .ToList();

            var brokenRooms = m_Rooms
                .Where(t => !t.isFixedEvent && t.y < settings.earlyPlacementCheckDepth && settings.earlyPlacementBlackList.Contains(t.roomEventType));
            foreach (var room in brokenRooms)
            {
                var possibility = m_Random.NextDouble();
                var accumulatedPossibility = 0.0f;
                
                foreach (var e in alteredProbability)
                {
                    accumulatedPossibility += e.possibility;
                    if (possibility > accumulatedPossibility)
                    {
                        continue;
                    }
                    
                    room.roomEventType = e.roomEventType;
                    break;
                }
            }
        }

        // Apply consecutive prevention rule to distinct the room events that should not be placed consecutively.
        private void ApplyConsecutivePreventionRule()
        {
            var extraProbability = settings.dynamicRoomEvents
                .Where(e => settings.consecutivePlacementBlackList.Contains(e.roomEventType))
                .Sum(e => e.possibility);
            extraProbability /= settings.dynamicRoomEvents.Length - settings.consecutivePlacementBlackList.Length;
            
            var alteredProbability = settings.dynamicRoomEvents
                .Where(e => !settings.consecutivePlacementBlackList.Contains(e.roomEventType))
                .Select(e => new DynamicRoomEventArgs() { roomEventType = e.roomEventType, possibility = e.possibility + extraProbability })
                .ToList();

            var firstRoom = m_Rooms.First(t => t.roomEventType == RoomEventType.InitialRoom);
            ResolveConsecutiveRoomEvents(firstRoom, null, ref alteredProbability);
        }
        
        // Resolve consecutive room events based on the room event type in consecutive manner.
        private void ResolveConsecutiveRoomEvents(Room target, Room before, ref List<DynamicRoomEventArgs> eventArgs)
        {
            if (target.nextRoomCount == 0)
            {
                return;
            }

            if (!target.isFixedEvent &&
                (before != null && before.roomEventType == target.roomEventType) &&
                settings.consecutivePlacementBlackList.Contains(target.roomEventType))
            {
                var possibility = m_Random.NextDouble();
                var accumulatedPossibility = 0.0f;
                
                foreach (var e in eventArgs)
                {
                    accumulatedPossibility += e.possibility;
                    if (possibility > accumulatedPossibility)
                    {
                        continue;
                    }
                    
                    target.roomEventType = e.roomEventType;
                    break;
                }
            }
            
            for (var i = 0; i < target.nextRoomCount; i += 1)
            {
                ResolveConsecutiveRoomEvents(target.nextRooms[i], target, ref eventArgs);
            }
        }

        // Initialize room event data based on the room event type.
        private void InitializeRoomEventData()
        {
            foreach (var room in m_Rooms)
            {
                switch (room.roomEventType)
                {
                    case RoomEventType.ChopShop: break;
                    case RoomEventType.ResourcePiles: InitializeResourcePilesEvent(room); break;
                    case RoomEventType.NormalEnemyWave: InitializeNormalEnemyWaveEvent(room); break;
                    case RoomEventType.EliteEnemyWave: break;
                    case RoomEventType.EnemyOutpost: break;
                    case RoomEventType.ScrapPiles: break;
                    case RoomEventType.SpaceCenter: break;
                    case RoomEventType.ShopKeeper: break;
                    case RoomEventType.BossWave: break;
                    case RoomEventType.InitialRoom: break;
                }
                
                var displacement = settings.displacementArgs
                    .First(e => e.roomEventType == room.roomEventType)
                    .displacement;
                room.displacementRequired = displacement;
            }
        }

        // Initialize room event type of `ResourcePiles`.
        private void InitializeResourcePilesEvent(Room room)
        {
            var resourceAmount = new Dictionary<ResourceType, ResourceVeinData>(8)
            {
                // the power stone should be spawned always
                {
                    ResourceType.PowerStone,
                    new ResourceVeinData {
                        resourceType = ResourceType.PowerStone,
                        amount = (int)settings.inGameResourcesSpawnArgs.First(e => e.resourceType == ResourceType.PowerStone).spawnAmountRange.FromRandom(m_Random) 
                    }
                }
            };

            // generate resource spawn context for the room
            var resourceSpawnCount = m_Random.Next(1, settings.spawnableResourceOfTypeCount);
            foreach (var arg in settings.inGameResourcesSpawnArgs)
            {
                switch (arg.resourceType)
                {
                    case ResourceType.Iron:
                    case ResourceType.Titanium:
                    case ResourceType.Mythrill:
                        resourceAmount.Add(arg.resourceType, new ResourceVeinData() { resourceType = arg.resourceType, amount = (int)arg.spawnAmountRange.FromRandom(m_Random) });
                        break;
                    case ResourceType.PowerStone:
                    case ResourceType.Adamantine:
                        break;
                }

                resourceSpawnCount -= 1;
                if (resourceSpawnCount <= 0)
                {
                    break;
                }
            }
            
            Debug.Assert(resourceSpawnCount == 0, $"{resourceSpawnCount} types of resource couldn't be spawned");
            room.resourceVeinData = resourceAmount.Values.ToList();
        }

        // Initialize room event type of `NormalEnemyWave`.
        private void InitializeNormalEnemyWaveEvent(Room room)
        {
            room.mobWavePreset = settings.mobSpawnProfiles[m_Random.Next(0, settings.mobSpawnProfiles.Length)];
        }

        private void SpawnStageSectors(Room room, int accessPointIndex = 0)
        {
            var railPath = FindObjectOfType<RailPath>();
            var position = Vector3.zero;
            var lastIndex = 0;
            var accessPointOffset = 0f;
            
            // if there is a room that already spawned in the scene, the new room should be spawned based on the last spawned one.
            if (m_SpawnedMapSectors.Count > 0)
            {
                position =
                    m_SpawnedMapSectors[^1].transform.localPosition + Vector3.forward * (m_SpawnedMapSectors[^1].SectorSize.z * 0.5f);
                lastIndex = m_SpawnedMapSectors.Count;
                accessPointOffset =
                    (m_SpawnedMapSectors[^1].DesignatedExitPointCount - 1) * -0.5f * m_SpawnedMapSectors[^1].DistanceBetweenExitPoints
                    + accessPointIndex * m_SpawnedMapSectors[^1].DistanceBetweenExitPoints;
            }

            var requiredSectorCount = room.displacementRequired + 2;
            for (var i = 0; i < requiredSectorCount; i += 1)
            {
                var sectorType = MapSectorType.EventSector;
                if (i == 0) sectorType = MapSectorType.EventSector;
                else if (i == requiredSectorCount - 1) sectorType = MapSectorType.CrossRoad;
                else sectorType = MapSectorType.NormalRoad;
                
                var sectorPrefab = settings.mapSectorPrefabCollection.GetPrefab(room.roomEventType, sectorType);
                var sectorSizeHalf = Vector3.forward * (sectorPrefab.SectorSize.z * 0.5f);
                position += sectorSizeHalf;

                var newMapSector = Instantiate(sectorPrefab, transform);
                newMapSector.transform.SetLocalPositionAndRotation(position, Quaternion.identity);
                m_SpawnedMapSectors.Add(newMapSector);
                
                newMapSector.RoomIndex = room.id;
                newMapSector.SectorType = sectorType;

                switch (sectorType)
                {
                    case MapSectorType.NormalRoad:
                        newMapSector.DesignatedExitPointCount = 1;
                        newMapSector.CreateSectorPaths(accessPointOffset);
                        
                        break;
                    case MapSectorType.EventSector:newMapSector.DesignatedExitPointCount = 1;
                        newMapSector.CreateSectorPaths(accessPointOffset);
                        SpawnEventInstances(room, newMapSector);
                        break;
                    case MapSectorType.CrossRoad:
                        newMapSector.DesignatedExitPointCount = room.nextRoomCount;
                        newMapSector.CreateSectorPaths(accessPointOffset);
                        break;
                }
                
                accessPointOffset = 0;

                position += sectorSizeHalf;
            }

            for (var i = lastIndex; i < m_SpawnedMapSectors.Count; i += 1)
            {
                railPath.AddPath(m_SpawnedMapSectors[i].Path.MainPath);
                
                if (m_SpawnedMapSectors[i].SectorType == MapSectorType.CrossRoad)
                {
                    m_SpawnedMapSectors[i].CreatePathSelectEvent();
                }
            }
            
            // m_SpawnedMapSectors[^1].UpdateNavMeshVolume(navMeshData);
            navMeshSurface.UpdateNavMesh(navMeshData);
        }

        public void SpawnMapSector(int selectedSubPathIndex)
        {
            var lastSpawnedRoom = m_Rooms[m_SpawnedMapSectors[^1].RoomIndex];
            
            selectedSubPathIndex = Mathf.Clamp(selectedSubPathIndex, 0, lastSpawnedRoom.nextRoomCount);
            var nextRoom = lastSpawnedRoom.nextRooms[selectedSubPathIndex];
            
            SpawnStageSectors(nextRoom, selectedSubPathIndex);
        }

        private void SpawnEventInstances(Room room, MapSector targetSector)
        {
            switch (room.roomEventType)
            {
                case RoomEventType.ChopShop:
                    break;
                case RoomEventType.ResourcePiles:
                    var resourceVeinSpawner = Instantiate(resourceVeinSpawnerPrefab, targetSector.transform);
                    resourceVeinSpawner.Spawn(targetSector, room.resourceVeinData);
                    break;
                case RoomEventType.NormalEnemyWave:
                case RoomEventType.EliteEnemyWave:
                case RoomEventType.EnemyOutpost:
                    var mobSpawner = Instantiate(mobSpawnerPrefab, targetSector.transform);
                    mobSpawner.SpawnableAreaBoundary = targetSector.SectorSize;
                    mobSpawner.Spawn(room.mobWavePreset);

                    var trigger = targetSector.CreateSectorEventTrigger(5f);
                    trigger.onEnterTrigger += mobSpawner.ActivateMobWave;
                    break;
                case RoomEventType.ScrapPiles:
                    break;
                case RoomEventType.ShopKeeper:
                    trigger = targetSector.CreateSectorEventTrigger(5f);
                    ShopKeeperEventExecutor.CreateFromTrigger(trigger, settings.shopKeeperItemsDefinition);
                    break;
            }
        }
    }
}