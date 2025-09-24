using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RandomChance
{
    public static bool Percent(int percent)
    {
        int a = Random.Range(1, 101);

        if (a <= percent)
            return true;
        else
            return false;
    }
}
