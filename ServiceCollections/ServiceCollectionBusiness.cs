using ArqPay.Infrastructure;
using ArqPay.Interfaces;
using ArqPay.Repositories;
using ArqPay.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ArqPay.ServiceCollection
{
  public static class ServiceCollectionBusiness
  {
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
      // Infrastructure
      services.AddSingleton<IDbConnectionFactory,DbConnectionFactory>();
      services.AddScoped<IDbConnectionFactory,DbConnectionFactory>();

      // Repositories
      services.AddScoped<IUserRepository, UserRepository>();
      services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

      // Services
      services.AddScoped<IUserService, UserService>();
      services.AddScoped<ILoginService, LoginService>();

      return services;
    }
  }
}
