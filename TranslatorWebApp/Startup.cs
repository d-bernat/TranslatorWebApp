using System;
using System.Linq;
using Amadeus.Leisure.Service.Web.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
using TranslatorWebApp.Clients;
using TranslatorWebApp.Model;
using TranslatorWebApp.Services;
using TranslatorWebApp.Settings;
using TranslatorWebApp.utilities;

namespace TranslatorWebApp
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
            services.AddAppSettingsConfiguration<TranslatorsSettings>(Configuration);
            
            var translatorsSettings = Configuration.GetSettingsFromConfiguration<TranslatorsSettings>();
            var msTranslator = translatorsSettings.Items.First(i => i.Vendor.ToLower() == Vendors.Microsoft);
            
            services.AddHttpClient("MicrosoftLanguageDetectorClient", client =>
            {
                client.BaseAddress = new Uri(string.Format(msTranslator.Endpoint, "detect"));
                client.DefaultRequestHeaders.Add("accept", "application/json");
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", msTranslator.SubscriptionKey);
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Region", msTranslator.Region);
                client.DefaultRequestHeaders.Add("X-ClientTraceId", Guid.NewGuid().ToString());
            }).AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(250)))
                .SetHandlerLifetime(TimeSpan.FromMinutes(5));
            
            services.AddHttpClient("MicrosoftTranslatorClient", client =>
                {
                    client.BaseAddress = new Uri(string.Format(msTranslator.Endpoint, "translate"));
                    client.DefaultRequestHeaders.Add("accept", "application/json");
                    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", msTranslator.SubscriptionKey);
                    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Region", msTranslator.Region);
            }).AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(250)))
                .SetHandlerLifetime(TimeSpan.FromMinutes(5));



            services.AddSingleton<MicrosoftLanguageDetectorClient>();
            services.AddSingleton<GoogleLanguageDetectorClient>();
            services.AddSingleton<Func<TranslatorVendorEnum, ILanguageDetectorClient>>(serviceProvider => key =>
                key switch
                {
                    TranslatorVendorEnum.Microsoft => serviceProvider.GetService<MicrosoftLanguageDetectorClient>(),
                    TranslatorVendorEnum.Google => serviceProvider.GetService<GoogleLanguageDetectorClient>(),
                    _ => null
                }
            );
            
            services.AddSingleton<MicrosoftTranslatorClient>();
            services.AddSingleton<GoogleTranslatorClient>();
            services.AddSingleton<Func<TranslatorVendorEnum, ITranslatorClient>>(serviceProvider => key =>
                key switch
                {
                    TranslatorVendorEnum.Microsoft => serviceProvider.GetService<MicrosoftTranslatorClient>(),
                    TranslatorVendorEnum.Google => serviceProvider.GetService<GoogleTranslatorClient>(),
                    _ => null
                }
            );

            services.AddSingleton<ITranslatorService, TranslatorService>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

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
