using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    public static partial class Database
    {
        public static Dictionary<string, ImageData> ImageSettingsData;
        public static string ImageCommonCategory;
        public static void AssignImageSettingsData(ScriptableObject asset)
        {
            ImageSettingsData = new Dictionary<string, ImageData>();

            if (asset is not ImageSettingsData imageSettingData)
            {
                Debug.LogError($"{asset.name} is not {nameof(Data.ImageSettingsData)}");
                return;
            }

            foreach (var Majordata in imageSettingData.imageSettings)
            {
                foreach (var data in Majordata.imageData)
                {
                    if (!ImageSettingsData.TryAdd(data.Identifier.Identifier, data))
                    {
                        Debug.LogError($"{data.Identifier.Identifier} is already assigned.");
                    }
                }
            }
            ImageCommonCategory = imageSettingData.imageCommonCategory;
        }
    }

    [CreateAssetMenu(menuName = "TrainCraft/Image Settings Data")]
    public class ImageSettingsData : ScriptableObject
    {
        public string imageCommonCategory;
        public List<ImageMajorCategory> imageSettings;
    }

    [System.Serializable]
    public struct ImageMajorCategory
    {
        public string categoryName;
        public List<ImageData> imageData;
    }

    [System.Serializable]
    public struct ImageData
    {
        public string name;
        public TextIdentifier Identifier;
        public Sprite sprite;
    }
}

