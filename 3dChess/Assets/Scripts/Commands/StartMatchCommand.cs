using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMatchCommand : CommandBase
{
    public StartMatchCommand() { Type = CommandType.StartMatch; }

    public StartMatchCommand(bool side) : base(side, CommandType.StartMatch)
    {
        Type = CommandType.StartMatch;
    }

    public override void Execute()
    {
        Ref.LoadingScreen.Toggle(false);
        ManageTimersRegular();
    }
}
