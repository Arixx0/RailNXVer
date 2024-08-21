using UnityEngine;

namespace Data
{
    [System.Serializable]
    public class UnitIdentifier : BaseIdentifier
    {
        public string UnitCategory => $"{category}_{objectType}";

        public override void Set(string identifier)
        {
            var split = identifier.Split('_');
            var referenceIndex = 0;
            
            category = split[referenceIndex++];
            objectType = split[referenceIndex++];

            // if identifier contains upgrade level
            if (split.Length > 3)
            {
                referenceIndex += 1;
            }
            
            subType = split[referenceIndex];
        }

        public string GetIdentifier(int upgradeLevel = -1)
        {
            if (upgradeLevel < 0)
            {
                return Identifier;
            }

            return $"{category}_{objectType}_{upgradeLevel}_{subType}";
        }

        public string GetImageIdentifier(int upgradeLevel = -1)
        {
            if (upgradeLevel < 0)
            {
                return $"{objectType}_0_{subType}";
            }

            return $"{objectType}_{upgradeLevel}_{subType}";
        }
    }
}