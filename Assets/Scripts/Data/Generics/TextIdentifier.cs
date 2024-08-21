namespace Data
{
    [System.Serializable]
    public class TextIdentifier : BaseIdentifier
    {
        public override string Identifier => !string.IsNullOrEmpty(subType) ? base.Identifier : $"{category}_{objectType}";

        public override void Set(string identifier)
        {
            var split = identifier.Split('_');
            var referenceIndex = 0;

            category = split[referenceIndex++];
            objectType = split[referenceIndex++];
            subType = string.Join("_", split, referenceIndex, split.Length - referenceIndex);
        }
    }
}