using Cysharp.Threading.Tasks;
using DragonBones;
using Hitzb;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Threading; // ��������ռ�
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Transform = UnityEngine.Transform;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PlaneController : MonoBehaviour
{
    private CancellationTokenSource cancellationTokenSource;
    private Vector2 detectionAreaSize = new Vector2(10f, 6f); // ��������С������
    public float planeSpeed = 2f;
    void Start()
    {
        // ��ʼ�� CancellationTokenSource
        cancellationTokenSource = new CancellationTokenSource();
        // �����첽���񣬴��� CancellationToken
        MovePlaneAndDropBombs(gameObject, cancellationTokenSource.Token).Forget();
    }

    void Update()
    {
        if (GameManage.Instance.gameState != GameState.Running)
        {
            // ȡ���첽����
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
                cancellationTokenSource.Dispose();
                cancellationTokenSource = null;
            }
            // ������Ϸ����
            Destroy(gameObject);
        }
        else
        {
            if (transform.position.y < 7f)
            {
                transform.Translate(Vector3.up * planeSpeed * Time.deltaTime, Space.World);
            }
            else 
            {
                // ������Ϸ����
                Destroy(gameObject);
            }
        }
    }

    void OnDestroy()
    {
        if (cancellationTokenSource != null)
        {
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
            cancellationTokenSource = null;
        }
    }

    private async UniTask MovePlaneAndDropBombs(GameObject plane, CancellationToken cancellationToken)
    {
        // ����ը��Ͷ�ŵ� y ��λ���б�
        List<float> dropPositionsY = new List<float> { -3f, -1.5f, 0f, 1.5f, 3f };
        int nextDropIndex = 0;
   

        while (plane != null && plane.activeSelf && plane.transform.position.y < 5f)
        {
            // ���ȡ������
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }
            // ����Ƿ�����һ��Ͷ��λ��
            if (nextDropIndex < dropPositionsY.Count && plane.transform.position.y >= dropPositionsY[nextDropIndex])
            {
                // �ڵ�ǰλ��Ͷ��ը����
                Vector3 bombPosition = plane.transform.position;
                DropBombGroup(bombPosition, cancellationToken);
                nextDropIndex++;
            }
            // ������������
            await UniTask.Yield();
        }
    }

    private async UniTask DropBombGroup(Vector3 bombPosition, CancellationToken cancellationToken)
    {
        // ���ȡ������
        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }
        // ���� BombGroup Ԥ����
        GameObject bombGroupPrefab = Resources.Load<GameObject>("Prefabs/BombGroup"); // ��ȷ��Ԥ����·����ȷ

        // ʵ����ը����
        GameObject bombGroup = Instantiate(bombGroupPrefab);
        bombGroup.transform.position = bombPosition;
        // ��ȡ��Ϊ "bomb" ���Ӷ����б�
        List<Transform> bombChildren = new List<Transform>();
        foreach (Transform child in bombGroup.transform)
        {
            if (child.name == "bomb")
            {
                bombChildren.Add(child);
            }
        }

        // ���� bomb �Ӷ����λ��
        if (bombChildren.Count >= 4)
        {
            // ��һ�� bomb ���� A �� BombGroup ��λ��
            bombChildren[0].localPosition = new Vector3(-1.76f,0f,0);

            // �������� bomb ������ A ������λ����һ����ƫ�ƣ����������
            System.Random rand = new System.Random();

            // ������С�����ƫ����
            float minOffsetX = 0.8f;
            float maxOffsetX = 1f;
            float minOffsetY = 0.2f;
            float maxOffsetY = 0.4f;

            for (int i = 1; i <= 3; i++)
            {
                // ����x��ƫ�ƣ���Χ��[minOffset, maxOffset]
                float offsetX = (float)(rand.NextDouble() * (maxOffsetX - minOffsetX) + minOffsetX);
                //// ���������������
                //offsetX = rand.Next(0, 2) == 0 ? offsetX : -offsetX;

                // ����y��ƫ�ƣ���Χ��[minOffset, maxOffset]
                float offsetY = (float)(rand.NextDouble() * (maxOffsetY - minOffsetY) + minOffsetY);
                // ���������������
                offsetY = rand.Next(0, 2) == 0 ? offsetY : -offsetY;

                // ����������Ϊ1��2��3������λ�ã������������Ϊ0������λ�ü���ƫ��
                bombChildren[i].localPosition = bombChildren[0].localPosition + new Vector3(offsetX, offsetY, 0f) * i;
            }
        }
        // �ȴ������������
        await UniTask.Delay(TimeSpan.FromSeconds(0.1), cancellationToken: cancellationToken);
        UnityArmatureComponent bombArmature = null;
        // ����ÿ�� bomb �Ӷ���� DragonBones ����
        foreach (Transform bombChild in bombChildren)
        {
            bombArmature = bombChild.GetComponent<UnityArmatureComponent>();
            if (bombArmature != null)
            {
                bombArmature.animation.Play("bomb", 1); // ����һ�� "bomb" ����
            }
        }
        // �ȴ������������
        await UniTask.Delay(TimeSpan.FromSeconds(bombArmature.animation.GetState("bomb")._duration), cancellationToken: cancellationToken);

        // ���ȡ������
        if (cancellationToken.IsCancellationRequested)
        {
            // ����ը�������
            Destroy(bombGroup);
            return;
        }

        // ʹ�����߼��ը����Χ�ڵĵ��˺ͱ���
        float DamageNum = ConfigManager.Instance.Tables.TableTransmitConfig.Get(ConfigManager.Instance.Tables.TableBoxcontent.Get(7).Fires[0]).AtkRate * ConfigManager.Instance.Tables.TablePlayerConfig.Get(GameFlowManager.Instance.currentLevelIndex).Total;
        int LayerEnemy = LayerMask.NameToLayer("Enemy");
        int LayerChest = LayerMask.NameToLayer("Chest");
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(bombGroup.transform.position, detectionAreaSize, LayerEnemy | LayerChest);

        foreach (Collider2D hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                EnemyController enemyController = hitCollider.GetComponent<EnemyController>();
                if (enemyController != null && !enemyController.isDead && enemyController.isVise)
                {
                    enemyController.TakeDamage((long)DamageNum, hitCollider.gameObject); // �Ե�����������˺�
                }
            }
            else if (hitCollider.CompareTag("Chest"))
            {
                ChestController chestController = hitCollider.GetComponent<ChestController>();
                if (chestController != null && chestController.isVise)
                {
                    chestController.TakeDamage((long)DamageNum, hitCollider.gameObject); // �Ա�����������˺�
                }
            }
        }

        // ����ը�������
        Destroy(bombGroup);
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, detectionAreaSize);
    }
}
