// AudioManage.cs 音效管理器
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

[System.Serializable]
public class Sound
{
    public string name;         // 音频剪辑的名称
    public AudioClip clip;      // 音频剪辑
    // [Range(0f, 1f)]
    // public float volume = 1f; // 音量大小
}

public class AudioManage : Singleton<AudioManage>
{
    // 音效数组
    public Sound[] musicSounds, sfxSounds;
    // 音乐和音效的AudioSource
    public AudioSource musicSource, sfxSource;
    public AudioItlem audioPrefab;
    public ObjectPool<AudioItlem> audioPool;

    private float sfxVolume = 1f; // 音效音量，默认1

    // 用于跟踪活动音效项的列表
    private List<AudioItlem> activeAudioItems = new List<AudioItlem>();
    public void Init()
    {
        // 初始化音效池
        audioPool = new ObjectPool<AudioItlem>(
            () => Create(audioPrefab.gameObject, transform),
            Get,
            Release,
            MyDestroy,
            true,
            500,
            2000
        );

        List<AudioItlem> tempObjList = new List<AudioItlem>();
        // 预制音效池
        int readyCount = 500;
        for (int j = 0; j < readyCount; j++)
            tempObjList.Add(audioPool.Get());
        for (int j = 0; j < tempObjList.Count; j++)
            audioPool.Release(tempObjList[j]);
        tempObjList.Clear();

        // 动态加载背景音乐 'BacMusic'
        Sound bacMusicSound = new Sound();
        bacMusicSound.name = ConfigManager.Instance.Tables.TableSoundConfig.Get(1).BeijingMP3;
        bacMusicSound.clip = Resources.Load<AudioClip>("Music/" + ConfigManager.Instance.Tables.TableSoundConfig.Get(1).BeijingMP3);

        // 动态加载音效 'zhandouMP3'
        Sound gunMusicSound = new Sound();
        gunMusicSound.name = ConfigManager.Instance.Tables.TableSoundConfig.Get(1).ZhandouMP3;
        gunMusicSound.clip = Resources.Load<AudioClip>("Music/" + ConfigManager.Instance.Tables.TableSoundConfig.Get(1).ZhandouMP3);

        // 将背景音乐添加到 musicSounds 数组
        musicSounds = new Sound[] { bacMusicSound , gunMusicSound };


        // 动态加载音效 'CoinMusic'
        Sound coinMusicSound = new Sound();
        coinMusicSound.name = ConfigManager.Instance.Tables.TableSoundConfig.Get(1).CoinMp3;
        coinMusicSound.clip = Resources.Load<AudioClip>("Music/" + ConfigManager.Instance.Tables.TableSoundConfig.Get(1).CoinMp3);


        // 动态加载音效 'bossshowMP3'
        Sound bossshowMP3Sound = new Sound();
        bossshowMP3Sound.name = ConfigManager.Instance.Tables.TableSoundConfig.Get(1).BossshowMP3;
        bossshowMP3Sound.clip = Resources.Load<AudioClip>("Music/" + ConfigManager.Instance.Tables.TableSoundConfig.Get(1).BossshowMP3);

        // 动态加载音效 'failMP3'
        Sound failMP3Sound = new Sound();
        failMP3Sound.name = ConfigManager.Instance.Tables.TableSoundConfig.Get(1).FailMP3;
        failMP3Sound.clip = Resources.Load<AudioClip>("Music/" + ConfigManager.Instance.Tables.TableSoundConfig.Get(1).FailMP3);


        // 动态加载音效 'winMP3'
        Sound winMP3Sound = new Sound();
        winMP3Sound.name = ConfigManager.Instance.Tables.TableSoundConfig.Get(1).WinMP3;
        winMP3Sound.clip = Resources.Load<AudioClip>("Music/" + ConfigManager.Instance.Tables.TableSoundConfig.Get(1).WinMP3);

        // 动态加载音效 'akMP3'
        Sound akMP3Sound = new Sound();
        akMP3Sound.name = ConfigManager.Instance.Tables.TableSoundConfig.Get(1).AkMP3;
        akMP3Sound.clip = Resources.Load<AudioClip>("Music/" + ConfigManager.Instance.Tables.TableSoundConfig.Get(1).AkMP3);

        // 动态加载音效 'buffMP3'
        Sound buffMP3Sound = new Sound();
        buffMP3Sound.name = ConfigManager.Instance.Tables.TableSoundConfig.Get(1).BuffMP3;
        buffMP3Sound.clip = Resources.Load<AudioClip>("Music/" + ConfigManager.Instance.Tables.TableSoundConfig.Get(1).BuffMP3);

        // 动态加载音效 'debuffMP3'
        Sound debuffMP3Sound = new Sound();
        debuffMP3Sound.name = ConfigManager.Instance.Tables.TableSoundConfig.Get(1).DebuffMP3;
        debuffMP3Sound.clip = Resources.Load<AudioClip>("Music/" + ConfigManager.Instance.Tables.TableSoundConfig.Get(1).DebuffMP3);

        // 动态加载音效 'monstershowMP3'
        Sound monstershowMP3Sound = new Sound();
        monstershowMP3Sound.name = ConfigManager.Instance.Tables.TableSoundConfig.Get(1).MonstershowMP3;
        monstershowMP3Sound.clip = Resources.Load<AudioClip>("Music/" + ConfigManager.Instance.Tables.TableSoundConfig.Get(1).MonstershowMP3);

        // 动态加载音效 'monstersdieMP3'
        Sound monstersdieMP3Sound = new Sound();
        monstersdieMP3Sound.name = ConfigManager.Instance.Tables.TableSoundConfig.Get(1).MonstersdieMP3;
        monstersdieMP3Sound.clip = Resources.Load<AudioClip>("Music/" + ConfigManager.Instance.Tables.TableSoundConfig.Get(1).MonstersdieMP3);

        // 动态加载音效 'boxopenMP3'
        Sound boxopenMP3Sound = new Sound();
        boxopenMP3Sound.name = ConfigManager.Instance.Tables.TableSoundConfig.Get(1).BoxopenMP3;
        boxopenMP3Sound.clip = Resources.Load<AudioClip>("Music/" + ConfigManager.Instance.Tables.TableSoundConfig.Get(1).BoxopenMP3);

        // 动态加载音效 'buttonMP3'
        Sound buttonMP3Sound = new Sound();
        buttonMP3Sound.name = ConfigManager.Instance.Tables.TableSoundConfig.Get(1).ButtonMP3;
        buttonMP3Sound.clip = Resources.Load<AudioClip>("Music/" + ConfigManager.Instance.Tables.TableSoundConfig.Get(1).ButtonMP3);

        // 动态加载音效 'bulletjammingMP3'
        Sound bulletjammingMP3Sound = new Sound();
        bulletjammingMP3Sound.name = ConfigManager.Instance.Tables.TableSoundConfig.Get(1).BulletjammingMP3;
        bulletjammingMP3Sound.clip = Resources.Load<AudioClip>("Music/" + ConfigManager.Instance.Tables.TableSoundConfig.Get(1).BulletjammingMP3);

        // 将音效添加到 sfxSounds 数组
        sfxSounds = new Sound[] { coinMusicSound, gunMusicSound , bossshowMP3Sound , failMP3Sound ,
            winMP3Sound , akMP3Sound , buffMP3Sound , debuffMP3Sound ,monstershowMP3Sound,monstersdieMP3Sound
            ,boxopenMP3Sound,buttonMP3Sound,};
    }

    private AudioItlem Create(GameObject prefab, Transform parent)
    {
        GameObject obj = Instantiate(prefab, parent);
        return obj.GetComponent<AudioItlem>();
    }

    private void Get(AudioItlem itlem)
    {
        // 将获取的音效项添加到活动列表
        activeAudioItems.Add(itlem);
        // 启用游戏对象
        itlem.gameObject.SetActive(true);
    }

    private void Release(AudioItlem itlem)
    {
        // 从活动列表中移除音效项
        activeAudioItems.Remove(itlem);
        // 禁用游戏对象
        itlem.gameObject.SetActive(false);
    }

    private void MyDestroy(AudioItlem itlem)
    {
        // 从活动列表中移除（以防万一）
        activeAudioItems.Remove(itlem);
        Destroy(itlem.gameObject);
    }

    // 播放音乐的方法
    public void PlayMusic(string name, bool isPlay)
    {
        // 从音乐Sounds数组中找到名字匹配的Sound对象
        Sound s = Array.Find(musicSounds, x => x.name == name);
        if (s == null)
        {
            Debug.LogWarning("未找到音乐: " + name);
            return;
        }
        // 设置音乐源的clip为找到的音频剪辑
        musicSource.clip = s.clip;

        if (isPlay)
        {
            // 如果isPlay为真，则播放音乐并设置为循环播放
            musicSource.loop = true;
            musicSource.enabled = true;
            musicSource.Play();
        }
        else
        {
            // 如果isPlay为假，则暂停音乐
            musicSource.Pause();
            musicSource.enabled = false;
        }
    }

    // 播放音效的方法
    public void PlaySFX(string name, GameObject obj)
    {
        Sound s = Array.Find(sfxSounds, x => x.name == name);
        if (s == null)
        {
            Debug.LogWarning("未找到音效: " + name);
            return;
        }

        // 获取一个AudioItlem实例并播放音效
        AudioItlem audioItlem = audioPool.Get();
        audioItlem.type = AudioItlem.AudioType.SFX; // 设置类型为音效
        audioItlem.GetComponent<AudioSource>().clip = s.clip;
        audioItlem.Init(obj, sfxVolume);
    }

    // 获取音效音量
    public float GetSFXVolume()
    {
        return sfxVolume;
    }

    // 设置音效音量
    public void SetSFXVolume(float value)
    {
        sfxVolume = Mathf.Clamp01(value);
        // 遍历所有活动的音效项，更新其音量
        foreach (var itlem in activeAudioItems)
        {
            if (itlem.audio != null && itlem.type == AudioItlem.AudioType.SFX)
            {
                itlem.audio.volume = sfxVolume;
            }
        }
    }
}
