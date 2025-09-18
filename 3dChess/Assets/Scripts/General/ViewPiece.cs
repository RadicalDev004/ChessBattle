using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ViewPiece : MonoBehaviour
{
    [Header("Logistics")]
    public Camera Basic;
    public Camera Piece;
    public GameObject Tab_Basic, Tab_Piece;
    private EntityData thisEntity;
    public bool State;

    [Header("Info")]
    public TMP_Text T_Name;
    public TMP_Text T_Level, T_Type;
    public Slider S_Health;

    public MoveUI OriginalMoveUI;   
    public GameObject MovesParent;
    public List<MoveUI> CurrentMoves;

    [Header("Models")]
    public List<GameObject> Pieces;
    public GameObject CurrentPiece;
    public Vector3 PodiumPos;

    [Header("Rotation")]
    public int Direction;

    private void FixedUpdate()
    {
        if (!State) return;

        if(Direction != 0)
        {
            CurrentPiece.transform.Rotate(0, 0, Direction);
        }
    }

    public void SetDirection(int dir) => Direction = dir;

    public void OpenViewPiece(EntityData e)
    {
        thisEntity = e;
        Basic.gameObject.SetActive(false);
        Tab_Basic.gameObject.SetActive(false);

        Piece.gameObject.SetActive(true);
        Tab_Piece.gameObject.SetActive(true);

        T_Name.text = e.Name;
        T_Type.text = e.PieceType.ToString();
        T_Level.text = "lvl " + e.Level;

        if(CurrentPiece != null) 
            Destroy(CurrentPiece);
        CurrentPiece = Instantiate(Pieces[(int)e.PieceType], transform);
        CurrentPiece.transform.localPosition = PodiumPos;
        CurrentPiece.SetActive(true);
        State = true;

        foreach(var m in CurrentMoves)
        {
            Destroy(m.gameObject);
        }
        CurrentMoves.Clear();

        foreach(var m in e.Moves)
        {
            var newMoveUI = Instantiate(OriginalMoveUI, MovesParent.transform);
            CurrentMoves.Add(newMoveUI);
            newMoveUI.gameObject.SetActive(true);
            newMoveUI.Create(m);           
        }
    }

    public void CloseViewPiece()
    {
        Basic.gameObject.SetActive(true);
        Tab_Basic.gameObject.SetActive(true);

        Piece.gameObject.SetActive(false);
        Tab_Piece.gameObject.SetActive(false);
        State = false;
    }
}

