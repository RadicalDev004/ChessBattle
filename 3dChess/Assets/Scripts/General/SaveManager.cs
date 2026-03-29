using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public PlayerBehaviour player;
    public bool isSaving = false;
    public SaveData latestSaveData;

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
            Pieces = player.PiecesInventory, 
            Potions = player.PotionInventory 
        };
        sv.Coins = ShopManager.Coins;
        sv.Position = player.transform.position;
        sv.PieceFoundData = player.pieceFoundData;

        sv.HouseIndex = GameRef.HouseManager.GetCurentHouseIndex();
        sv.QuestData = GameRef.QuestManager.GetQuestsData();

        string json = JsonConvert.SerializeObject(sv, Formatting.Indented);
        print("Saving game \n" + json);
        PlayerPrefs.SetString("save" + PlayerPrefs.GetString("currentSave"), json);
        latestSaveData = sv;

        isSaving = false;
    }
    public SaveData GetData()
    {
        return JsonConvert.DeserializeObject<SaveData>(PlayerPrefs.GetString("save" + PlayerPrefs.GetString("currentSave")));
    }
    public void LoadGame()
    {
        print("Loading game \n" + PlayerPrefs.GetString("save" + PlayerPrefs.GetString("currentSave")));
        SaveData sv = JsonConvert.DeserializeObject<SaveData>(PlayerPrefs.GetString("save" + PlayerPrefs.GetString("currentSave")));
        latestSaveData = sv;
        if (sv != null)
        {
            player.ChangePlayerPos(sv.Position ?? new Vector3(26, 1.46521699f, 46));
            player.pieceFoundData = sv.PieceFoundData ?? new();
            var allTrainers = FindObjectsOfType<Trainer>().ToList();

            foreach (var t in allTrainers)
            {
                var trainer = sv.TrainerData.Find(td => td.Name == t.Name);
                if (trainer == null) continue;
                t.Create(trainer);
            }

            if (sv.HouseIndex.HasValue && sv.HouseIndex != -1)
            {
                GameRef.HouseManager.GetHousebyIndex(sv.HouseIndex.Value).EnterHouse(false);
            }
        }

        ShopManager.Coins = sv?.Coins ?? 0;
        GameRef.QuestManager.SetQuestsData(sv?.QuestData ?? new());

        player.PiecesInventory = sv == null ? new() : sv.InventoryData.Pieces;
        player.PotionInventory = sv == null ? new() : sv.InventoryData.Potions;
        player.PiecesInventory ??= new();
        player.pieceFoundData ??= new();      
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }
}
