using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;
using UnityEngine;

namespace Data
{
    public partial class Database
    {
        public static GeneratorData GeneratorData;

        public static void InitializeGeneratorData(string csvContent)
        {
            GeneratorData = new GeneratorData();
            GeneratorData.ParseFromCsv(csvContent);
        }
    }

    public class GeneratorData : Dictionary<string, GeneratorDataTuple>
    {
        public void ParseFromCsv(string csvContent)
        {
            var csvLines = csvContent.Split("\r\n");
            var csvRegexPattern = @",(?=(?:[^""]*""[^""]*"")*[^""]*$)";

            var conflictedLineInformation = new StringBuilder();

            for (var i = 2; i < csvLines.Length; ++i)
            {
                var csvElement = Regex.Split(csvLines[i], csvRegexPattern);
                var tuple = new GeneratorDataTuple(csvElement);

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

    public class GeneratorDataTuple
    {
        public string Identifier;
        public int energyGenerate;

        public GeneratorDataTuple(params string[] csvElements)
        {
            if (csvElements.Length != 2)
            {
                throw new ArgumentException("Provided CSV Content seems to be contaminated");
            }

            Identifier = csvElements[0];
            energyGenerate = int.Parse(csvElements[1]);
        }
    }
}

