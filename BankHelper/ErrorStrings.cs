using System;
using System.Collections.Generic;
using System.Text;

namespace BankServiceFor1C8.BankHelper;

public static class ErrorStrings
{
    public static string TryGetValueFromEndStream(string valueName) => $"Спроба прочитати значення {valueName} в кінці потоку ! ";
    public static string InLine(int line, string error) => $"Помилка в рядку {line} : {error}";
    public static string DoubleParse(string value) => $"Помилка перетворення {value} в число!";
    public static string ErrorDateParse(string value) => $"Помилка перетворення {value} в дату!";
    public static string ErrorUnsupportBank(EBankType bankType) => $"Банк {bankType} не підпримується";
    public static string StrinLenght(int maxSize) => $"Довжина рядка не повинна перевищувати {maxSize} bytes!";
    public static string DetermineEndDelimiterForValue(string valueName) => $"Помилка визначення розділювача для {valueName}";
    public static string StringFormatErrorForValue(string valueName) => $"The string format error for value {valueName}";
    public static string StringEndedBeforeReadValue(string valueName) => $"The string ended before read value {valueName}";
    public static string TryGetValueFromEndString(string valueName) => $"Try get value {valueName} from end stream !!! ";
    public static string ConvertDataTo1C8FormatForField(string fieldName, string data) => $"Error convert data {data} to 1C8 format for field {fieldName}";
    public static string UnsupportedFormatFileForiFobs() => $"Unsupported format file for iFobs or unknown error";

    const string ErrorUnknownFormatFile = "Unknown format file, check file format!";
}
