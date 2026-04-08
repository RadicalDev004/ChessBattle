using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class ReleaseTab : MonoBehaviour
{
    private CanvasGroup canvasGroup;

    public TMP_Text T_Info, T_Coins;

    public Button B_Keep, B_Release;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Create(EntityData p, Action OnRelease)
    {
        print("Attempting to create release tab for " + p.Name);
        canvasGroup.Activate();
        var amount = p.GetCoinsOnRelease();

        T_Coins.text = amount.ToSpacedNumber();
        T_Info.text = $"Are you sure you want to release <i><b>{p.Name}</b></i> for coins?";

        B_Keep.onClick.RemoveAllListeners();
        B_Release.onClick.RemoveAllListeners();

        B_Keep.onClick.AddListener(() => canvasGroup.Deactivate());
        B_Release.onClick.AddListener(() =>
        {
            canvasGroup.Deactivate();
            ShopManager.Coins += amount;
            GameRef.PlayerBehaviour.ReleasePiece(p);
            OnRelease?.Invoke();
            GameRef.PlayerBehaviour.SaveManager.SaveGame();
        });
    }
}
