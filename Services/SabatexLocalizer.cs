using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace BankServiceFor1C8.Services
{
    public class SabatexLocalizer:IEnumerable<KeyValuePair<string,string>>
    {
        const string cultureSelector = "localCulture";
        const string defaultCulture = "uk";
        static Dictionary<string, string> culturePresents = new Dictionary<string, string>
        {
            { "uk", "Українська" },
            { "ru", "Русский" } 
        };

        readonly IJSRuntime js;
        public SabatexLocalizer(IJSRuntime jSRuntime)
        {
            js = jSRuntime;
        }

        public string GetCulturePresent(string culture) => culturePresents[culture];
 
        public async Task<string> GetCurrentCulturePresent()=>culturePresents[await GetStoredCulture()];
 

        public async Task<string> GetStoredCulture()
        {
            var storedCulture = await js.InvokeAsync<string>("localStorage.getItem", cultureSelector);
            if (string.IsNullOrWhiteSpace(storedCulture)) return defaultCulture;

            if (culturePresents.ContainsKey(storedCulture))
                return storedCulture;
            else
                return defaultCulture;
        }

        public async Task SetStoredCulture(string culture)
        {
            await js.InvokeVoidAsync("localStorage.setItem", cultureSelector, culture);
        }

        void setLocalCulture(string cultureString)
        {
            var culture = new CultureInfo(cultureString);
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
        }
        public async Task InitialStoredCulture()
        {
            var storedCulture = await GetStoredCulture();
            setLocalCulture(storedCulture);
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return culturePresents.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return culturePresents.GetEnumerator();
        }
    }
}