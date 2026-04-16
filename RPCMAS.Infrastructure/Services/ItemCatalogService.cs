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

        public Task<List<ItemCatalogModel>> GetItemCatalogs(string? filter)
        {
            return _itemCatalogRepository.GetItemCatalogs(filter);
        }

        public Task<ItemCatalogModel?> GetItemCatalogById(Guid id)
        {
            return _itemCatalogRepository.GetItemCatalogById(id);
        }
    }
}
