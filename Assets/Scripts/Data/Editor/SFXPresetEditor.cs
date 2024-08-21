using UnityEngine;
using UnityEditor;

namespace Data
{
    [CustomEditor(typeof(SFXPreset))]
    public class SFXPresetEditor : Editor
    {
        private SFXPreset m_Target;

        private void OnEnable()
        {
            m_Target = (SFXPreset)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField("SFX Sources", EditorStyles.boldLabel);

            for (int i = 0; i < m_Target.sfx.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();

                m_Target.sfx[i] = (AudioClip)EditorGUILayout.ObjectField($"SFX Source {i + 1}", m_Target.sfx[i], typeof(AudioClip), true);

                if (GUILayout.Button("Remove", GUILayout.Width(60)))
                {
                    m_Target.sfx.RemoveAt(i);
                }

                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Add SFX Source"))
            {
                m_Target.sfx.Add(null);
            }

            EditorGUILayout.Space();
            m_Target.isRandomPitch = EditorGUILayout.Toggle("Is Random Pitch", m_Target.isRandomPitch);

            if (m_Target.isRandomPitch)
            {
                m_Target.minPitch = EditorGUILayout.Slider("Min Pitch", m_Target.minPitch, -3f, 3f);
                m_Target.maxPitch = EditorGUILayout.Slider("Max Pitch", m_Target.maxPitch, -3f, 3f);
            }

            if (GUI.changed)
            {
                EditorUtility.SetDirty(m_Target);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
