using sabatex.Extensions.ClassExtensions;
using sabatex.Extensions.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BankServiceFor1C8.BankHelper
{
    public class IBankUA : ClientBankTo1CFormatConversion
    {
        const int BufferSize = 1024;
        const string delimiter = "\r\n";
        bool checkEnd(int position, ref char[] buffer)
        {
            foreach (var c in delimiter)
            {
                if (buffer[position++] != c) return false;
            }
            return true;
        }

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
                    throw new Exception(ErrorStrings.DetermineEndDelimiterForValue(valueName));
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

        string get1C8DateValue(ref int pos, string s, string valueName)
        {
            var ts = getValue(ref pos, s, valueName);
            if (!ts.TryDateTo1C8Date(out string result))
                throw new Exception(ErrorStrings.ConvertDataTo1C8FormatForField("'Дата операції'", ts));
            else
                return result;
        }

        decimal getDecimalDateValue(ref int pos, string s, string valueName)
        {
            var ts = getValue(ref pos, s, valueName);
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

        DocumentSection GetDocument(string s, string AccCode)
        {
            var document = new DocumentSection();
            int pos = 0;
            string EDRPOU = getValue(ref pos, s, "ЄДРПОУ");
            if (EDRPOU == "ЄДРПОУ") return null; // The header
            string MFO = getValue(ref pos, s, "МФО");
            string Account = getValue(ref pos, s, "Рахунок");
            string CurrencySymbolCode = getValue(ref pos, s, "Валюта");
            string DateOperation = get1C8DateValue(ref pos, s, "Дата операції");
            string OperationCode = getValue(ref pos, s, "Код операції");
            string ClientMFO = getValue(ref pos, s, "МФО банка");
            string ClientBankName = getValue(ref pos, s, "Назва банка");
            string ClientAccount = getValue(ref pos, s, "Рахунок кореспондента");
            string ClientEDRPOU = getValue(ref pos, s, "ЄДРПОУ кореспондента");
            string ClientName = getValue(ref pos, s, "Кореспондент");
            string DocummentNumber = getValue(ref pos, s, "Документ");
            string DocumentDate = get1C8DateValue(ref pos, s, "Дата документа");
            decimal Debit = getDecimalDateValue(ref pos, s, "Дебет");
            decimal Credit = getDecimalDateValue(ref pos, s, "Кредит");
            string Description = getValue(ref pos, s, "Призначення платежу");
            decimal Gryvna = getDecimalDateValue(ref pos, s, "Кредит");
            if (Debit != 0)
            {
                document.Сумма = Debit;
                document.ПолучательОКПО = ClientEDRPOU;
                document.ПолучательМФО = ClientMFO;
                document.ПолучательСчет = ClientAccount;
                document.Получатель = ClientName;

                document.ПлательщикОКПО = EDRPOU;
                document.ПлательщикМФО = MFO;
                document.ПлательщикСчет = Account;
            }
            else if (Credit != 0)
            {
                document.Сумма = Credit;
                document.ПолучательОКПО = EDRPOU;
                document.ПолучательМФО = MFO;
                document.ПолучательСчет = Account;

                document.ПлательщикОКПО = ClientEDRPOU;
                document.ПлательщикМФО = ClientMFO;
                document.ПлательщикСчет = ClientAccount;
                document.Плательщик = ClientName;
            }
            else if (OperationCode == "3")
            {
                return null;
            }
            else
                throw new Exception("В виписцы выдсутні сумми операції !!! ");

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
            bool checkEnd(int position, ref char[] buffer)
            {
                foreach (var c in delimiter)
                {
                    if (buffer[position++] != c) return false;
                }
                return true;
            }


            int readChars = await stream.ReadAsync(buffer, chars, BufferSize - chars);
            if (readChars == 0 && chars == 0)
                return (string.Empty, chars);
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
                    return (result.ToString(), chars);
                }
                result.Append(buffer[pos]);
                pos++;
            }
            chars = 0;
            return (result.ToString(), chars);
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
                        var doc = GetDocument(lineStr.result, AccNumber);
                        if (doc != null) AddDocument(doc);
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
                    throw new Exception($"Для клінтбанка iBank, файл з розширенням {fileExt} не підтримується. Можливі типи файлів CSV");

            }


        }
    }
}
