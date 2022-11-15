using Blog_MVC.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Blog_MVC.Data
{
    public static class DataUtility
    {
        private const string _adminRole = "Administrator";
        private const string _modRole = "Moderator";
        private const string _adminEmail = "ayang@Mailinator.com";
        private const string _modEmail = "moderator@Mailinator.com";

        // static only instantiated once at build, performs same function each time its called directly
        public static string GetConnectionString(IConfiguration configuration)
        {
            string? connectionString = configuration.GetConnectionString("DefaultConnection");  // local database connection
            string? databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL"); // remote database connection

            // is database url present in Environment? if yes, connection is local & return connectionString -> else, build the connection string from provided database url
            return string.IsNullOrEmpty(databaseUrl) ? connectionString : BuildConnectionString(databaseUrl);
            // can be reformatted as the below if statement

            // if (string.IsNullOrEmpty(databaseUrl))
            // {return connectionString;} else {
            // return BuildConnectionString(databaseUrl);}
        }

        private static string BuildConnectionString(string databaseUrl)
        {
            var databaseUri = new Uri(databaseUrl);
            var userInfo = databaseUri.UserInfo.Split(':');
            var builder = new NpgsqlConnectionStringBuilder()
            {
                Host = databaseUri.Host,
                Port = databaseUri.Port,
                Username = userInfo[0],
                Password = userInfo[1],
                Database = databaseUri.LocalPath.TrimStart('/'),
                SslMode = SslMode.Require,
                TrustServerCertificate = true
            };
            return builder.ToString();
        }

        public static async Task ManageDataAsync(IServiceProvider svcProvider)
        {
            // this is an injection, but injection engine hasn't started yet -> small workaround to allow database access
            // obtains necessary services based on the IServiceProvider parameter 
            var dbContextSvc = svcProvider.GetRequiredService<ApplicationDbContext>();
            var configurationSvc = svcProvider.GetRequiredService<IConfiguration>();
            var userManagerSvc = svcProvider.GetRequiredService<UserManager<BlogUser>>();
            var roleManagerSvc = svcProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // aligns local & production databases by checking migrations and syncing them
            await dbContextSvc.Database.MigrateAsync();

            // Seed Default Roles
            await SeedRolesAsync(roleManagerSvc);

            // Seed Default Users
            await SeedUsersAsync(dbContextSvc, configurationSvc, userManagerSvc);
        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            if (!await roleManager.RoleExistsAsync(_adminRole))
            {
                await roleManager.CreateAsync(new IdentityRole(_adminRole));
            }

            if (!await roleManager.RoleExistsAsync(_modRole))
            {
                await roleManager.CreateAsync(new IdentityRole(_modRole));
            }
        }

        private static async Task SeedUsersAsync(ApplicationDbContext context, IConfiguration configuration, UserManager<BlogUser> userManager)
        {
            // checks ASPnetUsers table for existence of admin and creates if missing
            try
            {
                if (!context.Users.Any(u => u.Email == _adminEmail))
                {
                    BlogUser adminUser = new()
                    {
                        Email = _adminEmail,
                        UserName = _adminEmail,
                        FirstName = "Alex",
                        LastName = "Yang",
                        EmailConfirmed = true
                    };
                    await userManager.CreateAsync(adminUser, configuration["AdminPassword"] ?? Environment.GetEnvironmentVariable("AdminPassword"));
                    await userManager.AddToRoleAsync(adminUser, _adminRole);
                }

                // seeds Moderator
                if (!context.Users.Any(u => u.Email == _modEmail))
                {
                    BlogUser modUser = new()
                    {
                        Email = _modEmail,
                        UserName = _modEmail,
                        FirstName = "Evil",
                        LastName = "Alex",
                        EmailConfirmed = true
                    };
                    await userManager.CreateAsync(modUser, configuration["ModeratorPassword"] ?? Environment.GetEnvironmentVariable("ModeratorPassword"));
                    await userManager.AddToRoleAsync(modUser, _modRole);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
