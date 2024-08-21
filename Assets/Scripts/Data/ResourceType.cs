namespace Data
{
    // Types of resources implemented in game
    public enum ResourceType
    {
        PowerStone,
        Iron,
        Titanium,
        Mythrill,
        Adamantine,
        Circuit,
        RestoreCore
    }
}

namespace Utility
{
    public static partial class EnumUtility
    {
        public static bool IdentifierToItemType(string identifier, out Data.ResourceType itemType)
        {
            const string ResourcePrefix = "Resource_";

            var resourceTypeStr = identifier;
            if (identifier.StartsWith(ResourcePrefix))
            {
                resourceTypeStr = identifier.Replace(ResourcePrefix, string.Empty);
            }

            return System.Enum.TryParse(resourceTypeStr, out itemType);
        }
    }
}