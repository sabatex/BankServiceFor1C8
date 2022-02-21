namespace BankServiceFor1C8.BankHelper;

/// <summary>
/// Секция платежного документа
/// </summary>
public class DocumentSection
{
    public DocumentSection()
    {
        ВидДокумента = "Платежное поручение";
    }
    
    /// <summary>
    /// "Платежное поручение"
    /// "Платежное требование-поручение"
    /// </summary>
    public string ВидДокумента { get; set; }
    /// <summary>
    /// Номер документа
    /// </summary>
    public string Номер { get; set; }
    /// <summary>
    /// Дата документа
    /// </summary>
    public string Дата { get; set; }
    /// <summary>
    /// Идентификатор документа
    /// </summary>
    public string ДокументИД { get; set; }
    /// <summary>
    /// Сумма платежа 14.2
    /// </summary>
    public decimal Сумма { get; set; }
    /// <summary>
    /// Валюта платежа 3 simbols
    /// </summary>
    public string КодВалюты { get; set; }
    /// <summary>
    /// Статус документа
    /// Проведен
    /// Отвергнут
    /// В ожидании
    /// </summary>
    //public string СтатусДокумента { get; set; }
    /// <summary>
    /// Расчетный счет плательщика 14 simbols
    /// </summary>
    public string ПлательщикСчет { get; set; }
    /// <summary>
    /// Наименование Плательщика
    /// </summary>
    public string Плательщик { get; set; }
    /// <summary>
    /// ОКПО плательщика (ЄДРПОУ)
    /// </summary>
    public string ПлательщикОКПО { get; set; }
    /// <summary>
    /// МФО банка плательщика
    /// </summary>
    public string ПлательщикМФО { get; set; }
    /// <summary>
    /// Расчетный счет получателя
    /// </summary>
    public string ПолучательСчет { get; set; }
    /// <summary>
    /// Дата поступления средств на р/с
    /// </summary>
    public string ДатаПоступило { get; set; }
    /// <summary>
    /// Наименование получателя
    /// </summary>
    public string Получатель { get; set; }
    /// <summary>
    /// ОКПО получателя (ЄДРПОУ)
    /// </summary>
    public string ПолучательОКПО { get; set; }
    /// <summary>
    /// Наименование банка получателя
    /// </summary>
    public string ПолучательБанк { get; set; }
    /// <summary>
    /// МФО банка получателя 6 simbols
    /// </summary>
    public string ПолучательМФО { get; set; }
    /// <summary>
    /// Назначение платежа
    /// </summary>
    public string НазначениеПлатежа { get => PurposeOfPayment; set => StringWorker(ref PurposeOfPayment, value); }

    private void StringWorker(ref string result, string incoming)
    {
        result = incoming.Replace('<', '[').Replace('>', ']');
    }


    private string PurposeOfPayment;


}



