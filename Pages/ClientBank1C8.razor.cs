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
        ElementReference downloadFile;
        async Task HandleSelection(InputFileChangeEventArgs files)
        {
            resultFiles.Clear();
            error = "";
            foreach (var f in files.GetMultipleFiles(3))
            {
                var result = new ResultConvert();

                try
                {
                    using (MemoryStream memStream = new MemoryStream())
                    {
                        await f.OpenReadStream().CopyToAsync(memStream);
                        string r = _1CClientBankExchange.ConvertTo1CFormat(bankType, memStream, accountNumber);
                        result.FileName = "kb_to_1c" + f.Name + ".xml";
                        result.FileUrl = await JSRuntime.InvokeAsync<string>("getFileUrl", r);
                        await JSRuntime.InvokeAsync<object>("downloadFileResult", result.FileUrl, result.FileName);
                        resultFiles.Add(result);
                        
                    }

                }
                catch (Exception e)
                {
                    error = error + e.Message;
                }

            }
        }

    }
}
