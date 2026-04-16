using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RPCMAS.Core.Entities;
using RPCMAS.Core.Models;

#nullable enable

namespace RPCMAS.Core.Interfaces
{
    public interface IItemCatalogRepository
    {
        Task<List<ItemCatalogModel>> GetItemCatalogs(ItemCatalogFilter? filter = null);

        Task<ItemCatalogModel?> GetItemCatalogById(Guid id);
        Task<ItemCatalogModel> CreateItemCatalog(ItemCatalogModel itemCatalog);
        Task<ItemCatalogModel?> UpdateItemCatalog(ItemCatalogModel itemCatalog);
        Task<bool> DeleteItemCatalog(Guid id);
    }
}

#nullable restore
