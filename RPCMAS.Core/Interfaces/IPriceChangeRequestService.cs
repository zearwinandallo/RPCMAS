using RPCMAS.Core.Entities;
using RPCMAS.Core.Models;

#nullable enable

namespace RPCMAS.Core.Interfaces
{
    public interface IPriceChangeRequestService
    {
        Task<List<PriceChangeRequestHeaderModel>> GetPriceChangeRequests(PriceChangeRequestFilter filter);
        Task<PriceChangeRequestHeaderModel?> GetPriceChangeRequestById(Guid id);
        Task<PriceChangeRequestHeaderModel> CreateRequest(PriceChangeRequestHeaderModel request);
        Task<PriceChangeRequestHeaderModel?> EditDraftRequest(Guid id, PriceChangeRequestHeaderModel request);
        Task<PriceChangeRequestHeaderModel?> SubmitRequest(Guid id);
        Task<PriceChangeRequestHeaderModel?> ApproveRequest(Guid id);
        Task<PriceChangeRequestHeaderModel?> RejectRequest(Guid id);
        Task<PriceChangeRequestHeaderModel?> ApplyRequest(Guid id);
        Task<PriceChangeRequestHeaderModel?> CancelRequest(Guid id);
    }
}

#nullable restore
