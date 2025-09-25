using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BookPieceUI : MonoBehaviour
{
    public Image Icon;
    public TMP_Text T_Type;
    public Slider S_Attack, S_Defence, S_Speed, S_Luck;
    private EntityData thisEntity;
    public GameObject Overlay;
    public float fitTo;


    public void Create(EntityData data)
    {
        thisEntity = data;
        T_Type.text = $"<b><i>{thisEntity.PieceType}</i></b>";
        T_Type.color = GameColors.GetColorByType(data.PieceType);

        S_Attack.maxValue = 10;
        S_Defence.maxValue = 1000;
        S_Speed.maxValue = 10;
        S_Luck.maxValue = 10;

        S_Attack.value = data.Attack;
        S_Defence.value = data.MaxHealth;
        S_Speed.value = data.Speed;
        S_Luck.value = data.Luck;

        GetIcon();
    }


    public void GetIcon()
    {
        Sprite mySprite = Resources.Load<Sprite>($"Icons/{thisEntity.PieceType}/{thisEntity.Variant}");

        Icon.sprite = mySprite != null ? mySprite : Resources.Load<Sprite>($"Icons/{thisEntity.PieceType}/basic");
        FitImageToSize(Icon, fitTo);
    }
    public void FitImageToSize(Image img, float size)
    {
        img.SetNativeSize();
        float max = Mathf.Max(img.rectTransform.sizeDelta.x, img.rectTransform.sizeDelta.y);
        float rate = max / size;
        img.rectTransform.sizeDelta = new Vector2(img.rectTransform.sizeDelta.x / rate, img.rectTransform.sizeDelta.y / rate);
    }
}
