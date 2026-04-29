using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndBattleCommand : CommandBase
{
    public EndBattleCommand() { Type = CommandType.EndBattle; }

    public EndBattleCommand(bool side) : base(side, CommandType.EndBattle)
    {
        Type = CommandType.EndBattle;
    }

    public override void Execute()
    {
        Ref.LoadingScreen.Toggle(false);

        if(!ChessManager.Local)
        {
            ManageTimersBefore();
        }
    }
}
