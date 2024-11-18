using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;


[System.Serializable]
public class Sound
{
    public string name;         // 音频剪辑的名称
    public AudioClip clip;      // 音频剪辑
    //[Range(0f, 1f)]
    //public float volume = 1f; // 音量大小
}
public class AudioManage : Singleton<AudioManage>
{
    //音效数组
    public Sound[] musicSounds, sfxSounds;
    //音乐和音效的AudioSource
    public AudioSource musicSource, sfxSource;
    public AudioItlem audioPrefab;
    public ObjectPool<AudioItlem> audioPool;

    void Start()
    {
        audioPool = new ObjectPool<AudioItlem>(() => Create(audioPrefab.gameObject, transform), Get, Release, MyDestroy, true, 500, 2000);

        List<AudioItlem> tempObjList = new List<AudioItlem>();

        //预制音效池
        int readyCount = 500;
        for (int j = 0; j < readyCount; j++)
            tempObjList.Add(audioPool.Get());
        for (int j = 0; j < tempObjList.Count; j++)
            audioPool.Release(tempObjList[j]);
        tempObjList.Clear();
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

    //播放音乐的方法
    public void PlayMusic(string name,bool isCancel)
    {
        //从音乐Sounds数组中找到名字匹配的Sound对象
        Sound s = Array.Find(musicSounds, x => x.name == name);
        if (s == null)
        {
            return;
        }
        // 设置音乐源的clip为找到的音频剪辑
        musicSource.clip = s.clip;

        if (isCancel)
        {
            // 如果isCancel为真，则播放音乐并设置为循环播放
            musicSource.loop = true;
            musicSource.enabled = true;
            musicSource.Play();
        }
        else
        {
            // 如果isCancel为假，则暂停音乐
            musicSource.Pause();
            musicSource.enabled = false;
        }
    }

    //播放音效的方法
    public void PlaySFX(string name,GameObject obj)
    {
        Sound s = Array.Find(sfxSounds, x => x.name == name); 
        if (s == null)
        {
           return;
        }
        //否则播放对应Sound的clip
        else
        {
            AudioItlem audioSource = audioPool.Get();
            audioSource.GetComponent<AudioSource>().clip = s.clip;
            audioSource.gameObject.SetActive(true);
            audioSource.Init(obj);
        }
    }
}

