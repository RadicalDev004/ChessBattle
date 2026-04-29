using Newtonsoft.Json;
using Pixelplacement;
using Pixelplacement.TweenSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements.Experimental;

[RequireComponent(typeof(CharacterController))]
public class PlayerBehaviour : MonoBehaviour
{
    public SaveManager SaveManager;
    public Camera Camera;
    public PieceFoundData pieceFoundData;
    private Movement Movement;

    public List<EntityData> PiecesInventory = new();
    public List<PotionData> PotionInventory = new();

    [HideInInspector]
    public Trainer TrainerInRange;
    [HideInInspector]
    public House HouseInRange;
    [HideInInspector]
    public LayoutEdit LayoutEdit;

    public Action<EntityData> OnGetPiece;
    public Action<PotionData> OnGetPotion;
    public BoxBehaviour BoxBehaviour;

    private Vector3 InitialCameraPos;
    private Quaternion InitialCameraRotation;

    public Transform InSkiChair;
    public bool IsInSkiChair = false;

    public int CurrentBiome;

    private void Start()
    {
        Movement = GetComponent<Movement>();
        InitialCameraPos = Camera.transform.localPosition;
        InitialCameraRotation = Camera.transform.rotation;

        AudioManager.StopAll();

        CurrentBiome = GetBiome();
        PlayBiomeSound();
    }


    private void Update()
    {
        if (IsInSkiChair && Input.GetKeyDown(KeyCode.Escape))
        {
            transform.localPosition = transform.localPosition + new Vector3(0, 0.5f, 0);            
            GameRef.UI.ShowSkiChairExitPrompt(false);
            IsInSkiChair = false;
            transform.SetParent(null);
            transform.localRotation = Quaternion.identity;
            transform.GetChild(0).localRotation = Quaternion.identity;

            Movement.IsPaused = false;
            Movement.StandUp();
            GetComponent<CharacterController>().enabled = true;
        }
        if (Input.GetKeyDown(KeyCode.T) && !Movement.IsPaused)
        {
            SaveManager.SaveGame();
        }

        if (TrainerInRange != null)
        {
            GameRef.UI.ShowBattleTrainerButton(TrainerInRange.Name);
            if(Input.GetKeyDown(KeyCode.Space) && !Movement.IsPaused)
            {
                TalkToNpc(TrainerInRange);
            }            
        }
        else
        {
            GameRef.UI.HideBattleTrainerBUtton();
        }

        GameRef.UI.ToggleEnterHouse(HouseInRange != null);
        if(HouseInRange != null && Input.GetKeyDown(KeyCode.E) && !Movement.IsPaused)
        {
            var current = GameRef.HouseManager.GetCurrentHouse();
            if(current != null) {
                current.IsInside = false;
            }
            HouseInRange.EnterHouse();
        }

        
        int biome = GetBiome();
        if(biome != CurrentBiome)
        {
            string previous = "biome" + CurrentBiome;
            CurrentBiome = biome;
            PlayBiomeSound(previous);
        }
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
        if(other.CompareTag("test") && LayoutEdit.FinishedSetup)
        {
            BoxBehaviour.PrepareBox();
        }
        if (other.CompareTag("test2"))
        {
            AddPotionToInventory(Variants.GetRandomPotion());
        }
        if (other.CompareTag("hospital"))
        {
            GameRef.UI.ActivateTab(GameRef.UI.Tab_Hospital);
            GameRef.HospitalEdit.RefreshListPiecesUI();
        }
        if (other.CompareTag("shop"))
        {
            GameRef.UI.ActivateTab(GameRef.UI.Tab_Shop);
            GameRef.ShopManager.UpdateCoins();
        }
        if (other.CompareTag("quests"))
        {
            GameRef.UI.ActivateTab(GameRef.UI.Tab_Quests);
            GameRef.QuestManager.RefreshQuestData();
        }

        if (other.CompareTag("ski_chair") && !IsInSkiChair)
        {
            IsInSkiChair = true;
            Movement.IsPaused = true;
            Movement.Sit();
            GetComponent<CharacterController>().enabled = false;

            transform.SetParent(other.transform);
            transform.SetLocalPositionAndRotation(InSkiChair.localPosition, InSkiChair.localRotation);
            transform.GetChild(0).localRotation = InSkiChair.GetChild(0).localRotation;

            GameRef.UI.ShowSkiChairExitPrompt(true);
        }
    }

    public EntityData AddPieceToInventory(EntityData e)
    {
        string p = e.Variant + "/" + e.PieceType;
        if(!pieceFoundData.PiecesFound.Contains(p))
            pieceFoundData.PiecesFound.Add(p);

        PiecesInventory.Add(e);
        SaveManager.SaveGame();
        LayoutEdit.RefreshListPiecesUI();
        OnGetPiece?.Invoke(e);
        return e;
    }

    public PotionData AddPotionToInventory(PotionData p)
    {
        PotionInventory.Add(p);
        SaveManager.SaveGame();
        OnGetPotion?.Invoke(p);
        return p;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Trainer t))
        {
            TrainerInRange = null;
        }
    }

    public void ChangePlayerPos(Vector3 pos)
    {
        GetComponent<CharacterController>().enabled = false;
        transform.position = pos;
        GetComponent<CharacterController>().enabled = true;
    }

    public void SpeakWithNpc(Trainer trainer)
    {
        CameraFollow.Enabled = false;
        SaveCameraPos();
        var camerPos = (transform.position + trainer.transform.position) / 2 + new Vector3(0, 4, -5);
        Quaternion targetRotation = InitialCameraRotation * Quaternion.Euler(-15f, 0f, 0f);

        Tween.Position(Camera.transform, camerPos, 0.5f, 0, Tween.EaseOut);
        Tween.Rotation(Camera.transform, targetRotation, 0.5f, 0, Tween.EaseOut);
    }

    private void TalkToNpc(Trainer trainer)
    {
        SpeakWithNpc(trainer);
        GameRef.TrainerSpeak.Create(trainer,
            () =>
            {
                SaveManager.SaveGame();
                this.ActionAfterTime(0.5f, () => { CameraFollow.Enabled = true; Movement.IsPaused = false; });

                print("AA\n" + JsonConvert.SerializeObject(trainer.GetInventory()));

                PlayerPrefsExtentions.SetBool("online", false);

                PlayerPrefs.SetString("trainer", JsonConvert.SerializeObject(trainer.GetInventory(), Formatting.Indented));
                SceneManager.LoadScene("Chess");
            });
    }

    public bool HasPieceTypeInLayout(EntityData.Type type)
    {
        return PiecesInventory.Any(p => p.PieceType == type && p.Position != -1);
    }

    public void SaveCameraPos()
    {
        InitialCameraPos = Camera.transform.localPosition;
        InitialCameraRotation = Camera.transform.rotation;
    }

    public void ResetCamera()
    {
        Tween.LocalPosition(Camera.transform, InitialCameraPos, 0.5f, 0, Tween.EaseOut);
        Tween.Rotation(Camera.transform, InitialCameraRotation, 0.5f, 0, Tween.EaseOut);
        this.ActionAfterTime(0.5f, () => { CameraFollow.Enabled = true; Movement.IsPaused = false; });
    }

    public List<PotionData> GetPotionsByName(string name)
    {
        return PotionInventory.Where(x => x.Name == name).ToList();
    }

    public bool TryGetPotionOnPosition(int pos, out PotionData potion)
    {
        potion = PotionInventory.FirstOrDefault(p => p.Position == pos);
        return potion != null;
    }

    public void TeleportTo(Vector3 pos, bool ignoreY = true)
    {
        if(ignoreY)
        {
            pos.y = transform.position.y;
        }
        GetComponent<CharacterController>().enabled = false;
        transform.position = pos;
        GetComponent<CharacterController>().enabled = true;
    }

    public void ReleasePiece(EntityData piece)
    {
        var found = PiecesInventory.Find(e => e == piece);
        PiecesInventory.Remove(found);
    }

    public bool HasPieceInLayout(EntityData piece)
    {
        return PiecesInventory.Any(p => p == piece);
    }

    public int GetBiome()
    {
        if(House.InsideAnyHouse)
        {
            return 4;
        }
        if (transform.position.x > 0 && transform.position.z > 0)
        {
            return 0;
        }
        else if(transform.position.x < 0 && transform.position.z > 0)
        {
            return 1;
        }
        else if(transform.position.x < 0 && transform.position.z < 0)
        {
            return 2;
        }
        else if (transform.position.x > 0 && transform.position.z < 0)
        {
            return 3;
        }

        return -1;
    }

    public void PlayBiomeSound(string previous = null)
    {
        if(CurrentBiome == -1)
        {
            AudioManager.StopAll();
            return;
        }
        if (previous != null)
        {
            if (previous == "biome" + CurrentBiome)
                return;
            AudioManager.FadeOut(previous, 2);
        }
        AudioManager.FadeIn("biome" + CurrentBiome, 2);
    }
}
