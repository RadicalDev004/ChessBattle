using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    public Dictionary<int, EntityData> PiecesInventory = new();

    private void Awake()
    {
        LoadPieces();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            SavePieces();
        }
        if(Input.GetKeyDown(KeyCode.G))
        {
            GiveLoadout();
        }
    }

    public void LoadPieces()
    {
        string piecesJson = PlayerPrefs.GetString("pieces");
        print(piecesJson);
        InventoryData data = JsonConvert.DeserializeObject<InventoryData>(piecesJson);
        PiecesInventory = data == null ? new() : data.Inventory;

        if(PiecesInventory == null)
            PiecesInventory = new();
    }

    public void GiveLoadout()
    {
        EntityData p1 = new("MyKing", EntityData.Type.King, 100, 100, 1, MovePool.GetRandomMove());
        EntityData p2 = new("MyPawn", EntityData.Type.Pawn, 150, 150, 1, MovePool.GetRandomMove());
        EntityData p3 = new("MyRook", EntityData.Type.Rook, 500, 500, 1, MovePool.GetRandomMove());

        PiecesInventory.Add(1, p1);
        PiecesInventory.Add(2, p2);
        PiecesInventory.Add(3, p3);
        print("Added");
    }

    public void SavePieces()
    {
        print(PiecesInventory.Count);
        InventoryData data = new()
        {
            Inventory = PiecesInventory
        };
        string json = JsonConvert.SerializeObject(data, Formatting.Indented);
        print(json);
        PlayerPrefs.SetString("pieces", json);
    }

    public void ChangePiecePosition(int oldPos, int newPos)
    {
        if (PiecesInventory.TryGetValue(oldPos, out EntityData e))
        {
            PiecesInventory.Remove(oldPos);
            PiecesInventory.Add(newPos, e);
            print("Changing piece pos: " + oldPos + " to " + newPos);
        }
    }
}
