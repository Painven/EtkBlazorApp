using Blazored.Toast;
using EtkBlazorApp.BL;
using EtkBlazorApp.BL.Templates.PriceListTemplates;
using EtkBlazorApp.DataAccess;
using EtkBlazorApp.Integration.Ozon;
using EtkBlazorApp.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Globalization;

namespace EtkBlazorApp
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
            //Blazor �����������
            services.AddRazorPages();
            services.AddServerSideBlazor().AddCircuitOptions(options => { options.DetailedErrors = true; });

            //Blazor ��������������
            services.AddHttpContextAccessor();

            //����������              
            services.AddTransient<IDatabaseProductCorrelator, HardOrdereSkuModelProductCorrelator>();
            services.AddTransient<IPriceLineLoadCorrelator, SimplePriceLineLoadCorrelator>();
            services.AddTransient<IOzonProductCorrelator, SimpleOzonProductCorrelator>();

            services.AddSingleton<ICurrencyChecker, CurrencyCheckerCbRf>();
            services.AddSingleton<IDatabaseAccess, EtkDatabaseDapperAccess>();
            services.AddSingleton<IProductStorage, ProductStorage>();
            services.AddSingleton<ITemplateStorage, TemplateStorage>();
            services.AddSingleton<IOrderStorage, OrderStorage>();
            services.AddSingleton<IManufacturerStorage, ManufacturerStorage>();
            services.AddSingleton<ILogStorage, LogStorage>();
            services.AddSingleton<ISettingStorage, SettingStorage>();
            services.AddSingleton<IAuthenticationDataStorage, AuthenticationDataStorage>();

            services.AddSingleton<SystemEventsLogger>();
            services.AddSingleton<NewOrdersNotificationService>();
            services.AddSingleton<UpdateManager>();
            services.AddSingleton<OzonSellerManager>();
            services.AddSingleton<PriceListManager>();
            services.AddSingleton<CronTaskService>();
            services.AddSingleton<RemoteTemplateFileLoaderFactory>();

            services.AddScoped<AuthenticationStateProvider, MyCustomAuthProvider>();
            services.AddScoped<UserLogger>();
            services.AddScoped<ReportManager>();      

            //���������
            services.AddBlazoredToast();

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });

            app.ApplicationServices.GetService<CronTaskService>();

            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("ru-RU");
        }
    }
}
