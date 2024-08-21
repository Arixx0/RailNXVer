using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utility
{
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField, HideInInspector] private List<TKey> keysSerializedCache;
        [SerializeField, HideInInspector] private List<TValue> valuesSerializedCache;

        public SerializableDictionary(int capacity = 16) : base(capacity)
        {
            keysSerializedCache = new(capacity);
            valuesSerializedCache = new(capacity);
        }
        
        public void OnBeforeSerialize()
        {
            keysSerializedCache = new List<TKey>(Count);
            valuesSerializedCache = new List<TValue>(Count);
            
            foreach (var pair in this)
            {
                keysSerializedCache.Add(pair.Key);
                valuesSerializedCache.Add(pair.Value);
            }
        }

        public void OnAfterDeserialize()
        {
            Clear();

            for (var i = 0; i < keysSerializedCache.Count; i++)
            {
                Add(keysSerializedCache[i], valuesSerializedCache[i]);
            }

            // TODO: uncomment below lines to avoid runtime memory leak
            // keys = null;
            // values = null;
        }
    }
}
