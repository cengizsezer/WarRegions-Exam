using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public static class Extensions
{
    private static readonly int charA = Convert.ToInt32('a');
    private static List<char> alphabet;


    private static string result;
    private static double firstDigit = 999, secondDigit;

    private const long tresholdNumber = 1000;

    private static readonly Dictionary<float, string> NumberSigns = new Dictionary<float, string>
    {
        {1000000000, "B" },
        {1000000, "M" },
        {1000,"K"}
    };

    public static string FormatNumberWithSigns(this long number)
    {
        if (number < tresholdNumber)
        {
            return FormatNumber(number);
        }

        foreach (KeyValuePair<float, string> numberSign in NumberSigns)
        {
            if (Mathf.Abs(number) >= numberSign.Key)
            {
                float newNumber = number / numberSign.Key;
                return $"{newNumber:#.#}{numberSign.Value}";
            }
        }

        return FormatNumber(number);
    }

    public static string FormatNumber(this long number)
    {
        return number.ToString("N0");
    }

    public static string FormatNumber(this int number)
    {
        return number.ToString("N0");
    }

    public static string FormatNumberWithSigns(this int number)
    {
        if (number < tresholdNumber)
        {
            return FormatNumber(number);
        }

        foreach (KeyValuePair<float, string> numberSign in NumberSigns)
        {
            if (Mathf.Abs(number) >= numberSign.Key)
            {
                float newNumber = number / numberSign.Key;
                return $"{newNumber:#.##}{numberSign.Value}";
            }
        }

        return FormatNumber(number);
    }

    public static float Remap(this float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    public static void ConvertLocalizedPrice(this TextMeshProUGUI label, decimal price, string currencyCode)
    {
        if (currencyCode == "USD")
        {
            label.text = "$" + price;
        }
        else
        {
            label.text = price + " " + currencyCode;
        }
    }

    public static float Sign(this float value)
    {
        if (Mathf.Approximately(value, 0f))
        {
            return 0f;
        }

        return value > 0f ? 1f : -1f;
    }

    public static DateTime EndOfDay(this DateTime @this)
    {
        var tomorrow = @this.AddDays(1).Date;
        return tomorrow.Subtract(new TimeSpan(0, 0, 0, tomorrow.Second));
    }
}
