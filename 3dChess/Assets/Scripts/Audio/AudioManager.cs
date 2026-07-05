using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;
    public static AudioManager Instance;

    private Coroutine FadeInC, FadeOutC;

    void Awake()
    {
        if (Instance == null) Instance = this;

        if(!PlayerPrefs.HasKey("volume"))
        {
            PlayerPrefs.SetFloat("volume", 0.5f);
        }
        if (!PlayerPrefs.HasKey("fxs"))
        {
            PlayerPrefs.SetFloat("fxs", 0.5f);
        }

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.loop = s.loop;
            s.source.playOnAwake = false;
        }
    }

    public static void UpdateVolume()
    {
        if(Instance == null)
            return;
        foreach (Sound s in Instance.sounds)
        {
            s.source.volume = s.effect ? (float)PlayerPrefs.GetFloat("fxs", 1) : (float)PlayerPrefs.GetFloat("volume", 1);
        }
    }

    public AudioSource GetSoundByName(string name)
    {
        Sound d = Array.Find(sounds, sound => sound.name == name);

        return d.source;
    }

    public void ChangeSoundVolume(string name, float volume)
    {
        Sound d = Array.Find(sounds, sound => sound.name == name);
        if (d == null)
            return;

        d.source.volume = volume;
    }

    public void PlaySound(string name)
    {
        Sound d = Array.Find(sounds, sound => sound.name == name);
        if (d == null)
            return;

        //d.source.pitch = 1f;
        d.source.Play();
    }
    public void PlayReversedSound(string name)
    {
        Sound d = Array.Find(sounds, sound => sound.name == name);
        if (d == null)
            return;

        d.source.pitch = -1;
        d.source.Play();
    }
    public void StopSound(string name)
    {
        Sound d = Array.Find(sounds, sound => sound.name == name);
        if (d == null)
            return;

        d.source.Stop();
    }

    public void FadeOutSound(string name, float duration)
    {
        Sound d = Array.Find(sounds, sound => sound.name == name);
        if (d == null)
            return;

        if(FadeOutC != null)
        {
            StopCoroutine(FadeOutC);
        }

        FadeOutC = StartCoroutine(FadeOutCoroutine(d.source, duration));
    }

    public void FadeInSound(string name, float duration)
    {
        Sound d = Array.Find(sounds, sound => sound.name == name);
        if (d == null)
            return;

        if(FadeInC != null)
        {
            StopCoroutine(FadeInC);
        }

        FadeInC = StartCoroutine(FadeInCoroutine(d.source, duration));
    }

    private IEnumerator FadeInCoroutine(AudioSource source, float duration)
    {
        float targetVolume = source.volume;
        source.volume = 0;
        source.Play();
        while (source.volume < targetVolume)
        {
            source.volume += targetVolume * Time.deltaTime / duration;
            yield return null;
        }
        source.volume = targetVolume;
    }

    private IEnumerator FadeOutCoroutine(AudioSource source, float duration)
    {
        float startVolume = source.volume;
        while (source.volume > 0)
        {
            source.volume -= startVolume * Time.deltaTime / duration;
            yield return null;
        }
        source.Stop();
        source.volume = startVolume;
    }

    public void StopAllSounds()
    {
        foreach (Sound s in sounds)
        {
            s.source.Stop();
        }
    }
    public bool IsPlayingSound(string name)
    {
        Sound d = Array.Find(sounds, sound => sound.name == name);
        return d.source.isPlaying;
    }


    public static AudioSource GetSound(string name)
    {
        try
        {
            return Instance.GetSoundByName(name);
        }
        catch (NullReferenceException)
        {
            Debug.LogWarning("Entering game from level scene will lead to loss of Audio and is not reccomended!");
            return null;
        }
    }

    public static void PlayRandom(params string[] sounds)
    {
        if (sounds.Length == 0)
            return;
        int index = UnityEngine.Random.Range(0, sounds.Length);
        Play(sounds[index]);
    }

    public static void Play(string name)
    {
        try
        {
             Instance.PlaySound(name);

        }
        catch (NullReferenceException)
        {
            Debug.LogWarning("Entering game from level scene will lead to loss of Audio and is not reccomended!");
        }

    }

    public static void FadeOut(string name, float duration)
    {
        try
        {
            Instance.FadeOutSound(name, duration);
        }
        catch (NullReferenceException)
        {
            Debug.LogWarning("Entering game from level scene will lead to loss of Audio and is not reccomended! " + name);
        }
    }

    public static void FadeIn(string name, float duration)
    {
        try
        {
            Instance.FadeInSound(name, duration);
        }
        catch (NullReferenceException)
        {
            Debug.LogWarning("Entering game from level scene will lead to loss of Audio and is not reccomended! " + name);
        }
    }

    public static void ChangeVolume(string name, float volume)
    {
        Instance.ChangeSoundVolume(name, volume);
    }

    public static void Stop(string name)
    {
        try
        {
            Instance.StopSound(name);
        }
        catch (NullReferenceException)
        {
            Debug.LogWarning("Entering game from level scene will lead to loss of Audio and is not reccomended! " + name);
        }
    }
    public static void StopAll()
    {
        try
        {
            Instance.StopAllSounds();
        }
        catch (NullReferenceException)
        {
            Debug.LogWarning("Entering game from level scene will lead to loss of Audio and is not reccomended!");
        }
    }

    public static bool IsPlaying(string name)
    {
        try
        {
            return Instance.IsPlayingSound(name);
        }
        catch(NullReferenceException)
        {
            Debug.LogWarning("Entering game from level scene will lead to loss of Audio and is not reccomended!");
        }
        return false;
    }
}
