using DragonBones;
using FluffyUnderware.DevTools.Extensions;
using Hitzb;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using Transform = UnityEngine.Transform;

public class SoldierController : MonoBehaviour
{
    [Header("�������")]
    public Transform FirePoint;          // ʿ���ķ���㣬��Ҫ��Inspector������

    private GameObject player;
    private Vector3 initialOffset;        // �洢����ҵĳ�ʼƫ����
    public float lifetime;
    private bool isShooting = false;      // ���ʿ���Ƿ��������

    private Coroutine shootCoroutine;     // ���Э������
    private UnityArmatureComponent armatureComponent;   // DragonBones Armature ���

    private List<BulletController> flyingBullets = new List<BulletController>(); // ����ʿ��������ӵ�

    private float detectionRange = 10f; // �����ⷶΧ

    private void Start()
    {
        FirePoint = transform.Find("FirePoint").transform;
        armatureComponent = transform.GetChild(0).GetComponent<UnityArmatureComponent>();
        lifetime = 0;

        if (player != null)
        {
            // �����ʼ����ҵ�ƫ����
            initialOffset = transform.position - player.transform.position;
        }

        armatureComponent.animation.Play("walk", -1);

        // �Զ���ʼ���
        StartShooting();
    }

    private void Update()
    {
        if (player != null)
        {
            FollowPlayer();
        }
        if (GameManage.Instance.gameState != GameState.Running)
        {
            Destroy(gameObject);
            return; // ��Ϸδ����ʱ��ִ���κ��߼�
        }
        if (GameManage.Instance.DestroySolider)
        {
            Destroy(gameObject);
            return; // ��Ϸδ����ʱ��ִ���κ��߼�
        }
    }

    /// <summary>
    /// ����ʿ����Ŀ�����
    /// </summary>
    /// <param name="player">Ŀ����Ҷ���</param>
    public void SetPlayer(GameObject player)
    {
        this.player = player;
        // �����ʼƫ����
        initialOffset = transform.position - player.transform.position;
    }

    /// <summary>
    /// ����ʿ���Ĵ��ʱ��
    /// </summary>
    /// <param name="time">���ʱ�䣨�룩</param>
    private Coroutine destroyCoroutine;   // ����Э������

    public void SetLifetime(float time)
    {
        lifetime += time; // ���Ӵ��ʱ��
        if (destroyCoroutine != null)
        {
            StopCoroutine(destroyCoroutine); // ֹ֮ͣǰ������Э��
        }
        destroyCoroutine = StartCoroutine(DestroyAfterLifetime(lifetime)); // ������������Э��
    }

    /// <summary>
    /// ��ָ��ʱ�������ʿ��
    /// </summary>
    /// <param name="time">�ȴ�ʱ�䣨�룩</param>
    private IEnumerator DestroyAfterLifetime(float time)
    {
        yield return new WaitForSeconds(time);
        StopShooting();
        // ��������е��ӵ��б�
        foreach (var bullet in flyingBullets)
        {
            if (bullet != null)
            {
                bullet.OnBulletDestroyed -= HandleBulletDestroyed;
                // ������Ը�����Ҫ�����ӵ�����������
            }
        }
        flyingBullets.Clear();
        Destroy(gameObject); // ���ٶ���
    }

    /// <summary>
    /// ������ҵ�λ��
    /// </summary>
    private void FollowPlayer()
    {
        // ������ҵ�λ�úͳ�ʼƫ�����ƶ�ʿ��
        transform.position = player.transform.position + initialOffset;
    }

    /// <summary>
    /// ��ʼ���
    /// </summary>
    public void StartShooting()
    {
        if (!isShooting)
        {
            isShooting = true;
            // ��ʼ���ʱ���л�����Ϊ��walk+hit��
            if (armatureComponent != null && armatureComponent.animation.lastAnimationName != "walk+hit")
            {
                armatureComponent.animation.Play("walk+hit", -1);
            }
            shootCoroutine = StartCoroutine(ShootCoroutine());
        }
    }

    /// <summary>
    /// ֹͣ���
    /// </summary>
    public void StopShooting()
    {
        if (isShooting)
        {
            isShooting = false;
            if (shootCoroutine != null)
            {
                StopCoroutine(shootCoroutine);
            }
            // ֹͣ���ʱ���л�����Ϊ��walk��
            if (armatureComponent != null && armatureComponent.animation.lastAnimationName != "walk")
            {
                armatureComponent.animation.Play("walk", -1);
            }
        }
    }

    /// <summary>
    /// ���Э��
    /// </summary>
    /// <returns></returns>
    private IEnumerator ShootCoroutine()
    {
        float timer = 0f;
        while (isShooting)
        {
            if (PreController.Instance.activeEnemyCount > 0)
            {
                // ֻ���ڼ�⵽ǰ���е��˲��ҷ����е��ӵ��������������ʱ�����
                if (ShouldShoot())
                {
                    // �����ǰ���ǡ�walk+hit���������л�����walk+hit��
                    if (armatureComponent != null && armatureComponent.animation.lastAnimationName != "walk+hit")
                    {
                        armatureComponent.animation.Play("walk+hit", -1);
                    }
                    ShootBullet();
                }
                else
                {
                    // �����ǰ���ǡ�walk���������л�����walk��
                    if (armatureComponent != null && armatureComponent.animation.lastAnimationName != "walk")
                    {
                        armatureComponent.animation.Play("walk", -1);
                    }
                }
            }
            else
            {
                // �����ǰ���ǡ�walk���������л�����walk��
                if (armatureComponent != null && armatureComponent.animation.lastAnimationName != "walk")
                {
                    armatureComponent.animation.Play("walk", -1);
                }
            }

            // ʹ���Զ���ļ�ʱ�����Ա��� GenerationIntervalBullet �ı�ʱ������Ч
            float interval = PreController.Instance.GenerationIntervalBullet;
            while (timer < interval)
            {
                yield return null;
                timer += Time.deltaTime;

                // ��� GenerationIntervalBullet �����仯�����¿�ʼ��ʱ
                if (interval != PreController.Instance.GenerationIntervalBullet)
                {
                    interval = PreController.Instance.GenerationIntervalBullet;
                    timer = 0f;
                }
            }
            timer = 0f;
        }
    }

    /// <summary>
    /// �ж��Ƿ�Ӧ�����
    /// </summary>
    /// <returns></returns>
    private bool ShouldShoot()
    {
        float HoridetectionRange = 0.5f;
        float VertialdetectionRange = 7f;
        if (IsEnemyInFront(HoridetectionRange, VertialdetectionRange))
        {
            float totalBulletDamage = GetTotalFlyingBulletDamage(HoridetectionRange, VertialdetectionRange);
            float totalEnemyHealth = GetTotalEnemyHealthInRange(HoridetectionRange, VertialdetectionRange);

            if (totalBulletDamage < totalEnemyHealth)
            {
                return true; // ��Ҫ���
            }
        }
        return false; // ����Ҫ���
    }

    /// <summary>
    /// �����ӵ�
    /// </summary>
    private void ShootBullet()
    {
        Gun currentGun = PlayInforManager.Instance.playInfor.currentGun;
        if (currentGun != null)
        {
            string bulletKey = currentGun.bulletType;

            // ���ӵ����л�ȡ�ӵ�
            if (PreController.Instance.bulletPools.TryGetValue(bulletKey, out var selectedBulletPool))
            {
                GameObject bullet = selectedBulletPool.Get();
                if (bullet != null)
                {
                    bullet.SetActive(true);
                    // ���ӵ���������б�
                    BulletController bulletController = bullet.GetComponent<BulletController>();
                    bulletController.isSoliderBullet = true;
                    PreController.Instance.FixSortLayer(bullet);
                    bullet.transform.position = FirePoint.position;
                    if (bulletController != null)
                    {
                        flyingBullets.Add(bulletController);
                        bulletController.OnBulletDestroyed += HandleBulletDestroyed; // ע���ӵ������¼�
                    }
                }
            }
            else
            {
                Debug.LogWarning($"δ�ҵ��ӵ���: {bulletKey}");
            }
        }
    }

    // ���ʿ����ǰ��һ����Χ���Ƿ��е���
    private bool IsEnemyInFront(float HoridetectionRange, float VertialdetectionRange)
    {
        Vector3 soldierPosition = FirePoint.position;

        // ��������������½Ǻ����Ͻ�
        Vector2 pointA = new Vector2(soldierPosition.x - HoridetectionRange, soldierPosition.y);
        Vector2 pointB = new Vector2(soldierPosition.x + HoridetectionRange, soldierPosition.y + VertialdetectionRange);
        // ����������ڵ�Layer
        int enemyLayerMask = LayerMask.GetMask("Enemy");
        int chestLayerMask = LayerMask.GetMask("Chest");
        // ��ȡ��������ڵ�������ײ��
        Collider2D[] colliders = Physics2D.OverlapAreaAll(pointA, pointB, enemyLayerMask | chestLayerMask);

        foreach (var collider in colliders)
        {
            if (collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                EnemyController enemy = collider.GetComponent<EnemyController>();
                if (enemy != null && enemy.gameObject.activeSelf)
                {
                    return true; // ��Χ���е���
                }
            }
            else if (collider.gameObject.layer == LayerMask.NameToLayer("Chest"))
            {
                ChestController chest = collider.GetComponent<ChestController>();
                if (chest != null && chest.gameObject.activeSelf)
                {
                    return true; // ��Χ���б���
                }
            }
        }
        return false; // ��Χ��û�е���
    }

    // ����������ӵ������˺�
    private float GetTotalFlyingBulletDamage(float HoridetectionRange, float VertialdetectionRange)
    {
        float totalDamage = 0f;
        // ��ȡʿ��λ��
        Vector3 soldierPosition = FirePoint.position;

        // ��������������½Ǻ����Ͻ�
        Vector2 pointA = new Vector2(soldierPosition.x - HoridetectionRange, soldierPosition.y);
        Vector2 pointB = new Vector2(soldierPosition.x + HoridetectionRange, soldierPosition.y + VertialdetectionRange);
        // �����ӵ����ڵ�Layer
        int bulletLayerMask = LayerMask.GetMask("Bullet");
        // ��ȡ��������ڵ�������ײ��
        Collider2D[] colliders = Physics2D.OverlapAreaAll(pointA, pointB, bulletLayerMask);
        foreach (var collider in colliders)
        {
            BulletController bullet = collider.GetComponent<BulletController>();
            if (bullet != null && bullet.gameObject.activeSelf)
            {
                totalDamage += bullet.firepower;
            }
        }
        return totalDamage;
    }

    // ������ǰ�����˵�������ֵ
    private float GetTotalEnemyHealthInRange(float HoridetectionRange, float VertialdetectionRange)
    {
        float totalHealth = 0f;
        Vector3 soldierPosition = FirePoint.position;
        Vector2 pointA = new Vector2(soldierPosition.x - HoridetectionRange, soldierPosition.y);
        Vector2 pointB = new Vector2(soldierPosition.x + HoridetectionRange, soldierPosition.y + VertialdetectionRange);
        // ����������ڵ�Layer
        int enemyLayerMask = LayerMask.GetMask("Enemy");
        int chestLayerMask = LayerMask.GetMask("Chest");
        // ��ȡ��������ڵ�������ײ��
        Collider2D[] colliders = Physics2D.OverlapAreaAll(pointA, pointB, enemyLayerMask | chestLayerMask);

        foreach (var collider in colliders)
        {
            if (collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                EnemyController enemy = collider.GetComponent<EnemyController>();
                if (enemy != null && enemy.gameObject.activeSelf)
                {
                    totalHealth += enemy.health;
                }
            }
            else if (collider.gameObject.layer == LayerMask.NameToLayer("Chest"))
            {
                ChestController chest = collider.GetComponent<ChestController>();
                if (chest != null && chest.gameObject.activeSelf)
                {
                    totalHealth += chest.chestHealth;
                }
            }
        }
        return totalHealth;
    }

    private void HandleBulletDestroyed(BulletController bullet)
    {
        flyingBullets.Remove(bullet);
    }

    private void OnDrawGizmos()
    {
        // ȷ��������ʱ�Ż��� Gizmos�����߸�����Ҫȥ�����ж�
        if (Application.isPlaying)
        {
            DrawDetectionArea();
        }
    }

    private void DrawDetectionArea()
    {
        // ��ȡʿ��λ��
        Vector3 soldierPosition = FirePoint.position;

        // ��������������½Ǻ����Ͻ�
        Vector2 pointA = new Vector2(soldierPosition.x - 0.5f, soldierPosition.y);
        Vector2 pointB = new Vector2(soldierPosition.x + 0.5f, soldierPosition.y + 8.06f);

        // ���� Gizmos ����ɫ
        Gizmos.color = Color.red;

        // ���ƾ��ε��ĸ���
        Vector3 bottomLeft = new Vector3(pointA.x, pointA.y, 0);
        Vector3 bottomRight = new Vector3(pointB.x, pointA.y, 0);
        Vector3 topRight = new Vector3(pointB.x, pointB.y, 0);
        Vector3 topLeft = new Vector3(pointA.x, pointB.y, 0);

        Gizmos.DrawLine(bottomLeft, bottomRight); // �±�
        Gizmos.DrawLine(bottomRight, topRight);   // �ұ�
        Gizmos.DrawLine(topRight, topLeft);       // �ϱ�
        Gizmos.DrawLine(topLeft, bottomLeft);     // ���
    }
}
