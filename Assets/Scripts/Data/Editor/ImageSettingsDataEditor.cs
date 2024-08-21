using UnityEditor;
using UnityEngine;

namespace Data
{
    [CustomEditor(typeof(ImageSettingsData))]
    public class ImageSettingsDataEditor : Editor
    {
        private SerializedProperty imageCommonCategory;
        private SerializedProperty imageSettings;

        private void OnEnable()
        {
            imageCommonCategory = serializedObject.FindProperty("imageCommonCategory");
            imageSettings = serializedObject.FindProperty("imageSettings");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(imageCommonCategory, new GUIContent("Image Common Category"));

            EditorGUILayout.LabelField("Image Settings", EditorStyles.boldLabel);
            for (int i = 0; i < imageSettings.arraySize; i++)
            {
                SerializedProperty majorCategory = imageSettings.GetArrayElementAtIndex(i);
                SerializedProperty categoryName = majorCategory.FindPropertyRelative("categoryName");
                SerializedProperty imageData = majorCategory.FindPropertyRelative("imageData");

                majorCategory.isExpanded = EditorGUILayout.Foldout(majorCategory.isExpanded, categoryName.stringValue);
                if (majorCategory.isExpanded)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(categoryName, new GUIContent("Major Category Name"));

                    EditorGUILayout.LabelField("Image Data", EditorStyles.boldLabel);
                    for (int j = 0; j < imageData.arraySize; j++)
                    {
                        SerializedProperty data = imageData.GetArrayElementAtIndex(j);
                        SerializedProperty name = data.FindPropertyRelative("name");
                        SerializedProperty identifier = data.FindPropertyRelative("Identifier");
                        SerializedProperty sprite = data.FindPropertyRelative("sprite");

                        data.isExpanded = EditorGUILayout.Foldout(data.isExpanded, name.stringValue);
                        if (data.isExpanded)
                        {
                            EditorGUI.indentLevel++;

                            EditorGUILayout.PropertyField(name, new GUIContent("Name"));
                            EditorGUILayout.PropertyField(sprite, new GUIContent("Sprite"));

                            SerializedProperty category = identifier.FindPropertyRelative("category");
                            SerializedProperty objectType = identifier.FindPropertyRelative("objectType");
                            SerializedProperty subType = identifier.FindPropertyRelative("subType");

                            EditorGUILayout.PropertyField(category, new GUIContent("Category"));
                            EditorGUILayout.PropertyField(objectType, new GUIContent("Object Type"));
                            EditorGUILayout.PropertyField(subType, new GUIContent("Sub Type"));

                            EditorGUI.indentLevel--;
                        }
                    }

                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Add Image Data"))
                    {
                        imageData.InsertArrayElementAtIndex(imageData.arraySize);
                    }
                    if (GUILayout.Button("Remove Last Image Data"))
                    {
                        if (imageData.arraySize > 0)
                            imageData.DeleteArrayElementAtIndex(imageData.arraySize - 1);
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUI.indentLevel--;
                }
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Category"))
            {
                imageSettings.InsertArrayElementAtIndex(imageSettings.arraySize);
            }
            if (GUILayout.Button("Remove Last Category"))
            {
                if (imageSettings.arraySize > 0)
                    imageSettings.DeleteArrayElementAtIndex(imageSettings.arraySize - 1);
            }
            EditorGUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }
    }
}