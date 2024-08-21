using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Data
{
    public static partial class Database
    {
        public static ResourcesSettingsData ResourcesSettingsData;
        
        // Initialize `Database.ResourcesSettingsData` with the given asset.
        // NOTE: This method is called from `DatabaseLoader` component
        public static void AssignResourcesSettingsData(ResourcesSettingsData data)
        {
            ResourcesSettingsData = data;
        }
    }
    
    [CreateAssetMenu]
    public class ResourcesSettingsData : ScriptableObject
    {
        [Header("PowerStone Options")]
        public float powerStoneMiningDuration;
        [Tooltip("동력석 광맥당 매장량")] public int powerStoneDepositAmountPerVein;
        public Environments.ResourceVein powerStoneVeinPrefab;

        [Header("Iron Options")]
        public float ironMiningDuration;
        [Tooltip("철 광맥당 매장량")] public int ironDepositAmountPerVein;
        public Environments.ResourceVein ironVeinPrefab;
        
        [Header("Titanium Options")]
        public float titaniumMiningDuration;
        [Tooltip("티타늄 광맥당 매장량")] public int titaniumDepositAmountPerVein;
        public Environments.ResourceVein titaniumVeinPrefab;
        
        [Header("Mithril Options")]
        public float mithrilMiningDuration;
        [Tooltip("미스릴 광맥당 매장량")] public int mithrilDepositAmountPerVein;
        [FormerlySerializedAs("mithrilVeinPrefab")] public Environments.ResourceVein mythrillVeinPrefab;
        
        [Header("Adamantium Options")]
        public float adamantiumMiningDuration;
        [Tooltip("아다만티움 광맥당 매장량")] public int adamantiumDepositAmountPerVein;
        [FormerlySerializedAs("adamantiumVeinPrefab")] public Environments.ResourceVein adamantineVeinPrefab;

        public float GetMiningDurationOfType(ResourceType type)
        {
            switch (type)
            {
                case ResourceType.PowerStone:
                    return powerStoneMiningDuration;
                case ResourceType.Iron:
                    return ironMiningDuration;
                case ResourceType.Titanium:
                    return ironMiningDuration;
                case ResourceType.Mythrill:
                    return mithrilMiningDuration;
                case ResourceType.Adamantine:
                    return adamantiumMiningDuration;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public int GetDepositPerVeinOfType(ResourceType type)
        {
            switch (type)
            {
                case ResourceType.PowerStone:
                    return powerStoneDepositAmountPerVein;
                case ResourceType.Iron:
                    return ironDepositAmountPerVein;
                case ResourceType.Titanium:
                    return titaniumDepositAmountPerVein;
                case ResourceType.Mythrill:
                    return mithrilDepositAmountPerVein;
                case ResourceType.Adamantine:
                    return adamantiumDepositAmountPerVein;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public Environments.ResourceVein GetVeinPrefabOfType(ResourceType type)
        {
            var prefab =  type switch
            {
                ResourceType.PowerStone => powerStoneVeinPrefab,
                ResourceType.Iron => ironVeinPrefab,
                ResourceType.Titanium => titaniumVeinPrefab,
                ResourceType.Mythrill => mythrillVeinPrefab,
                ResourceType.Adamantine => adamantineVeinPrefab,
                _ => null,
            };
            
            Debug.Assert(prefab != null, $"Vein prefab type of {type} is not set in the database.");
            return prefab;
        }
    }
}