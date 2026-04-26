using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartBattleCommand : CommandBase
{
    public StartBattleCommand() { Type = CommandType.StartBattle; }

    public StartBattleCommand(bool side) : base(side, CommandType.StartBattle)
    {
        Type = CommandType.StartBattle;
    }

    public override void Execute()
    {
        ManageTimersRegular(3);
    }
}
