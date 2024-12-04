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

        // ������������ӵ� musicSounds ����
        musicSounds = new Sound[] { bacMusicSound };

        // ��̬������Ч 'CoinMusic'
        Sound coinMusicSound = new Sound();
        coinMusicSound.name = ConfigManager.Instance.Tables.TableSoundConfig.Get(1).CoinMp3;
        coinMusicSound.clip = Resources.Load<AudioClip>("Music/" + ConfigManager.Instance.Tables.TableSoundConfig.Get(1).CoinMp3);

        // ��̬������Ч 'GunMusic'
        Sound gunMusicSound = new Sound();
        gunMusicSound.name = ConfigManager.Instance.Tables.TableSoundConfig.Get(1).ZhandouMP3;
        gunMusicSound.clip = Resources.Load<AudioClip>("Music/" + ConfigManager.Instance.Tables.TableSoundConfig.Get(1).ZhandouMP3);

        // ����Ч��ӵ� sfxSounds ����
        sfxSounds = new Sound[] { coinMusicSound, gunMusicSound };
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
