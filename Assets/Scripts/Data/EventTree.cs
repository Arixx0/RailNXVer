using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Utility;

namespace Data
{
    public static partial class Database
    {
        public static Dictionary<string, EventTree> EventTree;

        public static void AssignEventTree(ScriptableObject asset)
        {
            EventTree = new Dictionary<string, EventTree>();

            if (asset is not EventTree eventTree)
            {
                Debug.LogError($"{asset.name} is not {nameof(Data.EventTree)}");
                return;
            }

            if (!EventTree.TryAdd(eventTree.EventCategory, eventTree))
            {
                Debug.LogError($"{eventTree.EventCategory} is already assigned.");
            }
        }
    }

    [CreateAssetMenu(fileName = "EventTree", menuName = "TrainCraft/EventTree")]
    public class EventTree : ScriptableObject
    {
        [SerializeField] private string eventCategory;

        [FormerlySerializedAs("EventTreeLUT"), SerializeField]
        private EventTreeLUT eventTreeLUT = new();

        public EventContext this[string key] => eventTreeLUT[key];

        public string EventCategory => eventCategory;

        public bool TryGetValue(string key, out EventContext value)
        {
            return eventTreeLUT.TryGetValue(key, out value);
        }

        public List<string> FindPossibleEventTree(string baseNodeIdentifier)
        {
            var path = new List<string>() { baseNodeIdentifier };

            // search to parent node side
            var currentSearchTarget = baseNodeIdentifier;
            while (true)
            {
                if (!eventTreeLUT.Values.Any(e => e.nextEventNodeIdentifier.Contains(currentSearchTarget)))
                {
                    break;
                }

                var parent =
                    eventTreeLUT.First(e => e.Value.nextEventNodeIdentifier.Contains(currentSearchTarget));
                currentSearchTarget = parent.Key;
                path.Insert(0, currentSearchTarget);
            }

            return path;
        }

        [Serializable]
        public class EventTreeLUT : SerializableDictionary<string, EventContext> { }

        [Serializable]
        public struct EventContext
        {
            public string[] nextEventNodeIdentifier;
        }
    }
}

