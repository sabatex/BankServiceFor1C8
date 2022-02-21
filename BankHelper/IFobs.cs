using sabatex.Extensions.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;


namespace BankServiceFor1C8.BankHelper
{
    public class IFobs : ClientBankTo1CFormatConversion
    {
        const int BlockSize = 660;
        const int BufferSize = 1024;
        const string delimiter = "\r\n";

        string GetSubstring(string s, int start, int end)=>s.Substring(start - 1, end - start + 1).Trim();


        DocumentSection GetDocument(string s)
        {
            string convertDate(string str)=>"20" + str.Substring(0, 2) + "-" + str.Substring(2, 2) + "-" + str.Substring(4, 2);

            if (s.Length != BlockSize)
                throw new Exception(ErrorStrings.UnsupportedFormatFileForiFobs());

            // МФО банку платника 1-9 len=9
            string bankID = GetSubstring(s, 1, 9);
            // Номер рахунку платника 10-28 len=19
            string accountNumberOfPayer = GetSubstring(s, 10, 28);
            // IBAN платника 29-57 len=29
            string iBANOfPayer = GetSubstring(s, 29, 57);
            // МФО банку одержувача 58-66 len= 9
            string beneficiarysDankID = GetSubstring(s, 58, 66);
            // Номер рахунку одержувача 67-85 len=19
            string accountNumberOfBeneficiary = GetSubstring(s, 67, 85);
            // IBAN одержувача 86-114 len=29
            var iBANofBeneficiary = GetSubstring(s, 86, 114);
            // Тип фінансової операції. 1 (кредит) 0 дебет 115-115 len=1
            var typeOfFinancialOperation = GetSubstring(s, 115, 115);
            // Сума (у копійках) 116-131 len=16
            var amount = GetSubstring(s, 116, 131);
            // Вид документа 132-133 len=2
            var documentType = GetSubstring(s, 132, 133);
            // Номер документа 134-143 len=10
            var documentNumber = GetSubstring(s, 134, 143);
            // Код валюти 144-146
            var codeOfCurrency = GetSubstring(s, 144, 146);
            // Дата документа 147-152
            var documentDate = GetSubstring(s, 147, 152);
            // Дата отримання документа в банку 153-158
            var dateWhenDocumentWasReceivedInTheBank = GetSubstring(s, 153, 158);
            // Найменування платника 159-196
            var nameOfPayer = GetSubstring(s, 159, 196);
            // Найменування одержувача 197-234
            var nameOfBeneficiary = GetSubstring(s, 197, 234);
            // Призначення платежу 235-395
            var purposeOfPayment = GetSubstring(s, 235, 395);
            // Додаткові реквізити 396-454
            var additionalRequisites = GetSubstring(s, 396, 454);
            // Код призначення платежу 455-457
            var codeOfPurposeOfPayment = GetSubstring(s, 455, 457);
            // ------------ 458-459
            // Ідентифікаційний код платника (код ЄДРПОУ) 460-473
            var taxpayerNumberOfPayer = GetSubstring(s, 460, 473);
            // Ідентифікаційний код одержувача (код ЄДРПОУ) 474-487
            var taxpayerNumberOfBeneficiary = GetSubstring(s, 474, 487);
            // Унікальний номер документа 488-496
            var documentIdentifier = GetSubstring(s, 488, 496);
            // Дата создания документа 655-660
            var documentCreateDate = GetSubstring(s, 655, 660);

            if (documentType == "1")
            {
                DocumentSection doc = new DocumentSection();
                doc.Номер = documentNumber; //Номер документа
                doc.КодВалюты = codeOfCurrency;// Код валюты
                doc.НазначениеПлатежа = purposeOfPayment; //Назначение платежа

                if (documentCreateDate.Length == 6)
                {
                    doc.Дата = convertDate(documentCreateDate);
                }
                else if (documentDate.Length == 6)
                {
                    doc.Дата = convertDate(documentDate); ;
                }
                else
                {
                    throw new Exception();
                }

                //doc.ДокументИД = 
                if (typeOfFinancialOperation == "0") //Тип финансовой операции 
                {
                    doc.Плательщик = nameOfPayer; //Наименование плательщика 
                    doc.ПлательщикМФО = bankID;//МФО банка плательщика
                    doc.ПлательщикОКПО = taxpayerNumberOfPayer;//Идентификационный код плательщика (код ЕГРПОУ)
                    doc.ПлательщикСчет = iBANOfPayer.Length == 0 ? accountNumberOfPayer : iBANOfPayer; //Номер счета плательщика

                    doc.Получатель = nameOfBeneficiary; //Наименование получателя
                    doc.ПолучательМФО = beneficiarysDankID;//МФО банка получателя
                    doc.ПолучательОКПО = taxpayerNumberOfBeneficiary;//Идентификационный код плательщика (код ЕГРПОУ) 
                    doc.ПолучательСчет = iBANofBeneficiary.Length == 0 ? accountNumberOfBeneficiary : iBANofBeneficiary;//Номер счета получателя
                }
                else
                {
                    doc.Плательщик = nameOfBeneficiary; //Наименование получателя
                    doc.ПлательщикМФО = beneficiarysDankID;//МФО банка получателя
                    doc.ПлательщикОКПО = taxpayerNumberOfBeneficiary;//Идентификационный код плательщика (код ЕГРПОУ) 
                    doc.ПлательщикСчет = iBANofBeneficiary.Length == 0 ? accountNumberOfBeneficiary : iBANofBeneficiary;//Номер счета получателя


                    doc.Получатель = nameOfPayer;//Наименование плательщика
                    doc.ПолучательМФО = bankID;//МФО банка плательщика
                    doc.ПолучательОКПО = taxpayerNumberOfPayer;//Идентификационный код плательщика (код ЕГРПОУ)
                    doc.ПолучательСчет = iBANOfPayer.Length == 0 ? accountNumberOfPayer : iBANOfPayer; //Номер счета плательщика

                }
                if (decimal.TryParse(amount, out decimal sum))
                {
                    doc.Сумма = sum * 0.01m; //Сумма (в копейках)
                }
                else
                {
                    throw new Exception(ErrorStrings.DoubleParse(amount));
                }

                return doc;
            }
            return null;

        }
        async Task<Tuple<string,int>> GetLineFromStreamAsync(StreamReader stream, char[] buffer,int chars)
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
                return new Tuple<string, int>(string.Empty,chars);
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
                    return new Tuple<string, int>(result.ToString(),chars);
                }
                result.Append(buffer[pos]);
                pos++;
            }
            chars = 0;
            return new Tuple<string, int>(result.ToString(),chars);
        }
        public async Task ImportFromXML(Stream stream, string accNumber = "")
        {
            bool tryGetXMLDocument(XmlNode node,out DocumentSection doc)
        {
            var success = true;
            bool tryConvertDate(string str,out string result)
            {
                if (str.Length == 8)
                { 
                    result = str.Substring(0, 4) + "-" + str.Substring(4, 2) + "-" + str.Substring(6, 2);
                    return true;
                }
                else
                {
                    Errors.Add($"Помилка конвертації дати {str} в рядку {LineDoc}");
                    result = string.Empty;
                    return false;
                }
            }
            bool tryGetParametr(string attribute,string description,out string result)
            {
                try
                {
                    result = node.Attributes[attribute].Value;
                    return true;
                }
                catch (Exception e)
                {
                    result = string.Empty;
                    Warnings.Add($"Помилка зчитування параметра {attribute} ({description}) в рядку {LineDoc}: {e.Message}");
                    return false;
                }
            }
            
            doc = new DocumentSection();

            // ARCDATE Дата создания документа
            if (tryGetParametr("ARCDATE", "Дата создания документа", out string s))
            {
                if (tryConvertDate(s, out string date))
                    doc.Дата = date;
                else
                    success = false;

            }
            else success = false;

            // BANKDATE Дата отримання документа в банку 153-158
            // DOCUMENTDATE Дата документа
            
            if (tryGetParametr("DOCUMENTNO", "Номер документа", out s))
                doc.Номер = s;
            else
                success = false;

            // CURRSYMBOLCODE символьна назва валюти
            if (tryGetParametr("CURRENCYID", "Код валюти", out s))
                doc.КодВалюты = s;
            else
                success = false;

            if (tryGetParametr("PLATPURPOSE", "Призначення платежу", out s))
                doc.НазначениеПлатежа = s;
            else
                success = false;

            tryGetParametr("ACCOUNTNO", "Номер рахунку платника", out string accountNumberOfPayer);
            tryGetParametr("IBAN", "IBAN платника", out string iBANOfPayer);
            tryGetParametr("BANKID", "МФО банку платника", out string bankID);
            tryGetParametr("CORRBANKID","МФО банку одержувач",out string beneficiarysDankID);
            tryGetParametr("CORRACCOUNTNO", "Номер рахунку одержувача",out string accountNumberOfBeneficiary);
            tryGetParametr("CORRIBAN", "IBAN одержувача", out string iBANofBeneficiary);
            tryGetParametr("CORRIDENTIFYCODE", "Ідентифікаційний код одержувача (код ЄДРПОУ)", out string taxpayerNumberOfBeneficiary);
            tryGetParametr("CORRCONTRAGENTSNAME", "Найменування одержувача",out string nameOfBeneficiary);
            tryGetParametr("IDENTIFYCODE","Ідентифікаційний код платника (код ЄДРПОУ)",out string taxpayerNumberOfPayer);
            tryGetParametr("CONTRAGENTSNAME", "Найменування платника",out string nameOfPayer);
             
            if (tryGetParametr("OPERATIONID", "Тип фінансової операції. 1 (кредит) 0 дебет", out s))
            {
                if (s == "0") //Тип финансовой операции 
                {
                    doc.Плательщик = nameOfPayer; //Наименование плательщика 
                    doc.ПлательщикМФО = bankID;//МФО банка плательщика
                    doc.ПлательщикОКПО = taxpayerNumberOfPayer;//Идентификационный код плательщика (код ЕГРПОУ)
                    doc.ПлательщикСчет = iBANOfPayer.Length == 0 ? accountNumberOfPayer : iBANOfPayer; //Номер счета плательщика

                    doc.Получатель = nameOfBeneficiary; //Наименование получателя
                    doc.ПолучательМФО = beneficiarysDankID;//МФО банка получателя
                    doc.ПолучательОКПО = taxpayerNumberOfBeneficiary;//Идентификационный код плательщика (код ЕГРПОУ) 
                    doc.ПолучательСчет = iBANofBeneficiary.Length == 0 ? accountNumberOfBeneficiary : iBANofBeneficiary;//Номер счета получателя
                }
                else
                {
                    doc.Плательщик = nameOfBeneficiary; //Наименование получателя
                    doc.ПлательщикМФО = beneficiarysDankID;//МФО банка получателя
                    doc.ПлательщикОКПО = taxpayerNumberOfBeneficiary;//Идентификационный код плательщика (код ЕГРПОУ) 
                    doc.ПлательщикСчет = iBANofBeneficiary.Length == 0 ? accountNumberOfBeneficiary : iBANofBeneficiary;//Номер счета получателя


                    doc.Получатель = nameOfPayer;//Наименование плательщика
                    doc.ПолучательМФО = bankID;//МФО банка плательщика
                    doc.ПолучательОКПО = taxpayerNumberOfPayer;//Идентификационный код плательщика (код ЕГРПОУ)
                    doc.ПолучательСчет = iBANOfPayer.Length == 0 ? accountNumberOfPayer : iBANOfPayer; //Номер счета плательщика

                }
            }
            else
                success = false;
            if (tryGetParametr("SUMMA", "Сума (у копійках)", out string amount))
            {
                if (decimal.TryParse(amount, out decimal sum))
                {
                    doc.Сумма = sum * 0.01m; //Сумма (в копейках)
                }
                else
                {
                    Warnings.Add($"Помилка в рядку {LineDoc} : {ErrorStrings.DoubleParse(amount)}");
                    success = false;
                }

            }
            else
                success = false;

            tryGetParametr("DOCUMENTTYPEID", "тип операції 14,30,6....", out string documentType);
            // var documentType = node.Attributes["DOCUMENTTYPEID"].Value;
   
            //if (documentType == "14" || documentType == "30" || documentType == "6")
            //{
                

 

            //    //doc.ДокументИД = 
 
            //}
            return success;
        }

            StreamReader reader = new StreamReader(stream, new Encoding1251());
            string xmlText = await reader.ReadToEndAsync();

            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlText);
            LineDoc = 1;
            try
            {
                foreach (XmlNode row in xmlDoc.SelectNodes("ROWDATA/ROW"))
                {
                    if (tryGetXMLDocument(row,out DocumentSection document))
                        AddDocument(document);
                    LineDoc++;
                }

            }
            catch (Exception e)
            {
                throw new Exception(ErrorStrings.InLine(LineDoc, e.Message));
            }
        }
        public async Task ImportFromDAT(Stream stream, string accNumber = "")
        {
            using (StreamReader reader = new StreamReader(stream, new Encoding1251()))
            {
                var lineDoc = 1;
                
                var lineStr = new Tuple<string, int>(string.Empty, 0);
                char[] buffer = new char[BufferSize];
                try
                {
                    do
                    {
                        lineStr = await GetLineFromStreamAsync(reader, buffer, lineStr.Item2);
                        
                        if (lineStr.Item2 == 0 || lineStr.Item1.Length == 0)
                            continue;
                        var doc = GetDocument(lineStr.Item1);
                        if (doc != null)
                            AddDocument(doc);
                        lineDoc++;
                    } while (lineStr.Item2 != 0);
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
                case ".XML":
                    await ImportFromXML(stream, accNumber);
                    break;
                case ".DAT":
                    await ImportFromDAT(stream, accNumber);
                    break;

                case ".ZIP":
                    using (MemoryStream memStream = new MemoryStream(512000))
                    {
                        await stream.CopyToAsync(memStream);
                        using (ZipArchive zip = new ZipArchive(memStream))
                        {
                            foreach (var file in zip.Entries)
                            {
                                await ImportFromFileAsync(file.Open(), Path.GetExtension(file.Name));
                            }
                        }
                    }
                    break;


                default:
                    throw new Exception($"Для клінтбанка iFobs, файл з розширенням {fileExt} не підтримується. Можливі типи файлів XML,DAT");

            }
        }
    }
}
