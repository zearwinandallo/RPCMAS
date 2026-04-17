using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RPCMAS.Core.Data;
using RPCMAS.Core.Entities;
using RPCMAS.Core.Models;

namespace RPCMAS.Infrastructure.Seeder
{
    public static class UserRoleSeeder
    {
        public static async Task SeedAsync(AppDbContext dbContext, ILogger logger)
        {
            var users = await dbContext.Users.ToListAsync();
            var roles = await dbContext.Roles.ToListAsync();

            var userMap = users.ToDictionary(user => user.Username, StringComparer.OrdinalIgnoreCase);
            var roleMap = roles.ToDictionary(role => role.RoleName, StringComparer.OrdinalIgnoreCase);

            var seedAssignments = BuildSeedUserRoles(userMap, roleMap);
            if (seedAssignments.Count == 0)
            {
                logger.LogWarning("User role seed skipped. Required users or roles are missing.");
                return;
            }

            var existingAssignments = await dbContext.UserRoles
                .Where(userRole => seedAssignments.Select(assignment => assignment.UserID).Contains(userRole.UserID))
                .Select(userRole => new { userRole.UserID, userRole.RoleID })
                .ToListAsync();

            var existingAssignmentSet = existingAssignments
                .Select(assignment => $"{assignment.UserID}:{assignment.RoleID}")
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var assignmentsToInsert = seedAssignments
                .Where(assignment => !existingAssignmentSet.Contains($"{assignment.UserID}:{assignment.RoleID}"))
                .ToList();

            if (assignmentsToInsert.Count == 0)
            {
                logger.LogInformation("User role seed skipped. All seeded user-role mappings already exist.");
                return;
            }

            await dbContext.UserRoles.AddRangeAsync(assignmentsToInsert);
            await dbContext.SaveChangesAsync();

            logger.LogInformation("User role seed completed. Inserted {Count} user-role mappings.", assignmentsToInsert.Count);
        }

        private static List<UserRoleModel> BuildSeedUserRoles(
            IReadOnlyDictionary<string, UserModel> userMap,
            IReadOnlyDictionary<string, RoleModel> roleMap)
        {
            var assignments = new List<UserRoleModel>();

            AddAssignment(assignments, userMap, roleMap, "deptsupervisor", UserRoleEnum.DepartmentSupervisor);
            AddAssignment(assignments, userMap, roleMap, "merchmanager", UserRoleEnum.MerchandisingManager);
            AddAssignment(assignments, userMap, roleMap, "storemanager", UserRoleEnum.StoreManager);

            return assignments;
        }

        private static void AddAssignment(
            ICollection<UserRoleModel> assignments,
            IReadOnlyDictionary<string, UserModel> userMap,
            IReadOnlyDictionary<string, RoleModel> roleMap,
            string username,
            UserRoleEnum role)
        {
            if (!userMap.TryGetValue(username, out var user))
            {
                return;
            }

            var roleName = role.ToString();
            if (!roleMap.TryGetValue(roleName, out var roleModel))
            {
                return;
            }

            assignments.Add(new UserRoleModel
            {
                UserID = user.ID,
                RoleID = roleModel.ID
            });
        }
    }
}
