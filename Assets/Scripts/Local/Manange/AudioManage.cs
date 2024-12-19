// AudioManage.cs ��Ч������
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

[System.Serializable]
public class Sound
{
    public string name;         // ��Ƶ����������
    public AudioClip clip;      // ��Ƶ����
    // [Range(0f, 1f)]
    // public float volume = 1f; // ������С
}

public class AudioManage : Singleton<AudioManage>
{
    // ��Ч����
    public Sound[] musicSounds, sfxSounds;
    // ���ֺ���Ч��AudioSource
    public AudioSource musicSource, sfxSource;
    public AudioItlem audioPrefab;
    public ObjectPool<AudioItlem> audioPool;

    private float sfxVolume = 1f; // ��Ч������Ĭ��1

    // ���ڸ��ٻ��Ч����б�
    private List<AudioItlem> activeAudioItems = new List<AudioItlem>();
    public void Init()
    {
        // ��ʼ����Ч��
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
        // Ԥ����Ч��
        int readyCount = 500;
        for (int j = 0; j < readyCount; j++)
            tempObjList.Add(audioPool.Get());
        for (int j = 0; j < tempObjList.Count; j++)
            audioPool.Release(tempObjList[j]);
        tempObjList.Clear();

        // ��̬���ر������� 'BacMusic'
        Sound bacMusicSound = new Sound();
        bacMusicSound.name = ConfigManager.Instance.Tables.TableSoundConfig.Get(1).BeijingMP3;
        bacMusicSound.clip = Resources.Load<AudioClip>("Music/" + ConfigManager.Instance.Tables.TableSoundConfig.Get(1).BeijingMP3);

        // ��̬������Ч 'zhandouMP3'
        Sound gunMusicSound = new Sound();
        gunMusicSound.name = ConfigManager.Instance.Tables.TableSoundConfig.Get(1).ZhandouMP3;
        gunMusicSound.clip = Resources.Load<AudioClip>("Music/" + ConfigManager.Instance.Tables.TableSoundConfig.Get(1).ZhandouMP3);

        // ������������ӵ� musicSounds ����
        musicSounds = new Sound[] { bacMusicSound , gunMusicSound };


        // ��̬������Ч 'CoinMusic'
        Sound coinMusicSound = new Sound();
        coinMusicSound.name = ConfigManager.Instance.Tables.TableSoundConfig.Get(1).CoinMp3;
        coinMusicSound.clip = Resources.Load<AudioClip>("Music/" + ConfigManager.Instance.Tables.TableSoundConfig.Get(1).CoinMp3);


        // ��̬������Ч 'bossshowMP3'
        Sound bossshowMP3Sound = new Sound();
        bossshowMP3Sound.name = ConfigManager.Instance.Tables.TableSoundConfig.Get(1).BossshowMP3;
        bossshowMP3Sound.clip = Resources.Load<AudioClip>("Music/" + ConfigManager.Instance.Tables.TableSoundConfig.Get(1).BossshowMP3);

        // ��̬������Ч 'failMP3'
        Sound failMP3Sound = new Sound();
        failMP3Sound.name = ConfigManager.Instance.Tables.TableSoundConfig.Get(1).FailMP3;
        failMP3Sound.clip = Resources.Load<AudioClip>("Music/" + ConfigManager.Instance.Tables.TableSoundConfig.Get(1).FailMP3);


        // ��̬������Ч 'winMP3'
        Sound winMP3Sound = new Sound();
        winMP3Sound.name = ConfigManager.Instance.Tables.TableSoundConfig.Get(1).WinMP3;
        winMP3Sound.clip = Resources.Load<AudioClip>("Music/" + ConfigManager.Instance.Tables.TableSoundConfig.Get(1).WinMP3);

        // ��̬������Ч 'akMP3'
        Sound akMP3Sound = new Sound();
        akMP3Sound.name = ConfigManager.Instance.Tables.TableSoundConfig.Get(1).AkMP3;
        akMP3Sound.clip = Resources.Load<AudioClip>("Music/" + ConfigManager.Instance.Tables.TableSoundConfig.Get(1).AkMP3);

        // ��̬������Ч 'buffMP3'
        Sound buffMP3Sound = new Sound();
        buffMP3Sound.name = ConfigManager.Instance.Tables.TableSoundConfig.Get(1).BuffMP3;
        buffMP3Sound.clip = Resources.Load<AudioClip>("Music/" + ConfigManager.Instance.Tables.TableSoundConfig.Get(1).BuffMP3);

        // ��̬������Ч 'debuffMP3'
        Sound debuffMP3Sound = new Sound();
        debuffMP3Sound.name = ConfigManager.Instance.Tables.TableSoundConfig.Get(1).DebuffMP3;
        debuffMP3Sound.clip = Resources.Load<AudioClip>("Music/" + ConfigManager.Instance.Tables.TableSoundConfig.Get(1).DebuffMP3);

        // ��̬������Ч 'monstershowMP3'
        Sound monstershowMP3Sound = new Sound();
        monstershowMP3Sound.name = ConfigManager.Instance.Tables.TableSoundConfig.Get(1).MonstershowMP3;
        monstershowMP3Sound.clip = Resources.Load<AudioClip>("Music/" + ConfigManager.Instance.Tables.TableSoundConfig.Get(1).MonstershowMP3);

        // ��̬������Ч 'monstersdieMP3'
        Sound monstersdieMP3Sound = new Sound();
        monstersdieMP3Sound.name = ConfigManager.Instance.Tables.TableSoundConfig.Get(1).MonstersdieMP3;
        monstersdieMP3Sound.clip = Resources.Load<AudioClip>("Music/" + ConfigManager.Instance.Tables.TableSoundConfig.Get(1).MonstersdieMP3);

        // ��̬������Ч 'boxopenMP3'
        Sound boxopenMP3Sound = new Sound();
        boxopenMP3Sound.name = ConfigManager.Instance.Tables.TableSoundConfig.Get(1).BoxopenMP3;
        boxopenMP3Sound.clip = Resources.Load<AudioClip>("Music/" + ConfigManager.Instance.Tables.TableSoundConfig.Get(1).BoxopenMP3);

        // ��̬������Ч 'buttonMP3'
        Sound buttonMP3Sound = new Sound();
        buttonMP3Sound.name = ConfigManager.Instance.Tables.TableSoundConfig.Get(1).ButtonMP3;
        buttonMP3Sound.clip = Resources.Load<AudioClip>("Music/" + ConfigManager.Instance.Tables.TableSoundConfig.Get(1).ButtonMP3);

        // ��̬������Ч 'bulletjammingMP3'
        Sound bulletjammingMP3Sound = new Sound();
        bulletjammingMP3Sound.name = ConfigManager.Instance.Tables.TableSoundConfig.Get(1).BulletjammingMP3;
        bulletjammingMP3Sound.clip = Resources.Load<AudioClip>("Music/" + ConfigManager.Instance.Tables.TableSoundConfig.Get(1).BulletjammingMP3);

        // ����Ч��ӵ� sfxSounds ����
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
        // ����ȡ����Ч����ӵ���б�
        activeAudioItems.Add(itlem);
        // ������Ϸ����
        itlem.gameObject.SetActive(true);
    }

    private void Release(AudioItlem itlem)
    {
        // �ӻ�б����Ƴ���Ч��
        activeAudioItems.Remove(itlem);
        // ������Ϸ����
        itlem.gameObject.SetActive(false);
    }

    private void MyDestroy(AudioItlem itlem)
    {
        // �ӻ�б����Ƴ����Է���һ��
        activeAudioItems.Remove(itlem);
        Destroy(itlem.gameObject);
    }

    // �������ֵķ���
    public void PlayMusic(string name, bool isPlay)
    {
        // ������Sounds�������ҵ�����ƥ���Sound����
        Sound s = Array.Find(musicSounds, x => x.name == name);
        if (s == null)
        {
            Debug.LogWarning("δ�ҵ�����: " + name);
            return;
        }
        // ��������Դ��clipΪ�ҵ�����Ƶ����
        musicSource.clip = s.clip;

        if (isPlay)
        {
            // ���isPlayΪ�棬�򲥷����ֲ�����Ϊѭ������
            musicSource.loop = true;
            musicSource.enabled = true;
            musicSource.Play();
        }
        else
        {
            // ���isPlayΪ�٣�����ͣ����
            musicSource.Pause();
            musicSource.enabled = false;
        }
    }

    // ������Ч�ķ���
    public void PlaySFX(string name, GameObject obj)
    {
        Sound s = Array.Find(sfxSounds, x => x.name == name);
        if (s == null)
        {
            Debug.LogWarning("δ�ҵ���Ч: " + name);
            return;
        }

        // ��ȡһ��AudioItlemʵ����������Ч
        AudioItlem audioItlem = audioPool.Get();
        audioItlem.type = AudioItlem.AudioType.SFX; // ��������Ϊ��Ч
        audioItlem.GetComponent<AudioSource>().clip = s.clip;
        audioItlem.Init(obj, sfxVolume);
    }

    // ��ȡ��Ч����
    public float GetSFXVolume()
    {
        return sfxVolume;
    }

    // ������Ч����
    public void SetSFXVolume(float value)
    {
        sfxVolume = Mathf.Clamp01(value);
        // �������л����Ч�����������
        foreach (var itlem in activeAudioItems)
        {
            if (itlem.audio != null && itlem.type == AudioItlem.AudioType.SFX)
            {
                itlem.audio.volume = sfxVolume;
            }
        }
    }
}
