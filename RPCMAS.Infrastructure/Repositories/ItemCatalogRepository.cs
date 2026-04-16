using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RPCMAS.Core.Data;
using RPCMAS.Core.Entities;
using RPCMAS.Core.Interfaces;

namespace RPCMAS.Infrastructure.Repositories
{
    public class ItemCatalogRepository : IItemCatalogRepository
    {
        private readonly AppDbContext _appDbContext;
        public ItemCatalogRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public Task<List<ItemCatalogModel>> GetItemCatalogs()
        {
            return _appDbContext.ItemCatalogs.ToListAsync();
        }
    }
}
