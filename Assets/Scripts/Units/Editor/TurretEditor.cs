using Projectiles;
using Units.Turrets;
using UnityEditor;
using UnityEngine;

namespace Units.Turrets
{
    [CustomEditor(typeof(Turret))]
    public class TurretEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            Turret handler = (Turret)target;

            DrawDefaultInspectorExcept("minProjectileCount", "maxProjectileCount", "projectileRange",
                "gravity", "projectileLaunchAngle", "projectilePrefab", "projectileParticle", "projectileParticleHandler",
                "healthBar", "destructionCompositor", "navigationPathRequestInterval");

            if (handler.TurretType != TurretType.BasicRifle)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("projectileRange"), new GUIContent("Projectile Range"));
                if (handler.TurretType == TurretType.ShotGun)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("minProjectileCount"), new GUIContent("Min Projectile Count"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("maxProjectileCount"), new GUIContent("Max Projectile Count"));
                }
                else if (handler.TurretType == TurretType.Mortar)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("gravity"), new GUIContent("Gravity"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("projectileLaunchAngle"), new GUIContent("Projectile Launch Angle"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("projectilePrefab"), new GUIContent("Projectile Prefab"));
                }
            }
            if (handler.TurretType != TurretType.Mortar)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("projectileParticle"), new GUIContent("Projectile Particle"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("projectileParticleHandler"), new GUIContent("Projectile ParticleHandler"));
            }
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawDefaultInspectorExcept(params string[] excludedFields)
        {
            serializedObject.Update();
            SerializedProperty iterator = serializedObject.GetIterator();
            bool enterChildren = true;

            while (iterator.NextVisible(enterChildren))
            {
                enterChildren = false;
                if (excludedFields != null && System.Array.IndexOf(excludedFields, iterator.name) >= 0)
                    continue;

                EditorGUILayout.PropertyField(iterator, true);
            }
        }
    }
}