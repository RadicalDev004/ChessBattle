using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OnlinePlay : MonoBehaviour
{
    public TMP_InputField In_Name;
    public TMP_Text T_Error;
    public Toggle Tog_Private;

    public Sprite S_Selected, S_Normal;

    public Button B_Regular, B_CreateRoom, B_JoinRoom;
    public GameObject G_Regular, G_CreateRoom, G_JoinRoom;
    public TMP_InputField In_Code;
    public int PrivateInd;

    private void Awake()
    {
        G_Regular.SetActive(true);
        G_CreateRoom.SetActive(false);
        G_JoinRoom.SetActive(false);

        B_Regular.onClick.AddListener(() =>
        {
            B_Regular.image.sprite = S_Selected;
            B_CreateRoom.image.sprite = S_Normal;
            B_JoinRoom.image.sprite = S_Normal;
    
            G_Regular.SetActive(true);
            G_CreateRoom.SetActive(false);
            G_JoinRoom.SetActive(false);

            PrivateInd = 0;
        });

        B_CreateRoom.onClick.AddListener(() =>
        {
            B_Regular.image.sprite = S_Normal;
            B_CreateRoom.image.sprite = S_Selected;
            B_JoinRoom.image.sprite = S_Normal;

            G_Regular.SetActive(false);
            G_CreateRoom.SetActive(true);
            G_JoinRoom.SetActive(false);

            PrivateInd = 1;
        });

        B_JoinRoom.onClick.AddListener(() =>
        {
            B_Regular.image.sprite = S_Normal;
            B_CreateRoom.image.sprite = S_Normal;
            B_JoinRoom.image.sprite = S_Selected;

            G_Regular.SetActive(false);
            G_CreateRoom.SetActive(false);
            G_JoinRoom.SetActive(true);

            PrivateInd = 2;
        });
    }

    public void PlayOnline()
    {
        if(string.IsNullOrWhiteSpace(In_Name.text))
        {
            T_Error.text = "Please enter a username.";
            this.ActionAfterTime(2f, () => T_Error.text = "");
            return;
        }
        if(PrivateInd == 2)
        {
            var code = In_Code.text;
            if (string.IsNullOrWhiteSpace(code))
            {
                T_Error.text = "Private room code cannot be empty.";
                this.ActionAfterTime(2f, () => T_Error.text = "");
                return;
            }

            if (!Guid.TryParse(code, out _))
            {
                T_Error.text = "Invalid code.";
                this.ActionAfterTime(2f, () => T_Error.text = "");
                return;
            }

            PlayerPrefs.SetString("privateCode", code);
        }
        PlayerPrefs.SetInt("private", PrivateInd);

        GameRef.PlayerBehaviour.SaveManager.SetName(In_Name.text);
        GameRef.PlayerBehaviour.SaveManager.SaveGame();

        PlayerPrefsExtentions.SetBool("online", true);

        SceneManager.LoadScene("Chess");
    }
}
