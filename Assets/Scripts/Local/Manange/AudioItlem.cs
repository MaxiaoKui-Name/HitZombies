// AudioItlem.cs ������Ƶ��
using UnityEngine;

public class AudioItlem : MonoBehaviour
{
    // ��Ч����
    public AudioType type;
    [HideInInspector]
    public AudioSource audio;

    public float lift = 3f;
    float waitTime_lift = 0f;

    public enum AudioType
    {
        SFX, // ��Ч
        Bg   // ��������
    }

    void OnEnable()
    {
        // ��ȡ��Ч�������
        audio = GetComponent<AudioSource>();
    }

    /// <summary>
    /// ��ʼ����Ч��
    /// </summary>
    /// <param name="obj">��������Ϸ����</param>
    /// <param name="volume">������С</param>
    public void Init(GameObject obj, float volume)
    {
        // ����Ǳ�������
        if (type == AudioType.Bg)
        {
            // ��������
            audio.volume = volume;
            audio.Play();
            lift = audio.clip.length;
        }
        // �������Ч
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
