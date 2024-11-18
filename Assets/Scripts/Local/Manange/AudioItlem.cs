using EasyFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioItlem : MonoBehaviour
{
    //音效类型
    public AudioType type;
    [HideInInspector]
    public AudioSource audio;

    public float lift = 3f;
    float waitTime_lift = 0f;

    public enum AudioType
    {
        SFX,//音效
        Bg
    }
     void OnEnable()
    {
        //获取音效播放组件
        audio = GetComponent<AudioSource>();
    }

    public void Init(GameObject obj)
    {
        //如果是背景乐
        if (type == AudioType.Bg)
        {
            //获取存储的音效值
            audio.volume = 1f;
        }
        //如果是音效
        else
        {
            lift = GetComponent<AudioSource>().clip.length;
            audio.volume = 1f;
           
            audio.PlayOneShot(GetComponent<AudioSource>().clip);
        }
        waitTime_lift = lift;
        obj1 = obj;
    }
    public GameObject obj1;
    private void Update()
    {
        waitTime_lift -= Time.deltaTime;
        if (waitTime_lift <= 0f)
            AudioManage.Instance.audioPool.Release(this);
        if (obj1 != null && !obj1.activeSelf && this.gameObject.activeSelf)
            AudioManage.Instance.audioPool.Release(this);
    }
}
