using UnityEditor;
using UnityEngine;

namespace Environments
{
    [CustomEditor(typeof(MapSector))]
    public class MapSectorEditor : Editor
    {
        private SerializedProperty m_Script;
        private SerializedProperty m_Guid;
        
        private SerializedProperty m_SectorSize;
        private SerializedProperty m_SectorAccessPoints;
        private SerializedProperty m_SectorExitPoints;

        private SerializedProperty m_ChildPathPrefab;
        private SerializedProperty m_SectorEventTriggerPrefab;
        private SerializedProperty m_MapSectorPath;

        private SerializedProperty m_SectorType;
        private SerializedProperty m_DesignatedExitPointCount;
        private SerializedProperty m_DistanceBetweenExitPoints;

        private SerializedProperty m_SpawnMap;

        private SerializedProperty m_CrossRoadStartPointDistance;
        private SerializedProperty m_CrossRoadSectionLength;
        private SerializedProperty m_BezierPathControlLength;

        private void OnEnable()
        {
            serializedObject.Update();
            
            m_Script = serializedObject.FindProperty("m_Script");
            m_Guid = serializedObject.FindProperty("guid");
            
            m_SectorSize = serializedObject.FindProperty("sectorSize");
            m_SectorAccessPoints = serializedObject.FindProperty("sectorAccessPoints");
            m_SectorExitPoints = serializedObject.FindProperty("sectorExitPoints");
            
            m_ChildPathPrefab = serializedObject.FindProperty("childPathPrefab");
            m_SectorEventTriggerPrefab = serializedObject.FindProperty("sectorEventTriggerPrefab");
            m_MapSectorPath = serializedObject.FindProperty("mapSectorPath");

            m_SpawnMap = serializedObject.FindProperty("spawnMap");
            
            m_CrossRoadStartPointDistance = serializedObject.FindProperty("crossRoadStartPointDistance");
            m_CrossRoadSectionLength = serializedObject.FindProperty("crossRoadSectionLength");
            m_BezierPathControlLength = serializedObject.FindProperty("bezierPathControlLength");
            
            m_SectorType = serializedObject.FindProperty("sectorType");
            m_DesignatedExitPointCount = serializedObject.FindProperty("designatedExitPointCount");
            m_DistanceBetweenExitPoints = serializedObject.FindProperty("distanceBetweenExitPoints");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(m_Script);
            EditorGUILayout.PropertyField(m_Guid);
            
            EditorGUILayout.PropertyField(m_SectorSize);
            EditorGUILayout.PropertyField(m_SectorAccessPoints);
            EditorGUILayout.PropertyField(m_SectorExitPoints);
            
            EditorGUILayout.PropertyField(m_ChildPathPrefab);
            EditorGUILayout.PropertyField(m_SectorEventTriggerPrefab);
            EditorGUILayout.PropertyField(m_MapSectorPath);
            
            EditorGUILayout.PropertyField(m_CrossRoadStartPointDistance);
            EditorGUILayout.PropertyField(m_CrossRoadSectionLength);
            EditorGUILayout.PropertyField(m_BezierPathControlLength);

            EditorGUILayout.PropertyField(m_SpawnMap);
            
            EditorGUILayout.PropertyField(m_SectorType);
            EditorGUILayout.PropertyField(m_DesignatedExitPointCount);
            EditorGUILayout.PropertyField(m_DistanceBetweenExitPoints);
            
            serializedObject.ApplyModifiedProperties();
        }

        [DrawGizmo(GizmoType.Active | GizmoType.Selected | GizmoType.InSelectionHierarchy)]
        private static void DrawSectorAccessPoints(MapSector src, GizmoType gizmoType)
        {
            var transform = src.transform;
            var gizmosScale = 0.5f;
            var offset = Vector3.up * 5f;
            
            var serializedObject = new SerializedObject(src);
            serializedObject.Update();
            
            var accessPoints = serializedObject.FindProperty("sectorAccessPoints");
            for (var i = 0; i < accessPoints.arraySize; i++)
            {
                var accessPoint = accessPoints.GetArrayElementAtIndex(i);
                var position = transform.TransformPoint(accessPoint.vector3Value);
                
                Gizmos.color = Color.green;
                Gizmos.DrawLine(position, position + offset);
                Gizmos.DrawSphere(position + offset, gizmosScale);
            }

            var exitPoints = serializedObject.FindProperty("sectorExitPoints");
            for (var i = 0; i < exitPoints.arraySize; i++)
            {
                var exitPoint = exitPoints.GetArrayElementAtIndex(i);
                var position = transform.TransformPoint(exitPoint.vector3Value);
                
                Gizmos.color = Color.red;
                Gizmos.DrawLine(position, position + offset);
                Gizmos.DrawSphere(position + offset, gizmosScale);
            }
        }

        [DrawGizmo(GizmoType.Active | GizmoType.Selected | GizmoType.InSelectionHierarchy)]
        private static void DrawSpawnMapGrid(MapSector src, GizmoType gizmoType)
        {
            var transform = src.transform;
            var spawnMap = src.SpawnMap;
            var cellGizmoSize = new Vector3(1f, 0, 1f) * (spawnMap.CellSize * 0.9f);

            for (var i = 0; i < spawnMap.Length; i++)
            {
                var point = transform.TransformPoint(spawnMap[i].position);
                Gizmos.DrawCube(point, cellGizmoSize);
            }
        }

        [DrawGizmo(GizmoType.Active | GizmoType.Selected | GizmoType.InSelectionHierarchy)]
        private static void DrawSpawnBounds(MapSector src, GizmoType gizmoType)
        {
            Gizmos.color = Color.red;
            var center = src.transform.TransformPoint(src.SpawnMap.Bounds.center);
            var size = src.transform.TransformVector(src.SpawnMap.Bounds.size);
            
            Gizmos.DrawWireCube(center, size);
        }
    }
}