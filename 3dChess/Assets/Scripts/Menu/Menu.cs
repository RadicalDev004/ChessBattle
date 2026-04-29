using Pixelplacement;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public Image Title, I_Interactive;
    public Button B_Play, B_Exit;
    public GameObject Loads;
    public LoadUI OriginalLoadUI;

    public TMP_InputField In_NewSaveName;
    public TMP_Text T_Err;
    public TMP_Text T_NoSaves;

    public bool Animating;


    private void Awake()
    {
        GetSaves();
        Loads.SetActive(false);
        I_Interactive.rectTransform.anchoredPosition = new Vector3(-I_Interactive.rectTransform.anchoredPosition.x, I_Interactive.rectTransform.anchoredPosition.y, 0);
        Tween.LocalScale(Title.rectTransform, Vector3.one * 1.25f, 1.5f, 0, Tween.EaseInOut, Tween.LoopType.PingPong);
    }

    private void Start()
    {
        AudioManager.StopAll();
        AudioManager.Play("menu");
    }

    public void PlayNewSave()
    {
        if(string.IsNullOrEmpty(In_NewSaveName.text))
        {
            T_Err.text = "Name cannot be empty!";
            this.ActionAfterTime(2f, () => T_Err.text = "");
            return;
        }


        for (int i = 0; i < PlayerPrefs.GetInt("savesCount", 0); i++)
        {
            if(PlayerPrefs.GetString($"save{i}_name", "Unknown") == In_NewSaveName.text)
            {
                T_Err.text = "Name already exists!";
                this.ActionAfterTime(2f, () => T_Err.text = "");
                return;
            }
        }

        int newIndex = PlayerPrefs.GetInt("savesCount", 0);

        PlayerPrefs.SetInt("savesCount", newIndex + 1);
        PlayerPrefs.SetString($"save{newIndex}_name", In_NewSaveName.text);
        PlayerPrefs.SetInt("currentSave", newIndex);
        PlayerPrefs.SetString($"save{newIndex}_lastPlayedAt", DateTime.Now.ToString("g"));

        SceneManager.LoadScene("Game");
    }

    public void GetSaves()
    {
        int cnt = PlayerPrefs.GetInt("savesCount", 0);

        T_NoSaves.gameObject.SetActive(cnt == 0);

        for (int i = 0; i < cnt; i++)
        {
            LoadUI loadUI = Instantiate(OriginalLoadUI, OriginalLoadUI.transform.parent);
            loadUI.gameObject.SetActive(true);
            loadUI.Create(i);
        }
    }

    public void PlayGame()
    {
        if (Animating)
            return;
        Animating = true;

        Tween.LocalPosition(Title.rectTransform, Title.rectTransform.localPosition + new Vector3(550, 0, 0), 0.5f, 0f, Tween.EaseInOut);
        Tween.LocalPosition(B_Play.image.rectTransform, B_Play.image.rectTransform.localPosition + new Vector3(0, -400, 0), 0.5f, 0f, Tween.EaseInOut);
        Tween.LocalPosition(B_Exit.image.rectTransform, B_Exit.image.rectTransform.localPosition + new Vector3(-350, 0, 0), 0.5f, 0f, Tween.EaseInOut);

        Tween.AnchoredPosition(I_Interactive.rectTransform, new Vector3(-I_Interactive.rectTransform.anchoredPosition.x, I_Interactive.rectTransform.anchoredPosition.y, 0), 0.5f, 0f, Tween.EaseInOut);
        ResetAnimatingFlag(0.5f);
    }

    public void GoBack()
    {
        if (Animating)
            return;
        Animating = true;

        Loads.SetActive(false);
        Tween.LocalPosition(Title.rectTransform, Title.rectTransform.localPosition + new Vector3(-550, 0, 0), 0.5f, 0f, Tween.EaseInOut);
        Tween.LocalPosition(B_Play.image.rectTransform, B_Play.image.rectTransform.localPosition + new Vector3(0, 400, 0), 0.5f, 0f, Tween.EaseInOut);
        Tween.LocalPosition(B_Exit.image.rectTransform, B_Exit.image.rectTransform.localPosition + new Vector3(350, 0, 0), 0.5f, 0f, Tween.EaseInOut);

        Tween.AnchoredPosition(I_Interactive.rectTransform, new Vector3(-I_Interactive.rectTransform.anchoredPosition.x, I_Interactive.rectTransform.anchoredPosition.y, 0), 0.5f, 0f, Tween.EaseInOut);
        ResetAnimatingFlag(0.5f);
    }

    public void ResetAnimatingFlag(float time)
    {
        this.ActionAfterTime(time, () => Animating = false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ActivateTab(CanvasGroup canvasGroup)
    {
        canvasGroup.Activate();
    }
    public void DeActivateTab(CanvasGroup canvasGroup)
    {
        canvasGroup.Deactivate();
    }
}
