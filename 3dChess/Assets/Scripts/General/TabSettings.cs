using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TabSettings : MonoBehaviour
{
    public Slider S_Volume, S_Sfx;
    public TMP_Text T_VolumeValue, T_SfxValue;

    private void Start()
    {
        S_Volume.onValueChanged.AddListener(UpdateVolume);
        S_Sfx.onValueChanged.AddListener(UpdateSfx);

        S_Volume.value = PlayerPrefs.GetFloat("volume", 0.5f);
        S_Sfx.value = PlayerPrefs.GetFloat("fxs", 0.5f);

        S_Volume.onValueChanged.AddListener(UpdateVolume);
        S_Sfx.onValueChanged.AddListener(UpdateSfx);
    }

    public void UpdateVolume(float val)
    {
        PlayerPrefs.SetFloat("volume", val);
        AudioManager.UpdateVolume();
        T_VolumeValue.text = Mathf.RoundToInt(val * 100) + "%";
    }

    public void UpdateSfx(float val)
    {
        PlayerPrefs.SetFloat("fxs", val);
        AudioManager.UpdateVolume();
        T_SfxValue.text = Mathf.RoundToInt(val * 100) + "%";
    }
}
