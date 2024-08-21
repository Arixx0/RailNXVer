using System;
using UnityEditor;
using UnityEngine;

namespace UI
{
    [CustomEditor(typeof(PassageSelectUI))]
    public class PassageSelectUIEditor : Editor
    {
        private Camera m_Camera;
        
        private PassageSelectUI Target { get; set; }

        private void OnEnable()
        {
            Target = (PassageSelectUI)target;

            m_Camera = FindObjectOfType<Camera>();
        }

        private void OnSceneGUI()
        {
            DrawCameraProjectionSimulation();
        }

        private void DrawCameraProjectionSimulation()
        {
            if (!Application.isPlaying)
            {
                return;
            }
            
            Vector3 bottomLeft, bottomRight, topLeft, topRight;
            RaycastHit hit;

            var ray = m_Camera.ScreenPointToRay(new Vector3(Screen.currentResolution.width * 0.5f, Screen.currentResolution.height * 0.5f));
            if (Physics.Raycast(ray, out hit, 10000f, LayerMask.GetMask("Ground")))
            {
                Handles.color = Color.red;
                Handles.DrawWireDisc(hit.point, Vector3.up,
                    m_Camera.orthographicSize * 2f * Mathf.Sin(m_Camera.transform.rotation.eulerAngles.x * Mathf.Deg2Rad));
            }
            
            ray = m_Camera.ScreenPointToRay(Vector3.zero);
            if (Physics.Raycast(ray, out hit, 10000f, LayerMask.GetMask("Ground")))
            {
                bottomLeft = hit.point;
            }
            else
            {
                return;
            }

            ray = m_Camera.ScreenPointToRay(new Vector3(Screen.currentResolution.width, Screen.currentResolution.height));
            if (Physics.Raycast(ray, out hit, 10000f, LayerMask.GetMask("Ground")))
            {
                topRight = hit.point;
            }
            else
            {
                return;
            }

            ray = m_Camera.ScreenPointToRay(new Vector3(0, Screen.currentResolution.height));
            if (Physics.Raycast(ray, out hit, 10000f, LayerMask.GetMask("Ground")))
            {
                topLeft = hit.point;
            }
            else
            {
                return;
            }
            
            ray = m_Camera.ScreenPointToRay(new Vector3(Screen.currentResolution.width, 0));
            if (Physics.Raycast(ray, out hit, 10000f, LayerMask.GetMask("Ground")))
            {
                bottomRight = hit.point;
            }
            else
            {
                return;
            }
            
            Handles.color = Color.yellow;
            Handles.DrawLine(bottomLeft, bottomRight, 3f);
            Handles.DrawLine(bottomLeft, topLeft, 3f);
            Handles.DrawLine(topRight, topLeft, 3f);
            Handles.DrawLine(topRight, bottomRight, 3f);
        }
    }
}