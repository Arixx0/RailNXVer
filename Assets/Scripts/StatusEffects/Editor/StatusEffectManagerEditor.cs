using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace StatusEffects
{
    [CustomEditor(typeof(StatusEffectManager))]
    public class StatusEffectManagerEditor : Editor
    {
        private ScriptableObject m_EffectAsset;
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Add Status Effect"))
            {
                var effectAssetPath = EditorUtility.OpenFilePanel("Select Status Effect", "Assets/", "asset");
                if (string.IsNullOrEmpty(effectAssetPath))
                {
                    return;
                }
                
                effectAssetPath = effectAssetPath.Replace(Application.dataPath, "Assets");
                
                var effectAsset = AssetDatabase.LoadAssetAtPath<StatusEffect>(effectAssetPath);
                if (effectAsset == null)
                {
                    Debug.LogError("Selected asset is not a StatusEffect.");
                    return;
                }
                
                var manager = (StatusEffectManager)target;
                manager.AddStatusEffect(effectAsset);
            }

            EditorGUILayout.BeginHorizontal();
            m_EffectAsset = (ScriptableObject)EditorGUILayout.ObjectField(m_EffectAsset, typeof(ScriptableObject), false);
            GUI.enabled = m_EffectAsset != null && m_EffectAsset is StatusEffect;
            if (GUILayout.Button("Apply"))
            {
                var statusEffect = Instantiate(m_EffectAsset);
                
                var manager = (StatusEffectManager)target;
                manager.AddStatusEffect((StatusEffect)statusEffect);
                m_EffectAsset = null;
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}