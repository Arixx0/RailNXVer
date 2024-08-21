// ReSharper disable CheckNamespace

using System.Collections.Generic;

namespace Data
{
    public class UnitUpgradeHistory : List<string>
    {
        public UnitUpgradeHistory(int capacity) : base(capacity)
        {
        }
        
        public void Init(string baseUpgradeId)
        {
            Clear();
            Add(baseUpgradeId);
        }
    }
}