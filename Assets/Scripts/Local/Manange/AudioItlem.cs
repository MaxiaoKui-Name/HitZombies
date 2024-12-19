// AudioItlem.cs 管理音频项
using UnityEngine;

public class AudioItlem : MonoBehaviour
{
    // 音效类型
    public AudioType type;
    [HideInInspector]
    public AudioSource audio;

    public float lift = 3f;
    float waitTime_lift = 0f;

    public enum AudioType
    {
        SFX, // 音效
        Bg   // 背景音乐
    }

    void OnEnable()
    {
        // 获取音效播放组件
        audio = GetComponent<AudioSource>();
    }

    /// <summary>
    /// 初始化音效项
    /// </summary>
    /// <param name="obj">关联的游戏对象</param>
    /// <param name="volume">音量大小</param>
    public void Init(GameObject obj, float volume)
    {
        // 如果是背景音乐
        if (type == AudioType.Bg)
        {
            // 设置音量
            audio.volume = volume;
            audio.Play();
            lift = audio.clip.length;
        }
        // 如果是音效
        else
        {
            lift = audio.clip.length;
            audio.volume = volume;
            audio.PlayOneShot(audio.clip);
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

        if (obj1 != null && !obj1.activeSelf && gameObject.activeSelf)
            AudioManage.Instance.audioPool.Release(this);
    }
}
