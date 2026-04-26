using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaveCommand : CommandBase
{
    public LeaveCommand() { Type = CommandType.LeaveGame; }

    public LeaveCommand(bool side) : base(side, CommandType.LeaveGame)
    {
        Type = CommandType.LeaveGame;
    }

    public override void Execute()
    {
        Ref.ChessManager.EndMatch(!Side);
    }
}
