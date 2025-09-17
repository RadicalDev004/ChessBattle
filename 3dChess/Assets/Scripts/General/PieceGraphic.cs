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
    private RectTransform rectTransform;

    private EntityData thisEntity;
    public int position;
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
        if (pos != -1)
        {
            LayoutEdit.player.ChangePiecePosition(position, pos);
            position = pos;
        }
        Tween.AnchoredPosition(rectTransform, LayoutEdit.Squares[position].GetComponent<RectTransform>().localPosition, 0.1f, 0, Tween.EaseInOut);
    }
}
