using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace MoneyCenter.Schema
{
    public class Budget
    {
        [PrimaryKey]
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
