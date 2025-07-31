using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePool : MonoBehaviour
{
    public List<Move> Basic = new() { 
        new Move("Strike", "Speed towards the other piece at full speed dealing damage.", MoveType.Attack, 25),
        new Move("Heal", "Replenish some of the lost hitpoints.", MoveType.Heal, 20)
    };
}
