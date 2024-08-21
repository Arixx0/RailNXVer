using Environments;
using UnityEditor;
using UnityEngine;

namespace Manager
{
    [CustomEditor(typeof(MockupGameModeManager))]
    public class MockupGameModeManagerEditor : Editor
    {
        private MockupGameModeManager m_Target;
        private GameObject m_MapSectorPrefab;
        
        private void OnEnable()
        {
            m_Target = (MockupGameModeManager)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();
            m_MapSectorPrefab = (GameObject)EditorGUILayout.ObjectField(
                "Replacement MapSector Prefab", m_MapSectorPrefab, typeof(GameObject), false);
            if (GUILayout.Button("Replace Map Sector"))
            {
                m_MapSectorPrefab.TryGetComponent(out MapSector mapSector);
                ReplaceMapSector(mapSector);
            }
        }
        
        private void ReplaceMapSector(MapSector mapSector)
        {
            if (!Application.isPlaying || m_MapSectorPrefab == null)
            {
                return;
            }

            var prevSector = m_Target.mapSector;
            
            var instance = Instantiate(mapSector, Vector3.zero, Quaternion.identity, m_Target.root);
            m_Target.mapSector = instance;
            
            Destroy(prevSector.gameObject);
            
            m_Target.Setup();
        }
    }
}