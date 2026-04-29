using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound
{
    public string name;
    
    public AudioClip clip;

    public bool loop;

    public bool effect;

    [HideInInspector]
    public AudioSource source;
}
