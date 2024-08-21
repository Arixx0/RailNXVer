using UnityEngine;

namespace Data
{
    [System.Serializable]
    public class BaseIdentifier
    {
        [SerializeField] protected string category;
        [SerializeField] protected string objectType;
        [SerializeField] protected string subType;

        virtual public string Category => category;
        virtual public string ObjectType => objectType;
        virtual public string SubType => subType;

        virtual public string Identifier => $"{category}_{objectType}_{subType}";

        virtual public void Set(BaseIdentifier other)
        {
            category = other.category;
            objectType = other.objectType;
            subType = other.subType;
        }

        virtual public void Set(string identifier)
        {
            var split = identifier.Split('_');
            var referenceIndex = 0;

            category = split[referenceIndex++];
            objectType = split[referenceIndex++];
            subType = split[referenceIndex];
        }
    }
}