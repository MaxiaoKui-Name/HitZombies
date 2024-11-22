using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;


[System.Serializable]
public class Sound
{
    public string name;         // ��Ƶ����������
    public AudioClip clip;      // ��Ƶ����
    //[Range(0f, 1f)]
    //public float volume = 1f; // ������С
}
public class AudioManage : Singleton<AudioManage>
{
    //��Ч����
    public Sound[] musicSounds, sfxSounds;
    //���ֺ���Ч��AudioSource
    public AudioSource musicSource, sfxSource;
    public AudioItlem audioPrefab;
    public ObjectPool<AudioItlem> audioPool;

    public void Init()
    {
        audioPool = new ObjectPool<AudioItlem>(() => Create(audioPrefab.gameObject, transform), Get, Release, MyDestroy, true, 500, 2000);
        List<AudioItlem> tempObjList = new List<AudioItlem>();
        //Ԥ����Ч��
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
    private AudioItlem Create(GameObject b, Transform bulletParent)
    {
        GameObject obj = Instantiate<GameObject>(b, bulletParent);
        return obj.GetComponent<AudioItlem>();
    }

    private void Get(AudioItlem go)
    {
        //go.SetActive(true);
    }

    private void Release(AudioItlem go)
    {
        go.gameObject.SetActive(false);
    }

    private void MyDestroy(AudioItlem go)
    {
        Destroy(go.gameObject);
    }

    //�������ֵķ���
    public void PlayMusic(string name,bool isCancel)
    {
        //������Sounds�������ҵ�����ƥ���Sound����
        Sound s = Array.Find(musicSounds, x => x.name == name);
        if (s == null)
        {
            return;
        }
        // ��������Դ��clipΪ�ҵ�����Ƶ����
        musicSource.clip = s.clip;

        if (isCancel)
        {
            // ���isCancelΪ�棬�򲥷����ֲ�����Ϊѭ������
            musicSource.loop = true;
            musicSource.enabled = true;
            musicSource.Play();
        }
        else
        {
            // ���isCancelΪ�٣�����ͣ����
            musicSource.Pause();
            musicSource.enabled = false;
        }
    }

    //������Ч�ķ���
    public void PlaySFX(string name,GameObject obj)
    {
        Sound s = Array.Find(sfxSounds, x => x.name == name); 
        if (s == null)
        {
           return;
        }
        //���򲥷Ŷ�ӦSound��clip
        else
        {
            AudioItlem audioSource = audioPool.Get();
            audioSource.GetComponent<AudioSource>().clip = s.clip;
            audioSource.gameObject.SetActive(true);
            audioSource.Init(obj);
        }
    }
}

