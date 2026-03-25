using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Search;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public TMP_Text T_Coins;
    public GameObject ShopIcon;

    public static int Coins;

    private void Awake()
    {
        var currY = ShopIcon.transform.position.y;
        Tween.Position(ShopIcon.transform, new Vector3(ShopIcon.transform.position.x, currY + 0.15f, ShopIcon.transform.position.z), 1f, 0, Tween.EaseOut, loop: Tween.LoopType.PingPong);
    }

    private void Start()
    {
        UpdateCoins();
    }
    private void Update()
    {
        ShopIcon.transform.LookAt(GameRef.PlayerBehaviour.Camera.transform);
    }

    public void UpdateCoins()
    {
        T_Coins.text = Coins.ToSpacedNumber();
    }

    public void GivePotion(int index)
    {
        GameRef.PlayerBehaviour.AddPotionToInventory(Variants.GetPotionByIndex(index));
    }

    public void GiveCoins(int amount)
    {
        Coins += amount;
        UpdateCoins();
    }
}
