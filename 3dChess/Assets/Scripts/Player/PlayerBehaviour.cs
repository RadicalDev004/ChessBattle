using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    public List<EntityData> PiecesInventory = new();

    private void Awake()
    {
        PiecesInventory = new();
        LoadPieces();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            SavePieces();
        }
    }

    public void LoadPieces()
    {
        string piecesJson = PlayerPrefs.GetString("pieces");
        print(piecesJson);
        InventoryData data = JsonUtility.FromJson<InventoryData>(piecesJson);
        PiecesInventory = data == null ? new() : data.Inventory;
    }

    public void SavePieces()
    {
        InventoryData data = new()
        {
            Inventory = PiecesInventory
        };
        string json = JsonUtility.ToJson(data, true);
        print(json);
        PlayerPrefs.SetString("pieces", json);
    }
}
