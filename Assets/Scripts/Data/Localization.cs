using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Data
{
    public partial class Database
    {
        public static Localization Localization;

        // Initialize `Database.InitializeLocalization` with the provided CSV content
        // NOTE: This method is called from `DatabaseLoader` component
        public static void InitializeLocalization(string csvContent)
        {
            Localization = new Localization();
            Localization.ParseFromCsv(csvContent);
        }
    }
    
    // Collection contains localization data for the game
    public class Localization : Dictionary<string, LocalizationTuple>
    {
        public void ParseFromCsv(string csvContent)
        {
            var csvLines = csvContent.Split('\n');
            var csvRegexPattern = @",(?=(?:[^""]*""[^""]*"")*[^""]*$)";

            var conflictedLineInformation = new StringBuilder();
            
            // first two lines is field name and type, so skip those lines
            for (var i = 2; i < csvLines.Length; ++i)
            {
                var csvElement = Regex.Split(csvLines[i], csvRegexPattern);
                var tuple = new LocalizationTuple(csvElement);

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

    public class LocalizationTuple
    {
        public string Identifier;

        public string Kor;

        public string Eng;

        public LocalizationTuple(params string[] csvElements)
        {
            if (csvElements.Length < 3)
            {
                throw new ArgumentException("Not enough CSV content provided");
            }

            Identifier = csvElements[0];
            Kor = csvElements[1];
            Eng = csvElements[2];
        }
    }
}