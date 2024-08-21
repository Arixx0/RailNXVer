using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Data
{
    public partial class Database
    {
        public static UnitStatData UnitStatData;
        
        // Initialize `Database.UnitStatData` with the provided CSV content
        // NOTE: This method is called from `DatabaseLoader` component
        public static void InitializeUnitStatData(string csvContent)
        {
            UnitStatData = new UnitStatData();
            UnitStatData.ParseFromCsv(csvContent);
        }
    }
    
    // Collection contains unit's stat data sorted by the unit's identifier
    public class UnitStatData : Dictionary<string, UnitStatDataTuple>
    {
        public void ParseFromCsv(string csvContent)
        {
            var csvLines = csvContent.Split('\n');
            var csvRegexPattern = @",(?=(?:[^""]*""[^""]*"")*[^""]*$)";
            var conflictedLineInformation = new System.Text.StringBuilder();

            for (var i = 2; i < csvLines.Length; ++i)
            {
                var csvElement = Regex.Split(csvLines[i], csvRegexPattern);
                var tuple = new UnitStatDataTuple(csvElement);

                if (!TryAdd(tuple.Identifier, tuple))
                {
                    conflictedLineInformation.AppendLine($"identifier {tuple.Identifier} at line {(i + 1)} is conflicted");
                }
            }

            if (conflictedLineInformation.Length > 0)
            {
                Debug.LogError($"Conflicted line detected while parsing {nameof(CarCostData)} table\n{conflictedLineInformation}");
            }
        }
    }

    public class UnitStatDataTuple
    {
        public string Identifier;

        public float MaxHealth;
        public float Armor;
        public float MoveSpeed;
        public float AttackDamage;
        public float ArmorPierece;
        public float AttackSpeed;
        public float AttackRange;
        public float FuelEfficiency;
        public float EnergyCost;
        public float UnitSize;

        public UnitStatDataTuple(params string[] csvElement)
        {
            if (csvElement.Length != 11)
            {
                throw new System.ArgumentException("Provided CSV Content seems to be contaminated");
            }
            
            Identifier = csvElement[0];
            MaxHealth = float.Parse(csvElement[1]);
            Armor = float.Parse(csvElement[2]);
            MoveSpeed = float.Parse(csvElement[3]);
            AttackDamage = float.Parse(csvElement[4]);
            ArmorPierece = float.Parse(csvElement[5]);
            AttackSpeed = float.Parse(csvElement[6]);
            AttackRange = float.Parse(csvElement[7]);
            FuelEfficiency = float.Parse(csvElement[8]);
            EnergyCost = float.Parse(csvElement[9]);
            UnitSize = float.Parse(csvElement[10]);
        }
    }
}