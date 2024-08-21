using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Data
{
    public partial class Database
    {
        public static CarCostData CarCostData;
        
        // Initialize `Database.CarCostData` with the provided CSV content
        // NOTE: This method is called from `DatabaseLoader` component
        public static void InitializeCarCostData(string csvContent)
        {
            CarCostData = new CarCostData();
            CarCostData.ParseFromCsv(csvContent);
        }
    }
    
    // Collection contains Car's build cost and upgrade cost.
    public class CarCostData : Dictionary<string, CarCostDataTuple>
    {
        public void ParseFromCsv(string csvContent)
        {
            var csvLines = csvContent.Split('\n');
            var csvRegexPattern = @",(?=(?:[^""]*""[^""]*"")*[^""]*$)";
            var conflictedLineInformation = new System.Text.StringBuilder();

            for (var i = 2; i < csvLines.Length; ++i)
            {
                var csvElement = Regex.Split(csvLines[i], csvRegexPattern);
                var tuple = new CarCostDataTuple(csvElement);

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

    public class CarCostDataTuple
    {
        public string Identifier;

        public int ResourceSteel;
        public int ResourceTitanium;
        public int ResourceMithryl;
        public int ResourceAdamantium;
        public int CircuitNormal;

        public CarCostDataTuple()
        {
            Identifier = string.Empty;
            ResourceSteel = 0;
            ResourceTitanium = 0;
            ResourceMithryl = 0;
            ResourceAdamantium = 0;
            CircuitNormal = 0;
        }

        public CarCostDataTuple(params string[] csvElement)
        {
            if (csvElement.Length != 8)
            {
                throw new System.ArgumentException("Provided CSV Content seems to be contaminated");
            }

            Identifier = csvElement[0];
            ResourceSteel = Convert.ToInt32(csvElement[1]);
            ResourceTitanium = Convert.ToInt32(csvElement[2]);
            ResourceMithryl = Convert.ToInt32(csvElement[3]);
            ResourceAdamantium = Convert.ToInt32(csvElement[4]);
            CircuitNormal = Convert.ToInt32(csvElement[5]);
        }

        public static CarCostDataTuple operator +(CarCostDataTuple lhs, CarCostDataTuple rhs)
        {
            var result = new CarCostDataTuple
            {
                ResourceSteel = lhs.ResourceSteel + rhs.ResourceSteel,
                ResourceTitanium = lhs.ResourceTitanium + rhs.ResourceTitanium,
                ResourceMithryl = lhs.ResourceMithryl + rhs.ResourceMithryl,
                ResourceAdamantium = lhs.ResourceAdamantium + rhs.ResourceAdamantium,
                CircuitNormal = lhs.CircuitNormal + rhs.CircuitNormal,
            };

            return result;
        }

        public static CarCostDataTuple operator /(CarCostDataTuple lhs, float rhs)
        {
            var result = new CarCostDataTuple
            {
                ResourceSteel = Mathf.RoundToInt(lhs.ResourceSteel / rhs),
                ResourceTitanium = Mathf.RoundToInt(lhs.ResourceTitanium / rhs),
                ResourceMithryl = Mathf.RoundToInt(lhs.ResourceMithryl / rhs),
                ResourceAdamantium = Mathf.RoundToInt(lhs.ResourceAdamantium / rhs),
                CircuitNormal = Mathf.RoundToInt(lhs.CircuitNormal / rhs),
            };

            return result;
        }

        public static CarCostDataTuple operator *(CarCostDataTuple lhs, float rhs)
        {
            var result = new CarCostDataTuple
            {
                ResourceSteel = Mathf.RoundToInt(lhs.ResourceSteel * rhs),
                ResourceTitanium = Mathf.RoundToInt(lhs.ResourceTitanium * rhs),
                ResourceMithryl = Mathf.RoundToInt(lhs.ResourceMithryl * rhs),
                ResourceAdamantium = Mathf.RoundToInt(lhs.ResourceAdamantium * rhs),
                CircuitNormal = Mathf.RoundToInt(lhs.CircuitNormal * rhs),
            };
            
            return result;
        }
    }
}