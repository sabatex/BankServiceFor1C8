using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using sabatex.V1C8.BankHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BankServiceFor1C8.Pages
{
    public partial class ClientBank1C8
    {
        private EBankType bankType = EBankType.iBankUA_TXT;

        string accountNumber = "";
        string error = "";

        async Task HandleSelection(InputFileChangeEventArgs files)
        {
            byte[] buffer = new byte[512000];
            error = "";
            foreach (var f in files.GetMultipleFiles())
            {
                try
                {
                    using (MemoryStream memStream = new MemoryStream(512000))
                    {
                        await f.OpenReadStream().CopyToAsync(memStream);
                        string r = _1CClientBankExchange.ConvertTo1CFormat(bankType, memStream, accountNumber);
                        string fileName = "kb_to_1c" + f.Name + ".xml";
                        await JSRuntime.InvokeAsync<object>("Sabatex.downloadFile", fileName,r);
                    }
                }
                catch (Exception e)
                {
                    error = error + e.Message;
                }
            }
        }
        void BankTypeChange(EBankType value)
        {
            bankType = value;
        }

    }
}
