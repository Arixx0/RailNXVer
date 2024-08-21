using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;
using UnityEngine;

namespace Data
{
    public partial class Database
    {
        public static TextData TextData;
        
        public static void InitializeTextData(string csvContent)
        {
            TextData = new TextData();
            TextData.ParseFromCsv(csvContent);
        }
    }

    public class TextData : Dictionary<string, TextDataTuple>
    {
        public void ParseFromCsv(string csvContent)
        {
            var csvLines = csvContent.Split("\r\n");
            var csvRegexPattern = @",(?=(?:[^""]*""[^""]*"")*[^""]*$)";

            var conflictedLineInformation = new StringBuilder();

            for (var i = 2; i < csvLines.Length; ++i)
            {
                var csvElement = Regex.Split(csvLines[i], csvRegexPattern);
                var tuple = new TextDataTuple(csvElement);

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

    public class TextDataTuple
    {
        public string Identifier;
        public string speaker;
        public string korean;
        public string english;
        public string japanese;

        public TextDataTuple(params string[] csvElements)
        {
            if (csvElements.Length != 5)
            {
                throw new ArgumentException("Provided CSV Content seems to be contaminated");
            }

            Identifier = csvElements[0];
            speaker = csvElements[1];
            korean = csvElements[2];
            english = csvElements[3];
            japanese = csvElements[4];
        }
    }
}

