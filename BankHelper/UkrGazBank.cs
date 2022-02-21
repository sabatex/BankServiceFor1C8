using sabatex.Extensions.ClassExtensions;
using sabatex.Extensions.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;



namespace BankServiceFor1C8.BankHelper;

public class UkrGazBank : ClientBankTo1CFormatConversion
{
    const int BufferSize = 1024;
    const string delimiter = "\n";
    string getValue(ref int pos, string s, string valueName)
    {
        if (pos == s.Length)
            throw new Exception(ErrorStrings.TryGetValueFromEndStream(valueName));

        if (s[pos] == '\n' || s[pos] == '\r')
            throw new Exception(ErrorStrings.TryGetValueFromEndStream(valueName));

        int start = pos;
        if (s[pos] == '"')
        {
            pos++;
            start = pos;
        }
        else
        {
            pos = s.IndexOf(';', start);
            if (pos == -1)
            {
                throw new Exception(ErrorStrings.DetermineEndDelimiterForValue(valueName));
            }
            return s.Substring(start, pos++ - start);
        }

        var result = new StringBuilder();
        // special string
        while (pos < s.Length)
        {
            switch (s[pos])
            {
                case '"':
                    pos++;
                    if (pos == s.Length)
                        throw new Exception(ErrorStrings.StringEndedBeforeReadValue(valueName));

                    if (s[pos] == ';')
                    {
                        pos++;
                        return result.ToString();
                    }

                    if (s[pos] == '"')
                    {
                        result.Append('"');
                        pos++;
                        break;
                    }
                    throw new Exception(ErrorStrings.StringFormatErrorForValue(valueName));
                default:
                    result.Append(s[pos]);
                    pos++;
                    break;

            }
        }
        throw new Exception(ErrorStrings.DetermineEndDelimiterForValue(valueName));
    }
    string getDateValue(ref int pos, string s, string valueName)
    {
        var ts = getValue(ref pos, s, valueName).Trim();
        if (!ts.TryDateTo1C8Date(out string result))
            throw new Exception(ErrorStrings.ConvertDataTo1C8FormatForField("'Дата операції'", ts));
        else
            return result;
    }
    decimal getDecimalDateValue(ref int pos, string s, string valueName)
    {
        var ts = getValue(ref pos, s, valueName).Trim();
        if (ts.Length == 0)
        {
            return 0;
        }
        else
        {
            if (!ts.TryToDecimal(out decimal result))
                throw new Exception(ErrorStrings.DoubleParse(ts));
            else
                return result;
        }
    }

    bool checkEnd(int position, ref char[] buffer)
    {
        foreach (var c in delimiter)
        {
            if (buffer[position++] != c) return false;
        }
        return true;
    }

    DocumentSection GetDocument(string s, string AccCode)
    {
        var document = new DocumentSection();
        int pos = 0;
        string DateOperation = getDateValue(ref pos, s, "DATA_VYP").Trim();
        string MFO = getValue(ref pos, s, "MFO").Trim();
        getValue(ref pos, s, "AC");
        string EDRPOU = getValue(ref pos, s, "OKPO").Trim();
        getValue(ref pos, s, "NAME");
        string DocummentNumber = getValue(ref pos, s, "ND").Trim();
        string DocumentDate = getDateValue(ref pos, s, "DATA_D").Trim();
        string OperationCode = getValue(ref pos, s, "DK").Trim();
        string ClientMFO = getValue(ref pos, s, "MFO_KOR").Trim();
        getValue(ref pos, s, "AC_KOR");
        string ClientEDRPOU = getValue(ref pos, s, "OKPO_KOR").Trim();
        string ClientName = getValue(ref pos, s, "NAME_KOR").Trim();
        getValue(ref pos, s, "CUR_TAG");
        string CurrencySymbolCode = getValue(ref pos, s, "CUR_CODE").Trim();
        getValue(ref pos, s, "CUR_RATE");
        getValue(ref pos, s, "AC_CUR_TAG");
        getValue(ref pos, s, "AccountCur");
        decimal Summ = getDecimalDateValue(ref pos, s, "SUM_PD_NOM");
        getValue(ref pos, s, "SUM_PD_EQ");
        string Description = getValue(ref pos, s, "PURPOSE").Trim();
        getValue(ref pos, s, "IN_RST_NO");
        getValue(ref pos, s, "IN_RST_EQ");
        getValue(ref pos, s, "OUT_RST_NO");
        getValue(ref pos, s, "OUT_RST_EQ");
        getValue(ref pos, s, "DB_SUM_NOM");
        getValue(ref pos, s, "CR_SUM_NOM");
        getValue(ref pos, s, "DB_SUM_EQ");
        getValue(ref pos, s, "CR_SUM_EQ");
        getValue(ref pos, s, "DAT_OST_OB");
        string Account = getValue(ref pos, s, "DB_IBAN");
        string ClientAccount = getValue(ref pos, s, "CR_IBAN");
        
        //string ClientBankName = getValue(ref pos, s, "Назва банка");
        // decimal Gryvna = getDecimalDateValue(ref pos, s, "Кредит");
        if (OperationCode == "2")
        {
            document.Сумма = Summ;
            document.ПолучательОКПО = ClientEDRPOU;
            document.ПолучательМФО = ClientMFO;
            document.ПолучательСчет = ClientAccount;
            document.Получатель = ClientName;

            document.ПлательщикОКПО = EDRPOU;
            document.ПлательщикМФО = MFO;
            document.ПлательщикСчет = Account;
        }
        else 
        {
            document.Сумма = Summ;
            document.ПолучательОКПО = EDRPOU;
            document.ПолучательМФО = MFO;
            document.ПолучательСчет = Account;

            document.ПлательщикОКПО = ClientEDRPOU;
            document.ПлательщикМФО = ClientMFO;
            document.ПлательщикСчет = ClientAccount;
            document.Плательщик = ClientName;
        }

        document.КодВалюты = CurrencySymbolCode;
        //Doc.ДатаПоступило = rec.DateOperation;
        document.ДокументИД = DocummentNumber;
        document.Дата = DateOperation;  //rec.DocumentDate;
        document.НазначениеПлатежа = Description;
        document.Номер = DocummentNumber;

        return document;
    }
    async Task<(string result, int chars)> GetLineFromStreamAsync(StreamReader stream, char[] buffer, int chars)
    {
        int readChars = await stream.ReadAsync(buffer, chars, BufferSize - chars);
        if (readChars == 0 && chars == 0)
            return (string.Empty,chars);
        int pos = 0;
        chars += readChars;
        var result = new StringBuilder();
        while (pos < chars - 1)
        {
            if (pos >= BufferSize - 24)
                throw new Exception(ErrorStrings.StrinLenght(BufferSize - 24));


            if (checkEnd(pos, ref buffer))
            {
                chars = chars - pos - delimiter.Length;
                if (chars != 0)
                    Array.Copy(buffer, pos + delimiter.Length, buffer, 0, chars);
                return (result.ToString(),chars);
            }
            result.Append(buffer[pos]);
            pos++;
        }
        chars = 0;
        return (result.ToString(),chars);
    }

    async Task ImportFromCSV(Stream stream, string AccNumber)
    {
        using (StreamReader reader = new StreamReader(stream, new Encoding1251()))
        {
            var lineDoc = 1;
            char[] buffer = new char[BufferSize];
            (string result, int chars) lineStr = ("", 0);
            try
            {
                do
                {
                    lineStr = await GetLineFromStreamAsync(reader, buffer, lineStr.chars);
                    if (lineStr.chars == 0 || lineStr.result.Length == 0)
                        continue;
                    if (lineStr.result[0] == 'D') continue; // start line

                    var doc = GetDocument(lineStr.result.TrimEnd()+";", AccNumber);
                    if (doc != null) this.Documents.Add(doc);
                    lineDoc++;
                } while (lineStr.chars != 0);
            }
            catch (Exception e)
            {
                throw new Exception(ErrorStrings.InLine(lineDoc, e.Message));
            }
        }
    }

    public override async Task ImportFromFileAsync(Stream stream, string fileExt, string accNumber = "")
    {
        switch (fileExt.ToUpper())
        {
            case ".CSV":
                await ImportFromCSV(stream, accNumber);
                break;
            default:
                throw new Exception($"Для клінтбанка УКРГАЗБанк, файл з розширенням {fileExt} не підтримується. Можливі типи файлів CSV");

        }
    }
}
