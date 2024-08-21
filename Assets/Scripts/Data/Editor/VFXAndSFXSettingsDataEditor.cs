using UnityEditor;
using UnityEngine;

namespace Data
{
    [CustomEditor(typeof(VFXAndSFXSettingsData))]
    public class VFXAndSFXSettingsDataEditor : Editor
    {
        private SerializedProperty VFXCommonCategory;
        private SerializedProperty SFXCommonCategory;
        private SerializedProperty VFXAndSFXSettings;

        private void OnEnable()
        {
            VFXCommonCategory = serializedObject.FindProperty("VFXCommonCategory");
            SFXCommonCategory = serializedObject.FindProperty("SFXCommonCategory");
            VFXAndSFXSettings = serializedObject.FindProperty("VFXAndSFXSettings");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(VFXCommonCategory, new GUIContent("VFX 공통 카테고리"));
            EditorGUILayout.PropertyField(SFXCommonCategory, new GUIContent("SFX 공통 카테고리"));

            EditorGUILayout.LabelField("VFX And SFX 세팅", EditorStyles.boldLabel);
            for (int i = 0; i < VFXAndSFXSettings.arraySize; i++)
            {
                SerializedProperty majorCategory = VFXAndSFXSettings.GetArrayElementAtIndex(i);
                SerializedProperty categoryName = majorCategory.FindPropertyRelative("categoryName");
                //SerializedProperty imageData = majorCategory.FindPropertyRelative("imageData");
                SerializedProperty SFXData = majorCategory.FindPropertyRelative("SFXData");
                SerializedProperty VFXData = majorCategory.FindPropertyRelative("VFXData");

                majorCategory.isExpanded = EditorGUILayout.Foldout(majorCategory.isExpanded, categoryName.stringValue);
                if (majorCategory.isExpanded)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(categoryName, new GUIContent("Major Category Name"));

                    EditorGUILayout.LabelField("SFX Datas", EditorStyles.boldLabel);

                    for (int j = 0; j < SFXData.arraySize; j++)
                    {
                        SerializedProperty data = SFXData.GetArrayElementAtIndex(j);
                        SerializedProperty name = data.FindPropertyRelative("name");
                        SerializedProperty identifier = data.FindPropertyRelative("Identifier");
                        SerializedProperty sfxSource = data.FindPropertyRelative("sfxSource");

                        data.isExpanded = EditorGUILayout.Foldout(data.isExpanded, name.stringValue);
                        if (data.isExpanded)
                        {
                            EditorGUI.indentLevel++;

                            EditorGUILayout.PropertyField(name, new GUIContent("Name"));
                            EditorGUILayout.PropertyField(sfxSource, new GUIContent("SFX Source"));

                            SerializedProperty objectType = identifier.FindPropertyRelative("objectType");
                            SerializedProperty subType = identifier.FindPropertyRelative("subType");

                            EditorGUILayout.PropertyField(objectType, new GUIContent("Object Type"));
                            EditorGUILayout.PropertyField(subType, new GUIContent("Sub Type"));

                            EditorGUI.indentLevel--;
                        }
                    }

                    EditorGUILayout.LabelField("VFX Datas", EditorStyles.boldLabel);

                    for (int j = 0; j < VFXData.arraySize; j++)
                    {
                        SerializedProperty data = VFXData.GetArrayElementAtIndex(j);
                        SerializedProperty name = data.FindPropertyRelative("name");
                        SerializedProperty identifier = data.FindPropertyRelative("Identifier");
                        SerializedProperty vfxSource = data.FindPropertyRelative("vfxSource");

                        data.isExpanded = EditorGUILayout.Foldout(data.isExpanded, name.stringValue);
                        if (data.isExpanded)
                        {
                            EditorGUI.indentLevel++;

                            EditorGUILayout.PropertyField(name, new GUIContent("Name"));
                            EditorGUILayout.PropertyField(vfxSource, new GUIContent("VFX Source"));

                            SerializedProperty objectType = identifier.FindPropertyRelative("objectType");
                            SerializedProperty subType = identifier.FindPropertyRelative("subType");

                            EditorGUILayout.PropertyField(objectType, new GUIContent("Object Type"));
                            EditorGUILayout.PropertyField(subType, new GUIContent("Sub Type"));

                            EditorGUI.indentLevel--;
                        }
                    }

                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Add SFX Data"))
                    {
                        SFXData.InsertArrayElementAtIndex(SFXData.arraySize);
                    }
                    if (GUILayout.Button("Remove Last SFX Data"))
                    {
                        if (SFXData.arraySize > 0)
                            SFXData.DeleteArrayElementAtIndex(SFXData.arraySize - 1);
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Add VFX Data"))
                    {
                        VFXData.InsertArrayElementAtIndex(VFXData.arraySize);
                    }
                    if (GUILayout.Button("Remove Last VFX Data"))
                    {
                        if (VFXData.arraySize > 0)
                            VFXData.DeleteArrayElementAtIndex(VFXData.arraySize - 1);
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUI.indentLevel--;
                }
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Category"))
            {
                VFXAndSFXSettings.InsertArrayElementAtIndex(VFXAndSFXSettings.arraySize);
            }
            if (GUILayout.Button("Remove Last Category"))
            {
                if (VFXAndSFXSettings.arraySize > 0)
                    VFXAndSFXSettings.DeleteArrayElementAtIndex(VFXAndSFXSettings.arraySize - 1);
            }
            EditorGUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }
    }
}