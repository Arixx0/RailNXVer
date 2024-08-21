using UnityEngine;

// NOTE:
//  When the Canvas is billboard canvas, the canvas will have its back to the camera.
//  The screen/overlay canvas obeys this rule because its Transform.forward relative to the camera always matches the camera orientation.

namespace UI
{
    public class CanvasHelper : MonoBehaviour
    {
        [SerializeField] private bool isBillboardCanvas;
        [SerializeField] private bool lockXRotation;
        [SerializeField] private bool lockYRotation;
        [SerializeField] private bool lockZRotation;
        [SerializeField] private bool scaleWithOrthoSize;
        [SerializeField] private Vector3 baseScale;
        
        private Transform m_CachedTransform;
        private Camera m_CurrentCamera;
        private Transform m_CameraTransform;
        private Vector3 m_DefaultRotation;

        private void Awake()
        {
            m_CachedTransform = transform;
            
            m_CurrentCamera = FindObjectOfType<Camera>();
            m_CameraTransform = m_CurrentCamera.transform;
            
            m_DefaultRotation = m_CachedTransform.rotation.eulerAngles;
            
            ScaleCanvas();
        }

        private void Update()
        {
            ScaleCanvas();
            
            if (isBillboardCanvas)
            {
                // var targetForward = m_CameraTransform.forward;
                // m_CachedTransform.LookAt(m_CachedTransform.position + targetForward);

                var eulerAngle = m_CameraTransform.eulerAngles;
                eulerAngle.x = lockXRotation ? m_DefaultRotation.x : eulerAngle.x;
                eulerAngle.y = lockYRotation ? m_DefaultRotation.y : eulerAngle.y;
                eulerAngle.z = lockZRotation ? m_DefaultRotation.z : eulerAngle.z;
                
                m_CachedTransform.rotation = Quaternion.Euler(eulerAngle);
            }
        }

        private void ScaleCanvas()
        {
            if (!scaleWithOrthoSize)
            {
                return;
            }
            
            m_CachedTransform.localScale = baseScale / (Screen.height / (m_CurrentCamera.orthographicSize * 2));
        }
    }
}