using Data;
using UnityEditor;
using UnityEngine;

namespace Environments
{
    [CustomEditor(typeof(MobSpawner))]
    public class MobSpawnerEditor : Editor
    {
        private const string ENABLE_PREVIEW_KEY = "MobSpawnerEditor.EnablePreview";
        
        private const string ENABLE_TRANSFORM_HANDLE_KEY = "MobSpawnerEditor.EnableTransformHandle";
        
        private bool m_EnablePreview;
        
        private bool m_EnableTransformHandle;
        
        private bool m_EnableTransformHandleFromProject;
        
        private SerializedProperty m_MobWavePresetProperty;
        
        private MobWavePreset m_TargetMobWavePreset;
        
        private Editor m_PrimitiveMobWavePresetEditor;
        
        private MobWavePresetEditor m_MobWavePresetEditor;
        
        private bool m_FoldoutMobWavePresetInspector;
        
        private MobSpawner Target { get; set; }

        private void OnEnable()
        {
            Target = (MobSpawner)target;
            Debug.Assert(Target != null);
            
            serializedObject.Update();
            
            m_MobWavePresetProperty = serializedObject.FindProperty("mobWavePreset");
            if (m_MobWavePresetProperty.objectReferenceValue != null)
            {
                m_TargetMobWavePreset = (MobWavePreset)m_MobWavePresetProperty.objectReferenceValue;
                CreateCachedEditor(m_TargetMobWavePreset, typeof(MobWavePresetEditor), ref m_PrimitiveMobWavePresetEditor);
                m_MobWavePresetEditor = (MobWavePresetEditor)m_PrimitiveMobWavePresetEditor;
            }

            m_EnablePreview = EditorPrefs.GetBool(ENABLE_PREVIEW_KEY, false);

            m_EnableTransformHandleFromProject = Tools.hidden;
            m_EnableTransformHandle = EditorPrefs.GetBool(ENABLE_TRANSFORM_HANDLE_KEY, true);
            Tools.hidden = !m_EnableTransformHandle;
            SceneView.RepaintAll();
        }

        private void OnDestroy()
        {
            EditorPrefs.SetBool(ENABLE_PREVIEW_KEY, m_EnablePreview);
            
            EditorPrefs.SetBool(ENABLE_TRANSFORM_HANDLE_KEY, m_EnableTransformHandle);
            Tools.hidden = m_EnableTransformHandleFromProject;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (m_MobWavePresetProperty.objectReferenceValue != null ||
                m_MobWavePresetProperty.objectReferenceValue != m_TargetMobWavePreset)
            {
                m_TargetMobWavePreset = (MobWavePreset)m_MobWavePresetProperty.objectReferenceValue;
                CreateCachedEditor(m_TargetMobWavePreset, typeof(MobWavePresetEditor), ref m_PrimitiveMobWavePresetEditor);
                m_MobWavePresetEditor = (MobWavePresetEditor)m_PrimitiveMobWavePresetEditor;
            }
            
            EditorGUI.BeginChangeCheck();
            m_EnablePreview = EditorGUILayout.Toggle("Enable Preview", m_EnablePreview);
            if (EditorGUI.EndChangeCheck() && m_EnablePreview)
            {
                EditorPrefs.SetBool(ENABLE_PREVIEW_KEY, m_EnablePreview);
                SceneView.RepaintAll();
            }
            
            EditorGUI.BeginChangeCheck();
            m_EnableTransformHandle = EditorGUILayout.Toggle("Enable Transform Handle", m_EnableTransformHandle);
            if (EditorGUI.EndChangeCheck())
            {
                Tools.hidden = !m_EnableTransformHandle;
                EditorPrefs.SetBool(ENABLE_TRANSFORM_HANDLE_KEY, m_EnableTransformHandle);
                SceneView.RepaintAll();
            }

            if (EditorApplication.isPlaying)
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Force Spawn", EditorStyles.miniButtonLeft))
                {
                    var preset = m_MobWavePresetProperty.objectReferenceValue == null
                        ? null
                        : (MobWavePreset)m_MobWavePresetProperty.objectReferenceValue;

                    if (preset != null)
                    {
                        Target.Spawn(preset);
                    }
                }

                if (GUILayout.Button("Despawn All", EditorStyles.miniButtonRight))
                {
                    Target.DestroySpawnedMobs();
                }
                EditorGUILayout.EndHorizontal();
            }

            if (m_MobWavePresetEditor != null)
            {
                m_FoldoutMobWavePresetInspector =
                    EditorGUILayout.InspectorTitlebar(m_FoldoutMobWavePresetInspector, m_MobWavePresetEditor);
                if (m_FoldoutMobWavePresetInspector)
                {
                    m_MobWavePresetEditor.OnInspectorGUI();
                }
            }
        }

        private void OnSceneGUI()
        {
            if (m_MobWavePresetEditor != null && m_EnablePreview)
            {
                if (m_MobWavePresetEditor.PreviewSpawnPoints(Target.transform))
                {
                    EditorUtility.SetDirty(m_MobWavePresetEditor.target);
                }
            }
        }
    }
}