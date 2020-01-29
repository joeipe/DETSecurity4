using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DETSecurity4.API.Authorization;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DETSecurity4.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddAuthorization(authorizationOptions =>
            {
                authorizationOptions.AddPolicy(
                    "MustBeWeatherMan",
                    policyBuilder =>
                    {
                        policyBuilder.RequireAuthenticatedUser();
                        policyBuilder.AddRequirements(
                                new MustBeWeatherManRequirement());
                    });

            });

            services.AddScoped<IAuthorizationHandler, MustBeWeatherManHandler>();

            services.AddAuthentication(
                    IdentityServerAuthenticationDefaults.AuthenticationScheme
                ).AddIdentityServerAuthentication(options =>
                {
                    options.Authority = "https://localhost:44335";
                    options.ApiName = "detsecurity4api";
                    options.ApiSecret = "apisecret"; //this is required only id AccessTokenType = AccessTokenType.Reference in the 'GetClients()'
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseAuthentication();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
