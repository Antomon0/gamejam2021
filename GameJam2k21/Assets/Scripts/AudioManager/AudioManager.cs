using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    public static AudioManager instance;

    public AudioMixerGroup mixerGroup;

    public Sound[] sounds;

    List<SoundChange> changes = new List<SoundChange>();
    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.clip = s.clip;
            s.source.loop = s.loop;

            s.source.outputAudioMixerGroup = mixerGroup;
        }
    }

    public void Play(string sound)
    {
        Sound s = Array.Find(sounds, item => item.name == sound);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        // s.source.volume = s.volume * (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
        // s.source.pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));

        s.source.Play();
    }

    public void Stop(string sound)
    {
        Sound s = Array.Find(sounds, item => item.name == sound);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        s.source.Stop();
    }

    public void changeSoundParamFromTime(string sound, string param, float newValue, float time)
    {
        Sound s = Array.Find(sounds, item => item.name == sound);
        switch (param)
        {
            case "pitch":
                changes.Add(new SoundChange(time, newValue, param, s, s.source.pitch));
                break;
            case "volume":
                changes.Add(new SoundChange(time, newValue, param, s, s.source.volume));
                break;
        }

    }

    public void ChangeSoundParam(string sound, string param, float newValue)
    {
        Sound s = Array.Find(sounds, item => item.name == sound);
        switch (param)
        {
            case "pitch":
                s.source.pitch = newValue;
                break;
            case "volume":
                s.source.volume = newValue;
                break;
        }
    }

    void Update()
    {
        // for (int index = 0; index < changes.Count; index++)
        // {
        //     SoundChange c = changes[index];
        //     c.timeRemaining -= Time.deltaTime;
        //     if (c.timeRemaining < 0)
        //     {
        //         if (!c.isReverse)
        //         {
        //             changeSoundParam(c.sound.name, c.paramToChange, c.initialValue, c.timeForChange);
        //         }
        //         changes.RemoveAt(index);
        //         index--;
        //     }
        //     else
        //     {
        //         float resultValue = c.initialValue + (1 - c.timeRemaining / c.timeForChange) * c.diffValue;
        //         switch (c.paramToChange)
        //         {
        //             case "pitch":
        //                 c.sound.source.pitch = resultValue;
        //                 break;
        //             case "volume":
        //                 c.sound.source.volume = resultValue;
        //                 break;
        //         }
        //     }
        // }
        // if (Input.GetKeyDown("z"))
        // {
        //     changeSoundParam("Soundtrack", "volume", 1.5f, 1f);
        // }
        // if (Input.GetKeyDown("x"))
        // {
        //     changeSoundParam("Hover", "volume", 1f, 1f);
        // }
    }
}
