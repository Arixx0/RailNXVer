using Attributes;
using PathCreation;

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Environments
{
    public enum MapSectorType
    {
        NormalRoad,
        EventSector,
        CrossRoad,
    }
    
    public class MapSector : MonoBehaviour, IPassageSelectEventDataProvider
    {
        [SerializeField, Disabled] private string guid = Guid.NewGuid().ToString();
        
        [Header("Map Sector Properties")]
        [SerializeField] private Vector3 sectorSize;
        [SerializeField] private Vector3[] sectorAccessPoints = Array.Empty<Vector3>();
        [SerializeField] private Vector3[] sectorExitPoints = Array.Empty<Vector3>();
        
        [Space]
        [SerializeField] private PathCreator childPathPrefab;
        [SerializeField] private StageSectorEventTrigger sectorEventTriggerPrefab;
        [SerializeField] private MapSectorPath mapSectorPath;

        [Space]
        [SerializeField] private float crossRoadStartPointDistance = 10f;
        [SerializeField] private float crossRoadSectionLength = 5f;
        [SerializeField] private float bezierPathControlLength = 0.3f;

        [Space]
        [SerializeField] private MapSectorSpawnMap spawnMap;
        
        [Header("Runtime Metadata")]
        [SerializeField] private MapSectorType sectorType;
        [SerializeField] private int designatedExitPointCount;
        [SerializeField] private float distanceBetweenExitPoints = 8.0f;
        
        public Vector3 SectorSize => sectorSize;

        public MapSectorType SectorType { get => sectorType; set => sectorType = value; }

        public int DesignatedExitPointCount { get => designatedExitPointCount; set => designatedExitPointCount = value; }

        public MapSectorPath Path => mapSectorPath;

        public int RoomIndex { get; set; }
        
        public float DistanceBetweenExitPoints => distanceBetweenExitPoints;

        public MapSectorSpawnMap SpawnMap => spawnMap;

        private void Reset()
        {
            sectorSize = new Vector3(80, 4, 80);
            sectorAccessPoints = new[] { new Vector3(0, 0, sectorSize.z * -0.5f) };
            sectorExitPoints = new[] { new Vector3(0, 0, sectorSize.z * 0.5f) };
            mapSectorPath = GetComponentInChildren<MapSectorPath>();
        }

        private void OnValidate()
        {
            Debug.Assert(sectorAccessPoints.Length > 0, "Sector Access Points is empty.");
            Debug.Assert(sectorExitPoints.Length > 0, "Sector Exit Points is empty.");
        }
        
#if UNITY_EDITOR
        [ContextMenu("Assign GUID")]
        private void AssignGuid()
        {
            guid = Guid.NewGuid().ToString();
        }
#endif
        
#if UNITY_EDITOR
        [ContextMenu("Manipulate Spawn Map Data")]
        private void ManipulateSpawnMap()
        {
            // spawnMap.Manipulate();
        }
#endif

        public void CreateSectorPaths(float accessPointOffsetDelta = 0f)
        {
            List<Vector3> points = new List<Vector3>(4);
            PathCreator childPath;
            Vector3 p0, p1, p2, p3;
            
            p0 = sectorAccessPoints[0] + Vector3.right * accessPointOffsetDelta;
            
            if (sectorType != MapSectorType.CrossRoad ||
                (sectorType == MapSectorType.CrossRoad && designatedExitPointCount == 1))
            {
                p1 = p0 + Vector3.forward * 5f;
                p3 = sectorAccessPoints[0] + Vector3.forward * 10f;
                p2 = Vector3.Lerp(p1, p3, 0.5f);
                
                points.Add(p0);
                points.Add(p1);
                points.Add(p2);
                points.Add(p3);
                points.Add(sectorExitPoints[0]);
                
                childPath = Instantiate(childPathPrefab, transform);
                childPath.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                childPath.bezierPath = new BezierPath(points)
                {
                    AutoControlLength = bezierPathControlLength,
                    ControlPointMode = BezierPath.ControlMode.Aligned,
                    IsClosed = false,
                };
                childPath.TriggerPathUpdate();
                
                mapSectorPath.SetPaths(childPath);
                return;
            }
            
            var crossPointZ = crossRoadStartPointDistance / sectorSize.z;
            var crossPointNormalDist = crossRoadSectionLength / sectorSize.z;
            
            p0 = Vector3.Lerp(sectorAccessPoints[0], sectorExitPoints[0], crossPointZ);
            p1 = Vector3.Lerp(sectorAccessPoints[0], sectorExitPoints[0], crossPointZ + crossPointNormalDist * 0.5f);
            p2 = Vector3.Lerp(sectorAccessPoints[0], sectorExitPoints[0], crossPointZ + crossPointNormalDist * 1.5f);
            p3 = sectorExitPoints[0] + Vector3.right * -((designatedExitPointCount - 1) * 0.5f * distanceBetweenExitPoints);
            
            var directionDelta = (p1 - p0).normalized * bezierPathControlLength;
            
            // add main path (before the cross point)
            points.Add(sectorAccessPoints[0]);
            points.Add(p0);
            
            childPath = Instantiate(childPathPrefab, transform);
            childPath.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            childPath.bezierPath = new BezierPath(points)
            {
                AutoControlLength = bezierPathControlLength,
                ControlPointMode = BezierPath.ControlMode.Aligned,
                IsClosed = false,
            };
            childPath.TriggerPathUpdate();
            
            childPath.bezierPath.SetPoint(1, sectorAccessPoints[0] + (p0 - sectorAccessPoints[0]).normalized * bezierPathControlLength, true);
            childPath.bezierPath.SetPoint(2, p0 - (p0 - sectorAccessPoints[0]).normalized * bezierPathControlLength, true);
            childPath.TriggerPathUpdate();
            
            // add sub paths (after the cross point)
            var subPaths = new PathCreator[designatedExitPointCount];
            for (var i = 0; i < designatedExitPointCount; i += 1)
            {
                var modifiedP2 = new Vector3(p3.x, p2.y, p2.z);
                
                points.Clear();
                points.Add(p0);
                points.Add(p1);
                points.Add(modifiedP2);
                points.Add(p3);
                
                subPaths[i] = Instantiate(childPathPrefab, transform);
                subPaths[i].transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                subPaths[i].bezierPath = new BezierPath(points)
                {
                    AutoControlLength = bezierPathControlLength,
                    ControlPointMode = BezierPath.ControlMode.Aligned,
                    IsClosed = false,
                };
                subPaths[i].TriggerPathUpdate();
                
                // update p2 > p3 control handles for visual
                subPaths[i].bezierPath.SetPoint(1, p0 + directionDelta, true);
                subPaths[i].bezierPath.SetPoint(2, p1 - directionDelta, true);
                subPaths[i].bezierPath.SetPoint(4, p1 + directionDelta, true);
                subPaths[i].bezierPath.SetPoint(5, modifiedP2 + (subPaths[i].bezierPath.GetPoint(5) - modifiedP2).normalized * bezierPathControlLength, true);
                subPaths[i].bezierPath.SetPoint(7, modifiedP2 + (subPaths[i].bezierPath.GetPoint(7) - modifiedP2).normalized * bezierPathControlLength, true);
                subPaths[i].bezierPath.SetPoint(8, p3 - (p3 - modifiedP2).normalized * bezierPathControlLength, true);
                subPaths[i].TriggerPathUpdate();
                
                p3 += Vector3.right * distanceBetweenExitPoints;
            }
            
            mapSectorPath.SetPaths(childPath, subPaths);
        }

        // private void SmoothenPath(BezierPath path)
        // {
        //     var knotCount = path.NumSegments + 1;
        //     for (var i = 0; i < knotCount - 1; i++)
        //     {
        //         var point = path.GetPoint(i * 3);
        //         var nextPoint = path.GetPoint((i + 1) * 3);
        //
        //         var direction = (nextPoint - point).normalized * bezierPathControlLength;
        //         
        //         path.SetPoint(i * 3 + 1, point + direction);
        //         path.SetPoint(i * 3 + 2, nextPoint - direction);
        //     }
        // }

        // public void ManipulateRailwayPath()
        // {
        //     if (sectorType == MapSectorType.CrossRoad)
        //     {
        //         GenerateCrossRoadRailwayPath(); 
        //         return;
        //     }
        //
        //     var points = new List<Vector3>(2);
        //     points.Add(sectorAccessPoints[0]);
        //     points.Add(sectorExitPoints[0]);
        //
        //     var path = new BezierPath(points);
        //     var pathCreator = CreatePathCreator(path);
        //     
        //     // create path update notifier
        //     InstantiatePathUpdateNotifier(pathCreator);
        // }

        private void InstantiatePathUpdateNotifier(PathCreator pathCreator)
        {
            pathCreator.path.GetPointAndRotationAtDistance(out var point, out var rotation, pathCreator.path.length - 10f, EndOfPathInstruction.Stop);
            
            var pathUpdateNotifier = new GameObject("Path Update Notifier");
            pathUpdateNotifier.transform.SetParent(transform);
            pathUpdateNotifier.transform.SetPositionAndRotation(point, rotation);

            pathUpdateNotifier.AddComponent<BoxCollider>();
            pathUpdateNotifier.AddComponent<StageSectorEventTrigger>();
            pathUpdateNotifier.AddComponent<PathUpdateNotifier>();
        }

        private void GenerateCrossRoadRailwayPath()
        {
            var points = new List<Vector3>(4);
            PathCreator pathCreator;
            
            if (designatedExitPointCount <= 1)
            {
                // if there is only one exit point, create a straight path
                
                points.Add(sectorAccessPoints[0]);
                points.Add(sectorExitPoints[0]);
        
                var path = new BezierPath(points);
                pathCreator = CreatePathCreator(path);
                InstantiatePathUpdateNotifier(pathCreator);
                return;
            }
        
            var midPoint = Vector3.Lerp(sectorAccessPoints[0], sectorExitPoints[0], 0.3f);
            
            // add points to mid-point
            points.Clear();
            points.Add(sectorAccessPoints[0]);
            points.Add(midPoint);
            var forwardPath = new BezierPath(points);
            pathCreator = CreatePathCreator(forwardPath);
            InstantiatePathUpdateNotifier(pathCreator);
        
            var distanceDelta = Vector3.right * distanceBetweenExitPoints;
            var exitPointAlpha = sectorExitPoints[0] - distanceDelta * ((designatedExitPointCount - 1) * 0.5f);
            var curvePoint = Vector3.Lerp(sectorAccessPoints[0], sectorExitPoints[0], 0.6f);
        
            for (var i = 0; i < designatedExitPointCount; i += 1)
            {
                var tExitPoint = exitPointAlpha + distanceDelta * i;
                var tCurvePoint = new Vector3(tExitPoint.x, curvePoint.y, curvePoint.z);
                
                points.Clear();
                points.Add(midPoint);
                points.Add(tCurvePoint);
                points.Add(tExitPoint);
        
                var exitPath = new BezierPath(points);
                pathCreator = CreatePathCreator(exitPath);
                InstantiatePathUpdateNotifier(pathCreator);
            }
        }

        private PathCreator CreatePathCreator(BezierPath path)
        {
            var pathCreatorObj = new GameObject("Railway Path 0");
            pathCreatorObj.transform.SetParent(transform);
            pathCreatorObj.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        
            path.AutoControlLength = 0.1f;
            
            var pathCreator = pathCreatorObj.AddComponent<PathCreator>();
            pathCreator.bezierPath = path;
        
            return pathCreator;
        }

        public void CreatePathUpdateNotifier(MapSector nextSector, int subPathIndex = -1)
        {
            StageSectorEventTrigger trigger;
            Vector3 position;
            Quaternion rotation;
            
            if (subPathIndex == -1)
            {
                mapSectorPath.GetTransformAtDistance(mapSectorPath.GetLength() - 5f, out position, out rotation);
        
                trigger = Instantiate(sectorEventTriggerPrefab, transform);
                trigger.transform.SetPositionAndRotation(position, rotation);
                trigger.onEnterTrigger += () => Debug.Log($"Gonna move to {nextSector.name}");
        
                return;
            }
            
            mapSectorPath.GetTransformAtDistance(mapSectorPath.GetLength(subPathIndex) - 5f, subPathIndex, out position, out rotation);
        
            trigger = Instantiate(sectorEventTriggerPrefab, transform);
            trigger.transform.SetPositionAndRotation(position, rotation);
            trigger.onEnterTrigger += () => Debug.Log($"Gonna move to {nextSector.name}");
        }

        public void CreatePathSelectEvent()
        {
            var targetDistance = designatedExitPointCount == 1 ? 10f : mapSectorPath.GetLength() - 5f;
            mapSectorPath.GetTransformAtDistance(targetDistance, out var position, out var rotation);
            
            var trigger = Instantiate(sectorEventTriggerPrefab, position, rotation, transform);
            PassageSelectEventExecutor.CreateFromTrigger(trigger, this);
        }

        public StageSectorEventTrigger CreateSectorEventTrigger(float localDistance)
        {
            mapSectorPath.GetTransformAtDistance(localDistance, out var position, out var rotation);

            var trigger = Instantiate(sectorEventTriggerPrefab, position, rotation, transform);
            return trigger;
        }

        public int GetPassageCount()
        {
            return mapSectorPath.SubPathCount;
        }
        
        public PathCreator GetPathAtIndex(int index)
        {
            return mapSectorPath.GetPath(index);
        }
    }
}