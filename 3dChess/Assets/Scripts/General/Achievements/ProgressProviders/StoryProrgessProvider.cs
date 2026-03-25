using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryProrgessProvider : QuestProgressProvider
{
    public int Biome;
    public override int GetMin()
    {
        return 0;
    }
    public override int GetMax()
    {
        return 5;
    }
    public override int GetProgress()
    {
        return GameRef.StoryManager.GetTrainersDefeatedbyZone(Biome);
    }
}
