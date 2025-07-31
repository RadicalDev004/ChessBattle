using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Move 
{
    public string Name;
    public string Description;
    public MoveType Type;
    public float Action;

    public Move(string name, string description, MoveType type, float action)
    {
        Name = name;
        Description = description;
        Type = type;
        Action = action;
    }
}


public enum MoveType
{
    Attack,
    Heal,
    Posion,
    Defense,
    Weaken,
    Speed,
    Evasion
}