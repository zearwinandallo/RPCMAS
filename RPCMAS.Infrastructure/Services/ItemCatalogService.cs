using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RPCMAS.Core.Entities;
using RPCMAS.Core.Interfaces;
using RPCMAS.Core.Models;

namespace RPCMAS.Infrastructure.Services
{
    public class ItemCatalogService : IItemCatalogService
    {
        private readonly IItemCatalogRepository _itemCatalogRepository;

        public ItemCatalogService(IItemCatalogRepository itemCatalogRepository)
        {
            _itemCatalogRepository = itemCatalogRepository;
        }

        public Task<List<ItemCatalogModel>> GetItemCatalogs(ItemCatalogFilter? filter = null)
        {
            return _itemCatalogRepository.GetItemCatalogs(filter);
        }

        public Task<ItemCatalogModel?> GetItemCatalogById(Guid id)
        {
            return _itemCatalogRepository.GetItemCatalogById(id);
        }

        public Task<ItemCatalogModel> CreateItemCatalog(ItemCatalogModel itemCatalog)
        {
            return _itemCatalogRepository.CreateItemCatalog(itemCatalog);
        }

        public Task<ItemCatalogModel?> UpdateItemCatalog(ItemCatalogModel itemCatalog)
        {
            return _itemCatalogRepository.UpdateItemCatalog(itemCatalog);
        }

        public Task<bool> DeleteItemCatalog(Guid id)
        {
            return _itemCatalogRepository.DeleteItemCatalog(id);
        }
    }
}
