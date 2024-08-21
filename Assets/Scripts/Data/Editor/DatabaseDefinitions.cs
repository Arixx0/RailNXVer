using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(menuName = "TrainCraft/Editor/Database Definitions")]
    public class DatabaseDefinitions : ScriptableObject
    {
        private static bool IsInitialized = false;
        
        public List<DatabaseLoader.DataAssetDefinition> dataAssetsToLoad = new(32)
        {
            new DatabaseLoader.DataAssetDefinition { assetPath = "ResourceSettingsData", containerName = "ResourceSettingsData" },
            new DatabaseLoader.DataAssetDefinition { assetPath = "CarUpgradeTreeCombat", containerName = "UnitUpgradeTree" },
            new DatabaseLoader.DataAssetDefinition { assetPath = "CarUpgradeTreeGenerator", containerName = "UnitUpgradeTree" },
            new DatabaseLoader.DataAssetDefinition { assetPath = "CarUpgradeTreeScrapper", containerName = "UnitUpgradeTree" },
            new DatabaseLoader.DataAssetDefinition { assetPath = "CarUpgradeTreeUtility", containerName = "UnitUpgradeTree" },
            new DatabaseLoader.DataAssetDefinition { assetPath = "CarUpgradeTreeEngine", containerName = "UnitUpgradeTree" },
        };

        public List<DatabaseLoader.DataAssetDefinition> csvDataAssetsToLoad = new(32)
        {
            new DatabaseLoader.DataAssetDefinition { assetPath = "ScrapperDroneData", containerName = "ScrapperDroneData" },
            new DatabaseLoader.DataAssetDefinition { assetPath = "CarCostData", containerName = "CarCostData" },
            new DatabaseLoader.DataAssetDefinition { assetPath = "UnitStatData", containerName = "UnitStatData" },
            new DatabaseLoader.DataAssetDefinition { assetPath = "ShopItemsData", containerName = "ShopItemsData" }
        };

        private void OnValidate()
        {
            IsInitialized = false;
        }

        public static DatabaseDefinitions GetOrCreate()
        {
            const string path = "Assets/Editor Default Resources/DatabaseDefinitions.asset";

            var asset = AssetDatabase.LoadAssetAtPath<DatabaseDefinitions>(path);
            if (asset == null)
            {
                asset = CreateInstance<DatabaseDefinitions>();
                AssetDatabase.CreateAsset(asset, path);
                AssetDatabase.SaveAssets();
            }

            return asset;
        }

        public void Load()
        {
            if (IsInitialized)
            {
                return;
            }
            
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

            IsInitialized = true;
        }
    }
}