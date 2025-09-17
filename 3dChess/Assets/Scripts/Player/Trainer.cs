using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trainer : MonoBehaviour
{
    public string Name;
    [TextArea(10, 10)]
    public string PiecesJson;
    public bool Defeated;
}
