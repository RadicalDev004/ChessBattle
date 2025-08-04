using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    public string Name;
    public int Health;
    public int MaxHealth;
    public int Level = 1;
    public List<Move> Moves;
    public abstract void UpdateUI();
}
