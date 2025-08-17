using Infrastructure.Data;
using Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class Dependencies
{
    public static void ConfigureLocalDatabaseContexts(this IServiceCollection services, IConfiguration configuration)
    {
        // use real database
        // Requires LocalDB which can be installed with SQL Server Express 2016
        // https://www.microsoft.com/en-us/download/details.aspx?id=54284
        services.AddDbContext<ApplicationContext>(c =>
            c.UseSqlServer(configuration.GetConnectionString("QLToTrinhConnectionString")));

        // Add Identity DbContext
        services.AddDbContext<AppIdentityDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("QLToTrinhConnectionString")));
    }
}
