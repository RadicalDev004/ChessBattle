using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class EntityData
{
    public string Name;
    public Type PieceType;
    public int Position;
    public int Health;
    public int MaxHealth;
    
    public int Attack; // max 10
    public int Speed; // max 10
    public int Luck; // max 10

    public int Level {
        get
        {
            return Entity.Tresholds.Where(t => t <= Exp).DefaultIfEmpty(0).Max();
        }
        set
        {

        }
    }
    
    public float Exp;

    public List<Move> Moves = new();

    public enum Type
    {
        Pawn,
        Knight,
        Bishop,
        Rook,
        Queen,
        King
    }

    public EntityData()
    {

    }

    public EntityData(Entity entity)
    {
        Name = entity.Name;
        Health = entity.Health;
        MaxHealth = entity.MaxHealth;
        Level = entity.Level;
        Moves = new List<Move>();

        foreach (var move in entity.Moves) 
        {
            Moves.Add(move);
        }
    }

    public EntityData(string Name, Type PieceType, int Position, int Health, int MaxHealth, int Level, params Move[] Moves)
    {
        this.Name = Name;
        this.PieceType = PieceType;
        this.Position = Position;
        this.Health = Health;
        this.MaxHealth = MaxHealth;
        this.Level = Level;
        this.Moves.AddRange(Moves);
    }
}

