using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CommandBase
{
    [JsonProperty(Order = -1)]
    public CommandType Type;

    [JsonProperty(Order = -2)]
    public bool Side;

    [JsonProperty(Order = -3)]
    public DateTime Timestamp;

    protected CommandBase() { }
    public CommandBase(bool side, CommandType type)
    {
        Timestamp = DateTime.UtcNow;
        Side = side;
        Type = type;
        Debug.Log($"[COmmand] Created command of type {type} for side {(side ? "White" : "Black")} at {Timestamp}");
    }

    public abstract void Execute();


    public void ManageTimersRegular(int delaySeconds = 0)
    {
        Ref.TimerMy.Stop();
        Ref.TimerOpp.Stop();

        if (Side == ChessManager.Side)
        {
            Ref.TimerMy.Create(60, () => { }, Timestamp.AddSeconds(delaySeconds));
        }
        else
        {
            Ref.TimerOpp.Create(60, () => { }, Timestamp.AddSeconds(delaySeconds));
        }


    }
    public void ManageTimersBefore()
    {
        Ref.TimerMy.Stop();
        Ref.TimerOpp.Stop();

        if (Side == ChessManager.Side)
        {            
            Ref.TimerOpp.Create(60, () => { }, Timestamp);
        }
        else
        {            
            Ref.TimerMy.Create(60, () => { }, Timestamp);
        }
    }
}

public enum CommandType
{
    Move,
    BattleUseMove,
    BattleUsePotion,
    BattleSwitchPiece,
    BattleFlee,
    

    StartMatch,
    StartBattle,
    EndBattle,
    LeaveGame,
    Timeout,
}
