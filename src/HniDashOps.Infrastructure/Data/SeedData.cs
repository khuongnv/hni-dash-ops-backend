using Microsoft.EntityFrameworkCore;
using HniDashOps.Core.Entities;
using System.Security.Cryptography;
using System.Text;

namespace HniDashOps.Infrastructure.Data
{
    public static class SeedData
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            // Ensure database is created
            await context.Database.EnsureCreatedAsync();

            // Seed Groups
            await SeedGroupsAsync(context);

            // Seed Menus
            await SeedMenusAsync(context);

            // Seed Users
            await SeedUsersAsync(context);

            // Seed Group-User relationships
            await SeedGroupUsersAsync(context);

            // Seed Group-Menu relationships
            await SeedGroupMenusAsync(context);

            // Seed Categories
            await SeedCategoriesAsync(context);

            // Seed Departments
            await SeedDepartmentsAsync(context);

            await context.SaveChangesAsync();
        }

        private static async Task SeedGroupsAsync(ApplicationDbContext context)
        {
            var existingGroups = await context.Groups.ToListAsync();
            var groupNames = existingGroups.Select(g => g.Name).ToHashSet();
            
            var groupsToCreate = new List<Group>();
            
            var groupDefinitions = new[]
            {
                new { Name = "SuperAdmin", Description = "Super Administrator Group with full access" },
                new { Name = "SubAdmin", Description = "Sub Administrator Group with limited admin access" },
                new { Name = "Member", Description = "Regular Member Group with basic access" },
                new { Name = "Guest", Description = "Guest Group with read-only access" }
            };

            foreach (var groupDef in groupDefinitions)
            {
                if (!groupNames.Contains(groupDef.Name))
                {
                    groupsToCreate.Add(new Group
                    {
                        Name = groupDef.Name,
                        Description = groupDef.Description,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            if (groupsToCreate.Any())
            {
                await context.Groups.AddRangeAsync(groupsToCreate);
            }
        }

        private static async Task SeedUsersAsync(ApplicationDbContext context)
        {
            var existingUsers = await context.Users.ToListAsync();
            var usernames = existingUsers.Select(u => u.Username).ToHashSet();
            
            var usersToCreate = new List<User>();
            
            var userDefinitions = new[]
            {
                new { Username = "superadmin", Email = "superadmin@hnidashops.com", Password = "admin@123", FirstName = "Super", LastName = "Administrator", RoleId = UserRole.SuperAdmin },
                new { Username = "subadmin", Email = "subadmin@hnidashops.com", Password = "sub@123", FirstName = "Sub", LastName = "Administrator", RoleId = UserRole.SubAdmin },
                new { Username = "member", Email = "member@hnidashops.com", Password = "mem@123", FirstName = "Regular", LastName = "Member", RoleId = UserRole.Member },
                new { Username = "guest", Email = "guest@hnidashops.com", Password = "guest@123", FirstName = "Guest", LastName = "User", RoleId = UserRole.Guest }
            };

            foreach (var userDef in userDefinitions)
            {
                if (!usernames.Contains(userDef.Username))
                {
                    usersToCreate.Add(new User
                    {
                        Username = userDef.Username,
                        Email = userDef.Email,
                        PasswordHash = HashPassword(userDef.Password),
                        FirstName = userDef.FirstName,
                        LastName = userDef.LastName,
                        RoleId = userDef.RoleId,
                        EmailConfirmed = true,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            if (usersToCreate.Any())
            {
                await context.Users.AddRangeAsync(usersToCreate);
            }
        }

        private static async Task SeedMenusAsync(ApplicationDbContext context)
        {
            var existingMenus = await context.Menus.ToListAsync();
            var menuNames = existingMenus.Select(m => m.Name).ToHashSet();
            
            var menusToCreate = new List<Menu>();
            
            var menuDefinitions = new[]
            {
                new { Name = "Dashboard", Href = "/dashboard", Icon = "fas fa-tachometer-alt", Order = 1, ParentId = (int?)null, Level = 1, Description = "Main Dashboard" },
                new { Name = "User Management", Href = "/users", Icon = "fas fa-users", Order = 2, ParentId = (int?)null, Level = 1, Description = "Manage Users" },
                new { Name = "Group Management", Href = "/groups", Icon = "fas fa-layer-group", Order = 3, ParentId = (int?)null, Level = 1, Description = "Manage Groups" },
                new { Name = "Menu Management", Href = "/menus", Icon = "fas fa-bars", Order = 4, ParentId = (int?)null, Level = 1, Description = "Manage Menus" },
                new { Name = "Department Management", Href = "/departments", Icon = "fas fa-building", Order = 5, ParentId = (int?)null, Level = 1, Description = "Manage Departments" },
                new { Name = "Category Management", Href = "/categories", Icon = "fas fa-tags", Order = 6, ParentId = (int?)null, Level = 1, Description = "Manage Categories" },
                new { Name = "System Notifications", Href = "/notifications", Icon = "fas fa-bell", Order = 7, ParentId = (int?)null, Level = 1, Description = "System Notifications" },
                new { Name = "System Settings", Href = "/settings", Icon = "fas fa-cog", Order = 8, ParentId = (int?)null, Level = 1, Description = "System Settings" }
            };

            foreach (var menuDef in menuDefinitions)
            {
                if (!menuNames.Contains(menuDef.Name))
                {
                    menusToCreate.Add(new Menu
                    {
                        Name = menuDef.Name,
                        Href = menuDef.Href,
                        Icon = menuDef.Icon,
                        Order = menuDef.Order,
                        ParentId = menuDef.ParentId,
                        Level = menuDef.Level,
                        Description = menuDef.Description,
                        IsVisible = true,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            if (menusToCreate.Any())
            {
                await context.Menus.AddRangeAsync(menusToCreate);
            }
        }

        private static async Task SeedGroupUsersAsync(ApplicationDbContext context)
        {
            var existingGroupUsers = await context.GroupUsers.ToListAsync();
            if (existingGroupUsers.Any()) return; // Already seeded

            var users = await context.Users.ToListAsync();
            var groups = await context.Groups.ToListAsync();
            
            var groupUserMappings = new List<GroupUser>();

            foreach (var user in users)
            {
                var groupName = user.RoleId switch
                {
                    UserRole.SuperAdmin => "SuperAdmin",
                    UserRole.SubAdmin => "SubAdmin", 
                    UserRole.Member => "Member",
                    UserRole.Guest => "Guest",
                    _ => "Guest"
                };

                var group = groups.FirstOrDefault(g => g.Name == groupName);
                if (group != null)
                {
                    groupUserMappings.Add(new GroupUser
                    {
                        UserId = user.Id,
                        GroupId = group.Id,
                        AssignedAt = DateTime.UtcNow,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            if (groupUserMappings.Any())
            {
                await context.GroupUsers.AddRangeAsync(groupUserMappings);
            }
        }

        private static async Task SeedGroupMenusAsync(ApplicationDbContext context)
        {
            var existingGroupMenus = await context.GroupMenus.ToListAsync();
            if (existingGroupMenus.Any()) return; // Already seeded

            var groups = await context.Groups.ToListAsync();
            var menus = await context.Menus.ToListAsync();
            
            var groupMenuMappings = new List<GroupMenu>();

            foreach (var group in groups)
            {
                var menuIds = group.Name switch
                {
                    "SuperAdmin" => menus.Select(m => m.Id).ToList(), // All menus
                    "SubAdmin" => menus.Where(m => m.Name != "System Settings").Select(m => m.Id).ToList(),
                    "Member" => menus.Where(m => new[] { "Dashboard", "System Notifications" }.Contains(m.Name)).Select(m => m.Id).ToList(),
                    "Guest" => menus.Where(m => m.Name == "Dashboard").Select(m => m.Id).ToList(),
                    _ => new List<int>()
                };

                foreach (var menuId in menuIds)
                {
                    groupMenuMappings.Add(new GroupMenu
                    {
                        GroupId = group.Id,
                        MenuId = menuId,
                        AssignedAt = DateTime.UtcNow,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            if (groupMenuMappings.Any())
            {
                await context.GroupMenus.AddRangeAsync(groupMenuMappings);
            }
        }

        private static async Task SeedCategoriesAsync(ApplicationDbContext context)
        {
            var existingCategories = await context.Categories.ToListAsync();
            var categoryCodes = existingCategories.Select(c => c.Code).ToHashSet();
            
            var categoriesToCreate = new List<Category>();
            
            var categoryDefinitions = new[]
            {
                new { Code = "GENERAL", Name = "General", ParentId = (int?)null, Level = 1, Order = 1, Color = "#007bff", Icon = "fas fa-folder", Type = "general" },
                new { Code = "SYSTEM", Name = "System", ParentId = (int?)null, Level = 1, Order = 2, Color = "#28a745", Icon = "fas fa-cog", Type = "system" },
                new { Code = "USER_MGMT", Name = "User Management", ParentId = (int?)null, Level = 1, Order = 3, Color = "#ffc107", Icon = "fas fa-users", Type = "management" }
            };

            foreach (var catDef in categoryDefinitions)
            {
                if (!categoryCodes.Contains(catDef.Code))
                {
                    categoriesToCreate.Add(new Category
                    {
                        Code = catDef.Code,
                        Name = catDef.Name,
                        ParentId = catDef.ParentId,
                        Level = catDef.Level,
                        Order = catDef.Order,
                        Color = catDef.Color,
                        Icon = catDef.Icon,
                        Type = catDef.Type,
                        IsVisible = true,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            if (categoriesToCreate.Any())
            {
                await context.Categories.AddRangeAsync(categoriesToCreate);
            }
        }

        private static async Task SeedDepartmentsAsync(ApplicationDbContext context)
        {
            var existingDepartments = await context.Departments.ToListAsync();
            var departmentCodes = existingDepartments.Select(d => d.Code).ToHashSet();
            
            var departmentsToCreate = new List<Department>();
            
            var departmentDefinitions = new[]
            {
                new { Code = "IT", Name = "Information Technology", ParentId = (int?)null, Level = 1, MapId = "IT_DEPT" },
                new { Code = "HR", Name = "Human Resources", ParentId = (int?)null, Level = 1, MapId = "HR_DEPT" },
                new { Code = "FINANCE", Name = "Finance", ParentId = (int?)null, Level = 1, MapId = "FIN_DEPT" },
                new { Code = "MARKETING", Name = "Marketing", ParentId = (int?)null, Level = 1, MapId = "MKT_DEPT" }
            };

            foreach (var deptDef in departmentDefinitions)
            {
                if (!departmentCodes.Contains(deptDef.Code))
                {
                    departmentsToCreate.Add(new Department
                    {
                        Code = deptDef.Code,
                        Name = deptDef.Name,
                        ParentId = deptDef.ParentId,
                        Level = deptDef.Level,
                        MapId = deptDef.MapId,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            if (departmentsToCreate.Any())
            {
                await context.Departments.AddRangeAsync(departmentsToCreate);
            }
        }

        private static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
}