using DynamicFormWebApp.Domain.Data;
using DynamicFormWebApp.Domain.Models;
using DynamicFormWebApp.Statics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace DynamicFormWebApp.Extensions;

public static class ExtensionServices
{
    public static void AppendMigrations(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate();
    }

    public static async void ApplyRoles(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var roleMappings = AppRolesHelper.GetRoleNameMappings();

        foreach (var roleMapping in roleMappings)
        {
            if (!await roleManager.RoleExistsAsync(roleMapping.Key))
                await roleManager.CreateAsync(RoleCreator(roleMapping.Key, roleMapping.Value));
        }

        #region AdminUserConfig

        var superAdminEmail = "m.p_996@hotmail.com";
        var superAdminPass = "123@asdfA";
        ApplicationUser? user = await userManager.FindByNameAsync("1");

        if (user == null)
        {
            await userManager.CreateAsync(new ApplicationUser
            {
                Email = superAdminEmail,
                UserName = "1",
                FullName = "Admin",
                EmailConfirmed = true,
            }, superAdminPass);

            user = await userManager.FindByEmailAsync(superAdminEmail);
        }

        await userManager.AddToRoleAsync(user, RolesName.SuperAdmin);

        #endregion
    }
    
    static ApplicationRole RoleCreator(string name, string persianName)
    {
        return new ApplicationRole()
        {
            Name = name,
            PersianName = persianName
        };
    }
    
    public static IServiceCollection ConfigureIdentityPolicies(this IServiceCollection services)
    {
        services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddRoleManager<RoleManager<ApplicationRole>>()
                //.AddDefaultUI()
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<ApplicationDbContext>();

        services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequiredLength = 8;
            options.Password.RequiredUniqueChars = 0;

            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;

            options.User.AllowedUserNameCharacters = "1234567890";
            options.User.RequireUniqueEmail = false;

            options.SignIn.RequireConfirmedEmail = false;
            options.SignIn.RequireConfirmedAccount = false;
            options.SignIn.RequireConfirmedPhoneNumber = false;
        });

        services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.HttpOnly = true;
            options.LoginPath = "/Account/Login";
            options.AccessDeniedPath = "/Identity/Account/AccessDenied";
            options.ExpireTimeSpan = TimeSpan.FromMinutes(40);
            options.SlidingExpiration = true;
        });

        services.AddAuthorization(options =>
        {
            options.AddPolicy(RolesName.SuperAdmin, policy =>
            {
                policy.RequireRole(RolesName.SuperAdmin);
            });

        });

        return services;
    }

}
public class AppRolesHelper
{
    public static Dictionary<string, string> GetRoleNameMappings()
    {
        var englishRoleFields = typeof(RolesName)
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(field => field.IsLiteral && !field.IsInitOnly && field.FieldType == typeof(string));

        var persianRoleFields = typeof(RolesPersianName)
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(field => field.IsLiteral && !field.IsInitOnly && field.FieldType == typeof(string))
            .ToDictionary(field => field.Name, field => field.GetValue(null)?.ToString());

        var roleMappings = new Dictionary<string, string>();

        foreach (var field in englishRoleFields)
        {
            if (persianRoleFields.TryGetValue(field.Name, out var persianValue))
            {
                var englishValue = field.GetValue(null)?.ToString();
                if (!string.IsNullOrEmpty(englishValue))
                {
                    roleMappings[englishValue] = persianValue;
                }
            }
        }

        return roleMappings;
    }

    public static string? GetPersianName(string roleName)
    {
        var mappings = GetRoleNameMappings();
        return mappings.GetValueOrDefault(roleName);
    }
}