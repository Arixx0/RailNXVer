using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Manager;
using UnityEngine;

namespace Data
{
    public class DatabaseLoader : MonoBehaviour, IApplicationInitializerCallable
    {
        [SerializeField]
        private bool loadOnAwake = true;
        
        [SerializeField]
        private List<DataAssetDefinition> dataAssetsToLoad = new(32);
        
        [SerializeField]
        private List<DataAssetDefinition> csvDataAssetsToLoad = new(32);
        
        private void Awake()
        {
            if (loadOnAwake)
            {
                Load();
            }
        }

        private void Load()
        {
            foreach (var dataAsset in dataAssetsToLoad)
            {
                var asset = Resources.Load(dataAsset.assetPath);
                if (asset == null)
                {
                    Debug.LogError($"Failed to load data asset at path: {dataAsset.assetPath}");
                    continue;
                }
                
                var targetLoadMethod = typeof(Database).GetMethod(
                    $"Assign{dataAsset.containerName}",
                    BindingFlags.Static | BindingFlags.Public);
                if (targetLoadMethod == null)
                {
                    Debug.LogError($"Failed to assign data asset to Database: {dataAsset.containerName}");
                    continue;
                }
                
                targetLoadMethod.Invoke(null, new object[] {asset});
            }

            foreach (var csvDataAsset in csvDataAssetsToLoad)
            {
                var asset = Resources.Load<TextAsset>(csvDataAsset.assetPath);
                if (asset == null)
                {
                    Debug.LogError($"Failed to load data asset at path: {csvDataAsset.assetPath}");
                    continue;
                }
                
                var targetLoadMethod = typeof(Database).GetMethod(
                    $"Initialize{csvDataAsset.containerName}",
                    BindingFlags.Static | BindingFlags.Public);
                if (targetLoadMethod == null)
                {
                    Debug.LogError($"Failed to assign data asset to Database: {csvDataAsset.containerName}");
                    continue;
                }
                
                targetLoadMethod.Invoke(null, new object[] {asset.text});
            }
        }

        public IEnumerator Initialize()
        {
            if (loadOnAwake)
            {
                Debug.LogWarning($"{nameof(DatabaseLoader)} is set to load on awake. Assigned databases will not be loaded by {nameof(ApplicationInitializer)}", gameObject);
                yield break;
            }
            
            Load();
            yield break;
        }

        [Serializable]
        public struct DataAssetDefinition
        {
            [Tooltip("Relative path to the Resources folder of the data asset.\nFile extension is not required.")]
            public string assetPath;
            
            [Tooltip("Data container field name of Database class.")]
            public string containerName;
        }
    }
}