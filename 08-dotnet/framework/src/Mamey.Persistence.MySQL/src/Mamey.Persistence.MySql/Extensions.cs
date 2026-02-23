using Mamey.Persistence.SQL;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace Mamey.Persistence.MySql;


public static class Extensions
{
    public static IMameyBuilder AddMySqlDb(this IMameyBuilder builder)
    {
        var options = builder.Services.GetOptions<MySqlOptions>("mySql");
        builder.Services.AddSingleton(options);
        builder.Services.AddSingleton(new UnitOfWorkTypeRegistry());


        return builder;
    }
    public static IServiceCollection AddMySqlDb<T>(this IServiceCollection services) where T : DbContext
    {
        var options = services.GetOptions<MySqlOptions>("postgres");
        //services.AddDbContext<T>(x =>
        //    x.UseMySQL(options.ConnectionString, option
        //        new MySqlServerVersion(new Version(8, 0, 21))
        //    )
        //);
        throw new NotImplementedException();
        return services;
    }
}
