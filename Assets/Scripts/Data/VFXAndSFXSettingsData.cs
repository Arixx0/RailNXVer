using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    public static partial class Database
    {
        public static Dictionary<string, VFXData> VFXSettingsData;
        public static Dictionary<string, SFXData> SFXSettingsData;
        public static string VFXCommonCategory;
        public static string SFXCommonCategory;
        public static void AssignVFXAndSFXSettingsData(ScriptableObject asset)
        {
            VFXSettingsData = new Dictionary<string, VFXData>();
            SFXSettingsData = new Dictionary<string, SFXData>();

            if (asset is not VFXAndSFXSettingsData VFXAndSFXSettingsData)
            {
                Debug.LogError($"{asset.name} is not {nameof(Data.VFXAndSFXSettingsData)}");
                return;
            }

            VFXCommonCategory = VFXAndSFXSettingsData.VFXCommonCategory;
            SFXCommonCategory = VFXAndSFXSettingsData.SFXCommonCategory;

            foreach (var Majordata in VFXAndSFXSettingsData.VFXAndSFXSettings)
            {
                foreach (var vfxData in Majordata.VFXData)
                {
                    var modifiedIdentifier = $"{VFXCommonCategory}_{vfxData.Identifier.ObjectType}_{vfxData.Identifier.SubType}";
                    if (!VFXSettingsData.TryAdd(modifiedIdentifier, vfxData))
                    {
                        Debug.LogError($"{modifiedIdentifier} is already assigned.");
                    }
                }
                foreach (var sfxData in Majordata.SFXData)
                {
                    var modifiedIdentifier = $"{SFXCommonCategory}_{sfxData.Identifier.ObjectType}_{sfxData.Identifier.SubType}";
                    if (!SFXSettingsData.TryAdd(modifiedIdentifier, sfxData))
                    {
                        Debug.LogError($"{modifiedIdentifier} is already assigned.");
                    }
                }
            }
        }
    }

    [CreateAssetMenu(menuName = "TrainCraft/VFX And SFX Settings Data")]
    public class VFXAndSFXSettingsData : ScriptableObject
    {
        public string VFXCommonCategory;
        public string SFXCommonCategory;
        public List<MajorCategory> VFXAndSFXSettings;
    }

    [System.Serializable]
    public struct MajorCategory
    {
        public string categoryName;
        public List<SFXData> SFXData;
        public List<VFXData> VFXData;
    }

    [System.Serializable]
    public struct SFXData
    {
        public string name;
        public TextIdentifier Identifier;
        public AudioClip sfxSource;
    }

    [System.Serializable]
    public struct VFXData
    {
        public string name;
        public TextIdentifier Identifier;
        public VFX vfxSource;
    }
}

