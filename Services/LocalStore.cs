using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankServiceFor1C8.Services
{
    public class LocalStore
    {
        readonly IJSRuntime js;
        public LocalStore(IJSRuntime jSRuntime)
        {
            js = jSRuntime;
        }
        public async Task<string> GetLocalValue(string valueName) => await js.InvokeAsync<string>("Sabatex.getLocalValue",valueName);
        public async Task SetLocalValue(string valueName, string value) => await js.InvokeVoidAsync("Sabatex.setLocalValue", valueName,value);
    }
}
