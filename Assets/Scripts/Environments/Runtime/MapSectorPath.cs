using System.Collections.Generic;
using PathCreation;
using UnityEngine;
using Utility;

namespace Environments
{
    public class MapSectorPath : MonoBehaviour
    {
        [SerializeField] private PathCreator mainPath;
        [SerializeField] private PathCreator[] subPaths;
        [SerializeField, Range(1, 10)] private float pathWidth = 2f;
        [SerializeField] private RailMeshGenerator railMeshGeneratorPrefab;
        [SerializeField] private RailMeshGenerator[] railMeshGenerators;

        public PathCreator MainPath => mainPath;

        public int SubPathCount => subPaths.Length;

        public float GetLength(int subPathIndex = -1)
        {
            if (subPathIndex < 0)
            {
                // if `subPathIndex` is below zero, return the length of the main path
                return mainPath.path.length;
            }
            
            Debug.Assert(subPathIndex < subPaths.Length, $"subPathIndex {subPathIndex} is out of range.");
            return mainPath.path.length + subPaths[subPathIndex].path.length;
        }

        public List<Bounds> GetRailBounds()
        {
            var boundsList = new List<Bounds>(5);

            boundsList.Add(mainPath.path.bounds);
            for (var i = 0; i < subPaths.Length; i++)
            {
                boundsList.Add(subPaths[i].path.bounds);
            }
            
            return boundsList;
        }

        public void SetPaths(PathCreator mainPath)
        {
            DestroyExistingPathInstances();
            
            this.mainPath = mainPath;
            
            UpdateSplinesData();
            GenerateRailMesh();
        }

        public void SetPaths(PathCreator mainPath, PathCreator[] subPaths)
        {
            DestroyExistingPathInstances();

            this.mainPath = mainPath;
            this.subPaths = subPaths;
            
            UpdateSplinesData();
            GenerateRailMesh();
        }

        private void GenerateRailMesh()
        {
            for (var i = 0; i < railMeshGenerators.Length; i++)
            {
                railMeshGenerators[i].RebuildRailMesh();
            }
        }

        private void DestroyExistingPathInstances()
        {
#if UNITY_EDITOR
            if (mainPath != null)
            {
                DestroyImmediate(mainPath.gameObject);
            }

            for (var i = 0; i < subPaths.Length; i++)
            {
                DestroyImmediate(subPaths[i].gameObject);
            }
#endif
        }

        private void UpdateSplinesData()
        {
            var pathsCount = 1 + subPaths.Length;
            railMeshGenerators = new RailMeshGenerator[pathsCount];
            for (var i = 0; i < pathsCount; i++)
            {
                var railMeshGenerator = Instantiate(railMeshGeneratorPrefab, transform);
                railMeshGenerator.name = $"RailMeshGenerator_{i}";
                railMeshGenerators[i] = railMeshGenerator;
            }
            
            railMeshGenerators[0].GenerateSplineFromBezierPath(mainPath.bezierPath);
            for (var i = 0; i < subPaths.Length; i++)
            {
                railMeshGenerators[i + 1].GenerateSplineFromBezierPath(subPaths[i].bezierPath);
            }
        }
        
        public PathCreator GetPath(int subPathIndex = -1)
        {
            if (subPathIndex < 0)
            {
                return mainPath;
            }
            
            Debug.Assert(subPathIndex < subPaths.Length, $"subPathIndex {subPathIndex} is out of range.");
            return subPaths[subPathIndex];
        }
        
        public void GetTransformAtTime(float time, out Vector3 position, out Quaternion rotation)
        {
            var totalDistance = mainPath.path.length;
            var distance = Mathf.InverseLerp(0, totalDistance, time);
            mainPath.path.GetPointAndRotationAtDistance(out position, out rotation, distance, EndOfPathInstruction.Stop);
        }

        public void GetTransformAtTime(float time, int subPathIndex, out Vector3 position, out Quaternion rotation)
        {
            Debug.Assert(subPathIndex < subPaths.Length, $"Sub path index {subPathIndex} is out of range.");
            
            var totalDistance = mainPath.path.length + subPaths[subPathIndex].path.length;
            var distance = Mathf.InverseLerp(0, totalDistance, time);

            if (distance < mainPath.path.length)
            {
                mainPath.path.GetPointAndRotationAtDistance(out position, out rotation, distance, EndOfPathInstruction.Stop);
                return;
            }

            var offsetDistance = distance - mainPath.path.length;
            subPaths[subPathIndex].path.GetPointAndRotationAtDistance(out position, out rotation, offsetDistance, EndOfPathInstruction.Stop);
        }
        
        public void GetTransformAtDistance(float distance, out Vector3 position, out Quaternion rotation)
        {
            mainPath.path.GetPointAndRotationAtDistance(out position, out rotation, distance, EndOfPathInstruction.Stop);
        }

        public void GetTransformAtDistance(float distance, int subPathIndex, out Vector3 position, out Quaternion rotation)
        {
            Debug.Assert(subPathIndex < subPaths.Length, $"Sub path index {subPathIndex} is out of range.");

            var mainPathLength = mainPath.path.length;
            var totalPathLength = mainPathLength + subPaths[subPathIndex].path.length;
            Debug.Assert(totalPathLength > distance, $"Distance {distance} is out of range.");

            if (distance < mainPathLength)
            {
                mainPath.path.GetPointAndRotationAtDistance(out position, out rotation, distance, EndOfPathInstruction.Stop);
                return;
            }

            var offsetDistance = distance - mainPathLength;
            subPaths[subPathIndex].path.GetPointAndRotationAtDistance(out position, out rotation, offsetDistance, EndOfPathInstruction.Stop);
        }

        public bool IsCollidedFromPoint(Vector3 worldPoint, out Vector3 translationVector)
        {
            // REMARK:
            //  This code assumes that there is only one path that is subject to collision resolution.
            //  If there are multiple paths that are subject to collision resolution, this code should be updated.
            
            translationVector = Vector3.zero;
            
            var sqrPathWidth = pathWidth * pathWidth;
            for (var i = -1; i < SubPathCount; i++)
            {
                var nearestPointOnPath = GetPath(i).path.GetClosestPointOnPath(worldPoint);
                var directionDelta = (worldPoint - nearestPointOnPath).GetXZ();

                if (directionDelta.sqrMagnitude < sqrPathWidth)
                {
                    translationVector = directionDelta.normalized * (pathWidth - directionDelta.magnitude);
                    return true;
                }
            }
            
            return false;
        }
    }
}