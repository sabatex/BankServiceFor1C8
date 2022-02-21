using System.Text;
using System.Collections.Specialized;
using System;


namespace BankServiceFor1C8.BankHelper;

public static class String1C8Extension
{

    /// <summary>
    /// Convert date from format "xx.xx.xxxx" to "xxxx.xx.xx"
    /// </summary>
    /// <param name="value">string in format "xx.xx.xxxx"</param>
    /// <param name="line"></param>
    /// <returns></returns>
    public static string DateTo1C8Date(this string value)
    {
        

        if (value.TryDateTo1C8Date(out string result))
        {
            return result;
        }
        throw new Exception(string.Format("Error Convert date string to 1C8 format {0}", value));
    }
    public static bool TryDateTo1C8Date(this string value,out string result)
    {
        if (value.Length >= 10)
        {
            int day;
            int mounth;
            int year;
            if (int.TryParse(value.Substring(0, 2), out day))
            {
                if (int.TryParse(value.Substring(3, 2), out mounth))
                {
                    if (int.TryParse(value.Substring(6, 4), out year))
                    {
                        result = string.Format("{0}-{1}-{2}", year.ToString("0000"), mounth.ToString("00"), day.ToString("00"));
                        return true;
                    }
                }
            }
        }
        result = string.Empty;
        return false;


    }

}
