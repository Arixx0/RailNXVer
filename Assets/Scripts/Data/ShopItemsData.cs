using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Data
{
    public partial class Database
    {
        public static ShopItemsData ShopItemsData;

        public static void InitializeShopItemsData(string csvContent)
        {
            ShopItemsData = new ShopItemsData();
            ShopItemsData.ParseFromCsv(csvContent);
        }
    }

    public class ShopItemsData : Dictionary<string, ShopItemData>
    {
        public void ParseFromCsv(string csvContent)
        {
            var csvLines = csvContent.Split('\n');
            var csvRegexPattern = @",(?=(?:[^""]*""[^""]*"")*[^""]*$)";
            var conflictedLineInformation = new StringBuilder();

            for (var i = 2; i < csvLines.Length; i += 1)
            {
                var elements = Regex.Split(csvLines[i], csvRegexPattern);
                var data = new ShopItemData(elements);

                if (!TryAdd(data.Identifier, data))
                {
                    conflictedLineInformation.AppendLine($"Identifier {data.Identifier} at line {(i + 1)} seems to be duplicated.");
                }
            }

            if (conflictedLineInformation.Length > 0)
            {
                Debug.LogError($"Conflicted line detected while parsing {nameof(ShopItemsData)}\n{conflictedLineInformation}");
            }
        }
    }

    public class ShopItemData
    {
        public string Identifier;

        public ItemType ItemType;

        public int DropAmount;

        public float DropPossibility;

        public int Cost;

        public int Tier;

        public int PurchaseCountPerEvent;
        
        public ShopItemData(params string[] csvElement)
        {
            if (csvElement.Length < 7)
            {
                throw new ArgumentException("Not enough CSV content provided");
            }

            Identifier = csvElement[0];
            ItemType = (ItemType)Enum.Parse(typeof(ItemType), csvElement[1]);
            DropAmount = Convert.ToInt32(csvElement[2]);
            DropPossibility = Convert.ToSingle(csvElement[3]);
            Cost = Convert.ToInt32(csvElement[4]);
            Tier = Convert.ToInt32(csvElement[5]);
            PurchaseCountPerEvent = Convert.ToInt32(csvElement[6]);
        }
        
        public override string ToString()
        {
            return $"Type: {ItemType}, DropAmount: {DropAmount}, DropPossibility: {DropPossibility}, Cost: {Cost}, Tier: {Tier}, PurchaseCountPerEvent: {PurchaseCountPerEvent}";
        }

        [ContextMenu("Copy To Clipboard")]
        public void CopyToClipboard()
        {
            GUIUtility.systemCopyBuffer = Identifier;
        }
    }
}