using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace BankServiceFor1C8.Services
{
    public class SabatexLocalizer
    {
        readonly IJSRuntime js;
        public SabatexLocalizer(IJSRuntime jSRuntime)
        {
            js = jSRuntime;
        }
        
        static string currentCulture;

        void setLocalCulture(string cultureString)
        {
            currentCulture = cultureString;
            var culture = new CultureInfo(cultureString);
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

        }

        public async Task<string> GetCulture()
        {
            if (string.IsNullOrEmpty(currentCulture))
            {
                string storedCulture = await js.InvokeAsync<string>("Sabatex.culture.get");
                if (string.IsNullOrEmpty(storedCulture))
                {
                    await SetCulture("uk");
                }
                else
                {
                    setLocalCulture(storedCulture);
                }
            }
            return currentCulture;
        }
        public async Task SetCulture(string culture)
        {
            await js.InvokeVoidAsync("Sabatex.culture.set", culture);
            setLocalCulture(culture);
        }


    }
}
