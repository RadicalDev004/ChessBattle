using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectPiecesProgressProvider : QuestProgressProvider

{
    public override int GetMax()
    {
        return Variants.PiecesVariants.Count;
    }

    public override int GetMin()
    {
        return 0;
    }

    public override int GetProgress()
    {
        return GameRef.PlayerBehaviour.SaveManager.GetCurrentFoundPiecesCount();
    }
}
