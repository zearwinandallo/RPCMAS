using RPCMAS.Core.Entities;
using RPCMAS.Core.Models;

#nullable enable

namespace RPCMAS.Core.Interfaces
{
    public interface IPriceChangeRequestRepository
    {
        Task<List<PriceChangeRequestHeaderModel>> GetPriceChangeRequests(PriceChangeRequestFilter filter);
        Task<PriceChangeRequestHeaderModel?> GetPriceChangeRequestById(Guid id);
        Task<PriceChangeRequestHeaderModel?> GetPriceChangeRequestByIdForUpdate(Guid id);
        Task<List<PriceChangeRequestDetailModel>> GetDetailsByHeaderId(Guid priceChangeRequestHeaderId);
        Task<ItemCatalogModel?> GetItemCatalogBySku(string sku);
        Task AddPriceChangeRequest(PriceChangeRequestHeaderModel request);
        Task SaveChanges();
    }
}

#nullable restore
