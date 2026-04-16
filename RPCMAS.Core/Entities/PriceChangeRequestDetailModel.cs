using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPCMAS.Core.Entities
{
    public class PriceChangeRequestDetailModel
    {
        public Guid Id { get; set; }

        public string SKU { get; set; }
        public string ItemName { get; set; }

        public decimal CurrentPrice { get; set; }
        public decimal ProposedNewPrice { get; set; }

        public decimal MarkdownPercentage { get; set; }

        public DateTime EffectiveDate { get; set; }

        public string Remarks { get; set; }

        public PriceChangeRequestHeaderModel PriceChangeRequestHeader { get; set; }
    }
}
