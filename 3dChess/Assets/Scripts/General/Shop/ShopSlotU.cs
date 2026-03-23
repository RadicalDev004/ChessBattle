using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ShopSlotU : MonoBehaviour
{
    public string Name;
    public int Cost;
    public Image I_Icon;
    public TMP_Text T_Name, T_Cost;
    public Button B_Buy;
    public UnityEvent OnBuy;
    public string IconName;

    private void Awake()
    {
        T_Name.text = Name;
        T_Cost.text = Cost.ToString();

        B_Buy.onClick.AddListener(Buy);

        I_Icon.sprite = Resources.Load<Sprite>("Icons/Shop/" + IconName);
        I_Icon.Fit(100);
    }

    public void Buy()
    {
        if (PlayerPrefs.GetInt("coins", 0) < Cost)
            return;

        PlayerPrefs.SetInt("coins", PlayerPrefs.GetInt("coins", 0) - Cost);
        GameRef.ShopManager.UpdateCoins();

        OnBuy.Invoke();
    }
}
