using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HouseManager : MonoBehaviour
{
    public List<House> Houses = new();

    private void Awake()
    {
        Houses = new(FindObjectsOfType<House>().OrderBy(h => h.name));
    }

    public int GetCurentHouseIndex()
    {
        for (int i = 0; i < Houses.Count; i++)
        {
            if (Houses[i].IsInside)
            {
                return i;
            }
        }
        return -1;
    }

    public House GetHousebyIndex(int index)
    {
        return Houses[index]; 
    }

    public House GetCurrentHouse()
    {
        var index = GetCurentHouseIndex();
        if (index == -1)
        {
            return null;
        }
        return Houses[index];
    }

    public Hotel GetHotelByFloor(int floor)
    {
        return Houses.OfType<Hotel>().FirstOrDefault(h => h.Floor == floor);
    }
}
