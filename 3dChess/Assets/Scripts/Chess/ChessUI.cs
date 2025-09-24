using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessUI : MonoBehaviour
{
    public GameObject Tab_Win, Tab_Lose, Tab_Settings;

    public void WinUI()
    {
        Tab_Win.SetActive(true);
    }

    public void LoseUI()
    {
        Tab_Lose.SetActive(true);
    }

    public void ToggleSettings(bool state)
    {
        Tab_Settings.SetActive(state);
        Time.timeScale = state ? 0.0f : 1.0f;
    }
}
