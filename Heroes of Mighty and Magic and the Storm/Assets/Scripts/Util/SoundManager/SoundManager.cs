using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum SoundGroup { Music, Effect, Death, Hit, Attack, Walk, UI, Impact }

public class SoundManager : Singleton<SoundManager>
{
    //同时最大播放音效的数量
    public int maxAudioPlayCountAtOneTime = 12;
    //同时播放的最大同种音效数量
    public int maxSameAudioPlayCountAtOneTime = 5;

    List<AudioSource> audioSources;

    Dictionary<AudioSource, Sound> soundDic;

    public AudioMixerGroup audioGroup_music;
    public AudioMixerGroup audioGroup_effect;

    //用来保存某种音效的同时播放数量
    Dictionary<Sound, int> soundNumber;

    //初始化
    void Awake()
    {
        //创建音效播放器
        audioSources = new List<AudioSource>();
        for (int i = 0; i < maxAudioPlayCountAtOneTime; i++)
        {
            audioSources.Add(gameObject.AddComponent<AudioSource>());
        }

        soundDic = new Dictionary<AudioSource, Sound>();
        soundNumber = new Dictionary<Sound, int>();
    }

    //播放音效
    public void PlaySound(Sound _sound)
    {
        if (_sound.clips.Length == 0)
            Debug.LogError("空的声音：" + _sound.name);

        //同时播放的这种音效数量超过限制，则不播放
        if (soundNumber.ContainsKey(_sound) && soundNumber[_sound] >= maxSameAudioPlayCountAtOneTime)
            return;

        //否则加入键或者键值+1
        if (soundNumber.ContainsKey(_sound))
            soundNumber[_sound]++;
        else
            soundNumber.Add(_sound, 1);

        //获取闲置的声音组件并初始化
        AudioSource source = GetAvailableSource();
        if (source == null)
            return;

        InitAudioSource(source, _sound);

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
        soundDic.Add(source, _sound);

        StartCoroutine(FinishPlaying(source));
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

        //设置声音组
        if (_sound.group == SoundGroup.Music)
            _source.outputAudioMixerGroup = audioGroup_music;
        else
            _source.outputAudioMixerGroup = audioGroup_effect;

        //开始时间
        _source.time = _sound.startingTime;
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
        List<AudioSource> sourceList = new List<AudioSource>();
        foreach (var item in soundDic)
        {
            if (item.Value == _sound)
            {
                sourceList.Add(item.Key);
            }
        }

        foreach (var item in sourceList)
        {
            item.Stop();

            RemoveSound(item);
        }
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
            if (!soundDic.ContainsKey(audioSources[i]))
            {
                return (audioSources[i]);
            }
        }

        return null;
    }

    //等待音效播放完毕
    IEnumerator FinishPlaying(AudioSource _source)
    {
        yield return new WaitForSeconds(_source.clip.length - _source.time);

        RemoveSound(_source);
    }

    //声音播放完毕，或被停止
    void RemoveSound(AudioSource _source)
    {
        //从音效数量字典移除
        if (soundNumber[soundDic[_source]] > 1)
            soundNumber[soundDic[_source]]--;
        else
            soundNumber.Remove(soundDic[_source]);

        //从字典移除
        soundDic.Remove(_source);
    }
}
