using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum SoundGroup { Music, Effect, Death, Hit, Attack, Walk }

public class SoundManager : Singleton<SoundManager>
{
    public float randomPitch = 0.3f;

    //同时最大播放音效的数量
    public int maxAudioPlayCountAtOneTime = 6;
    List<AudioSource> audioSources;

	Dictionary<Sound, AudioSource> soundDic;

	public AudioMixerGroup audioMixerGroup;

	//初始化
	void Awake()
    {
        //创建音效播放器
        audioSources = new List<AudioSource>();
        for (int i = 0; i < maxAudioPlayCountAtOneTime; i++)
        {
            audioSources.Add(gameObject.AddComponent<AudioSource>());
        }

		soundDic = new Dictionary<Sound, AudioSource>();
    }

    //播放音效
    public void PlaySound(Sound _sound)
    {
        //获取闲置的声音组件并初始化
        AudioSource source = GetAvailableSource();
        InitAudioSource(source, _sound);

        if (_sound.clips.Length == 0)
            Debug.LogError("空的声音：" + _sound.name);

        //如果有多个声音片段则随机选择
        if (_sound.clips.Length > 1)
        {
            int random = Random.Range(0, _sound.clips.Length);
            source.clip = _sound.clips[random];
        }
		else
		{
			source.clip = _sound.clips[0];
		}

		//播放
		source.Play();

		//加入声音字典
		soundDic.Add(_sound, source);
    }
    public void PlaySound(string _name)
    {
        PlaySound(GetSound(_name));
    }

    //初始化声音组件
    void InitAudioSource(AudioSource _source, Sound _sound)
    {
        _source.volume = _sound.volume;
        _source.pitch = _sound.pitch;
        _source.loop = _sound.loop;
        //_source.outputAudioMixerGroup = _sound.audioMixerGroup;
    }

    //通过名字获取声音
    Sound GetSound(string _name)
    {
		Sound[] sounds = Resources.LoadAll<Sound>("ScriptableObject/Sound");

		foreach (Sound item in sounds)
		{
			if (item.name == _name)
				return item;
		}
		Debug.LogWarning("未能找到：" + _name);
		return null;
	}

	//停止播放
	public void StopPlay(Sound _sound)
	{
		soundDic[_sound].Stop();

		//移除字典
		soundDic.Remove(_sound);
	}
    public void StopPlay(string _name)
    {
		StopPlay(GetSound(_name));
    }

    //获取闲置的音效播放器
    AudioSource GetAvailableSource()
    {
        for (int i = 0; i < maxAudioPlayCountAtOneTime; i++)
        {
            if (audioSources[i].isPlaying == false)
            {
				//从字典移除
				
                return (audioSources[i]);
            }
        }

        return null;
    }
}
