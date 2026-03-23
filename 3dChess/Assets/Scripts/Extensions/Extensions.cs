using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public static class Extensions 
{
    public static string ToSpacedNumber(this int number)
    {
        return number
             .ToString("N0", CultureInfo.InvariantCulture)
             .Replace(",", " ");
    }
}
