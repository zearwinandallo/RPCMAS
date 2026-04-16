using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPCMAS.Core.Entities
{
    public class PriceChangeRequestHeaderModel
    {
        public Guid Id { get; set; }

        public string RequestNumber { get; set; }
        public DateTime RequestDate { get; set; }

        public string Department { get; set; }
        public string RequestedBy { get; set; }

        public ChangeTypeEnum ChangeType { get; set; }

        public string ReasonOrJustification { get; set; }

        public RequestStatusEnum Status { get; set; }
    }
}
