using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RPCMAS.Core.Entities;

namespace RPCMAS.Core.Interfaces
{
    public interface IItemCatalogService
    {
        Task<List<ItemCatalogModel>> GetItemCatalogs();
    }
}
