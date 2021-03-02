using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Globalization;
using Microsoft.JSInterop;
using MudBlazor.Services;
using BankServiceFor1C8.Services;

namespace BankServiceFor1C8
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddMudServices();
            builder.Services.AddSingleton<SabatexLocalizer>();
            builder.Services.AddLocalization(options=> {
                options.ResourcesPath = "Resources";
            });
            var host = builder.Build();
            var js = host.Services.GetRequiredService<IJSRuntime>();
            var localCulture = await js.InvokeAsync<string>("localStorage.getItem", AppOptions.cultureSelector);
            if (string.IsNullOrWhiteSpace(localCulture))
            {
                await AppOptions.SetCulture(js, "uk");
            }
            else
            {
                if (!AppOptions.culturePresents.ContainsKey(localCulture))
                {
                    await AppOptions.SetCulture(js, "uk");
                }
                else
                {
                    await AppOptions.SetCulture(js, localCulture, false);
                }
            }
            await host.RunAsync();
       }
    }
}
