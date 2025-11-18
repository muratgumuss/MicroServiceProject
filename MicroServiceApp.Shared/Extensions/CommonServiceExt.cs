using FluentValidation;
using FluentValidation.AspNetCore;
using MicroServiceApp.Shared.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MicroServiceApp.Shared.Extensions
{
    public static class CommonServiceExt
    {
        public static IServiceCollection AddCommonServiceExt(this IServiceCollection services, Type assembly)
        {
            services.AddHttpContextAccessor();
            services.AddMediatR(x => x.RegisterServicesFromAssemblyContaining(assembly));

            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblyContaining(assembly);
            services.AddScoped<IIdentityService, IdentityServiceFake>();

            services.AddAutoMapper(cfg => { }, assembly.Assembly);
            //services.AddExceptionHandler<GlobalExceptionHandler>();
            return services;
        }
    }
}
