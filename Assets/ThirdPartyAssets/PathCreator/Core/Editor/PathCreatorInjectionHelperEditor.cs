using PathCreation;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace PathCreationEditor
{
    [CustomEditor(typeof(PathCreatorInjectionHelper))]
    public class PathCreatorInjectionHelperEditor : Editor
    {
        private PathCreatorInjectionHelper m_Helper;
        private ReorderableList m_WorldPointsList;

        private void OnEnable()
        {
            m_Helper = (PathCreatorInjectionHelper)target;
            
            m_WorldPointsList = new ReorderableList(m_Helper.worldPoints, typeof(Transform),
                true, true, true, true);
            
            m_WorldPointsList.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "World Points");

            m_WorldPointsList.drawElementCallback = (rect, index, active, focused) =>
            {
                var valueRect = new Rect(rect.x, rect.y + 2, rect.width, EditorGUIUtility.singleLineHeight);
                var value = (Vector3)m_WorldPointsList.list[index];

                m_WorldPointsList.list[index] = EditorGUI.Vector3Field(valueRect, GUIContent.none, value);
            };

            m_WorldPointsList.onAddCallback += list =>
            {
                list.list.Add(list.list.Count <= 0 ? m_Helper.transform.position : m_Helper.worldPoints[^1]);
            };

            m_WorldPointsList.elementHeightCallback = index =>
            {
                return EditorGUIUtility.singleLineHeight + 4;
            };
            
            m_WorldPointsList.onSelectCallback += list =>
            {
                if (m_WorldPointsList.index == -1)
                {
                    return;
                }
                
                SceneView.lastActiveSceneView.Repaint();
            };

            m_WorldPointsList.index = -1;
        }

        private void OnSceneGUI()
        {
            if (m_WorldPointsList == null)
            {
                return;
            }
            
            var hit = new RaycastHit();
            var ray = new Ray();
            var maxDist = 100f;
            var groundMask = m_Helper.groundMask;
            var triggerInteraction = QueryTriggerInteraction.Ignore;
            
            for (var i = 0; i < m_WorldPointsList.list.Count; i += 1)
            {
                if (i == m_WorldPointsList.index)
                {
                    continue;
                }
                
                ray.origin = (Vector3)m_WorldPointsList.list[i];
                ray.direction = Vector3.down;
                var isValidPoint = PathCreatorInjectionHelper.Raycast(ref ray, out hit, maxDist, groundMask, triggerInteraction);
                
                Handles.color = isValidPoint ? Color.red : Color.magenta;
                Handles.DrawWireCube(ray.origin + ray.direction * hit.distance, Vector3.one);
            }

            // draw current selected point gizmos
            if (m_WorldPointsList.index != -1 && m_WorldPointsList.list.Count > 0)
            {
                ray.origin = (Vector3)m_WorldPointsList.list[m_WorldPointsList.index];
                ray.direction = Vector3.down;
                var isValidPoint = PathCreatorInjectionHelper.Raycast(ref ray, out hit, maxDist, groundMask, triggerInteraction);
                
                Handles.color = isValidPoint ? Color.green : Color.yellow;
                
                Handles.DrawWireCube(ray.origin + ray.direction * hit.distance, Vector3.one);
            }

            // draw current selected point position handle
            if (m_WorldPointsList.index != -1 && m_WorldPointsList.list.Count > 0)
            {
                var position = (Vector3)m_WorldPointsList.list[m_WorldPointsList.index];
                m_WorldPointsList.list[m_WorldPointsList.index] = Handles.DoPositionHandle(position, Quaternion.identity);
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            m_WorldPointsList.DoLayoutList();
            
            if (GUILayout.Button("Calculate Path"))
            {
                m_Helper.CalculatePath();
            }
            
            if (GUILayout.Button("Transform Points to Local"))
            {
                m_Helper.TransformPointsToLocal();
            }
            
            if (GUILayout.Button("Transform Points to World"))
            {
                m_Helper.TransformPointsToWorld();
            }

            if (GUILayout.Button("Sync Consecutive Path Start Point with End Point"))
            {
                m_Helper.ForceSyncConsecutivePathStartPoint();
            }
        }
    }
}