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

            return await query.ToListAsync();
        }

        public Task<ItemCatalogModel?> GetItemCatalogById(Guid id)
        {
            return _appDbContext.ItemCatalogs
                .AsNoTracking()
                .FirstOrDefaultAsync(item => item.Id == id);
        }

        public async Task<ItemCatalogModel> CreateItemCatalog(ItemCatalogModel itemCatalog)
        {
            if (itemCatalog.Id == Guid.Empty)
            {
                itemCatalog.Id = Guid.NewGuid();
            }

            _appDbContext.ItemCatalogs.Add(itemCatalog);
            await _appDbContext.SaveChangesAsync();

            return itemCatalog;
        }

        public async Task<ItemCatalogModel?> UpdateItemCatalog(ItemCatalogModel itemCatalog)
        {
            var existingItem = await _appDbContext.ItemCatalogs
                .FirstOrDefaultAsync(item => item.Id == itemCatalog.Id);

            if (existingItem == null)
            {
                return null;
            }

            existingItem.SKU = itemCatalog.SKU;
            existingItem.ItemName = itemCatalog.ItemName;
            existingItem.Department = itemCatalog.Department;
            existingItem.Category = itemCatalog.Category;
            existingItem.Brand = itemCatalog.Brand;
            existingItem.CurrentPrice = itemCatalog.CurrentPrice;
            existingItem.Cost = itemCatalog.Cost;
            existingItem.Status = itemCatalog.Status;

            await _appDbContext.SaveChangesAsync();

            return existingItem;
        }

        public async Task<bool> DeleteItemCatalog(Guid id)
        {
            var existingItem = await _appDbContext.ItemCatalogs
                .FirstOrDefaultAsync(item => item.Id == id);

            if (existingItem == null)
            {
                return false;
            }

            _appDbContext.ItemCatalogs.Remove(existingItem);
            await _appDbContext.SaveChangesAsync();

            return true;
        }
    }
}
