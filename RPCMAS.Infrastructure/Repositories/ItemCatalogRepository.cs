using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RPCMAS.Core.Data;
using RPCMAS.Core.Entities;
using RPCMAS.Core.Interfaces;
using RPCMAS.Core.Models;

namespace RPCMAS.Infrastructure.Repositories
{
    public class ItemCatalogRepository : IItemCatalogRepository
    {
        private readonly AppDbContext _appDbContext;

        public ItemCatalogRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<List<ItemCatalogModel>> GetItemCatalogs(string? filter)
        {

            var query = _appDbContext.ItemCatalogs.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter))
            {
                var keyword = filter.Trim();

                query = query.Where(item =>
                    item.SKU.Contains(keyword) ||
                    item.ItemName.Contains(keyword));
            }

            return await query.OrderBy(x => x.Department).ToListAsync();
        }

        public Task<ItemCatalogModel?> GetItemCatalogById(Guid id)
        {
            return _appDbContext.ItemCatalogs
                .AsNoTracking()
                .FirstOrDefaultAsync(item => item.Id == id);
        }
    }
}
