using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class QuestProgressProvider : MonoBehaviour
{
    public abstract int GetMin();
    public abstract int GetMax();
    public abstract int GetProgress();
}
