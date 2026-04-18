using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using RPCMAS.Core.Entities;
using RPCMAS.Core.Interfaces;
using RPCMAS.Core.Models;

namespace RPCMAS.Infrastructure.Services
{
    public class ItemCatalogService : IItemCatalogService
    {
        private readonly IItemCatalogRepository _itemCatalogRepository;
        private readonly IDistributedCache _distributedCache;

        public ItemCatalogService(
            IItemCatalogRepository itemCatalogRepository,
            IDistributedCache distributedCache
            )
        {
            _itemCatalogRepository = itemCatalogRepository;
            _distributedCache = distributedCache;
        }

        public async Task<List<ItemCatalogModel>> GetItemCatalogs(string? filter)
        {
            if (!string.IsNullOrWhiteSpace(filter))
            {
                return await _itemCatalogRepository.GetItemCatalogs(filter);
            }

            var cacheValue = await _distributedCache.GetStringAsync("list_itemCatalog");

            if (!string.IsNullOrEmpty(cacheValue))
            {
                return JsonConvert.DeserializeObject<List<ItemCatalogModel>>(cacheValue) ?? new List<ItemCatalogModel>();
            }

            var items = await _itemCatalogRepository.GetItemCatalogs(filter);
            await _distributedCache.SetStringAsync("list_itemCatalog", JsonConvert.SerializeObject(items));
            return items;
        }

        public Task<ItemCatalogModel?> GetItemCatalogById(Guid id)
        {
            return _itemCatalogRepository.GetItemCatalogById(id);
        }
    }
}
