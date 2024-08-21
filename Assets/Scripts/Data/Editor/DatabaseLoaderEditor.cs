using UnityEditor;
using UnityEngine;

namespace Data
{
    [CustomPropertyDrawer(typeof(DatabaseLoader.DataAssetDefinition))]
    public class DataAssetDefinitionPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, GUIContent.none, property);
            
            var assetPathProperty = property.FindPropertyRelative("assetPath");
            var containerNameProperty = property.FindPropertyRelative("containerName");
            
            var assetPathRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            var containerNameRect = new Rect(position.x, assetPathRect.yMax + 4, position.width, EditorGUIUtility.singleLineHeight);
            
            EditorGUI.PropertyField(assetPathRect, assetPathProperty, new GUIContent(assetPathProperty.displayName));
            EditorGUI.PropertyField(containerNameRect, containerNameProperty, new GUIContent(containerNameProperty.displayName));
            
            EditorGUI.EndProperty();
        }
        
        [MenuItem("GameObject/Train Craft/Database Loader")]
        private static void CreateDatabaseLoaderObject()
        {
            if (Object.FindObjectOfType<DatabaseLoader>() != null)
            {
                return;
            }
            
            var databaseLoaderObject = new GameObject("DatabaseLoader");
            databaseLoaderObject.AddComponent<DatabaseLoader>();
        }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 2 + 4;
        }
    }
}