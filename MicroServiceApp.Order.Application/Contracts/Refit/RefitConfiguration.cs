using MicroServiceApp.Order.Application.Contracts.Refit.PaymentService;
using MicroServiceApp.Shared.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroServiceApp.Order.Application.Contracts.Refit
{
    public static class RefitConfiguration
    {
        public static IServiceCollection AddRefitConfigurationExt(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddScoped<AuthenticatedHttpClientHandler>();
            services.AddScoped<ClientAuthenticatedHttpClientHandler>();

            services.AddOptions<IdentityOption>().BindConfiguration(nameof(IdentityOption)).ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddSingleton<IdentityOption>(sp => sp.GetRequiredService<IOptions<IdentityOption>>().Value);


            services.AddOptions<ClientSecretOption>().BindConfiguration(nameof(ClientSecretOption))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddSingleton<ClientSecretOption>(sp =>
                sp.GetRequiredService<IOptions<ClientSecretOption>>().Value);


            services.AddRefitClient<IPaymentService>().ConfigureHttpClient(configure =>
            {
                var addressUrlOption = configuration.GetSection(nameof(AddressUrlOption)).Get<AddressUrlOption>();


                configure.BaseAddress = new Uri(addressUrlOption!.PaymentUrl);
            }).AddHttpMessageHandler<AuthenticatedHttpClientHandler>()
               .AddHttpMessageHandler<ClientAuthenticatedHttpClientHandler>();


            return services;
        }
    }
}
