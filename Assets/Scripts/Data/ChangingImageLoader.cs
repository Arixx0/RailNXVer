using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace Data
{
    public class ChangingImageLoader : MonoBehaviour
    {
        [SerializeField] private Image targetImage;
        [SerializeField,Tooltip("Identifier의 Object Type")] private string imageType;

        public Image TargetImage => targetImage;

        public void ChangeImage(string identifier)
        {
            if (DatabaseUtility.TryGetImageData(Database.ImageSettingsData, identifier, imageType, out var imageData))
            {
                targetImage.sprite = imageData.sprite;
            }
        }
    }
}
