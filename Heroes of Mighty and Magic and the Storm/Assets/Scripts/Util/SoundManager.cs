using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : Singleton<SoundManager>
{
    public Sound[] sounds = new Sound[1];

    public float randomPitch = 0.3f;

    //同时最大播放音效的数量
    public int maxAudioPlayCountAtOneTime = 6;
    List<AudioSource> audioSources;

    void Start() 
	{
        //初始化预设音效
        foreach (Sound s in sounds)
        {
            if (s.multipleSound)
                continue;

            CreateSoundComponent(s);
        }

        //初始创建音效播放器
        audioSources = new List<AudioSource>();
        for (int i = 0; i < maxAudioPlayCountAtOneTime; i++)
        {
            audioSources.Add(gameObject.AddComponent<AudioSource>());
        }
    }

    AudioSource CreateSoundComponent(Sound s)
    {
        s.source = gameObject.AddComponent<AudioSource>();
        if (s.audioMixerGroup != null)
            s.source.outputAudioMixerGroup = s.audioMixerGroup;
        s.source.clip = s.clip;
        s.source.volume = s.volume;
        s.source.pitch = s.pitch;
        s.source.loop = s.loop;

        return s.source;
    }

    public AudioSource Play(string name)
    {
        Sound s = null;
        foreach (Sound item in sounds)
        {
            if(name == item.name)
            {
                s = item;
            }
        }
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return null;
        }

        if(!s.multipleSound)
        {
            if(s.clips.Length == 0)
                s.source.Play();
            else
            {
                int randomID = Random.Range(0, s.clips.Length);
                s.source.clip = s.clips[randomID];
                s.source.Play();
            }
        }
        else
        {
            //同时可能有多个的声音，而且会单独关闭
            AudioSource source = CreateSoundComponent(s);
            source.Play();
            return source;
        }

        return null;
    }

    public void StopPlay(string name)
    {
        Sound s = null;
        foreach (Sound item in sounds)
        {
            if (name == item.name)
            {
                s = item;
            }
        }
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        s.source.Stop();
    }

    //播放任意音效
    public void PlaySound(AudioClip _clip, bool _random = false)
    {
        if (_clip == null)
            return;

        //如果有没在播放的音效播放器
        AudioSource audioSource = GetAvailableSource();

        if (audioSource != null)
        {
            audioSource.clip = _clip;

            //随机音高
            if (_random)
                audioSource.pitch = 1 + Random.Range(0, 1f) * randomPitch;

            audioSource.Play();
        }
    }
    //播放修饰过的音效
    public void PlaySound(FixedSound _sound, bool _random = false)
    {
        AudioSource audioSource = GetAvailableSource();
        if (audioSource != null)
        {
            //是多个音效则随机播放一个
            if(_sound.clips.Length > 0)
                audioSource.clip = _sound.clips[Random.Range(0, _sound.clips.Length)];
            else
                audioSource.clip = _sound.clip;
            audioSource.time = _sound.skipDuration;
            audioSource.Play();
        }
    }

    //获取没在播放的音效播放器
    AudioSource GetAvailableSource()
    {
        for (int i = 0; i < maxAudioPlayCountAtOneTime; i++)
        {
            if (audioSources[i].isPlaying == false)
            {
                return (audioSources[i]);
            }
        }

        return null;
    }
}

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    public AudioClip[] clips;

    [Range(0f, 1f)]
    public float volume = 1;
    [Range(.1f, 3f)]
    public float pitch = 1;

    public bool loop;

    [HideInInspector]
    public AudioSource source;

    public AudioMixerGroup audioMixerGroup;

    public bool multipleSound;
}


[System.Serializable]
public class FixedSound
{
    public AudioClip clip;
    public AudioClip[] clips;
    public float skipDuration;
}


