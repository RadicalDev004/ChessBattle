using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LayoutEdit : MonoBehaviour
{
    public PlayerBehaviour player;
    public GameObject LayoutParent;

    public List<Image> Squares = new();
    public List<PieceGraphic> PieceGraphics = new();

    public Image OrgSquare;
    public PieceGraphic OrgPieceGraphic;

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

    public void PlacePieces()
    {
        foreach(var p in player.PiecesInventory)
        {
            if (p.Position == -1) continue;
            var newPieceGraphic = Instantiate(OrgPieceGraphic, LayoutParent.transform);
            newPieceGraphic.Create(p, p.Position);
            newPieceGraphic.LayoutEdit = this;

            newPieceGraphic.GetComponent<RectTransform>().localPosition = Squares[p.Position].GetComponent<RectTransform>().localPosition;
            newPieceGraphic.gameObject.SetActive(true);
            PieceGraphics.Add(newPieceGraphic);
        }
    }

    public int GetTileIndexFronPosition(Vector3 pos)
    {
        for (int i = 0; i < 4 * 8; i++)
        {
            if (IsInsideSquare(pos, Squares[i].GetComponent<RectTransform>().localPosition, 80) && player.PiecesInventory.Find(e => e.Position == i) != null)
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
}
