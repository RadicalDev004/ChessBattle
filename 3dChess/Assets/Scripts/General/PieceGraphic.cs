using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PieceGraphic : MonoBehaviour, IDragHandler,IEndDragHandler
{
    private Image Icon;
    public TMP_Text T_Name;
    public bool isHeld;
    [HideInInspector]
    public RectTransform rectTransform;
    [HideInInspector]
    public EntityData thisEntity;
    public int position = -1;
    [HideInInspector]
    public LayoutEdit LayoutEdit;

    private void Awake()
    {
        
        rectTransform = GetComponent<RectTransform>();
    }

    public void Create(EntityData e, int position)
    {
        Icon = GetComponent<Image>();
        thisEntity = e;
        T_Name.text = e.Name;
        this.position = position;
        GetIcon();
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

    public void GetIcon()
    {
        Sprite mySprite = Resources.Load<Sprite>($"{thisEntity.PieceType}/{thisEntity.Variant}");

        Icon.sprite = mySprite;
        FitImageToSize(Icon, Icon.rectTransform.sizeDelta.x);
    }
    public void FitImageToSize(Image img, float size)
    {
        img.SetNativeSize();
        float max = Mathf.Max(img.rectTransform.sizeDelta.x, img.rectTransform.sizeDelta.y);
        float rate = max / size;
        img.rectTransform.sizeDelta = new Vector2(img.rectTransform.sizeDelta.x / rate, img.rectTransform.sizeDelta.y / rate);
    }
}
