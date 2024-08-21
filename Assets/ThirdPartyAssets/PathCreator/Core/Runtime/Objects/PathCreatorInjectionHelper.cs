using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

namespace PathCreation
{
    [ExecuteInEditMode]
    public class PathCreatorInjectionHelper : MonoBehaviour
    {
        [HideInInspector]
        public List<Vector3> worldPoints = new();
        
        public LayerMask groundMask;
        
        public float pathHeightOffset = 0.05f;
        
        public List<PathCreatorInjectionHelper> possibleConsecutivePaths = new();

        [ContextMenu("Transform Points to Local")]
        public void TransformPointsToLocal()
        {
            for (var index = 0; index < worldPoints.Count; index += 1)
            {
                worldPoints[index] = transform.InverseTransformPoint(worldPoints[index]);
            }
        }
        
        [ContextMenu("Transform Points to World")]
        public void TransformPointsToWorld()
        {
            for (var index = 0; index < worldPoints.Count; index += 1)
            {
                worldPoints[index] = transform.TransformPoint(worldPoints[index]);
            }
        }

        [ContextMenu("Calculate Path")]
        public void CalculatePath()
        {
            // Calculate the path points to stick with the ground
            var pathPoints = new List<Vector3>();
            for (var index = 0; index < worldPoints.Count; index += 1)
            {
                var t = worldPoints[index];
                var ray = new Ray(t, Vector3.down);
                if (Raycast(ref ray, out var hit, 100f, groundMask, QueryTriggerInteraction.Ignore))
                {
                    pathPoints.Add(hit.point + Vector3.up * pathHeightOffset);
                    continue;
                }

                // If the raycast fails, log an error and return
                Debug.LogError($"Failed to find a valid point for {t}(index: {index})", gameObject);
                return;
            }

            // Add the path points to the path creator
            var pathCreator = GetComponent<PathCreator>();
            for (var i = 0; i < pathPoints.Count; i += 1)
            {
                var localPoint = transform.InverseTransformPoint(pathPoints[i]);
                
                // if there are valid anchor points...
                if (i < pathCreator.bezierPath.NumAnchorPoints)
                {
                    // NOTE:
                    //  *BezierPath.NumAnchorPoints* means the number of anchor points of the path(not the tangent points)
                    
                    pathCreator.bezierPath.MovePoint(i * 3, localPoint);
                    continue;
                }
                
                // if the path is closed(i.e. the path is infinite loop)...
                if (pathCreator.bezierPath.IsClosed)
                {
                    // NOTE:
                    //  *BezierPath.AddSegmentToEnd* does not work if the path is closed.
                    //  Should use *BezierPath.SplitSegment* instead.
                    
                    pathCreator.bezierPath.SplitSegment(localPoint, 0, 0.1f);
                    continue;
                }
                
                pathCreator.bezierPath.AddSegmentToEnd(localPoint);
            }

            // Update the tangent points
            for (var i = 0; i < pathPoints.Count; i += 1)
            {
                // calculate the forward direction first
                var forward = pathPoints[(i + 1) % pathPoints.Count] - pathPoints[i];
                forward.y = 0;
                forward = forward.normalized * 0.3f;
                
                // adjust forward tangent point
                if (pathCreator.bezierPath.IsClosed || i != (pathPoints.Count - 1))
                {
                    var forwardPoint = transform.InverseTransformPoint(pathPoints[i] + forward);
                    pathCreator.bezierPath.MovePoint(i * 3 + 1, forwardPoint);
                }
                
                // adjust backward tangent point
                var backwardPoint = new Vector3();
                if (!pathCreator.bezierPath.IsClosed)
                {
                    // if the bezier path is not closed, the first anchor's backward tangent point is invalid
                    if (i == 0)
                    {
                        continue;
                    }
                    
                    var backward = pathPoints[i - 1] - pathPoints[i];
                    backward.y = 0;
                    backward = backward.normalized * 0.3f;
                    
                    backwardPoint = transform.InverseTransformPoint(pathPoints[i] + backward);
                    pathCreator.bezierPath.MovePoint(i * 3 - 1, backwardPoint);
                    continue;
                }
                
                backwardPoint = transform.InverseTransformPoint(pathPoints[i] - forward);
                pathCreator.bezierPath.MovePoint(i == 0 ? (pathCreator.bezierPath.NumPoints - 1) : (i * 3 - 1), backwardPoint);

                // var pointForward = pathPoints[(i + 1) % pathPoints.Count] - pathPoints[i];
                // pointForward.y = 0;
                // pointForward = pointForward.normalized * 0.3f;
                //
                // if (pathCreator.bezierPath.IsClosed || i != (pathPoints.Count - 1))
                // {
                //     var pos = transform.InverseTransformPoint(pathPoints[i] + pointForward);
                //     pathCreator.bezierPath.MovePoint(i * 3 + 1, pos);
                // }
                //
                // if (pathCreator.bezierPath.IsClosed || i != 0)
                // {
                //     var index = i == 0 ? (pathPoints.Count - 1) * 3 + 2 : i * 3 - 1;
                //     var pos = pointForward * -1;
                //
                //     if (!pathCreator.bezierPath.IsClosed && i == (pathPoints.Count - 1))
                //     {
                //         pos = (pathPoints[i - 1] - pathPoints[i]).normalized * 0.3f;
                //     }
                //
                //     pos = transform.InverseTransformPoint(pathPoints[i] + pos);
                //     
                //     pathCreator.bezierPath.MovePoint(index, pos);
                // }
            }
        }

        [ContextMenu("Sync Consecutive Paths Start Point with End Point")]
        public void ForceSyncConsecutivePathStartPoint()
        {
            foreach (var helper in possibleConsecutivePaths)
            {
                helper.worldPoints[0] = worldPoints[^1];
            }
        }

        [ContextMenu("Create Path Update Notifier(SectorEventTrigger)")]
        public void CreatePathUpdateNotifier()
        {
            var pathCreator = GetComponent<PathCreator>();
            
            var notifierObject = new GameObject("PathUpdateNotifier");
            notifierObject.transform.SetParent(transform.parent);
            
            var collider = notifierObject.AddComponent<BoxCollider>();
            collider.isTrigger = true;
            
            pathCreator.path.GetPointAndRotationAtDistance(out var position, out var rotation,
                pathCreator.path.length - 3f);
            notifierObject.transform.SetPositionAndRotation(position, rotation);
        }
        
        public PathCreator[] GetConsecutivePathCreators()
        {
            var pathCreators = possibleConsecutivePaths
                .Select(e => e.GetComponent<PathCreator>())
                .ToArray();
            return pathCreators;
        }
        
        public static bool Raycast(ref Ray ray, out RaycastHit hit, float maxDistance, LayerMask groundMask,
            QueryTriggerInteraction triggerInteraction)
        {
            if (Physics.Raycast(ray, out hit, maxDistance, groundMask, triggerInteraction))
            {
                return true;
            }
            
            ray.direction *= -1;
            if (Physics.Raycast(ray, out hit, maxDistance, groundMask, triggerInteraction))
            {
                return true;
            }

            ray.origin = new Vector3(ray.origin.x, 100f, ray.origin.z);
            ray.direction *= -1;
            maxDistance *= 2;
            if (Physics.Raycast(ray, out hit, maxDistance, groundMask, triggerInteraction))
            {
                return true;
            }

            hit = default;
            return false;
        }
    }
}