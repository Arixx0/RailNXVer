// ReSharper disable CheckNamespace

using UnityEditor;
using UnityEngine;

namespace Units.Stats
{
    public class StatModifiersViewerWindow : EditorWindow
    {
        private SerializedProperty m_Modifiers;

        private void OnGUI()
        {
            if (m_Modifiers == null)
            {
                Close();
                return;
            }
            
            EditorGUILayout.PropertyField(m_Modifiers);
        }
    }
    
    [CustomEditor(typeof(UnitStatComponent))]
    public class UnitStatComponentEditor : Editor
    {
        private UnitStatComponent Target { get; set; }
        
        private void OnEnable()
        {
            Target = (UnitStatComponent)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            if (GUILayout.Button("Set Default Stats"))
            {
                if (Target.TryGetComponent(out TrainScripts.Car car))
                {
                    Data.DatabaseDefinitions.GetOrCreate().Load();
                    
                    var identifier = car.IdentifierData.GetIdentifier(Target.UpgradeLevel);
                    if (!Data.Database.UnitStatData.TryGetValue(identifier, out var data))
                    {
                        return;
                    }
                    
                    Target.Set(data);
                    EditorUtility.SetDirty(Target);
                    return;
                }
                
                if (Target.TryGetComponent(out Enemies.EnemyUnit enemyUnit))
                {
                    Data.DatabaseDefinitions.GetOrCreate().Load();
                    
                    var identifier = enemyUnit.EnemyUnitIdentifier.Identifier;
                    if (!Data.Database.UnitStatData.TryGetValue(identifier, out var data))
                    {
                        return;
                    }
                    
                    Target.Set(data);
                    EditorUtility.SetDirty(Target);
                    return;
                }

                if (Target.TryGetComponent(out Enemies.Barricade barricade))
                {
                    Data.DatabaseDefinitions.GetOrCreate().Load();
                    
                    var identifier = barricade.Identifier.Identifier;
                    if (!Data.Database.UnitStatData.TryGetValue(identifier, out var data))
                    {
                        return;
                    }
                    
                    Target.Set(data);
                    EditorUtility.SetDirty(Target);
                    return;
                }
                
                Debug.LogError("There was no Identifier Provider Component.");
            }

            if (GUILayout.Button("Remove All Modifiers"))
            {
                Target.RemoveAllModifiers();
            }
        }
    }
}