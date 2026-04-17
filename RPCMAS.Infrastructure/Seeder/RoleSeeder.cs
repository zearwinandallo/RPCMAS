using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RPCMAS.Core.Data;
using RPCMAS.Core.Entities;
using RPCMAS.Core.Models;

namespace RPCMAS.Infrastructure.Seeder
{
    public static class RoleSeeder
    {
        public static async Task SeedAsync(AppDbContext dbContext, ILogger logger)
        {
            var roleNames = Enum.GetNames<UserRoleEnum>();

            var existingRoleNames = await dbContext.Roles
                .Where(role => roleNames.Contains(role.RoleName))
                .Select(role => role.RoleName)
                .ToListAsync();

            var existingRoleNameSet = existingRoleNames.ToHashSet(StringComparer.OrdinalIgnoreCase);
            var rolesToInsert = roleNames
                .Where(roleName => !existingRoleNameSet.Contains(roleName))
                .Select(roleName => new RoleModel
                {
                    RoleName = roleName
                })
                .ToList();

            if (rolesToInsert.Count == 0)
            {
                logger.LogInformation("Role seed skipped. All seeded roles already exist.");
                return;
            }

            await dbContext.Roles.AddRangeAsync(rolesToInsert);
            await dbContext.SaveChangesAsync();

            logger.LogInformation("Role seed completed. Inserted {Count} roles.", rolesToInsert.Count);
        }
    }
}
