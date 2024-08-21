using System.Collections.Generic;
using Augments;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace TrainScripts
{
    [CustomEditor(typeof(Train))]
    public class TrainEditor : Editor
    {
        private Train Target { get; set; }

        private void OnEnable()
        {
            Target = (Train)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            EditorGUILayout.Space();
            if (GUILayout.Button("Select and Add Augment"))
            {
                var augmentPath = EditorUtility.OpenFilePanel("Select Augment", "Assets/Resources/", "asset");
                augmentPath = "Assets/" + augmentPath.TrimStart(Application.dataPath.ToCharArray());
                
                var selectedAsset = AssetDatabase.LoadAssetAtPath<Augment>(augmentPath);
                if (selectedAsset != null)
                {
                    Target.AddAugment(selectedAsset);
                }
            }
        }
        
        [MenuItem("GameObject/Train Craft/Default Train (Editor Only)")]
        private static void CreateEditorDefaultGameObject()
        {
            const string defaultPrefabPath = "Assets/Editor Default Resources/prefab_train-default.prefab";

            var asset = (GameObject)EditorGUIUtility.Load(defaultPrefabPath);
            if (asset == null || !asset.TryGetComponent(out Train _))
            {
                throw new System.Exception("Default prefab asset is missing or Prefab is not an object of type Train.");
            }

            var trainObject = Instantiate(asset, null, true);
            trainObject.name = nameof(Train);
        }
    }
}