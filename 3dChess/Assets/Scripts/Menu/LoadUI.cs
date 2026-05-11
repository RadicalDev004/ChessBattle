using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class LoadUI : MonoBehaviour
{
    public TMP_Text T_Name, T_Trainers, T_LastPlayedAt;
    public int Index;

    public void Create(int ind)
    {
        Index = ind;
        T_Name.text = PlayerPrefs.GetString($"save{ind}_name", "Unknown");
        T_LastPlayedAt.text = "Last played " + PlayerPrefs.GetString($"save{ind}_lastPlayedAt", "Unknown");

        string saveJson = PlayerPrefs.GetString($"save{ind}", "");
        SaveData data = string.IsNullOrWhiteSpace(saveJson) ? null : JsonUtility.FromJson<SaveData>(saveJson);
        int defeatedTrainers = data?.TrainerData?.Count(td => td.Defeated) ?? 0;
        T_Trainers.text = $"Trainers: {defeatedTrainers}/20";

        GetComponent<Button>().onClick.AddListener(() =>
        {
            print("Loading save " + Index);
            PlayerPrefs.SetInt("currentSave", Index);
            PlayerPrefs.SetString($"save{Index}_lastPlayedAt", System.DateTime.Now.ToString("g"));
            SceneManager.LoadScene("Game");
        });
    }
}
