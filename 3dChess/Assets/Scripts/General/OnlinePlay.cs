using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OnlinePlay : MonoBehaviour
{
    public TMP_InputField In_Name;
    public TMP_Text T_Error;

    public void PlayOnline()
    {
        if(string.IsNullOrWhiteSpace(In_Name.text))
        {
            T_Error.text = "Please enter a username.";
            this.ActionAfterTime(2f, () => T_Error.text = "");
            return;
        }
        GameRef.PlayerBehaviour.SaveManager.SetName(In_Name.text);
        GameRef.PlayerBehaviour.SaveManager.SaveGame();

        PlayerPrefsExtentions.SetBool("online", true);

        SceneManager.LoadScene("Chess");
    }
}
