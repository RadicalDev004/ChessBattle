using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    public TMP_Text T_Info;
    public Button B_Cancel;
    public GameObject Tab_LoadingScreen;

    private void Awake()
    {
        B_Cancel.onClick.RemoveAllListeners();
        B_Cancel.onClick.AddListener(async () => {
            await Cancel();
        });
    }

    public void SetInfo(string info)
    {
        T_Info.text = info;
    }

    public void Toggle(bool toggle)
    {
        Tab_LoadingScreen.SetActive(toggle);
    }

    public void HideCancel()
    {
        B_Cancel.gameObject.SetActive(false);
    }

    public async Task Cancel()
    {
        SetInfo("Cancelling...");
        HideCancel();

        if (await Ref.OnlineManager.CancelMatch())
        { 
            SceneManager.LoadScene("Game"); 
        }
        else
        {
            SetInfo("Failed Cancel");
        }
    }
}
