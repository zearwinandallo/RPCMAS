using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace RPCMAS.Core.Models
{
    public class ItemCatalogFilter
    {
        public string? Search { get; set; }
        public string? SKU { get; set; }
        public string? ItemName { get; set; }
        public string? Department { get; set; }
        public string? Category { get; set; }
        public string? Brand { get; set; }
        public decimal? CurrentPrice { get; set; }
        public decimal? Cost { get; set; }
        public ItemStatus? Status { get; set; }
    }
}

#nullable restore
