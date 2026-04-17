using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RPCMAS.Core.Data;

namespace RPCMAS.Infrastructure.Seeder
{
    public static class UserAccessSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();

            await RoleSeeder.SeedAsync(
                dbContext,
                loggerFactory.CreateLogger("RoleSeeder"));

            await UserSeeder.SeedAsync(
                dbContext,
                loggerFactory.CreateLogger("UserSeeder"));

            await UserRoleSeeder.SeedAsync(
                dbContext,
                loggerFactory.CreateLogger("UserRoleSeeder"));
        }
    }
}
