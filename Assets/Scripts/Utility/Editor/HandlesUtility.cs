using UnityEditor;
using UnityEngine;

namespace Utility
{
    public static class HandlesUtility
    {
        private static void DrawWireArc(Vector3 center, Vector3 normal, Vector3 forward, Vector3 rotator, float radius,
            float thickness = 1.0f)
        {
            var right = Vector3.Cross(forward, normal);

            var rotatorHalf = rotator * 0.5f;

            var from =
                Quaternion.AngleAxis(-rotatorHalf.x, right) *
                Quaternion.AngleAxis(-rotatorHalf.y, normal) *
                Quaternion.AngleAxis(-rotatorHalf.z, forward) *
                forward;
            var to =
                Quaternion.AngleAxis(rotatorHalf.x, right) *
                Quaternion.AngleAxis(rotatorHalf.y, normal) *
                Quaternion.AngleAxis(rotatorHalf.z, forward) *
                forward;
            
            Handles.DrawLine(center, center + from * radius, thickness);
            Handles.DrawLine(center, center + to * radius, thickness);
            Handles.DrawWireArc(center, normal, from, rotator.y, radius, thickness);
        }
    }
}