using Newtonsoft.Json;
using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChessManager : MonoBehaviour
{
    public InventoryData WhiteData, BlackData;
    private InventoryData WhiteFullData, BlackFullData;
    public SaveData saveData;
    public string OpponentName;
    public InventoryData MyData { get { return Side ? WhiteData : BlackData; }  }
    public List<Piece> OrgPieces = new();

    public static int Turn = 0;

    public List<Piece> WhitePieces = new(), BlackPieces = new();
    public ChessUI ChessUI;
    public static bool Local = true;

    public static bool Side = true;

    public static bool Ended = false;

    public TMP_Text T_OppName, T_MyName;
    public Image I_Avatar;

    public GameObject MyInfo;

    [Header("Debug")]
    public bool debLocal;
    public bool debSide;
    public int debTurn;


    private void Start()
    {
        AudioManager.StopAll();

        Ended = false;
        MyInfo.SetActive(false);
        if (Local)
        {
            PrepareLocalMatch();
            PreparePieces(WhiteData, BlackData);
            Turn = 0;
            Ref.AI.CreateChess();
        }
    }

    private void Update()
    {
        debLocal = Local;
        debSide = Side;
        debTurn = Turn;
    }

    //Match preparation
    public void PreparePieces(string incoming, bool side)
    {
        MyInfo.SetActive(true);
        print("Starting chess match with side:\n" + side);
        var myFullInventory = GetMyFullInventory();
        var myBattleInventory = CreateBattleInventory(myFullInventory);
        var incomingInventory = NormalizeInventory(JsonConvert.DeserializeObject<InventoryData>(incoming));
        Side = side;

        if(!side)
        {
            Ref.ManageTiles.SwitchBoard();
            PreparePieces(incomingInventory, myBattleInventory);
            BlackFullData = myFullInventory;
        }
        else
        {
            PreparePieces(myBattleInventory, incomingInventory);
            WhiteFullData = myFullInventory;
        }

        if(Side)
        {
            Ref.TimerMy.Create(60, () => { });
        }
        else
        {
            Ref.TimerOpp.Create(60, () => { });
        }
    }

    public Timer GetTimer(bool side)
    {
        return side == Side ? Ref.TimerMy : Ref.TimerOpp;
    }

    public void PreparePieces(InventoryData white, InventoryData black)
    {
        AudioManager.FadeIn("chess", 2);

        white ??= new InventoryData();
        black ??= new InventoryData();

        white.Pieces ??= new();
        white.Potions ??= new();
        black.Pieces ??= new();
        black.Potions ??= new();

        var oppName = Side ? black.Name : white.Name;
        T_OppName.text = oppName;
        T_MyName.text = Side ? white.Name : black.Name;

        var trainerSprite = Resources.Load<Sprite>($"Icons/Trainers/{oppName}");
        if (trainerSprite != null)
        {
            I_Avatar.sprite = trainerSprite;
            I_Avatar.Fit(100);
        }     

        WhiteFullData = white;
        BlackFullData = black;
        WhiteData = CreateBattleInventory(white);
        BlackData = CreateBattleInventory(black);

        foreach (var piece in WhiteData.Pieces)
        {
            var p = Instantiate(OrgPieces[(int)piece.PieceType]);
            p.gameObject.SetActive(true);
            p.Create(piece.Position, piece);

            foreach(var move in piece.Moves)
            {
                move.Count = move.MaxCount;
            }
            
            WhitePieces.Add(p);
        }

        foreach (var piece in BlackData.Pieces)
        {
            var p = Instantiate(OrgPieces[(int)piece.PieceType]);
            p.gameObject.SetActive(true);
            p.side = false;
            p.Create(64 - piece.Position, piece);

            foreach (var move in piece.Moves)
            {
                move.Count = move.MaxCount;
            }

            BlackPieces.Add(p);
        }
    }

    public void EndGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void EndMatch(bool winner)
    {
        Ended = true;
        var myFullData = Side ? WhiteFullData : BlackFullData;
        saveData.InventoryData ??= new InventoryData();
        saveData.InventoryData.Pieces = myFullData?.Pieces ?? new();
        saveData.InventoryData.Potions = myFullData?.Potions ?? new();

        if (Local)
            saveData.TrainerData.Find(t => t.Name == OpponentName).Defeated = winner;

        if (winner == Side)
        {
            saveData.TelemetryData.Wins++;
            ChessUI.WinUI();
        }
        else
        {
            saveData.TelemetryData.Losses++;
            ChessUI.LoseUI();
        }

        string json = JsonConvert.SerializeObject(saveData, Formatting.Indented);
        PlayerPrefs.SetString("save" + PlayerPrefs.GetInt("currentSave"), json);
    }

    public void PrepareLocalMatch()
    {
        string white = PlayerPrefs.GetString("save" + PlayerPrefs.GetInt("currentSave"));
        print("deserializing white data:\n" + white);
        string black = PlayerPrefs.GetString("trainer");

        SaveData whiteData = JsonConvert.DeserializeObject<SaveData>(white);
        saveData = whiteData;

        InventoryData whiteInventory = NormalizeInventory(whiteData.InventoryData);

        InventoryData blackInventory = NormalizeInventory(JsonConvert.DeserializeObject<InventoryData>(black));
        OpponentName = blackInventory.Name;

        WhiteData = whiteInventory;
        BlackData = blackInventory;
    }

    public string GetMyInventoryData()
    {
        return JsonConvert.SerializeObject(CreateBattleInventory(GetMyFullInventory()));
    }

    public void GiveUp()
    {
        if (Local)
        { 
            EndMatch(false);
            return; 
        }

        Ref.CommandManager.AddCommandLocal(new LeaveCommand(Side));
    }


    //Match logic
    public void MovePiece(bool side, int pieceInd, int tileInd)
    {
        GetTimer(side).Stop();

        var piece = side ? WhitePieces[pieceInd] : BlackPieces[pieceInd];
        var tile = Ref.ManageTiles.GetTile(tileInd);
        MovePiece(side, piece, tile);
    }
    public void MovePiece(bool side, Piece piece, Tile tile)
    {     
        if (tile.currentPiece != null)
        {
            BattleManager.Ongoing = true;

            if(side == Side)
            { 
                this.ActionAfterTime(0.5f, () => { 
                    Ref.CommandManager.AddCommandLocal(new StartBattleCommand(side)); 
                });      
            }

            Ref.VersusUI.Create(piece, tile.currentPiece, () =>
            {
                Ref.BattleManager.StartBattle(piece, tile.currentPiece, tile, side);
            });      
        }
        else
        {
            piece.GoToTile(tile);
            IncreaseTurn();
        }
    }



    public void PrepareMove(bool side, Piece piece, Tile tile)
    {
        var pieceInd = GetPieceIndex(piece);
        var tileInd = tile.GetIndex();

        Ref.CommandManager.AddCommandLocal(new MoveCommand(side, pieceInd, tileInd));
    }

    //Helpers
    public int GetPieceIndex(Piece piece)
    {
        var pieces = piece.side ? WhitePieces : BlackPieces;
        return pieces.IndexOf(piece);
    }
    public int GetPieceIndex(bool side, Piece piece)
    {
        var pieces = side ? WhitePieces : BlackPieces;
        return pieces.IndexOf(piece);
    }

    public Piece GetPieceByIndex(bool side, int pieceInd)
    {
        var pieces = side ? WhitePieces : BlackPieces;
        return pieces[pieceInd];
    }

    public PotionData GetPotionByIndex(bool side, int potionInd)
    {
        print("Trying to get potion " + potionInd + " for side " + side);
        var inventory = side ? WhiteData : BlackData;
        print(string.Join(", ", inventory.Potions));
        return inventory.Potions.Where(p => p.Position == potionInd).First();
    }
    public void RemovePotionAtIndex(bool side, int potionInd)
    {
        var inventory = side ? WhiteData : BlackData;
        var potion = GetPotionByIndex(side, potionInd);
        inventory.Potions.Remove(potion);

        var fullInventory = side ? WhiteFullData : BlackFullData;
        fullInventory?.Potions?.Remove(potion);
    }

    public static bool IsMyTurn()
    {
        return Side == (Turn % 2 == 0);
    }

    public static bool IsPlayerTurn(bool side)
    {
        return side == (Turn % 2 == 0);
    }

    public static void IncreaseTurn()
    {
        Debug.LogWarning("Chess turn increased");
        Turn++;
    }

    private InventoryData CreateBattleInventory(InventoryData inventory)
    {
        inventory = NormalizeInventory(inventory);

        return new InventoryData
        {
            Name = inventory.Name,
            Pieces = inventory.Pieces.Where(p => p.Position > -1).ToList(),
            Potions = inventory.Potions.Where(p => p.Position > -1).ToList()
        };
    }

    private InventoryData GetMyFullInventory()
    {
        string white = PlayerPrefs.GetString("save" + PlayerPrefs.GetInt("currentSave"));

        SaveData whiteData = JsonConvert.DeserializeObject<SaveData>(white);
        saveData = whiteData;

        return NormalizeInventory(whiteData.InventoryData);
    }

    private InventoryData NormalizeInventory(InventoryData inventory)
    {
        inventory ??= new InventoryData();
        inventory.Pieces ??= new();
        inventory.Potions ??= new();
        return inventory;
    }
}
