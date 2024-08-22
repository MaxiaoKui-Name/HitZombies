using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;   // ��������ƶ��ٶ�
    //public GameObject bulletPrefab; // �ӵ�Ԥ�Ƽ�
    public Transform firePoint;     // �ӵ������
    //public GameObject EnemyPrefab; // ����Ԥ����
    //public Transform EnemyParents;
    //public Transform BulletParents;
    public float leftBoundary = -1.5f;  // ��߽�����
    public float rightBoundary = 1.5f;  // �ұ߽�����
    private float horizontalInput;
    private float fireTimer = 0f;    // ���ڼ�ʱ�Ķ�ʱ��
    public float fireRate = 0.3f;    // �������ʣ�ÿ0.5�뷢��һ�Σ�
   
    void Update()
    {
        // ��ȡ��ҵ�����
        horizontalInput = Input.GetAxis("Horizontal");
        // �����µ�λ��
        Vector3 newPosition = transform.position + new Vector3(horizontalInput * moveSpeed * Time.deltaTime, 0);
        // �������λ���ڱ߽���
        newPosition.x = Mathf.Clamp(newPosition.x, leftBoundary, rightBoundary);
        // �������λ��
        transform.position = newPosition;

        // ���Ӽ�ʱ��
        fireTimer += Time.deltaTime;

        // ����ʱ������������ʱ�������ӵ�
        if (fireTimer >= fireRate)
        {
            Shoot();
            fireTimer = 0f;  // ���ü�ʱ��
        }
    }

    void Shoot()
    {
        GameObject Bullet = PreController.Instance.BulletPool.Get();
        Bullet.SetActive(true);
        Bullet.transform.position = firePoint.position;
    }
}
