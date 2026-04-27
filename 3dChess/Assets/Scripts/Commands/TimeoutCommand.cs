using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeoutCommand : CommandBase
{
    public TimeoutCommand() { Type = CommandType.Timeout; }

    public TimeoutCommand(bool side) : base(side, CommandType.Timeout)
    {
        Type = CommandType.Timeout;
    }

    public override void Execute()
    {
        Ref.ChessManager.EndMatch(!Side);
    }
}
