using Cysharp.Threading.Tasks;
using DragonBones;
using Hitzb;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Threading; // 添加命名空间
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Transform = UnityEngine.Transform;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PlaneController : MonoBehaviour
{
    private CancellationTokenSource cancellationTokenSource;
    private Vector2 detectionAreaSize = new Vector2(10f, 6f); // 检测区域大小（长宽）
    public float planeSpeed = 2f;
    void Start()
    {
        // 初始化 CancellationTokenSource
        cancellationTokenSource = new CancellationTokenSource();
        // 启动异步任务，传递 CancellationToken
        MovePlaneAndDropBombs(gameObject, cancellationTokenSource.Token).Forget();
    }

    void Update()
    {
        if (GameManage.Instance.gameState != GameState.Running)
        {
            // 取消异步任务
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
                cancellationTokenSource.Dispose();
                cancellationTokenSource = null;
            }
            // 销毁游戏对象
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
                // 销毁游戏对象
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
        // 定义炸弹投放的 y 轴位置列表
        List<float> dropPositionsY = new List<float> { -3f, -1.5f, 0f, 1.5f, 3f };
        int nextDropIndex = 0;
   

        while (plane != null && plane.activeSelf && plane.transform.position.y < 5f)
        {
            // 检查取消请求
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }
            // 检查是否到了下一个投放位置
            if (nextDropIndex < dropPositionsY.Count && plane.transform.position.y >= dropPositionsY[nextDropIndex])
            {
                // 在当前位置投放炸弹组
                Vector3 bombPosition = plane.transform.position;
                DropBombGroup(bombPosition, cancellationToken);
                nextDropIndex++;
            }
            // 允许其他操作
            await UniTask.Yield();
        }
    }

    private async UniTask DropBombGroup(Vector3 bombPosition, CancellationToken cancellationToken)
    {
        // 检查取消请求
        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }
        // 加载 BombGroup 预制体
        GameObject bombGroupPrefab = Resources.Load<GameObject>("Prefabs/BombGroup"); // 请确保预制体路径正确

        // 实例化炸弹组
        GameObject bombGroup = Instantiate(bombGroupPrefab);
        bombGroup.transform.position = bombPosition;
        // 获取名为 "bomb" 的子对象列表
        List<Transform> bombChildren = new List<Transform>();
        foreach (Transform child in bombGroup.transform)
        {
            if (child.name == "bomb")
            {
                bombChildren.Add(child);
            }
        }

        // 设置 bomb 子对象的位置
        if (bombChildren.Count >= 4)
        {
            // 第一个 bomb 对象 A 在 BombGroup 的位置
            bombChildren[0].localPosition = new Vector3(-1.76f,0f,0);

            // 其他三个 bomb 对象在 A 附近，位置有一定的偏移，产生错落感
            System.Random rand = new System.Random();

            // 定义最小和最大偏移量
            float minOffsetX = 0.8f;
            float maxOffsetX = 1f;
            float minOffsetY = 0.2f;
            float maxOffsetY = 0.4f;

            for (int i = 1; i <= 3; i++)
            {
                // 生成x轴偏移，范围在[minOffset, maxOffset]
                float offsetX = (float)(rand.NextDouble() * (maxOffsetX - minOffsetX) + minOffsetX);
                //// 随机决定正负方向
                //offsetX = rand.Next(0, 2) == 0 ? offsetX : -offsetX;

                // 生成y轴偏移，范围在[minOffset, maxOffset]
                float offsetY = (float)(rand.NextDouble() * (maxOffsetY - minOffsetY) + minOffsetY);
                // 随机决定正负方向
                offsetY = rand.Next(0, 2) == 0 ? offsetY : -offsetY;

                // 设置索引号为1、2、3的物体位置，相对于索引号为0的物体位置加上偏移
                bombChildren[i].localPosition = bombChildren[0].localPosition + new Vector3(offsetX, offsetY, 0f) * i;
            }
        }
        // 等待动画播放完成
        await UniTask.Delay(TimeSpan.FromSeconds(0.1), cancellationToken: cancellationToken);
        UnityArmatureComponent bombArmature = null;
        // 播放每个 bomb 子对象的 DragonBones 动画
        foreach (Transform bombChild in bombChildren)
        {
            bombArmature = bombChild.GetComponent<UnityArmatureComponent>();
            if (bombArmature != null)
            {
                bombArmature.animation.Play("bomb", 1); // 播放一次 "bomb" 动画
            }
        }
        // 等待动画播放完成
        await UniTask.Delay(TimeSpan.FromSeconds(bombArmature.animation.GetState("bomb")._duration), cancellationToken: cancellationToken);

        // 检查取消请求
        if (cancellationToken.IsCancellationRequested)
        {
            // 销毁炸弹组对象
            Destroy(bombGroup);
            return;
        }

        // 使用射线检测炸弹范围内的敌人和宝箱
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
                    enemyController.TakeDamage((long)DamageNum, hitCollider.gameObject); // 对敌人造成致命伤害
                }
            }
            else if (hitCollider.CompareTag("Chest"))
            {
                ChestController chestController = hitCollider.GetComponent<ChestController>();
                if (chestController != null && chestController.isVise)
                {
                    chestController.TakeDamage((long)DamageNum, hitCollider.gameObject); // 对宝箱造成致命伤害
                }
            }
        }

        // 销毁炸弹组对象
        Destroy(bombGroup);
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, detectionAreaSize);
    }
}
