using Cinemachine;
using Data;
using System.Linq;
using UnityEngine;

namespace CameraUtility
{
    public class CameraController : MonoBehaviour
    {
        [Header("Component References"), SerializeField]
        private CinemachineVirtualCamera targetVirtualCamera;
        
        [Header("Camera Composition Options"), SerializeField]
        private Transform orientationTransform;
        
        [SerializeField]
        private Transform followTarget;
        
        [SerializeField, Range(0, 1)]
        private float followAcceleration;
        
        [SerializeField]
        private float defaultOrthoSize;
        
        [SerializeField]
        private Range zoomRange;
        
        [SerializeField]
        private float rotateSpeed;

        [SerializeField]
        private float rotateSmoothTime;

        [SerializeField]
        private Quaternion defaultOrientation;
        
        [SerializeField]
        private float zoomSpeed;

        [SerializeField]
        private float moveSpeed;

        [SerializeField]
        private float armLength;

        private float m_ZoomVelocity;

        private float m_CurrentZoomVelocity;
        
        private float m_RotateVelocity;

        private float m_CurrentRotateVelocity;
        
        private bool m_EnableFollowTarget;

        private Transform m_CachedTransform;
        
        private Camera m_Camera;
        private Camera[] m_OverlayCameras;
        private Vector3 m_MoveOffset;

        private Vector3 m_MoveVelocity;

        private Vector3 m_CurrentMoveVelocity;
        
        [Header("Audio Listener Options"), SerializeField]
        private Transform globalAudioListenerTransform;
        
        [SerializeField]
        private Range audioListenerOffsetRange;

        public Transform CachedTransform
        {
            get
            {
#if UNITY_EDITOR
                if (m_CachedTransform == null)
                {
                    m_CachedTransform = null;
                }
#endif
                
                return (m_CachedTransform ??= GetComponent<Transform>());
            }
        }

        public Transform Orientantion => orientationTransform;
        
        public Transform FollowTarget
        {
            get => followTarget;
            set
            {
                followTarget = value;
                m_EnableFollowTarget = followTarget != null;
                m_MoveOffset = Vector3.zero;
                m_CurrentMoveVelocity = Vector3.zero;
            }
        }
        
        public Transform GlobalAudioListener => globalAudioListenerTransform;

        public Range ZoomScale => zoomRange;

        public float ArmLength => armLength;

        public Vector3 MoveVelocity => m_CurrentMoveVelocity;
        
        private void OnEnable()
        {
            if (!TryGetComponent(out m_Camera))
            {
                Debug.LogError($"{nameof(Camera)} is missing from {gameObject.name}. {nameof(CameraController)} has dependency on {nameof(Camera)}", gameObject);
            }

            if (m_Camera != null)
            {
                var overlayCameras = GetComponentsInChildren<Camera>(false);

                m_OverlayCameras = overlayCameras.Where(camera => camera != m_Camera).ToArray();
            }

            m_EnableFollowTarget = followTarget != null;

            globalAudioListenerTransform = globalAudioListenerTransform == null ? null : globalAudioListenerTransform;

            orientationTransform.rotation = defaultOrientation;
        }

        private void Update()
        {
            var deltaTime = Time.unscaledDeltaTime;
            
            var targetMoveVelocity = m_MoveVelocity * moveSpeed;
            m_CurrentMoveVelocity = Vector3.Lerp(m_CurrentMoveVelocity, targetMoveVelocity, followAcceleration);
            m_MoveOffset += orientationTransform.TransformDirection(m_CurrentMoveVelocity) * deltaTime;
            
            var targetPosition = m_MoveOffset;
            if (m_EnableFollowTarget)
            {
                targetPosition += followTarget.position;
            }

            orientationTransform.position = Vector3.Lerp(orientationTransform.position, targetPosition, followAcceleration);

            var zoomVelocity = m_ZoomVelocity * zoomSpeed;
            m_CurrentZoomVelocity = Mathf.Lerp(m_CurrentZoomVelocity, zoomVelocity, 0.15f);
            m_Camera.orthographicSize = zoomRange.Clamp(m_Camera.orthographicSize + m_CurrentZoomVelocity * deltaTime);
            foreach (var camera in m_OverlayCameras)
            {
                camera.orthographicSize = zoomRange.Clamp(m_Camera.orthographicSize + m_CurrentZoomVelocity * deltaTime);
            }

            var rotateVelocity = m_RotateVelocity * rotateSpeed;
            m_CurrentRotateVelocity = Mathf.Lerp(m_CurrentRotateVelocity, rotateVelocity, 0.15f);
            orientationTransform.Rotate(Vector3.up, m_CurrentRotateVelocity * deltaTime);
            
            if (globalAudioListenerTransform is not null)
            {
                var audioListenerOffset = audioListenerOffsetRange.GetNormalizedValue(m_Camera.orthographicSize);
                globalAudioListenerTransform.localPosition = Vector3.up * audioListenerOffsetRange.FromNormalizedValue(audioListenerOffset);
            }
        }

        private void OnValidate()
        {
            if (Application.isPlaying)
            {
                return;
            }
            
            var camera = GetComponentInChildren<Camera>();
            if (camera != null)
            {
                camera.orthographicSize = defaultOrthoSize;
            }

            // armLength = Mathf.Tan((90 - CachedTransform.localRotation.eulerAngles.x) * Mathf.Deg2Rad) * zoomRange.max + 10;
            CachedTransform.position = orientationTransform.position + CachedTransform.forward * -armLength;

            if (orientationTransform != null)
            {
                var fixedRotation = defaultOrientation.eulerAngles;
                fixedRotation.x = 0;
                fixedRotation.z = 0;
                defaultOrientation = Quaternion.Euler(fixedRotation);
                
                orientationTransform.rotation = defaultOrientation;
            }
        }
        
        public void SetZoomVelocity(float value)
        {
            m_ZoomVelocity = value;
        }

        public void SetRotateVelocity(float value)
        {
            m_RotateVelocity = value;
        }

        public void SetMoveVelocity(Vector2 value)
        {
            m_MoveVelocity = new Vector3(value.x, 0, value.y);
        }

        public void ResetMoveOffset()
        {
            m_MoveOffset = Vector3.zero;
            m_MoveVelocity = Vector3.zero;
            m_CurrentMoveVelocity = Vector3.zero;
        }

        public void ResetOrientation()
        {
            orientationTransform.rotation = defaultOrientation;
            m_RotateVelocity = 0f;
            m_MoveVelocity = Vector3.zero;
            m_CurrentMoveVelocity = Vector3.zero;
        }
    }
}