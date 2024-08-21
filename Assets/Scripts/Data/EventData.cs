using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;
using UnityEngine.Events;
using UnityEngine;
using Unity.VisualScripting;

namespace Data
{
    public partial class Database
    {
        public static EventData EventData;

        // Initialize `Database.EventData` with the provided CSV content
        // NOTE: This method is called from `DatabaseLoader` component
        public static void InitializeEventData(string csvContent)
        {
            EventData = new EventData();
            EventData.ParseFromCsv(csvContent);
        }
    }

    public class EventData : Dictionary<string, EventDataTuple>
    {
        public void ParseFromCsv(string csvContent)
        {
            var csvLines = csvContent.Split("\r\n");
            var csvRegexPattern = @",(?=(?:[^""]*""[^""]*"")*[^""]*$)";

            var conflictedLineInformation = new StringBuilder();
            // first two lines is field name and type, so skip those lines
            for (var i = 2; i < csvLines.Length; ++i)
            {
                var csvElement = Regex.Split(csvLines[i], csvRegexPattern);
                var tuple = new EventDataTuple(csvElement);

                if (!TryAdd(tuple.Identifier, tuple))
                {
                    conflictedLineInformation.AppendLine($"identifier {tuple.Identifier} at line {(i + 1)} is conflicted");
                }
            }

            if (conflictedLineInformation.Length > 0)
            {
                Debug.LogError($"Conflicted line detected while parsing Localization table\n{conflictedLineInformation}");
            }
        }
    }

    public class EventDataTuple
    {
        public string Identifier;

        public string EventName;
        public string EventDescription;
        public List<string> EventOptions = new List<string>();
        public List<string> EventRewardIconIndex = new List<string>();
        public List<int> EventRewardQuantity = new List<int>();

        public EventDataTuple(params string[] csvElements)
        {
            if (csvElements.Length != 12)
            {
                throw new ArgumentException("Provided CSV Content seems to be contaminated");
            }

            Identifier = csvElements[0];
            EventName = csvElements[1];
            EventDescription = csvElements[2];
            if (!string.IsNullOrEmpty(csvElements[3])) EventRewardIconIndex.Add(csvElements[3]);
            if (!string.IsNullOrEmpty(csvElements[4])) EventRewardQuantity.Add(int.Parse(csvElements[4]));
            if (!string.IsNullOrEmpty(csvElements[5])) EventRewardIconIndex.Add(csvElements[5]);
            if (!string.IsNullOrEmpty(csvElements[6])) EventRewardQuantity.Add(int.Parse(csvElements[6]));
            if (!string.IsNullOrEmpty(csvElements[7])) EventRewardIconIndex.Add(csvElements[7]);
            if (!string.IsNullOrEmpty(csvElements[8])) EventRewardQuantity.Add(int.Parse(csvElements[8]));
            if (!string.IsNullOrEmpty(csvElements[9])) EventOptions.Add(csvElements[9]);
            if (!string.IsNullOrEmpty(csvElements[10])) EventOptions.Add(csvElements[10]);
            if (!string.IsNullOrEmpty(csvElements[11])) EventOptions.Add(csvElements[11]);
        }
    }
}

