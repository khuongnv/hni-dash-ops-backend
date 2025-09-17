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

            // Seed Roles
            await SeedRolesAsync(context);

            // Seed Permissions
            await SeedPermissionsAsync(context);

            // Seed Users
            await SeedUsersAsync(context);

            // Seed Role-Permission relationships
            await SeedRolePermissionsAsync(context);

            // Seed User-Role relationships
            await SeedUserRolesAsync(context);

            await context.SaveChangesAsync();
        }

        private static async Task SeedRolesAsync(ApplicationDbContext context)
        {
            var existingRoles = await context.Roles.ToListAsync();
            var roleNames = existingRoles.Select(r => r.Name).ToHashSet();
            
            var rolesToCreate = new List<Role>();
            
            var roleDefinitions = new[]
            {
                new { Name = "SuperAdmin", Description = "Super Administrator with full system access" },
                new { Name = "Admin", Description = "Administrator with management access" },
                new { Name = "Manager", Description = "Manager with limited administrative access" },
                new { Name = "User", Description = "Regular user with basic access" },
                new { Name = "Guest", Description = "Guest user with read-only access" }
            };

            foreach (var roleDef in roleDefinitions)
            {
                if (!roleNames.Contains(roleDef.Name))
                {
                    rolesToCreate.Add(new Role
                    {
                        Name = roleDef.Name,
                        Description = roleDef.Description,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            if (rolesToCreate.Any())
            {
                await context.Roles.AddRangeAsync(rolesToCreate);
            }
        }

        private static async Task SeedPermissionsAsync(ApplicationDbContext context)
        {
            var existingPermissions = await context.Permissions.ToListAsync();
            var permissionNames = existingPermissions.Select(p => p.Name).ToHashSet();
            
            var permissionsToCreate = new List<Permission>();
            
            var permissionDefinitions = new[]
            {
                new { Name = "users.read", Description = "Read user information", Resource = "users", Action = "read" },
                new { Name = "users.create", Description = "Create new users", Resource = "users", Action = "create" },
                new { Name = "users.update", Description = "Update user information", Resource = "users", Action = "update" },
                new { Name = "users.delete", Description = "Delete users", Resource = "users", Action = "delete" },
                new { Name = "roles.read", Description = "Read role information", Resource = "roles", Action = "read" },
                new { Name = "roles.create", Description = "Create new roles", Resource = "roles", Action = "create" },
                new { Name = "roles.update", Description = "Update role information", Resource = "roles", Action = "update" },
                new { Name = "roles.delete", Description = "Delete roles", Resource = "roles", Action = "delete" },
                new { Name = "permissions.read", Description = "Read permission information", Resource = "permissions", Action = "read" },
                new { Name = "permissions.create", Description = "Create new permissions", Resource = "permissions", Action = "create" },
                new { Name = "permissions.update", Description = "Update permission information", Resource = "permissions", Action = "update" },
                new { Name = "permissions.delete", Description = "Delete permissions", Resource = "permissions", Action = "delete" },
                new { Name = "system.admin", Description = "Full system administration access", Resource = "system", Action = "admin" },
                new { Name = "system.monitor", Description = "Monitor system status and logs", Resource = "system", Action = "monitor" }
            };

            foreach (var permDef in permissionDefinitions)
            {
                if (!permissionNames.Contains(permDef.Name))
                {
                    permissionsToCreate.Add(new Permission
                    {
                        Name = permDef.Name,
                        Description = permDef.Description,
                        Resource = permDef.Resource,
                        Action = permDef.Action,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            if (permissionsToCreate.Any())
            {
                await context.Permissions.AddRangeAsync(permissionsToCreate);
            }
        }

        private static async Task SeedUsersAsync(ApplicationDbContext context)
        {
            var existingUsers = await context.Users.ToListAsync();
            var usernames = existingUsers.Select(u => u.Username).ToHashSet();
            
            var usersToCreate = new List<User>();
            
            var userDefinitions = new[]
            {
                new { Username = "superadmin", Email = "superadmin@hnidashops.com", Password = "SuperAdmin@123", FirstName = "Super", LastName = "Administrator", Phone = "+84901234567", EmailConfirmed = true },
                new { Username = "admin", Email = "admin@hnidashops.com", Password = "Admin@123", FirstName = "System", LastName = "Administrator", Phone = "+84901234568", EmailConfirmed = true },
                new { Username = "manager1", Email = "manager1@hnidashops.com", Password = "Manager@123", FirstName = "John", LastName = "Manager", Phone = "+84901234569", EmailConfirmed = true },
                new { Username = "user1", Email = "user1@hnidashops.com", Password = "User@123", FirstName = "Jane", LastName = "User", Phone = "+84901234570", EmailConfirmed = true },
                new { Username = "guest1", Email = "guest1@hnidashops.com", Password = "Guest@123", FirstName = "Guest", LastName = "User", Phone = "+84901234571", EmailConfirmed = false }
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
                        PhoneNumber = userDef.Phone,
                        EmailConfirmed = userDef.EmailConfirmed,
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

        private static async Task SeedRolePermissionsAsync(ApplicationDbContext context)
        {
            // Only seed if no role permissions exist
            if (await context.RolePermissions.AnyAsync())
            {
                return;
            }

            var superAdminRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "SuperAdmin");
            var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
            var managerRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Manager");
            var userRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "User");
            var guestRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Guest");

            var allPermissions = await context.Permissions.ToListAsync();
            var rolePermissions = new List<RolePermission>();

            // SuperAdmin gets all permissions
            if (superAdminRole != null)
            {
                foreach (var permission in allPermissions)
                {
                    rolePermissions.Add(new RolePermission
                    {
                        RoleId = superAdminRole.Id,
                        PermissionId = permission.Id,
                        AssignedAt = DateTime.UtcNow,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            // Admin gets most permissions except system.admin
            if (adminRole != null)
            {
                var adminPermissions = allPermissions.Where(p => p.Name != "system.admin").ToList();
                foreach (var permission in adminPermissions)
                {
                    rolePermissions.Add(new RolePermission
                    {
                        RoleId = adminRole.Id,
                        PermissionId = permission.Id,
                        AssignedAt = DateTime.UtcNow,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            // Manager gets read permissions and limited write permissions
            if (managerRole != null)
            {
                var managerPermissions = allPermissions.Where(p => 
                    p.Action == "read" || 
                    (p.Resource == "users" && p.Action == "update") ||
                    p.Name == "system.monitor").ToList();
                
                foreach (var permission in managerPermissions)
                {
                    rolePermissions.Add(new RolePermission
                    {
                        RoleId = managerRole.Id,
                        PermissionId = permission.Id,
                        AssignedAt = DateTime.UtcNow,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            // User gets basic read permissions
            if (userRole != null)
            {
                var userPermissions = allPermissions.Where(p => 
                    p.Action == "read" && 
                    (p.Resource == "users" || p.Resource == "roles")).ToList();
                
                foreach (var permission in userPermissions)
                {
                    rolePermissions.Add(new RolePermission
                    {
                        RoleId = userRole.Id,
                        PermissionId = permission.Id,
                        AssignedAt = DateTime.UtcNow,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            // Guest gets only read permissions for users
            if (guestRole != null)
            {
                var guestPermissions = allPermissions.Where(p => 
                    p.Name == "users.read").ToList();
                
                foreach (var permission in guestPermissions)
                {
                    rolePermissions.Add(new RolePermission
                    {
                        RoleId = guestRole.Id,
                        PermissionId = permission.Id,
                        AssignedAt = DateTime.UtcNow,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            if (rolePermissions.Any())
            {
                await context.RolePermissions.AddRangeAsync(rolePermissions);
            }
        }

        private static async Task SeedUserRolesAsync(ApplicationDbContext context)
        {
            // Only seed if no user roles exist
            if (await context.UserRoles.AnyAsync())
            {
                return;
            }

            var superAdminUser = await context.Users.FirstOrDefaultAsync(u => u.Username == "superadmin");
            var adminUser = await context.Users.FirstOrDefaultAsync(u => u.Username == "admin");
            var managerUser = await context.Users.FirstOrDefaultAsync(u => u.Username == "manager1");
            var regularUser = await context.Users.FirstOrDefaultAsync(u => u.Username == "user1");
            var guestUser = await context.Users.FirstOrDefaultAsync(u => u.Username == "guest1");

            var superAdminRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "SuperAdmin");
            var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
            var managerRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Manager");
            var userRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "User");
            var guestRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Guest");

            var userRoles = new List<UserRole>();

            if (superAdminUser != null && superAdminRole != null)
            {
                userRoles.Add(new UserRole
                {
                    UserId = superAdminUser.Id,
                    RoleId = superAdminRole.Id,
                    AssignedAt = DateTime.UtcNow,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                });
            }

            if (adminUser != null && adminRole != null)
            {
                userRoles.Add(new UserRole
                {
                    UserId = adminUser.Id,
                    RoleId = adminRole.Id,
                    AssignedAt = DateTime.UtcNow,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                });
            }

            if (managerUser != null && managerRole != null)
            {
                userRoles.Add(new UserRole
                {
                    UserId = managerUser.Id,
                    RoleId = managerRole.Id,
                    AssignedAt = DateTime.UtcNow,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                });
            }

            if (regularUser != null && userRole != null)
            {
                userRoles.Add(new UserRole
                {
                    UserId = regularUser.Id,
                    RoleId = userRole.Id,
                    AssignedAt = DateTime.UtcNow,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                });
            }

            if (guestUser != null && guestRole != null)
            {
                userRoles.Add(new UserRole
                {
                    UserId = guestUser.Id,
                    RoleId = guestRole.Id,
                    AssignedAt = DateTime.UtcNow,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                });
            }

            if (userRoles.Any())
            {
                await context.UserRoles.AddRangeAsync(userRoles);
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