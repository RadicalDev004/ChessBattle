using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerBehaviour : MonoBehaviour
{
    public List<EntityData> PiecesInventory = new();
    [HideInInspector]
    public Trainer TrainerInRange;
    public UI UI;
    [HideInInspector]
    public LayoutEdit LayoutEdit;

    private void Awake()
    {
        LoadPieces();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T) && !Movement.IsPaused)
        {
            SavePieces();
        }
        if(Input.GetKeyDown(KeyCode.G) && !Movement.IsPaused)
        {
            GiveLoadout();
        }

        if (TrainerInRange != null)
        {
            UI.ShowBattleTrainerButton(TrainerInRange.Name);
            if(Input.GetKeyDown(KeyCode.Space) &&!Movement.IsPaused)
            {
                PlayerPrefs.SetString("trainer", TrainerInRange.PiecesJson);
                SceneManager.LoadScene("Chess");
            }            
        }
        else
        {
            UI.HideBattleTrainerBUtton();
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
        PiecesInventory.Clear();
        EntityData p1 = new("MyKing", EntityData.Type.King, 1, 250, 250, 5, 3, 6, 10, "basic", MovePool.GetRandomMove());
        EntityData p2 = new("MyPawn", EntityData.Type.Pawn, 2, 200, 200, 3, 7, 7, 5, "basic", MovePool.GetRandomMove());
        EntityData p3 = new("MyRook", EntityData.Type.Rook, 3, 500, 500, 4, 2, 3, 40, "basic", MovePool.GetRandomMove());

        PiecesInventory.Add(p1);
        PiecesInventory.Add(p2);
        PiecesInventory.Add(p3);
        print("Added");
    }

    public void SavePieces()
    {
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
        var found = PiecesInventory.Find(e => e.Position == oldPos);
        found.Position = newPos;
    }

    private void OnTriggerEnter(Collider other)
    {

        if(other.TryGetComponent(out Trainer t))
        {
            TrainerInRange = t;
        }
        if(other.CompareTag("test"))
        {
            var piece = Variants.GetRandom();
            PiecesInventory.Add(piece);
            SavePieces();
            LayoutEdit.RefreshListPiecesUI();
        }
        if (other.CompareTag("test2"))
        {
            var piece = Variants.GetRandomOfType(EntityData.Type.King);
            PiecesInventory.Add(piece);
            SavePieces();
            LayoutEdit.RefreshListPiecesUI();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Trainer t))
        {
            TrainerInRange = null;
        }
    }
}
