using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace BankServiceFor1C8
{
    public static class AppOptions
    {
        public const string cultureSelector = "localCulture";
        public const string defaultCulture = "uk";
        public static Dictionary<string, string> culturePresents = new Dictionary<string, string> { { "uk", "Українська" }, { "en", "English" } };
        public static async Task SetCulture(IJSRuntime js, string culture,bool store = true)
        {
            if (store) await js.InvokeVoidAsync("localStorage.setItem", AppOptions.cultureSelector, culture);
            var cultureInfo = new CultureInfo(culture);
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;


        }
    }
}
