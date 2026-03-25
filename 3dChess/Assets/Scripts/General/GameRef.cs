using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRef : MonoBehaviour
{
    public static GameRef Instance;

    public TrainerSpeak trainerSpeak;
    public static TrainerSpeak TrainerSpeak { get { return Instance.trainerSpeak; } }

    public PlayerBehaviour playerBehaviour;
    public static PlayerBehaviour PlayerBehaviour {  get { return Instance.playerBehaviour; } }

    public LayoutEdit layoutEdit;
    public static LayoutEdit LayoutEdit {  get { return Instance.layoutEdit; } }

    public PotionsEdit potionsEdit;
    public static PotionsEdit PotionsEdit { get { return Instance.potionsEdit; } }

    public Canvas mainCanvas;
    public static Canvas MainCanvas { get { return Instance.mainCanvas; } }

    public UI ui;
    public static UI UI { get { return Instance.ui; } }

    public HouseManager houseManager;
    public static HouseManager HouseManager { get { return Instance.houseManager; } }

    public SearchableManager searchableManager;
    public static SearchableManager SearchableManager { get { return Instance.searchableManager; } }

    public HospitalEdit hospitalEdit;
    public static HospitalEdit HospitalEdit { get { return Instance.hospitalEdit; } }

    public ShopManager shopManager;
    public static ShopManager ShopManager { get { return Instance.shopManager; } }

    public ReleaseTab releaseTab;
    public static ReleaseTab ReleaseTab { get { return Instance.releaseTab; } }

    public BoxBehaviour boxBehaviour;
    public static BoxBehaviour BoxBehaviour { get { return Instance.boxBehaviour; } }

    public StoryManager storyManager;
    public static StoryManager StoryManager { get { return Instance.storyManager; } }

    public QuestManager questManager;
    public static QuestManager QuestManager { get { return Instance.questManager; } }

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
    }
}
