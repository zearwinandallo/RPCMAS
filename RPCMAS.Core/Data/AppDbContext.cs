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
        public DbSet<UserModel> Users { get; set; }
        public DbSet<UserRoleModel> UserRoles { get; set; }
        public DbSet<RoleModel> Roles { get; set; }
        public DbSet<RefreshTokenModel> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PriceChangeRequestDetailModel>()
            .HasOne<PriceChangeRequestHeaderModel>()
            .WithMany(header => header.Details)
            .HasForeignKey(d => d.PriceChangeRequestHeaderId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserRoleModel>()
            .HasOne(userRole => userRole.User)
            .WithMany(user => user.UserRoles)
            .HasForeignKey(userRole => userRole.UserID)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserRoleModel>()
            .HasOne(userRole => userRole.Role)
            .WithMany()
            .HasForeignKey(userRole => userRole.RoleID)
            .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
