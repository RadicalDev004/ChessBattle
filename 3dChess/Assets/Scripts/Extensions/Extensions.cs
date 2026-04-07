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

    public static List<T> GetRandomElements<T>(this List<T> list, int x)
    {
        if (x > list.Count)
            return new(list);

        List<T> copy = new(list);

        for (int i = 0; i < x; i++)
        {
            int randomIndex = Random.Range(i, copy.Count);
            (copy[i], copy[randomIndex]) = (copy[randomIndex], copy[i]);
        }

        return copy.GetRange(0, x);
    }
}
