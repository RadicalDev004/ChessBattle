using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class PieceGraphic : MonoBehaviour, IDragHandler,IEndDragHandler
{
    public TMP_Text T_Name;
    public bool isHeld;
    [HideInInspector]
    public RectTransform rectTransform;

    private EntityData thisEntity;
    public int position = -1;
    [HideInInspector]
    public LayoutEdit LayoutEdit;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Create(EntityData e, int position)
    {
        thisEntity = e;
        T_Name.text = e.Name;
        this.position = position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        int pos = LayoutEdit.GetTileIndexFronPosition(rectTransform.localPosition);
        if (pos == -2 ||(pos == -1 && position == -1))
        {
            thisEntity.Position = -1;           
            LayoutEdit.RemovePieceGraphic(this);
            LayoutEdit.UpdateLimit();
            LayoutEdit.UpdateAllListPieces();
            return;
        }
        if (pos != -1)
        {
            thisEntity.Position = pos;
            position = pos;
        }

        Tween.AnchoredPosition(rectTransform, LayoutEdit.Squares[position].GetComponent<RectTransform>().localPosition, 0.1f, 0, Tween.EaseInOut);
        LayoutEdit.UpdateAllListPieces();
        LayoutEdit.UpdateLimit();
    }
}
