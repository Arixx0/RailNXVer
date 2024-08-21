using UnityEngine;
using UnityEditor;

namespace Projectiles
{
    [CustomEditor(typeof(ProjectileParticleHandler))]
    public class ProjectileParticleHandlerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            ProjectileParticleHandler handler = (ProjectileParticleHandler)target;

            DrawDefaultInspectorExcept("minProjectileCount", "maxProjectileCount", "projectileRange");

            if (handler.ProjectileType == ProjectileType.ShotGun)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("minProjectileCount"), new GUIContent("Min Projectile Count"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("maxProjectileCount"), new GUIContent("Max Projectile Count"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("projectileRange"), new GUIContent("Projectile Range"));
            }
            else if (handler.ProjectileType == ProjectileType.FlameThrower)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("projectileRange"), new GUIContent("Projectile Range"));
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
