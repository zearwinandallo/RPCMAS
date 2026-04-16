using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RPCMAS.Core.Entities;

namespace RPCMAS.Core.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<ItemCatalogModel> ItemCatalogs { get; set; }
        public DbSet<PriceChangeRequestDetailModel> PriceChangeRequestDetails { get; set; }
        public DbSet<PriceChangeRequestHeaderModel> PriceChangeRequestHeaders { get; set; }
    }
}
