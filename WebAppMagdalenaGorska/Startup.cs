using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AzureProjectMagdalenaGorska.Services;
using Microsoft.Azure.Cosmos;

namespace WebApp_OpenIDConnect_DotNet
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApp(Configuration.GetSection("AzureAd"));

            services.AddControllersWithViews(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            });
           services.AddRazorPages()
                .AddMicrosoftIdentityUI();

           services.AddSingleton<IMovieCosmosService>(options =>
            {
                string url = Configuration.GetSection("AzureCosmosDbSettings")
                .GetValue<string>("URL");
                string primaryKey = Configuration.GetSection("AzureCosmosDbSettings")
                .GetValue<string>("PrimaryKey");
                string dbName = Configuration.GetSection("AzureCosmosDbSettings")
                .GetValue<string>("DatabaseName");
                string containerName = Configuration.GetSection("AzureCosmosDbSettings")
                .GetValue<string>("ContainerName");

                var cosmosClient = new CosmosClient(
                    url,
                    primaryKey
                );

                return new MovieCosmosService(cosmosClient, dbName, containerName);
            });

            //var appInsightsConnString = Configuration.GetSection("ApplicationInsights")
            //    .GetValue<string>("ConnectionString");
            services.AddApplicationInsightsTelemetry(Configuration["APPINSIGHTS_CONNECTIONSTRING"]);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
