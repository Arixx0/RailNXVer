using UnityEditor;
using UnityEngine;

namespace CameraUtility
{
    [CustomEditor(typeof(CameraController))]
    public class CameraControllerEditor : Editor
    {
        private Vector3[] m_CameraBoundingBoxSegments = new Vector3[8];
        
        private CameraController Target { get; set; }
        
        private Camera Camera { get; set; }
        
        private void OnEnable()
        {
            Target = (CameraController)target;
            Camera = Target.GetComponent<Camera>();
        }

        private void OnSceneGUI()
        {
            var instanceId = GetInstanceID();
            
            Handles.CubeHandleCap(
                instanceId, Target.Orientantion.position, Target.Orientantion.rotation, 1f, EventType.Repaint);
            
            DrawMaximumCameraBounds();
            DrawOrientationDisplacement();
            DrawMoveVelocity();
            DrawGlobalAudioListenerPoint();
        }

        private void DrawMoveVelocity()
        {
            Handles.color = Color.red;
            Handles.DrawLine(Target.Orientantion.position, Target.Orientantion.position + Target.MoveVelocity, 3f);
        }

        private void DrawGlobalAudioListenerPoint()
        {
            int instanceId = GetInstanceID();
            
            Handles.color = new Color(0.988f, 0.753f, 0.027f);
            Handles.CubeHandleCap(instanceId, Target.GlobalAudioListener.position, Quaternion.identity, 1f, EventType.Repaint);
            Handles.Label(Target.GlobalAudioListener.position, $"{nameof(Target.GlobalAudioListener)}");
        }

        private void DrawOrientationDisplacement()
        {
            var p1 = Target.CachedTransform.position;
            var p2 = Target.Orientantion.position;
            var p3 = new Vector3(p1.x, p2.y, p1.z);
            
            Handles.color = Color.yellow;
            Handles.DrawLine(p1, p2);
            Handles.DrawLine(p2, p3);
            Handles.DrawLine(p3, p1);

            Handles.color = Color.blue;
            Handles.DrawWireArc(p2, Vector3.up, Vector3.right, 360, Target.ArmLength, 2.5f);

            Handles.color = Color.blue;
            Handles.DrawLine(p2, p2 + Vector3.forward * 3f, 2f);

            Handles.color = Color.red;
            Handles.DrawLine(p2, p2 + Vector3.right * 3f, 2f);
        }

        private void DrawMaximumCameraBounds()
        {
            // draw the maximum orthographic size
            var maxOrthoSize = Target.ZoomScale.max;
            var horizontalOrthoSize = maxOrthoSize * Camera.aspect;

            var p1 = Target.CachedTransform.position + Target.CachedTransform.up * maxOrthoSize +
                     Target.CachedTransform.right * -horizontalOrthoSize;
            var p2 = Target.CachedTransform.position + Target.CachedTransform.up * maxOrthoSize +
                     Target.CachedTransform.right * horizontalOrthoSize;
            var p3 = Target.CachedTransform.position + Target.CachedTransform.up * -maxOrthoSize +
                     Target.CachedTransform.right * horizontalOrthoSize;
            var p4 = Target.CachedTransform.position + Target.CachedTransform.up * -maxOrthoSize +
                     Target.CachedTransform.right * -horizontalOrthoSize;

            m_CameraBoundingBoxSegments[0] = m_CameraBoundingBoxSegments[7] = p1;
            m_CameraBoundingBoxSegments[1] = m_CameraBoundingBoxSegments[2] = p2;
            m_CameraBoundingBoxSegments[3] = m_CameraBoundingBoxSegments[4] = p3;
            m_CameraBoundingBoxSegments[5] = m_CameraBoundingBoxSegments[6] = p4;
            
            Handles.color = Color.magenta;
            Handles.DrawLines(m_CameraBoundingBoxSegments);
            
            Handles.Label(p2, "Max Zoom Bounds");
            
            var minOrthoSize = Target.ZoomScale.min;
            horizontalOrthoSize = minOrthoSize * Camera.aspect;

            p1 = Target.CachedTransform.position + Target.CachedTransform.up * minOrthoSize +
                     Target.CachedTransform.right * -horizontalOrthoSize;
            p2 = Target.CachedTransform.position + Target.CachedTransform.up * minOrthoSize +
                     Target.CachedTransform.right * horizontalOrthoSize;
            p3 = Target.CachedTransform.position + Target.CachedTransform.up * -minOrthoSize +
                     Target.CachedTransform.right * horizontalOrthoSize;
            p4 = Target.CachedTransform.position + Target.CachedTransform.up * -minOrthoSize +
                     Target.CachedTransform.right * -horizontalOrthoSize;

            m_CameraBoundingBoxSegments[0] = m_CameraBoundingBoxSegments[7] = p1;
            m_CameraBoundingBoxSegments[1] = m_CameraBoundingBoxSegments[2] = p2;
            m_CameraBoundingBoxSegments[3] = m_CameraBoundingBoxSegments[4] = p3;
            m_CameraBoundingBoxSegments[5] = m_CameraBoundingBoxSegments[6] = p4;
            
            Handles.color = new Color(0.918f, 0.488f, 0.203f);
            Handles.DrawLines(m_CameraBoundingBoxSegments);
            
            Handles.Label(p2, "Min Zoom Bounds");
        }
    }
}