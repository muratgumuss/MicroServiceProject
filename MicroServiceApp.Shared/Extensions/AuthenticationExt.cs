using MicroServiceApp.Shared.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace MicroServiceApp.Shared.Extensions
{
    public static class AuthenticationExt
    {
        public static IServiceCollection AddAuthenticationAndAuthorizationExt(this IServiceCollection services,
            IConfiguration configuration)
        {
            // Sign
            // Aud  => payment.api
            // Issuer => http://localhost:8080/realms/udemyTenant
            // TokenLifetime
            var identityOptions = configuration.GetSection(nameof(IdentityOption)).Get<IdentityOption>();


            services.AddAuthentication().AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.Authority = identityOptions.Address;
                options.Audience = identityOptions.Audience;
                options.RequireHttpsMetadata = false;
                options.MapInboundClaims = false;
                options.MetadataAddress = $"{identityOptions.Address.TrimEnd('/')}/.well-known/openid-configuration";
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    RoleClaimType = "roles",   
                    NameClaimType = "preferred_username"
                    //RoleClaimType = ClaimTypes.Role,
                    //NameClaimType = ClaimTypes.NameIdentifier
                };


                // AutomaticRefreshInterval: otomatik aralıkla metadata/JWKS yenileme (ör. 24saat)
                options.AutomaticRefreshInterval = TimeSpan.FromHours(24);

                // RefreshInterval: RequestRefresh() çağrıldıktan sonra beklenen min süre (ör. 30s)
                options.RefreshInterval = TimeSpan.FromSeconds(30);
            }).AddJwtBearer("ClientCredentialSchema", options =>
            {
                options.Authority = identityOptions.Address;
                options.Audience = identityOptions.Audience;
                options.RequireHttpsMetadata = false;
                options.MapInboundClaims = false;
                options.MetadataAddress = $"{identityOptions.Address.TrimEnd('/')}/.well-known/openid-configuration";

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidateIssuer = true
                };
            });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("InstructorPolicy", policy =>
                {
                    policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                    policy.RequireAuthenticatedUser();

                    // ClaimTypes.Email YERİNE direkt "email"
                    policy.RequireClaim("email");

                    // ClaimTypes.Role YERİNE direkt "roles" veya role kontrolü
                    // Yukarıda RoleClaimType = "roles" yaptığımız için RequireRole çalışacaktır
                    policy.RequireRole("instructor");
                });

                options.AddPolicy("Password", policy =>
                {
                    policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("email"); // Burada da "email" kullanın
                });

                options.AddPolicy("ClientCredential", policy =>
                {
                    policy.AuthenticationSchemes.Add("ClientCredentialSchema");
                    policy.RequireAuthenticatedUser();
                });
            });

            return services;
        }
    }
}
