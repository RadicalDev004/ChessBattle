using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ref : MonoBehaviour
{
    public static Ref Instance;

    public ManageTiles manageTiles;
    public static ManageTiles ManageTiles { get {  return Instance.manageTiles; } }


    public Camera cam;
    public static Camera Camera { get { return Instance.cam; } }

    public Camera battleCamera;
    public static Camera BattleCamera {  get { return Instance.battleCamera; } }


    public BattleManager battleManager;
    public static BattleManager BattleManager {  get { return Instance.battleManager; } }

    public PieceUI orgPieceUI;
    public static PieceUI OrgPieceUI {  get { return Instance.orgPieceUI; } }

    public AI aI;
    public static AI AI { get { return Instance.aI; } }


    public BattleUI battleUI;
    public static BattleUI BattleUI { get { return Instance.battleUI; } }


    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }
}
