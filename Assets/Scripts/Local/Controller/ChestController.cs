using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DragonBones;
using Transform = UnityEngine.Transform;
using Cysharp.Threading.Tasks;
using UnityEngine.Pool;

public class ChestController : MonoBehaviour
{
    public float chestHealth = 100;  // 宝箱的初始血量
    public Transform coinTarget;   // 金币目标（比如UI上的金币位置）
    public float coinsToSpawn = 5;   // 生成的金币数量
    public UnityArmatureComponent armatureComponent;
    private bool isOpened = false; // 标记宝箱是否已经打开
    public float moveSpeed = 1f; // 设置物体向下移动的速度
    public float hideYPosition = -10f; // 超出屏幕的Y坐标
    public bool isMove = false;
    public GameObject planePrefab;  // 飞机的预制体
    public GameObject bombPrefab;   // 炸弹的预制体
    public float planeSpeed = 2f;   // 飞机移动速度
    public float bombDropInterval;  // 炸弹投掷间隔时间
    public float coinBase = 100f;   // 基础金币数量

    private Collider2D chestCollider; // 宝箱的碰撞体

    void OnEnable()
    {
        isMove = true;
        chestHealth = ConfigManager.Instance.Tables.TableBoxgenerate.Get(GameFlowManager.Instance.currentLevelIndex).Boxhp;
        coinTarget = GameObject.Find("CointargetPos").transform;
        coinsToSpawn = 0;
        bombDropInterval = 0.25f;
        coinBase = 20f;
        armatureComponent = transform.GetComponent<UnityArmatureComponent>();
        chestCollider = GetComponent<Collider2D>(); // 获取碰撞体组件
        if (chestCollider != null)
        {
            chestCollider.enabled = true; // 确保碰撞体启用
        }
        StartCoroutine(Start());
    }

    IEnumerator Start()
    {
        yield return new WaitForSeconds(0.5f); // 等待1秒，确保动画组件已初始化

        // 播放初始动画
        if (armatureComponent != null)
        {
            armatureComponent.animation.Play("breath");
        }
    }

    void Update()
    {
        if (isMove)
        {
            MoveDown();
        }

        if (transform.position.y < hideYPosition)
        {
            isMove = false;
            Destroy(transform.gameObject);
        }
    }

    // 物体向下移动
    private void MoveDown()
    {
        transform.Translate(Vector3.down * moveSpeed * Time.deltaTime);
    }

    // 处理宝箱受伤
    public async void TakeDamage(float damage, GameObject bulletobj)
    {
        if (isOpened) return; // 如果已经打开，直接返回

        // 播放hit动画并等待完成
        if (armatureComponent != null)
        {
            armatureComponent.animation.Play("hit1",1);
        }

        chestHealth -= damage;
        coinsToSpawn = bulletobj.transform.GetComponent<BulletController>().bulletcost;

        // 如果宝箱血量小于等于0，播放开箱动画并生成金币和飞机
        if (chestHealth <= 0 && !isOpened)
        {
            isOpened = true;
            Debug.Log("Chest opened!");
            if (armatureComponent != null)
            {
                armatureComponent.animation.Play("open",1);
            }

            // 禁用碰撞体，防止进一步的碰撞事件
            if (chestCollider != null)
            {
                chestCollider.enabled = false;
            }

            Vector3 deathPosition = transform.position;
            GetProbability(deathPosition).Forget();
            SpawnPlane().Forget(); // 生成飞机
            gameObject.SetActive(false); // 立即禁用宝箱
        }
    }

    // 计算概率并决定是否生成金币
    public async UniTask GetProbability(Vector3 deathPosition)
    {
        float probabilityTotal = 0f;
        for (int i = 0;i < ConfigManager.Instance.Tables.TableBoxcontent.DataMap.Count; i++)
        {
            probabilityTotal += ConfigManager.Instance.Tables.TableBoxcontent.Get(i + 1).Weight;
        }          
        float probability = ConfigManager.Instance.Tables.TableBoxcontent.Get(8).Weight / probabilityTotal;
        int randomNum = Random.Range(1, 100);
        Debug.Log($"Probability: {probability * 100}%, Random Num: {randomNum}");
        if (randomNum < probability * 100)
        {
            coinsToSpawn *= ConfigManager.Instance.Tables.TableBoxcontent.Get(8).Rewardres;
            await SpawnAndMoveCoins(coinBase, deathPosition);
        }
    }

    // 生成金币并移动到UI标识
    public async UniTask SpawnAndMoveCoins(float coinCount, Vector3 deathPosition)
    {
        List<UniTask> coinTasks = new List<UniTask>();

        for (int i = 0; i < coinCount; i++)
        {
            string CoinName = "gold";
            if (PreController.Instance.CoinPools.TryGetValue(CoinName, out var selectedCoinPool))
            {
                GameObject coinObj = selectedCoinPool.Get();
                coinObj.transform.position = deathPosition;
                coinObj.SetActive(true);

                UnityArmatureComponent coinArmature = coinObj.transform.GetChild(0).GetComponent<UnityArmatureComponent>();
                if (coinArmature != null)
                {
                    coinArmature.animation.Play("newAnimation");
                }

                // 添加每个金币的移动任务
                coinTasks.Add(MoveCoinToUI(coinObj, selectedCoinPool));
            }
        }

        // 等待所有金币移动完成
        await UniTask.WhenAll(coinTasks);
    }

    // 移动金币到UI标识
    public async UniTask MoveCoinToUI(GameObject coinObj, ObjectPool<GameObject> CoinPool)
    {
        float duration = 0.5f; // 增加持续时间以便更明显的移动效果
        float elapsedTime = 0f;
        Vector3 startPosition = coinObj.transform.position;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            Vector3 currentPosition = Vector3.Lerp(startPosition, coinTarget.position, t);
            currentPosition.z = -0.1f;
            coinObj.transform.position = currentPosition;
            await UniTask.Yield();
        }

        if (coinObj.activeSelf)
            CoinPool.Release(coinObj);
        PlayInforManager.Instance.playInfor.AddCoins(1);
    }

    // 生成并移动飞机（异步）
    public async UniTask SpawnPlane()
    {
        GameObject plane = Instantiate(Resources.Load<GameObject>("Prefabs/explode_bomber"), new Vector3(0, -7f, 0), Quaternion.identity);  // 生成飞机在屏幕底部
        Debug.Log("Plane spawned!");
        await MovePlaneAndDropBombs(plane);
        PlayInforManager.Instance.playInfor.AddCoins((int)(coinsToSpawn - coinBase));
    }

    // 飞机移动并投放炸弹的异步方法
    private async UniTask MovePlaneAndDropBombs(GameObject plane)
    {
        float dropTime = 0f;
        if (plane == null || !plane.activeSelf)
        {
            Destroy(transform.gameObject);
            Debug.LogWarning("plane 已经被回收或禁用！");
            return;
        }
        while (plane.transform.position.y < 6f)
        {
            plane.transform.Translate(Vector3.up * planeSpeed * Time.deltaTime);
            dropTime += Time.deltaTime;

            if (dropTime >= bombDropInterval)
            {
                dropTime = 0f;
                Vector3 bombPosition = PreController.Instance.RandomPosition(plane.transform.position);
                DropBomb(bombPosition).Forget();
            }

            await UniTask.Yield();
        }
        Destroy(plane);
        Destroy(transform.gameObject);
    }

    // 投放炸弹（异步）
    private async UniTask DropBomb(Vector3 planePosition)
    {
        GameObject bomb = Instantiate(Resources.Load<GameObject>("Prefabs/explode_01"), planePosition, Quaternion.identity);
        await BombExplosion(bomb);
    }

    // 炸弹爆炸动画，并消灭敌人（异步）
    private async UniTask BombExplosion(GameObject bomb)
    {
        await UniTask.Delay(1000);

        UnityArmatureComponent bombArmature = bomb.GetComponentInChildren<UnityArmatureComponent>();
        if (bombArmature != null)
        {
            bombArmature.animation.Play("fly");
        }

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            if (enemy.activeSelf && enemy.transform.localPosition.y < 4f)
            {
                EnemyController enemyController = enemy.GetComponent<EnemyController>();
                if (enemy.activeSelf && enemyController.health > 0)
                {
                    enemyController.Enemycoins2 = 1;
                    enemyController.Die(enemy);
                }
            }
         
        }
        await UniTask.Delay(1000);
        Destroy(bomb);
    }

    // 等待动画播放完成
    private async UniTask WaitForAnimationComplete(UnityArmatureComponent armature, string animationName)
    {
        var tcs = new UniTaskCompletionSource();

        // 定义事件处理程序
        void OnAnimationComplete(string type, EventObject eventObject)
        {
            if (eventObject.animationState.name == animationName)
            {
                armature.RemoveDBEventListener(EventObject.COMPLETE, OnAnimationComplete);  // 移除监听器
                tcs.TrySetResult();
            }
        }

        // 添加事件监听器
        armature.AddDBEventListener(EventObject.COMPLETE, OnAnimationComplete);

        // 等待任务完成
        await tcs.Task;
    }

}
