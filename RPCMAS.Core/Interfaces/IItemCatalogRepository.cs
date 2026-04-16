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
        Task<List<ItemCatalogModel>> GetItemCatalogs(string? filter);
        Task<ItemCatalogModel?> GetItemCatalogById(Guid id);
    }
}

#nullable restore
