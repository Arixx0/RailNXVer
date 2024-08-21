using Environments;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Data
{
    [CustomEditor(typeof(MobWavePreset))]
    public class MobWavePresetEditor : UnityEditor.Editor
    {
        private MobWavePreset m_Target;
        
        private Camera m_SceneViewCamera;
        
        private ReorderableList m_SpawnProfileList;
        
        private bool m_ForceDisablePreview;
        
        private void OnEnable()
        {
            m_Target = (MobWavePreset)target;

            var sceneView = SceneView.lastActiveSceneView;
            m_ForceDisablePreview = sceneView == null;

            if (sceneView != null)
            {
                m_SceneViewCamera = sceneView.camera;
                m_ForceDisablePreview = m_SceneViewCamera == null;
            }

            m_SpawnProfileList = new ReorderableList(m_Target.spawnProfiles, typeof(MobSpawnProfile),
                false, true, true, true);
            m_SpawnProfileList.index = -1;
            
            m_SpawnProfileList.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Spawn Profiles");

            m_SpawnProfileList.drawElementCallback = (rect, index, active, focused) =>
            {
                var profile = (MobSpawnProfile)m_SpawnProfileList.list[index];
            
                var didChanged = false;
            
                EditorGUI.BeginChangeCheck();
                var prefabRect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
                profile.prefab = (GameObject)EditorGUI.ObjectField(prefabRect, "Prefab", profile.prefab, typeof(GameObject), false);
                didChanged |= EditorGUI.EndChangeCheck();
            
                EditorGUI.BeginChangeCheck();
                var positionRect = new Rect(rect.x, prefabRect.yMax + 2, rect.width, EditorGUIUtility.singleLineHeight);
                profile.localPosition = EditorGUI.Vector3Field(positionRect, "Position", profile.localPosition);
                didChanged |= EditorGUI.EndChangeCheck();
            
                if (didChanged)
                {
                    m_SpawnProfileList.list[index] = profile;
                    EditorUtility.SetDirty(target);
                }
            };

            m_SpawnProfileList.onAddCallback = list =>
            {
                list.list.Add(new MobSpawnProfile());
            };
            
            m_SpawnProfileList.onSelectCallback = list =>
            {
                SceneView.RepaintAll();
            };

            m_SpawnProfileList.elementHeightCallback = index =>
            {
                var elementCount = 2;

                if (Screen.width <= 331f)
                {
                    elementCount += 1;
                }
                
                return (EditorGUIUtility.singleLineHeight + 2) * elementCount;
            };
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            m_SpawnProfileList.DoLayoutList();
        }

        /// <returns>TRUE if any positions changed</returns>
        public bool PreviewSpawnPoints(Transform baseTransform)
        {
            if (m_ForceDisablePreview)
            {
                return false;
            }
            
            if (m_SpawnProfileList == null || m_SpawnProfileList.list.Count <= 0)
            {
                return false;
            }

            var handleSize = 1f;
            var snapScale = Vector3.one * handleSize;
            var selectedProfileIndex = m_SpawnProfileList.index;
            var didChanged = false;
            
            for (var i = 0; i < m_SpawnProfileList.list.Count; ++i)
            {
                var profile = (MobSpawnProfile)m_SpawnProfileList.list[i];
                var position = baseTransform.TransformPoint(profile.localPosition);

                Handles.color = Color.blue;
                Handles.DrawWireCube(position, snapScale);
                if (i != selectedProfileIndex)
                {
                    continue;
                }
                
                var newPos = Handles.PositionHandle(position, Quaternion.identity);
                if ((newPos - position).magnitude <= 0.0001f)
                {
                    continue;
                }

                didChanged = true;
                Undo.RecordObject(target, "Changed Spawn Points");
                
                profile.localPosition = baseTransform.InverseTransformPoint(newPos);
                m_SpawnProfileList.list[i] = profile;
            }

            return didChanged;
        }
    }
}