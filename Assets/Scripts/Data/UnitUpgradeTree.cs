using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utility;

namespace Data
{
    public static partial class Database
    {
        public static Dictionary<string, UnitUpgradeTree> UnitUpgradeTree;
        
        public static void AssignUnitUpgradeTree(ScriptableObject asset)
        {
            UnitUpgradeTree ??= new Dictionary<string, UnitUpgradeTree>();
            
            if (asset is not UnitUpgradeTree unitUpgradeTree)
            {
                Debug.LogError($"{asset.name} is not {nameof(Data.UnitUpgradeTree)}");
                return;
            }

            if (!UnitUpgradeTree.TryAdd(unitUpgradeTree.UnitCategory, unitUpgradeTree))
            {
                Debug.LogError($"{unitUpgradeTree.UnitCategory} is already assigned.");
            }
        }
    }
    
    [CreateAssetMenu(fileName = "CarUpgradeTree", menuName = "TrainCraft/CarUpgradeTree")]
    public class UnitUpgradeTree : ScriptableObject
    {
        [SerializeField] private string unitCategory;
        
        [UnityEngine.Serialization.FormerlySerializedAs("carUpgradeTreeLUT"), SerializeField]
        private UnitUpgradeTreeLUT unitUpgradeTreeLUT = new();
        
        public UnitUpgradeContext this[string key] => unitUpgradeTreeLUT[key];
        
        public string UnitCategory => unitCategory;
        
        public bool TryGetValue(string key, out UnitUpgradeContext value)
        {
            return unitUpgradeTreeLUT.TryGetValue(key, out value);
        }

        public List<string> FindPossibleUpgradeTree(string baseNodeIdentifier)
        {
            var path = new List<string>() { baseNodeIdentifier };
            
            // search to parent node side
            var currentSearchTarget = baseNodeIdentifier;
            while (true)
            {
                if (!unitUpgradeTreeLUT.Values.Any(e => e.nextUpgradeNodeIdentifier.Contains(currentSearchTarget)))
                {
                    break;
                }

                var parent =
                    unitUpgradeTreeLUT.First(e => e.Value.nextUpgradeNodeIdentifier.Contains(currentSearchTarget));
                currentSearchTarget = parent.Key;
                path.Insert(0, currentSearchTarget);
            }

            return path;
        }

        [System.Serializable]
        public class UnitUpgradeTreeLUT : SerializableDictionary<string, UnitUpgradeContext> { }

        // TODO: `UnitUpgradeContext.model` should be referenced by outer scope for management.
        [System.Serializable]
        public struct UnitUpgradeContext
        {
            public string[] nextUpgradeNodeIdentifier;
            
            public GameObject model;
        }
    }
}