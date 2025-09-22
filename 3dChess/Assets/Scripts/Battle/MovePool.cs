using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePool : MonoBehaviour
{
    public static List<Move> Basic = new() { 
        new Move("Strike", "Lunge towards the other piece at full speed dealing damage.", MoveType.Attack, 50),
        new Move("Hit", "Give a medium hit to the opponent.", MoveType.Attack, 40),
        new Move("Touch", "Deal damage to the opposing piece.", MoveType.Attack, 30),
        new Move("Heal", "Replenish some of the lost hitpoints.", MoveType.Heal, 40),
        
    };

    public static Move GetRandomMove()
    {
        return Basic[Random.Range(0, Basic.Count)];
    }

}
