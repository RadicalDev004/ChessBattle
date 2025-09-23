using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ListPieceUI : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public RawImage Icon;
    public TMP_Text T_Name, T_Type, T_Level;
    public Slider S_Health;
    private EntityData thisEntity;
    public LayoutEdit LayoutEdit;
    public PieceGraphic PieceGraphic;

    private UnityEngine.UI.Outline Outline;


    public void Create(EntityData data)
    {
        thisEntity = data;
        T_Name.text = thisEntity.Name;
        T_Type.text = $"<b><i>{thisEntity.PieceType}</b></i>";
        T_Level.text = "lvl " + thisEntity.Level;
        S_Health.maxValue = thisEntity.MaxHealth;
        S_Health.value = thisEntity.Health;

        GetComponent<Button>().onClick.RemoveAllListeners();
        GetComponent<Button>().onClick.AddListener(() => {
            FindObjectOfType<ViewPiece>().OpenViewPiece(thisEntity);
        });

        Outline = GetComponent<UnityEngine.UI.Outline>();

        UpdateOutline();
    }

    public void UpdateOutline()
    {
        Outline.enabled = thisEntity.Position != -1;
    }



    public void OnDrag(PointerEventData eventData)
    {
        if (thisEntity.Position != -1) return;
        if (LayoutEdit.PiecesOnBoard >= LayoutEdit.Limit) return;
        if(PieceGraphic == null)
        {
            PieceGraphic = LayoutEdit.CreatePieceGraphic(eventData.position, thisEntity);
        }
        else
        {
            PieceGraphic.rectTransform.anchoredPosition += eventData.delta;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(PieceGraphic != null)
        {
            PieceGraphic.OnEndDrag(eventData);
            PieceGraphic = null;
        }
    }
}
