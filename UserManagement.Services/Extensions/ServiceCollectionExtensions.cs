using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using UserManagement.Models.Users;
using UserManagement.Services.Implementations;
using UserManagement.Services.Implementations.Validation;
using UserManagement.Services.Interfaces;

namespace UserManagement.Services.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        return services
            .AddScoped<IUserService, UserService>()
            .AddScoped<IValidator<CreateUserViewModel>, CreateUserViewModelValidator>()
            .AddScoped<IValidator<EditUserViewModel>, EditUserViewModelValidator>()
            .AddScoped<ICurrentDateProvider, CurrentDateProvider>();
    }
}
