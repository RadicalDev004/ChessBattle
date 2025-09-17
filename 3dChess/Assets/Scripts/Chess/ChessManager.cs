using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessManager : MonoBehaviour
{
    
    public List<Piece> OrgPieces = new();
    public static int Turn = 0;

    private void Start()
    {
        PreparePieces();
        Turn = 0;
    }

    public void PreparePieces()
    {
        string white = PlayerPrefs.GetString("pieces");
        string black = PlayerPrefs.GetString("trainer");

        print(white);
        print(black);

        InventoryData whiteData = JsonConvert.DeserializeObject<InventoryData>(white);
        InventoryData blackData = JsonConvert.DeserializeObject<InventoryData>(black);        

        foreach(var piece in whiteData.Inventory)
        {
            if (piece.Position == -1) continue;
            var p = Instantiate(OrgPieces[(int)piece.PieceType]);
            p.gameObject.SetActive(true);
            p.Create(piece.Position, piece);
        }

        foreach (var piece in blackData.Inventory)
        {
            if (piece.Position == -1) continue;
            var p = Instantiate(OrgPieces[(int)piece.PieceType]);
            p.gameObject.SetActive(true);
            p.side = false;
            p.Create(64 - piece.Position, piece);           
        }
    }
}
