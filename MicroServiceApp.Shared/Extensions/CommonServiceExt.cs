using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroServiceApp.Shared.Extensions
{
    public static class CommonServiceExt
    {
        public static IServiceCollection AddCommonServiceExt(this IServiceCollection services, Type assembly)
        {
            services.AddHttpContextAccessor();
            services.AddMediatR(x => x.RegisterServicesFromAssemblyContaining(assembly));

            //services.AddFluentValidationAutoValidation();
            //services.AddValidatorsFromAssemblyContaining(assembly);
            //services.AddScoped<IIdentityService, IdentityService>();

            //services.AddAutoMapper(assembly);
            //services.AddExceptionHandler<GlobalExceptionHandler>();
            return services;
        }
    }
}
