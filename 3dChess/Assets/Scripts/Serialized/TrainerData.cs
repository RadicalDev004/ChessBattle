using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TrainerData
{
    public string Name;
    public bool Defeated;

    public TrainerData() { }

    public TrainerData(Trainer t)
    {
        Defeated = t.Defeated;
        Name = t.Name;
    }
}
