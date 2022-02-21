using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Linq;
using System.Text.RegularExpressions;
using sabatex.Extensions.Text;
using sabatex.Extensions.ClassExtensions;
using System.Xml.Serialization;

namespace BankServiceFor1C8.BankHelper;

/// <summary>
/// Внутренний признак файла обмена
/// </summary>
public abstract class ClientBankTo1CFormatConversion
{
    public  ClientBankTo1CFormatConversion()
    {
        Documents = new List<DocumentSection>();
        CurrencyCode = new Dictionary<string, int>()
        {
            {"EUR", 978 },
            {"RUB", 643 },
            {"UAH", 980 },
            {"USD", 840 }
        };
    }
    //public abstract string ConvertTo1CFormat(Stream stream, string AccNumber);
    public abstract Task ImportFromFileAsync(Stream stream, string fileExt, string accNumber = "");


    /// <summary>
    /// Поточна лінія обробляємого документа
    /// </summary>
    public int LineDoc { get; set; } = 1;

    public List<string> Errors { get; set; } = new List<string>();
    public List<string> Warnings { get; set; } = new List<string>();
    public List<DocumentSection> Documents { get; set; }
    private static Dictionary<string,int> CurrencyCode { get; set; }

    public static string[] supportFileTupe = new string[]
    {
        "XML","DAT","CSV"
    };

    public int Count() => Documents.Count();
    public string GetAsXML1()
    {
        // Процедура выгружает платежные поручения в XML.
        StringBuilder result = new StringBuilder();
        result.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        result.AppendLine("<_1CClientBankExchange xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">");
        result.AppendLine("<ВерсияФормата>2.00</ВерсияФормата>");
        result.AppendLine("<Отправитель>bilart.co</Отправитель>");
        result.AppendLine("<Получатель>1C8.X</Получатель>");
        foreach (DocumentSection doc in Documents)
        {
            result.AppendLine("    <СекцияДокумент>");
            result.AppendLine(string.Format("        <ВидДокумента>{0}</ВидДокумента>", doc.ВидДокумента));
            result.AppendLine(string.Format("        <Номер>{0}</Номер>", doc.Номер));
            result.AppendLine(string.Format("        <Дата>{0}</Дата>", doc.Дата));
            result.AppendLine(string.Format("        <ДокументИД>{0}</ДокументИД>", doc.ДокументИД));
            result.AppendLine(string.Format("        <Сумма>{0}</Сумма>", doc.Сумма.ToString("#############0.00")));
            result.AppendLine(string.Format("        <КодВалюты>{0}</КодВалюты>", CurrencyCode.TryGetValue(doc.КодВалюты, out int value) ? value.ToString() : doc.КодВалюты));
            result.AppendLine(string.Format("        <ПлательщикСчет>{0}</ПлательщикСчет>", doc.ПлательщикСчет));
            result.AppendLine(string.Format("        <Плательщик>{0}</Плательщик>", doc.Плательщик));
            result.AppendLine(string.Format("        <ПлательщикОКПО>{0}</ПлательщикОКПО>", doc.ПлательщикОКПО));
            result.AppendLine(string.Format("        <ПлательщикМФО>{0}</ПлательщикМФО>", doc.ПлательщикМФО));
            result.AppendLine(string.Format("        <ПолучательСчет>{0}</ПолучательСчет>", doc.ПолучательСчет));
            result.AppendLine(string.Format("        <Получатель>{0}</Получатель>", doc.Получатель));
            result.AppendLine(string.Format("        <ПолучательБанк>{0}</ПолучательБанк>", doc.ПолучательБанк));
            result.AppendLine(string.Format("        <ПолучательМФО>{0}</ПолучательМФО>", doc.ПолучательМФО));
            result.AppendLine(string.Format("        <ПолучательОКПО>{0}</ПолучательОКПО>", doc.ПолучательОКПО));
            result.AppendLine(string.Format("        <НазначениеПлатежа>{0}</НазначениеПлатежа>", doc.НазначениеПлатежа));
            //result.AppendLine(string.Format("        <ДатаПоступило>{0}</ДатаПоступило>", doc.ДатаПоступило));

            result.AppendLine("    </СекцияДокумент>");
        }


        result.AppendLine("</_1CClientBankExchange>");
        return result.ToString();
    }
    public string GetAsXML()
    {
        try
        {
            StringWriter sw = new StringWriter();
            XmlWriter xmlWriter = XmlWriter.Create(sw, new XmlWriterSettings { Encoding = Encoding.UTF8 });
            xmlWriter.WriteProcessingInstruction("xml", "version=\"1.0\" encoding=\"UTF-8\"");
            xmlWriter.WriteStartElement("_1CClientBankExchange");
            xmlWriter.WriteAttributeString("xmlns","xsi",null, "http://www.w3.org/2001/XMLSchema-instance");
            xmlWriter.WriteElementString("ВерсияФормата", "2.0");
            xmlWriter.WriteElementString("Отправитель", "sabatex");
            xmlWriter.WriteElementString("Получатель", "1C8.3");
            foreach (DocumentSection doc in Documents)
            {
                xmlWriter.WriteStartElement("СекцияДокумент");
                xmlWriter.WriteElementString("ВидДокумента", doc.ВидДокумента);
                xmlWriter.WriteElementString("Номер", doc.Номер);
                xmlWriter.WriteElementString("Дата", doc.Дата);
                xmlWriter.WriteElementString("ДокументИД", doc.ДокументИД);
                xmlWriter.WriteElementString("Сумма", doc.Сумма.ToString("#############0.00"));
                xmlWriter.WriteElementString("КодВалюты", CurrencyCode.TryGetValue(doc.КодВалюты, out int value) ? value.ToString() : doc.КодВалюты);
                xmlWriter.WriteElementString("ПлательщикСчет", doc.ПлательщикСчет);
                xmlWriter.WriteElementString("Плательщик", doc.Плательщик);
                xmlWriter.WriteElementString("ПлательщикОКПО", doc.ПлательщикОКПО);
                xmlWriter.WriteElementString("ПлательщикМФО", doc.ПлательщикМФО);
                xmlWriter.WriteElementString("ПолучательСчет", doc.ПолучательСчет);
                xmlWriter.WriteElementString("Получатель", doc.Получатель);
                xmlWriter.WriteElementString("ПолучательБанк", doc.ПолучательБанк);
                xmlWriter.WriteElementString("ПолучательМФО", doc.ПолучательМФО);
                xmlWriter.WriteElementString("ПолучательОКПО", doc.ПолучательОКПО);
                xmlWriter.WriteElementString("НазначениеПлатежа", doc.НазначениеПлатежа);
                xmlWriter.WriteEndElement();
            }



            xmlWriter.WriteEndElement();

            xmlWriter.Close();
            return sw.ToString();
        }
        catch (Exception e)
        {
            var s = e.Message;
            throw new Exception(s);
        }
    }
    public void AddDocument(DocumentSection documentSection)
    {
        Documents.Add(documentSection);
    }

    public static ClientBankTo1CFormatConversion GetConvertor(EBankType bankType)
    {
        switch (bankType)
        {
            case EBankType.iFobs:
                return new IFobs();
            case EBankType.iBankUA:
                return new IBankUA();
            case EBankType.PrivatUA:
                return new PrivatUA();
            case EBankType.OtpBankSK:
                return new OtpBankSK();
            case EBankType.PrimaBankSK:
                return new PrimaBankSk();
            case EBankType.UkrGazBank:
                return new UkrGazBank();
            default:
                throw new Exception(ErrorStrings.ErrorUnsupportBank(bankType));
        }

    }
}



