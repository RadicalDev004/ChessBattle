using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    public InventoryData InventoryData;
    public List<TrainerData> TrainerData;
    public List<QuestData> QuestData;

    public Vector3Data Position;
    public PieceFoundData PieceFoundData;
    public int? HouseIndex;
    public int Coins;
}
