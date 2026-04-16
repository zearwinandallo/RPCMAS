using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RPCMAS.Core.Entities;
using RPCMAS.Core.Interfaces;

namespace RPCMAS.Infrastructure.Services
{
    public class ItemCatalogService : IItemCatalogService
    {
        private readonly IItemCatalogRepository _itemCatalogRepository;
        public ItemCatalogService(IItemCatalogRepository itemCatalogRepository)
        {
            _itemCatalogRepository = itemCatalogRepository;
        }
        public Task<List<ItemCatalogModel>> GetItemCatalogs()
        {
           return _itemCatalogRepository.GetItemCatalogs();
        }
    }
}
