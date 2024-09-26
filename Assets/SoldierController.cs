using System.Collections;
using UnityEngine;

public class SoldierController : MonoBehaviour
{
    private GameObject player;
    private Vector3 initialOffset; // �洢��ʼƫ����
    private float lifetime;

    private void Start()
    {
        lifetime = 0;
        if (player != null)
        {
            // �����ʼ����ҵ�ƫ����
            initialOffset = transform.position - player.transform.position;
        }
    }

    private void Update()
    {
        if (player != null)
        {
            FollowPlayer();
        }
    }

    public void SetPlayer(GameObject player)
    {
        this.player = player;
        // �����ʼƫ����
        initialOffset = transform.position - player.transform.position;
    }
    public void SetLifetime(float time)
    {
        if (lifetime > 0)
        {
            lifetime += time; // ���Ӵ��ʱ��
        }
        else
        {
            lifetime = time; // ��һ������
            StartCoroutine(DestroyAfterLifetime(lifetime)); // ��ʼЭ��
        }
    }

    private IEnumerator DestroyAfterLifetime(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject); // ���ٶ���
    }

    private void FollowPlayer()
    {
        // ������ҵ�λ�úͳ�ʼƫ�����ƶ�ʿ��
        transform.position = player.transform.position + initialOffset;
    }
}
