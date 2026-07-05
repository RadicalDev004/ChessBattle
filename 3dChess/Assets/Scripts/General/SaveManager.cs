using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    public PlayerBehaviour player;
    public bool isSaving = false;
    public SaveData latestSaveData;
    public TelemetryData telemetryData;
    public string PlayerName = string.Empty;

    public int GetLossesToWinsBalance()
    {
        if(latestSaveData == null || latestSaveData.TelemetryData == null)
            return 0;
        return latestSaveData.TelemetryData.Losses - latestSaveData.TelemetryData.Wins;
    }

    public void IncreaseBoxesOpened()
    {
        telemetryData.BoxesOpened++;
    }

    public void SetName(string name)
    {
        PlayerName = name;
    }

    public void QuitGame()
    {
        SaveGame();
        SceneManager.LoadScene("Menu");
    }

    public int GetCurrentFoundPiecesCount()
    {
        if (latestSaveData == null)
            return 0;
        if (latestSaveData.PieceFoundData == null)
            return 0;

        return latestSaveData.PieceFoundData.PiecesFound.Count;
    }    
    

    private void Awake()
    {
        LoadGame();
        Time.timeScale = 1;
    }

    public void SaveGame()
    {
        if (isSaving)
            return;
        isSaving = true;

        SaveData sv = new();
        var allTrainers = FindObjectsOfType<Trainer>().ToList();
        sv.TrainerData = allTrainers.ConvertAll(t => new TrainerData(t));
        sv.InventoryData = new InventoryData() 
        { 
            Name = PlayerName,
            Pieces = player.PiecesInventory, 
            Potions = player.PotionInventory 
        };
        sv.Coins = ShopManager.Coins;
        sv.Position = player.transform.position;
        sv.PieceFoundData = player.pieceFoundData;
        sv.TelemetryData = telemetryData;

        sv.HouseIndex = GameRef.HouseManager.GetCurentHouseIndex();
        sv.QuestData = GameRef.QuestManager.GetQuestsData();

        string json = JsonConvert.SerializeObject(sv, Formatting.Indented);
        print("Saving game \n" + json);
        PlayerPrefs.SetString("save" + PlayerPrefs.GetInt("currentSave"), json);
        latestSaveData = sv;

        isSaving = false;
    }
    public SaveData GetData()
    {
        return DeserializeSaveData(PlayerPrefs.GetString("save" + PlayerPrefs.GetInt("currentSave"), string.Empty));
    }
    public void LoadGame()
    {
        string saveJson = PlayerPrefs.GetString("save" + PlayerPrefs.GetInt("currentSave"), string.Empty);
        print($"Loading game [{PlayerPrefs.GetInt("currentSave")}]\n" + saveJson);
        SaveData sv = DeserializeSaveData(saveJson);
        latestSaveData = sv;
        var allTrainers = FindObjectsOfType<Trainer>().ToList();
        if (sv != null)
        {
            player.ChangePlayerPos(sv.Position ?? new Vector3(26, 1.46521699f, 46));
            player.pieceFoundData = sv.PieceFoundData ?? new();

            foreach (var t in allTrainers)
            {
                var trainer = sv.TrainerData?.Find(td => td.Name == t.Name);
                if (trainer == null)
                {
                    t.CreateNewGame();
                    continue;
                }
                t.Create(trainer);
            }

            if (sv.HouseIndex.HasValue && sv.HouseIndex != -1)
            {
                GameRef.HouseManager.GetHousebyIndex(sv.HouseIndex.Value).EnterHouse(false);
            }
        }

        player.PiecesInventory = sv?.InventoryData?.Pieces ?? new();
        player.PotionInventory = sv?.InventoryData?.Potions ?? new();
        player.PiecesInventory ??= new();
        player.PotionInventory ??= new();
        player.pieceFoundData ??= new();
        player.pieceFoundData.PiecesFound ??= new();

        if (sv == null)
        {
            AddStarterKingToPlayerInventory();
            allTrainers.ForEach(t =>
            {
                t.CreateNewGame();
            });
        }

        ShopManager.Coins = sv?.Coins ?? 0;
        GameRef.QuestManager.SetQuestsData(sv?.QuestData ?? new());

        telemetryData = sv?.TelemetryData ?? new TelemetryData();
    }

    private void AddStarterKingToPlayerInventory()
    {
        var starterKing = Variants.PiecesVariants
            .FirstOrDefault(p => p.PieceType == EntityData.Type.King && p.Variant == "basic")
            ?.Copy();

        if (starterKing == null)
        {
            Debug.LogWarning("Could not add starter king. No basic king variant was found.");
            return;
        }

        starterKing.Position = -1;
        starterKing.Level = 1;
        starterKing.Health = starterKing.MaxHealth;
        starterKing.Moves = MovePool.Pool
            .Where(m => m.Rarity == MoveRarity.Common && m.Variants.Contains(starterKing.Variant))
            .OrderByDescending(m => m.Type == MoveType.Attack)
            .Take(4)
            .Select(m => m.Copy())
            .ToList();

        player.PiecesInventory.Add(starterKing);

        player.pieceFoundData.PiecesFound ??= new();
        string pieceFoundKey = starterKing.Variant + "/" + starterKing.PieceType;
        if (!player.pieceFoundData.PiecesFound.Contains(pieceFoundKey))
            player.pieceFoundData.PiecesFound.Add(pieceFoundKey);
    }

    private SaveData DeserializeSaveData(string saveJson)
    {
        if (string.IsNullOrWhiteSpace(saveJson))
            return null;

        try
        {
            return JsonConvert.DeserializeObject<SaveData>(saveJson);
        }
        catch (JsonException exception)
        {
            Debug.LogWarning($"Could not load save data, starting a new game state. {exception.Message}");
            return null;
        }
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }
}
