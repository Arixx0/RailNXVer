using System.Collections.Generic;
using UnityEngine;

namespace Utility
{
    public class DatabaseUtility : MonoBehaviour
    {
        public static bool TryGetData<TKey, TValue>(IDictionary<TKey, TValue> dictionary, TKey key, out TValue value)
        {
            value = default(TValue);

            if (dictionary == null)
            {
                Debug.LogError($"Check Database Loader, Data is null");
                return false;
            }

            if (dictionary.TryGetValue(key, out value))
            {
                return true;
            }
            Debug.LogWarning($"The data({key}) does not exist in {dictionary}.");
            return false;
        }

        public static bool TryGetData<TKey, TValue>(IDictionary<TKey, TValue> dictionary, TKey baseKey, string name, string desc, out TValue titleValue, out TValue descValue)
        {
            titleValue = default(TValue);
            descValue = default(TValue);

            if (dictionary == null)
            {
                Debug.LogError($"The {dictionary} does not exist in Database.");
                return false;
            }

            bool titleExists = name == ""
                ? dictionary.TryGetValue((TKey)(object)$"{baseKey}", out titleValue)
                : dictionary.TryGetValue((TKey)(object)$"{baseKey}_{name}", out titleValue);
            bool descExists = dictionary.TryGetValue((TKey)(object)$"{baseKey}_{desc}", out descValue);

            if (titleExists || descExists)
            {
                return true;
            }

            Debug.LogWarning($"The data({baseKey}_{name} or {baseKey}_{desc}) does not exist in {dictionary}.");
            return false;
        }

        public static bool TryGetImageData<TKey, TValue>(IDictionary<TKey, TValue> dictionary, TKey key, string imageType, out TValue value)
        {
            value = default(TValue);

            if (dictionary == null)
            {
                Debug.LogError($"Check Database Loader, Data is null");
                return false;
            }
            
            if (dictionary.TryGetValue((TKey)(object)$"{Data.Database.ImageCommonCategory}_{imageType}_{key}", out value))
            {
                return true;
            }
            Debug.LogWarning($"The data({Data.Database.ImageCommonCategory}_{imageType}_{key}) does not exist in {dictionary}.");
            return false;
        }
    }
}


