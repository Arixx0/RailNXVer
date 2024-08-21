using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Data
{
    public partial class Database
    {
        public static ScrapperDroneData ScrapperDroneData;
        
        public static void InitializeScrapperDroneData(string csvContent)
        {
            ScrapperDroneData = new ScrapperDroneData();
            ScrapperDroneData.ParseFromCsv(csvContent);
        }
    }
    
    public class ScrapperDroneData : Dictionary<string, ScrapperDroneDataTuple>
    {
        public void ParseFromCsv(string csvContent)
        {
            var csvLines = csvContent.Split('\n');
            var csvRegexPattern = @",(?=(?:[^""]*""[^""]*"")*[^""]*$)";
            var conflictedLineInformation = new StringBuilder();

            for (var i = 2; i < csvLines.Length; ++i)
            {
                var csvElement = Regex.Split(csvLines[i], csvRegexPattern);
                var tuple = new ScrapperDroneDataTuple(csvElement);

                if (!TryAdd(tuple.Identifier, tuple))
                {
                    conflictedLineInformation.AppendLine($"identifier {tuple.Identifier} at line {(i + 1)} is conflicted");
                }
            }

            if (conflictedLineInformation.Length > 0)
            {
                Debug.LogError($"Conflicted line detected while parsing {nameof(ScrapperDroneData)} table\n{conflictedLineInformation}");
            }
        }
    }

    public class ScrapperDroneDataTuple
    {
        public string Identifier;

        public int MiningAmount;

        public ScrapperDroneDataTuple(params string[] csvElement)
        {
            if (csvElement.Length < 2)
            {
                throw new ArgumentException("Not enough CSV content provided");
            }

            Identifier = csvElement[0];
            MiningAmount = Convert.ToInt32(csvElement[1]);
        }
    }
}