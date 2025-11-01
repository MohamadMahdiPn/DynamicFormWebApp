using DynamicFormWebApp.Domain.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DynamicFormWebApp.Domain;

public static class DynamicFormWebAppDomainRegistration
{
    public static IServiceCollection ConfigureDomainRegistrations(this IServiceCollection services,
        IConfiguration configuration)
    {
        #region ConnectionString

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseLazyLoadingProxies().UseSqlServer(configuration.GetConnectionString("DefaultConnection"), sqlOptions => sqlOptions.CommandTimeout(120));
        });


        #endregion



        return services;
    }
}