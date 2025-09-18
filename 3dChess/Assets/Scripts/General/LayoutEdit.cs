using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LayoutEdit : MonoBehaviour
{
    public PlayerBehaviour player;
    public GameObject LayoutParent;
    public GameObject ListPieceParent;

    public List<Image> Squares = new();
    public List<PieceGraphic> PieceGraphics = new();
    public List<ListPieceUI> ListPiecesUI = new();

    public Image OrgSquare;
    public PieceGraphic OrgPieceGraphic;
    public ListPieceUI OrgListPiece;

    public TMP_Text T_Limit;
    public TMP_Text T_Warning;

    public int Limit = 8;

    public Color C1, C2;

    private void Start()
    {
        GenerateSquares();       
        Invoke(nameof(PlacePieces), 0.5f);
    }

    public void GenerateSquares()
    {
        for(int i = 0; i < 8; i++)
        {
            for(int j = 0; j < 8; j++)
            {
                var newSquare = Instantiate(OrgSquare, LayoutParent.transform);
                Squares.Add(newSquare);
                newSquare.gameObject.SetActive(true);
                newSquare.color = (i + j) % 2 == 1 ? C1 : C2;
            }
        }
    }

    public int PiecesOnBoard { get { return PieceGraphics.Where(p => p.position != -1).Count(); } }

    public void PlacePieces()
    {
        foreach(var p in player.PiecesInventory)
        {
            var newListPiece = Instantiate(OrgListPiece, ListPieceParent.transform);
            newListPiece.gameObject.SetActive(true);
            newListPiece.Create(p);
            ListPiecesUI.Add(newListPiece);

            if (p.Position == -1) continue;

            var newPieceGraphic = Instantiate(OrgPieceGraphic, LayoutParent.transform);
            newPieceGraphic.Create(p, p.Position);
            newPieceGraphic.LayoutEdit = this;

            newPieceGraphic.GetComponent<RectTransform>().localPosition = Squares[p.Position].GetComponent<RectTransform>().localPosition;
            newPieceGraphic.gameObject.SetActive(true);
            PieceGraphics.Add(newPieceGraphic);
        }
        UpdateLimit();
    }

    public void UpdateLimit()
    {
        T_Limit.text = PiecesOnBoard + "/" + Limit;
    }

    public void RemovePieceGraphic(PieceGraphic pg)
    {
        PieceGraphics.Remove(pg);
        Destroy(pg.gameObject);
    }

    public PieceGraphic CreatePieceGraphic(Vector3 pos, EntityData p)
    {
        var newPieceGraphic = Instantiate(OrgPieceGraphic, LayoutParent.transform);
        newPieceGraphic.Create(p, p.Position);
        newPieceGraphic.LayoutEdit = this;
        newPieceGraphic.GetComponent<RectTransform>().position = pos;
        newPieceGraphic.gameObject.SetActive(true);
        PieceGraphics.Add(newPieceGraphic);
        return newPieceGraphic;
    }

    public int GetTileIndexFronPosition(Vector3 pos)
    {
        if (IsToTheRightOfLayout(pos)) return -2;

        for (int i = 0; i < 4 * 8; i++)
        {
            if (IsInsideSquare(pos, Squares[i].GetComponent<RectTransform>().localPosition, 80) && player.PiecesInventory.Find(e => e.Position == i) == null)
                return i;
        }
        return -1;
    }

    bool IsInsideSquare(Vector2 point, Vector2 center, float sideLength)
    {
        float halfSide = sideLength / 2f;

        return
            point.x >= center.x - halfSide &&
            point.x <= center.x + halfSide &&
            point.y >= center.y - halfSide &&
            point.y <= center.y + halfSide;
    }

    bool IsToTheRightOfLayout(Vector2 point)
    {
        return point.x > LayoutParent.GetComponent<RectTransform>().sizeDelta.x / 2;
    }

    public void UpdateAllListPieces()
    {
        ListPiecesUI.ForEach(p => { p.UpdateOutline(); });
    }

    public void CloseTab(CanvasGroup Tab_Layout)
    {
        if(PieceGraphics.Where(p => p.thisEntity.PieceType == EntityData.Type.King).Count() == 0)
        {
            ShowWarning("Loadout has no Kings!");
            return;
        }
        if (PieceGraphics.Where(p => p.thisEntity.PieceType == EntityData.Type.King).Count() > 1)
        {
            ShowWarning("Loadout has more than one King!");
            return;
        }

        Tab_Layout.alpha = 0;
        Tab_Layout.interactable = false;
        Tab_Layout.blocksRaycasts = false;
        player.SavePieces();
    }


    private Coroutine warningCor;
    public void ShowWarning(string text)
    {
        if(warningCor != null)
            StopCoroutine(warningCor);
        warningCor = StartCoroutine(ShowWarningCor(text, 2));
    }

    private IEnumerator ShowWarningCor(string text, float time)
    {
        T_Warning.text = text;
        yield return new WaitForSecondsRealtime(time);
        T_Warning.text = "";
    }
}
