using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessManager : MonoBehaviour
{
    
    public List<Piece> OrgPieces = new();
    private void Start()
    {
        PreparePieces();
    }

    public void PreparePieces()
    {
        string white = PlayerPrefs.GetString("pieces");
        string black = PlayerPrefs.GetString("trainer");

        InventoryData whiteData = JsonConvert.DeserializeObject<InventoryData>(white);
        InventoryData blackData = JsonConvert.DeserializeObject<InventoryData>(black);

        print(white);
        print(black);

        foreach(var pair in whiteData.Inventory)
        {
            var p = Instantiate(OrgPieces[(int)pair.Value.PieceType]);
            p.gameObject.SetActive(true);
            p.Create(pair.Key, pair.Value);
        }

        foreach (var pair in blackData.Inventory)
        {
            var p = Instantiate(OrgPieces[(int)pair.Value.PieceType]);
            p.gameObject.SetActive(true);
            p.Create(64 - pair.Key, pair.Value);
            p.side = false;
        }
    }
}
