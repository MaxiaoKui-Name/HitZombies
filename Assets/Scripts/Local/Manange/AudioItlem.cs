using EasyFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioItlem : MonoBehaviour
{
    //��Ч����
    public AudioType type;
    [HideInInspector]
    public AudioSource audio;

    public float lift = 3f;
    float waitTime_lift = 0f;

    public enum AudioType
    {
        SFX,//��Ч
        Bg
    }
     void OnEnable()
    {
        //��ȡ��Ч�������
        audio = GetComponent<AudioSource>();
    }

    public void Init(GameObject obj)
    {
        //����Ǳ�����
        if (type == AudioType.Bg)
        {
            //��ȡ�洢����Чֵ
            audio.volume = 1f;
        }
        //�������Ч
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
