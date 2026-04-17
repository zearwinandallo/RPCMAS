using Microsoft.EntityFrameworkCore;
using RPCMAS.Core.Data;
using RPCMAS.Core.Entities;
using RPCMAS.Core.Interfaces;
using RPCMAS.Core.Models;

namespace RPCMAS.Infrastructure.Repositories
{
    public class PriceChangeRequestRepository : IPriceChangeRequestRepository
    {
        private readonly AppDbContext _appDbContext;

        public PriceChangeRequestRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<List<PriceChangeRequestHeaderModel>> GetPriceChangeRequests(PriceChangeRequestFilter filter)
        {
            var query = _appDbContext.PriceChangeRequestHeaders
                .AsNoTracking()
                .Include(x => x.Details)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.RequestNumber))
            {
                var requestNumber = filter.RequestNumber.Trim();
                query = query.Where(x => x.RequestNumber.Contains(requestNumber));
            }

            if (filter.Status.HasValue)
            {
                query = query.Where(x => x.Status == filter.Status.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.Department))
            {
                var department = filter.Department.Trim();
                query = query.Where(x => x.Department == department);
            }

            if (filter.ChangeType.HasValue)
            {
                query = query.Where(x => x.ChangeType == filter.ChangeType.Value);
            }

            if (filter.RequestDate.HasValue)
            {
                var requestDate = filter.RequestDate.Value.Date;
                query = query.Where(x => x.RequestDate.Date == requestDate);
            }

            return await query
                .OrderByDescending(x => x.RequestDate)
                .ThenByDescending(x => x.RequestNumber)
                .ToListAsync();
        }

        public Task<PriceChangeRequestHeaderModel?> GetPriceChangeRequestById(Guid id)
        {
            return _appDbContext.PriceChangeRequestHeaders
                .AsNoTracking()
                .Include(x => x.Details)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public Task<PriceChangeRequestHeaderModel?> GetPriceChangeRequestByIdForUpdate(Guid id)
        {
            return _appDbContext.PriceChangeRequestHeaders
                .Include(x => x.Details)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public Task<PriceChangeRequestHeaderModel?> GetPriceChangeRequestHeaderByIdForUpdate(Guid id)
        {
            return _appDbContext.PriceChangeRequestHeaders
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public Task<List<PriceChangeRequestDetailModel>> GetDetailsByHeaderId(Guid priceChangeRequestHeaderId)
        {
            return _appDbContext.PriceChangeRequestDetails
                .AsNoTracking()
                .Where(x => x.PriceChangeRequestHeaderId == priceChangeRequestHeaderId)
                .ToListAsync();
        }

        public Task<ItemCatalogModel?> GetItemCatalogBySku(string sku)
        {
            return _appDbContext.ItemCatalogs.FirstOrDefaultAsync(x => x.SKU == sku);
        }

        public async Task AddPriceChangeRequest(PriceChangeRequestHeaderModel request)
        {
            await _appDbContext.PriceChangeRequestHeaders.AddAsync(request);
        }

        public async Task DeletePriceChangeRequestDetailsByHeaderId(Guid headerId)
        {
            var details = await _appDbContext.PriceChangeRequestDetails
                .Where(x => x.PriceChangeRequestHeaderId == headerId)
                .ToListAsync();

            if (details.Count != 0)
            {
                _appDbContext.PriceChangeRequestDetails.RemoveRange(details);
            }
        }

        public async Task AddPriceChangeRequestDetails(IEnumerable<PriceChangeRequestDetailModel> details)
        {
            await _appDbContext.PriceChangeRequestDetails.AddRangeAsync(details);
        }

        public Task SaveChanges()
        {
            return _appDbContext.SaveChangesAsync();
        }
    }
}
