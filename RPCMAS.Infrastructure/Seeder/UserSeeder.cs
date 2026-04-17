using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RPCMAS.Core.Data;
using RPCMAS.Core.Entities;

namespace RPCMAS.Infrastructure.Seeder
{
    public static class UserSeeder
    {
        public static async Task SeedAsync(AppDbContext dbContext, ILogger logger)
        {
            var seededUsers = BuildSeedUsers();
            var seededUsernames = seededUsers.Select(user => user.Username).ToList();

            var existingUsernames = await dbContext.Users
                .Where(user => seededUsernames.Contains(user.Username))
                .Select(user => user.Username)
                .ToListAsync();

            var existingUsernameSet = existingUsernames.ToHashSet(StringComparer.OrdinalIgnoreCase);
            var usersToInsert = seededUsers
                .Where(user => !existingUsernameSet.Contains(user.Username))
                .ToList();

            if (usersToInsert.Count == 0)
            {
                logger.LogInformation("User seed skipped. All seeded users already exist.");
                return;
            }

            await dbContext.Users.AddRangeAsync(usersToInsert);
            await dbContext.SaveChangesAsync();

            logger.LogInformation("User seed completed. Inserted {Count} users.", usersToInsert.Count);
        }

        private static List<UserModel> BuildSeedUsers()
        {
            return
            [
                new UserModel
                {
                    Username = "deptsupervisor",
                    Password = "123qwe"
                },
                new UserModel
                {
                    Username = "merchmanager",
                    Password = "123qwe"
                },
                new UserModel
                {
                    Username = "storemanager",
                    Password = "123qwe"
                }
            ];
        }
    }
}
