using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Quest
{
    public int Index;
    public string Name, Description;
    public int Reward;
    public QuestProgressProvider ProgressProvider;
    public QuestType Type;

    public enum QuestType
    {
        InProgress,
        Completed,
        Claimed
    }
}
