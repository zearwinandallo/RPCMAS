#nullable enable

namespace RPCMAS.Core.Models
{
    public class PriceChangeRequestFilter
    {
        public string? RequestNumber { get; set; }
        public RequestStatusEnum? Status { get; set; }
        public string? Department { get; set; }
        public ChangeTypeEnum? ChangeType { get; set; }
        public DateTime? RequestDate { get; set; }
    }
}

