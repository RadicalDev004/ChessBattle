using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameColors : MonoBehaviour
{
    public static List<Color> PiecesText = new();

    public Color pawnText, bishopText, knightText, rookText, queenText, kingText;

    public static Color GetColorByType(EntityData.Type p)
    {
        return PiecesText[(int)p];
    }

    public static GameColors Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        PiecesText = new() { Instance.pawnText, Instance.knightText, Instance.bishopText, Instance.rookText, Instance.queenText, Instance.kingText };
    }
}
