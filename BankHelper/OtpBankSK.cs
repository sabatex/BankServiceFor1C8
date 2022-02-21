using sabatex.Extensions.ClassExtensions;
using sabatex.Extensions.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;



namespace BankServiceFor1C8.BankHelper
{
    public class OtpBankSK : ClientBankTo1CFormatConversion
    {
        const string delimiter = "\r\n";
        const int BufferSize = 1024;

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
                {
                    if (valueName == "Mena")
                        return s.Substring(start);
                    else
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
                            return result.ToString();

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
        DocumentSection GetDocument(string s, string AccCode)
        {
            var doc = new DocumentSection();
            int pos = 0;



            string Dátumútovania = getValue(ref pos,s,"Dátumútovania");
            if (Dátumútovania == "Dátum útovania") return null;
            string Dátumvaluty = getValue(ref pos, s, "Dátumvaluty");
            string Protistrana = getValue(ref pos, s, "Protistrana");
            string Kódbanky = getValue(ref pos, s, "Kódbanky");
            string Komentár = getValue(ref pos, s, "Komentár");
            string CS = getValue(ref pos, s, "CS");
            string VS = getValue(ref pos, s, "VS");
            string SS = getValue(ref pos, s, "SS");
            string DB_CR = getValue(ref pos, s,"DB_CR");
            string Suma = getValue(ref pos, s, "Suma");
            string Mena = getValue(ref pos, s, "Mena");
            // next line

            if (Dátumútovania == "Dátum útovania") return null;
            try
            {
                doc.Дата = Dátumvaluty.DateTo1C8Date();
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("{0} for parametr Dátumvaluty", e.Message));
            }

            doc.КодВалюты = Mena;
            if (Suma.TryToDecimal(out decimal summ))
                doc.Сумма = summ;
            //doc.ДокументИД = 
            doc.НазначениеПлатежа = Komentár;
            //doc.Номер =  doc.Дата + summ.ToString();
            if (DB_CR != "DEBIT")
            {
                doc.ПлательщикСчет = Protistrana;
                doc.ПолучательСчет = AccCode;
            }
            else
            {
                doc.ПолучательСчет = Protistrana;
                doc.ПлательщикСчет = AccCode;
            }
            return doc;
        }
        string GetLineFromStream(StreamReader stream, char[] buffer, ref int chars)
        {
            int readChars = stream.Read(buffer, chars, BufferSize - chars);
            if (readChars == 0 && chars == 0)
                return string.Empty;
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
                    return result.ToString();
                }
                result.Append(buffer[pos]);
                pos++;
            }
            chars = 0;
            return result.ToString();
        }

        string ConvertTo1CFormat(Stream stream, string AccNumber)
        {
            using (StreamReader reader = new StreamReader(stream, new Encoding1251()))
            {
                var lineDoc = 1;
                var chars = 0;
                char[] buffer = new char[BufferSize];
                try
                {
                    do
                    {
                        var lineStr = GetLineFromStream(reader, buffer, ref chars);
                        if (chars == 0 || lineStr.Length == 0)
                            continue;
                        var doc = GetDocument(lineStr, AccNumber);
                        if (doc != null) Documents.Add(doc);
                        lineDoc++;
                    } while (chars != 0);
                    return GetAsXML();
                }
                catch (Exception e)
                {
                    throw new Exception(ErrorStrings.InLine(lineDoc, e.Message));
                }
            }
        }

        public override async Task ImportFromFileAsync(Stream stream, string fileExt, string accNumber = "")
        {
            await Task.FromResult(ConvertTo1CFormat(stream, accNumber));
        }

    }
}
