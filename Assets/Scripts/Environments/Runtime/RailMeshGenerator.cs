using PathCreation;
using sc.modeling.splines.runtime;
using UnityEngine;
using UnityEngine.Splines;

namespace Environments
{
    [RequireComponent(typeof(SplineContainer))]
    public class RailMeshGenerator : MonoBehaviour
    {
        [SerializeField] private SplineContainer splineContainer;
        [SerializeField] private SplineMesher splineMesher;

        public Spline Spline => splineContainer.Spline;

        public void GenerateSplineFromBezierPath(BezierPath bezierPath)
        {
            splineContainer.AddSpline();
            splineContainer.Spline.Clear();

            var knotCount = bezierPath.NumSegments + 1;
            for (var i = 0; i < knotCount; i++)
            {
                var point = bezierPath.GetPoint(i * 3);
                var tangentIn = (i != 0 ? bezierPath.GetPoint(i * 3 - 1) : point) - point;
                var tangentOut = (i != knotCount - 1 ? bezierPath.GetPoint(i * 3 + 1) : point) - point;

                splineContainer.Spline.Add(new BezierKnot(point, tangentIn, tangentOut));
            }
        }

        public void RebuildRailMesh()
        {
            splineMesher.Rebuild();
        }
    }
}
