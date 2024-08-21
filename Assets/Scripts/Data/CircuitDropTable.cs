using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Data
{
    public partial class Database
    {
        public static CircuitDropTable CircuitDropTable;

        public static void InitializeCircuitDropTable(string csvContent)
        {
            CircuitDropTable = new CircuitDropTable();
            CircuitDropTable.ParseFromCsv(csvContent);
        }
    }

    public class CircuitDropTable : Dictionary<string, CircuitDropTableTuple>
    {
        public void ParseFromCsv(string csvContent)
        {
            var csvLines = csvContent.Split("\r\n");
            var csvRegexPattern = @",(?=(?:[^""]*""[^""]*"")*[^""]*$)";

            var conflictedLineInformation = new StringBuilder();

            for (var i = 2; i < csvLines.Length; ++i)
            {
                var csvElement = Regex.Split(csvLines[i], csvRegexPattern);
                var tuple = new CircuitDropTableTuple(csvElement);

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

    public class CircuitDropTableTuple
    {
        public string Identifier;
        public int amount;
        public float possibility;

        public CircuitDropTableTuple(params string[] csvElements)
        {
            if (csvElements.Length != 3)
            {
                throw new ArgumentException("Provided CSV Content seems to be contaminated");
            }

            Identifier = csvElements[0];
            amount = int.Parse(csvElements[1]);
            possibility = float.Parse(csvElements[2]);
        }
    }
}