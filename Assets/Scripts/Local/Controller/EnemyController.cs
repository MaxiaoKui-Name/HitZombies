using Cysharp.Threading.Tasks;
using DragonBones;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;
using Random = UnityEngine.Random;
using Transform = UnityEngine.Transform;

public class EnemyController : MonoBehaviour
{
    public float moveSpeed = 1f;  // 敌人移动速度
    public float attackRange = 0.05f;  // 敌人攻击距离
    public float detectionRange = 2f;  // 敌人开始向玩家移动的距离
    public float damage = 10f;  // 敌人攻击时对玩家造成的伤害
    public float attackCooldown = 1f;  // 攻击冷却时间
    public Vector3 targetScale;
    public float health = 100f;  // 敌人的初始血量
    private float maxHealth;    // 敌人的最大血量，用于计算血条比例
    private Transform HitTarget;  // 玩家对象的引用
    public EnemyType enemyType;
    public int Enemycoins1;
    public int Enemycoins2 = 10;
    private Collider2D collider;

    private Camera mainCamera;
    //public List<int> coinProbilityList;

    public Slider healthSlider;  // 血量显示的Slider
    public Image redImage;  // 用于显示当前血量的红色图片
    public Image blackBackground; // 黑色背景
    public Text CoinText; // 血条所在的Canvas
    public Transform healthBarCanvas; // 血条所在的Canvas
    public Vector3 addVector = Vector3.zero;
    public Vector3 ScaleVector;

    public UnityArmatureComponent armatureComponent; // 用于控制DragonBones动画

    private bool isAttacking = false;  // 标志位，表示敌人是否正在攻击
    public bool hasStartedMovingTowardsPlayer = false; // 标志位，表示敌人是否已经开始朝玩家移动
    public Renderer[] enemyRenderers; // 用于控制材质球
    //public Color emissionColor = new Color(255, 0, 0); // 敌人受击时发光的颜色
    private Color originalEmissionColor; // 敌人材质的原始发光颜色
    public bool isStopped = false; // 是否停止移动

    public GameMainPanelController gameMainPanelController;
    //private Transform coinTargetPos;
    public float probabilityBase;
    public bool isDead;
    public bool isVise;
    public bool isFrozen;
    public float hideYPosition = -10f; // 超出屏幕的Y坐标

    void OnEnable()
    {

        // 找到玩家对象（假设玩家的Tag是"HitTarget"）
        HitTarget = GameObject.FindGameObjectWithTag("HitTarget").transform;
        armatureComponent = transform.GetChild(0).GetComponent<UnityArmatureComponent>();
        enemyRenderers = transform.GetChild(0).GetComponentsInChildren<MeshRenderer>();
        if (GameObject.Find("UICanvas/GameMainPanel(Clone)") != null)
        {
            gameMainPanelController = GameObject.Find("UICanvas/GameMainPanel(Clone)").GetComponent<GameMainPanelController>();
        }
        CoinText = healthBarCanvas.transform.Find("CoinText").GetComponent<Text>();
        CoinText.gameObject.SetActive(false);
        //coinTargetPos = GameObject.Find("CointargetPos").transform;
        collider = transform.GetComponent<Collider2D>();
        collider.isTrigger = false;
        isDead = false;
        isVise = false;
        isFrozen = false;
        isStopped = false;
        // 获取主摄像机
        mainCamera = Camera.main;
        probabilityBase = 0;
        // 数值初始化
        Init();
        Enemycoins2 = 10;
        transform.localScale = targetScale;
        // 初始化血条UI
        if (healthSlider != null)
        {
            healthSlider.maxValue = health;
            healthSlider.value = health;
        }

        StartCoroutine(Start1());
    }
    void Start()
    {
        EventDispatcher.instance.Regist(EventNameDef.GAME_OVER, (v) => RecycleEnemy(gameObject));
    }

    IEnumerator Start1()
    {
        yield return new WaitForSeconds(1f); // 等待1秒，确保动画组件已初始化

        // 播放下落动画
        if (armatureComponent != null)
        {
            armatureComponent.animation.Play("walk", -1);
        }
    }

    private void Init()
    {
        // coinProbilityList = new List<int>();
        Enemycoins1 = 0;
        moveSpeed = 1f; // 初始化移动速度
        health = 100f; // 初始化血量
        damage = 10f; // 初始化伤害
        attackRange = 0.05f;
        detectionRange = 2f;
        GetTypeValue(enemyType);
        // 获取材质的原始发光颜色（如果存在Emission属性）
        for (int i = 0; i < enemyRenderers.Length; i++)
        {
            if (enemyRenderers[i] != null && enemyRenderers[i].material.HasProperty("_EmissionColor"))
            {
                enemyRenderers[i].material.SetFloat("_EmissionToggle", 0.0f);
            }
        }
    }
    public void GetTypeValue(EnemyType enemyType)
    {
        switch (enemyType)
        {
            case EnemyType.CuipiMonster1:
                addVector.y = 0.7f;
                ScaleVector = new Vector3(0.01f, 0.01f, 0.01f);
                damage = ConfigManager.Instance.Tables.TableMonsterConfig[1].Atk;
                moveSpeed = ConfigManager.Instance.Tables.TableMonsterConfig[1].Spd;
                health = ConfigManager.Instance.Tables.TableMonsterConfig[1].Hp * ConfigManager.Instance.Tables.TableDanConfig[GameFlowManager.Instance.currentLevelIndex == 0 ? +1 : GameFlowManager.Instance.currentLevelIndex].BloodCoefficient;
                probabilityBase = ConfigManager.Instance.Tables.TableMonsterConfig[1].MoneyProbability;
                targetScale = Vector3.one * ConfigManager.Instance.Tables.TableMonsterConfig[1].Scale;
                //coinProbilityList = ConfigManager.Instance.Tables.TablePhysiqueReslevelConfig.Get(1).CuipiMoney;
                Enemycoins1 = (int)(Random.Range(ConfigManager.Instance.Tables.TableMonsterConfig[1].MoneyMin, ConfigManager.Instance.Tables.TableMonsterConfig[1].MoneyMax) * ConfigManager.Instance.Tables.TableDanConfig[GameFlowManager.Instance.currentLevelIndex == 0 ? +1 : GameFlowManager.Instance.currentLevelIndex].BloodCoefficient);
                break;
            case EnemyType.CuipiMonster2:
                addVector.y = 0.7f;
                ScaleVector = new Vector3(0.01f, 0.01f, 0.01f);
                damage = ConfigManager.Instance.Tables.TableMonsterConfig[2].Atk;
                moveSpeed = ConfigManager.Instance.Tables.TableMonsterConfig[2].Spd;
                health = ConfigManager.Instance.Tables.TableMonsterConfig[2].Hp * ConfigManager.Instance.Tables.TableDanConfig[GameFlowManager.Instance.currentLevelIndex == 0 ? +1 : GameFlowManager.Instance.currentLevelIndex].BloodCoefficient;
                probabilityBase = ConfigManager.Instance.Tables.TableMonsterConfig[2].MoneyProbability;
                targetScale = Vector3.one * ConfigManager.Instance.Tables.TableMonsterConfig[2].Scale;
                //coinProbilityList = ConfigManager.Instance.Tables.TablePhysiqueReslevelConfig.Get(1).CuipiMoney;
                Enemycoins1 = (int)(Random.Range(ConfigManager.Instance.Tables.TableMonsterConfig[2].MoneyMin, ConfigManager.Instance.Tables.TableMonsterConfig[2].MoneyMax) * ConfigManager.Instance.Tables.TableDanConfig[GameFlowManager.Instance.currentLevelIndex == 0 ? +1 : GameFlowManager.Instance.currentLevelIndex].BloodCoefficient);
                break;
            case EnemyType.ShortMonster:
                addVector.y = 2.4f;
                ScaleVector = new Vector3(0.01f, 0.01f, 0.01f);
                damage = ConfigManager.Instance.Tables.TableMonsterConfig[3].Atk;
                moveSpeed = ConfigManager.Instance.Tables.TableMonsterConfig[3].Spd;
                health = ConfigManager.Instance.Tables.TableMonsterConfig[3].Hp * ConfigManager.Instance.Tables.TableDanConfig[GameFlowManager.Instance.currentLevelIndex == 0 ? +1 : GameFlowManager.Instance.currentLevelIndex].BloodCoefficient;
                probabilityBase = ConfigManager.Instance.Tables.TableMonsterConfig[3].MoneyProbability;
                targetScale = Vector3.one * ConfigManager.Instance.Tables.TableMonsterConfig[3].Scale;
                // coinProbilityList = ConfigManager.Instance.Tables.TablePhysiqueReslevelConfig.Get(1).JinMoney;
                Enemycoins1 = (int)(Random.Range(ConfigManager.Instance.Tables.TableMonsterConfig[3].MoneyMin, ConfigManager.Instance.Tables.TableMonsterConfig[3].MoneyMax) * ConfigManager.Instance.Tables.TableDanConfig[GameFlowManager.Instance.currentLevelIndex == 0 ? +1 : GameFlowManager.Instance.currentLevelIndex].BloodCoefficient);
                break;
            case EnemyType.DisMonster:
                addVector.y = 0.7f;
                ScaleVector = new Vector3(0.007f, 0.007f, 0.007f); ;
                damage = ConfigManager.Instance.Tables.TableMonsterConfig.Get(4).Atk;
                moveSpeed = ConfigManager.Instance.Tables.TableMonsterConfig[4].Spd;
                health = ConfigManager.Instance.Tables.TableMonsterConfig[4].Hp * ConfigManager.Instance.Tables.TableDanConfig[GameFlowManager.Instance.currentLevelIndex == 0 ? +1 : GameFlowManager.Instance.currentLevelIndex].BloodCoefficient;
                probabilityBase = ConfigManager.Instance.Tables.TableMonsterConfig[4].MoneyProbability;
                targetScale = Vector3.one * ConfigManager.Instance.Tables.TableMonsterConfig[4].Scale;
                //coinProbilityList = ConfigManager.Instance.Tables.TablePhysiqueReslevelConfig.Get(1).YuanMoney;
                Enemycoins1 = (int)(Random.Range(ConfigManager.Instance.Tables.TableMonsterConfig[4].MoneyMin, ConfigManager.Instance.Tables.TableMonsterConfig[4].MoneyMax) * ConfigManager.Instance.Tables.TableDanConfig[GameFlowManager.Instance.currentLevelIndex == 0 ? +1 : GameFlowManager.Instance.currentLevelIndex].BloodCoefficient);
                break;
            case EnemyType.ElitesMonster:
                addVector.y = 2f;
                ScaleVector = new Vector3(0.01f, 0.01f, 0.01f);
                damage = ConfigManager.Instance.Tables.TableMonsterConfig[5].Atk;
                moveSpeed = ConfigManager.Instance.Tables.TableMonsterConfig[5].Spd;
                health = ConfigManager.Instance.Tables.TableMonsterConfig[5].Hp * ConfigManager.Instance.Tables.TableDanConfig[GameFlowManager.Instance.currentLevelIndex == 0 ? +1 : GameFlowManager.Instance.currentLevelIndex].BloodCoefficient; 
                probabilityBase = ConfigManager.Instance.Tables.TableMonsterConfig[5].MoneyProbability;
                targetScale = Vector3.one * ConfigManager.Instance.Tables.TableMonsterConfig[5].Scale;
                //coinProbilityList = ConfigManager.Instance.Tables.TablePhysiqueReslevelConfig.Get(1).JingMoney;
                Enemycoins1 = (int)(Random.Range(ConfigManager.Instance.Tables.TableMonsterConfig[5].MoneyMin, ConfigManager.Instance.Tables.TableMonsterConfig[5].MoneyMax) * ConfigManager.Instance.Tables.TableDanConfig[GameFlowManager.Instance.currentLevelIndex == 0 ? +1 : GameFlowManager.Instance.currentLevelIndex].BloodCoefficient);
                break;
            case EnemyType.Boss:
                addVector.y = 2f;
                ScaleVector = new Vector3(0.01f, 0.01f, 0.01f);
                damage = ConfigManager.Instance.Tables.TableMonsterConfig[100].Atk;
                moveSpeed = ConfigManager.Instance.Tables.TableMonsterConfig[100].Spd;
                health = ConfigManager.Instance.Tables.TableMonsterConfig[100].Hp * ConfigManager.Instance.Tables.TableDanConfig[GameFlowManager.Instance.currentLevelIndex == 0 ? +1 : GameFlowManager.Instance.currentLevelIndex].BloodCoefficient;
                probabilityBase = ConfigManager.Instance.Tables.TableMonsterConfig[100].MoneyProbability;
                //coinProbilityList = ConfigManager.Instance.Tables.TablePhysiqueReslevelConfig.Get(1).BossMoney;
                Enemycoins1 =(int)(Random.Range(ConfigManager.Instance.Tables.TableMonsterConfig[100].MoneyMin, ConfigManager.Instance.Tables.TableMonsterConfig[100].MoneyMax) * ConfigManager.Instance.Tables.TableDanConfig[GameFlowManager.Instance.currentLevelIndex == 0 ? +1 : GameFlowManager.Instance.currentLevelIndex].BloodCoefficient);
                break;
        }
        // 设置最大生命值
        maxHealth = health;
    }

    void Update()
    {
        if (isFrozen || GameManage.Instance.isFrozen)
        {
            return; // 冻结时不执行任何逻辑
        }
        if (GameManage.Instance.gameState != GameState.Running)
        {
            RecycleEnemy(gameObject);
            return; // 冻结时不执行任何逻辑
        }

        if (hasStartedMovingTowardsPlayer)
        {
            MoveTowardsPlayer();
        }
        else
        {
            if (!isStopped)
            {
                MoveVerticallyDown();
            }
        }
        if (transform.position.y < hideYPosition)
        {
            isStopped = true;
            RecycleEnemy(gameObject);
        }
        UpdateHealthBarPosition(); // 每帧更新血条位置
    }

    void MoveVerticallyDown()
    {
        // Move the enemy down the screen
        transform.position += Vector3.down * moveSpeed * Time.deltaTime;
    }

    void MoveTowardsPlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, HitTarget.position);

        if (distanceToPlayer <= attackRange)
        {
            if (!isAttacking && armatureComponent != null)
            {
                armatureComponent.animation.Play("attack", -1);
                isAttacking = true;
                moveSpeed = 0; // Stop moving
            }
        }
        else
        {
            if (isAttacking) // If previously attacking, reset state
            {
                isAttacking = false;
                if (armatureComponent != null)
                {
                    armatureComponent.animation.Play("walk",-1); // Resume walk animation
                }
            }
            moveSpeed = Mathf.Max(moveSpeed, 0.1f); // Ensure moveSpeed is positive
            transform.position += (HitTarget.position - transform.position).normalized * moveSpeed * Time.deltaTime;
        }
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Shield")) // 假设防护罩的Tag是"Shield"
        {
            StopMovement();
        }
    }
    // 更新血条位置
    void UpdateHealthBarPosition()
    {
        if (healthBarCanvas != null)
        {
           
            healthBarCanvas.position = transform.position + addVector;
            healthBarCanvas.localScale = ScaleVector;
        }
    }

    // 处理敌人受到伤害
    public void TakeDamage(float damageAmount, GameObject enemyObj)
    {
        //if (healthSlider != null)
        //{
        //    health -= damageAmount;
        //    health = Mathf.Max(health, 0);

        //    UpdateHealthUI();
        //    if (health <= 0)
        //    {
        //        Die(enemyObj);
        //    }
        //}
        //if (!IsEnemyOnScreen(enemyObj)) return;
        health -= damageAmount;
        health = Mathf.Max(health, 0);
        if (health <= 0)
        {
            isDead = true;
            collider.isTrigger = true;
            GameManage.Instance.KilledMonsterNun++;
            PreController.Instance.DecrementActiveEnemy();
        }
        if (healthSlider != null)
        {
            UpdateHealthUI();
        }
        StartCoroutine(FlashEmission(enemyObj)); // 执行发光效果
    }
    public bool IsEnemyOnScreen(GameObject enemy)
    {
        Vector3 viewportPoint = mainCamera.WorldToViewportPoint(enemy.transform.position);
        // 检查敌人是否在屏幕的可见范围内（0到1的范围为屏幕内，Y轴为1表示屏幕顶部）
        return viewportPoint.x >= 0 && viewportPoint.x <= 1 && viewportPoint.y >= 0 && viewportPoint.y <= 1;
    }
    // 更新血量UI
    void UpdateHealthUI()
    {
        if (healthSlider != null)
        {
            healthSlider.value = health;
        }
    }
    // 协程来实现敌人受击时的发光效果
    // 协程来实现敌人受击时的发光效果
    IEnumerator FlashEmission(GameObject enemyObj)
    {
        for (int i = 0; i < enemyRenderers.Length; i++)
        {
            if (enemyRenderers[i] != null && enemyRenderers[i].material.HasProperty("_EmissionColor"))
            {
                // 启用发光效果
                enemyRenderers[i].material.SetFloat("_EmissionToggle", 1.0f);
            }
        }
        yield return new WaitForSeconds(0.15f);
        for (int i = 0; i < enemyRenderers.Length; i++)
        {
            if (enemyRenderers[i] != null && enemyRenderers[i].material.HasProperty("_EmissionColor"))
            {
                enemyRenderers[i].material.SetFloat("_EmissionToggle", 0f);
            }
        }
        if (isDead || health <= 0)
        {
            Die(enemyObj); // 敌人死亡
        }
    }


    public async UniTask Die(GameObject enemyObj)
    {
        if (armatureComponent != null)
        {
            Debug.Log("播放死亡动画");
            if(enemyObj.name != "zombieelite_005(Clone)")
            {
                await PlayAndWaitForAnimation(armatureComponent, "die", 1);  // 播放一次hit动画
               
            }
            Vector3 deathPosition = transform.position;
            if (enemyObj.activeSelf)
            {
                RecycleEnemy(enemyObj);
                await GetProbability(deathPosition, enemyObj);
                // 减少活跃敌人数量
            }
        }
        else
        {
            Debug.Log("动画为空");
        }
    }

    // 等待动画播放完成
    private async UniTask PlayAndWaitForAnimation(UnityArmatureComponent armature, string animationName, int playTimes = 1)
    {
        var tcs = new UniTaskCompletionSource();

        // 定义事件处理程序
        void OnAnimationComplete(string type, EventObject eventObject)
        {
            if (eventObject.animationState.name == animationName)
            {
                armature.RemoveDBEventListener(EventObject.COMPLETE, OnAnimationComplete);  // 移除监听器
                tcs.TrySetResult(); // 完成任务
            }
        }

        // 添加事件监听器
        armature.AddDBEventListener(EventObject.COMPLETE, OnAnimationComplete);

        // 播放指定动画，并指定播放次数
        armature.animation.Play(animationName, playTimes);

        // 等待任务完成
        await tcs.Task;
        if(animationName == "die")
        {
            CoinText.gameObject.SetActive(true);
            CoinText.text = $"+{FormatCoinCount(Enemycoins1)}";
            await UniTask.Delay(200);
            CoinText.gameObject.SetActive(false);
        }
    }
    // 格式化金币数量
    private string FormatCoinCount(long coinCount)
    {
        if (coinCount >= 1_000_000_000_000) // 1 trillion
        {
            return $"{coinCount / 1_000_000_000_000.0:F1}T"; // T for trillion
        }
        else if (coinCount >= 1_000_000_000) // 1 billion
        {
            return $"{coinCount / 1_000_000_000.0:F1}B"; // B for billion
        }
        else if (coinCount >= 1_000_000) // 1 million
        {
            return $"{coinCount / 1_000_000.0:F1}M"; // M for million
        }
        else if (coinCount >= 1_000) // 1 thousand
        {
            return $"{coinCount / 1_000.0:F1}K"; // K for thousand
        }

        return coinCount.ToString(); // Return the number as is if less than a thousand
    }

    public async UniTask GetProbability(Vector3 deathPosition, GameObject enemyObj)
    {
        float probability = probabilityBase;
        int randomNum = Random.Range(1, 100);
        Debug.Log(probability * 100 + "获得金币的概率" + randomNum);
        if (randomNum < probability * 100)
        {
            Debug.Log(Enemycoins1 + "获得金币");
            await SpawnAndMoveCoins(Enemycoins2, deathPosition, enemyObj);
            PlayInforManager.Instance.playInfor.AddCoins(Enemycoins1 - Enemycoins2);
        }
    }

    public async UniTask SpawnAndMoveCoins(int coinCount, Vector3 deathPosition, GameObject enemyObj)
    {
        for (int i = 0; i < coinCount; i++)
        {
            string CoinName = "gold";
            if (PreController.Instance.CoinPools.TryGetValue(CoinName, out var selectedCoinPool))
            {
                GameObject coinObj = selectedCoinPool.Get();
                coinObj.transform.position = deathPosition;
                UnityArmatureComponent coinArmature = coinObj.transform.GetChild(0).GetComponent<UnityArmatureComponent>();
                if (coinArmature != null)
                {
                    coinArmature.animation.Play("newAnimation", -1);
                }
                Gold gold = coinObj.GetComponent<Gold>();
                Transform CointargetPos = GameObject.Find("CointargetPos").transform;
                gold.AwaitMove(selectedCoinPool,CointargetPos);
            }
            await UniTask.Delay(TimeSpan.FromSeconds(0.05f));
        }
    }

    //public async UniTask MoveCoinToUI(GameObject coinObj, ObjectPool<GameObject> CoinPool)
    //{
    //    float duration = 0.5f;
    //    float elapsedTime = 0f;
    //    Vector3 startPosition = coinObj.transform.position;
    //    Vector3 targetPosition = coinTargetPos.position;

    //    if (coinObj == null || !coinObj.activeSelf)
    //    {
    //        Debug.LogWarning("coinObj 已经被回收或禁用！");
    //        return;
    //    }

    //    while (elapsedTime < duration)
    //    {
    //        elapsedTime += Time.deltaTime;
    //        float t = Mathf.Clamp01(elapsedTime / duration);
    //        Vector3 currentPosition = Vector3.Lerp(startPosition, targetPosition, t);
    //        currentPosition.z = -0.1f;

    //        if (coinObj == null || !coinObj.activeSelf)
    //        {
    //            Debug.LogWarning("coinObj 已经被回收或禁用！");
    //            return;
    //        }

    //        coinObj.transform.position = currentPosition;
    //        await UniTask.Yield();
    //    }

    //    if (coinObj.activeSelf)
    //    {
    //        CoinPool.Release(coinObj);
    //        PlayInforManager.Instance.playInfor.AddCoins(1);
    //    }
    //}

    public void RecycleEnemy(GameObject enemyObj)
    {
        if(enemyObj != null && enemyObj.activeSelf)
        {
            var enemyPool = PreController.Instance.GetEnemyPoolMethod(enemyObj);
            enemyObj.SetActive(false);
            Debug.Log("敌人回收完成");
            enemyPool.Release(enemyObj);
        }
    }

    private void StopMovement()
    {
        isStopped = true;
        moveSpeed = 0f;
        if (armatureComponent != null)
        {
            if (enemyType == EnemyType.ShortMonster)
            {
                armatureComponent.animation.Play("hit");
            }
            else
            {
                armatureComponent.animation.Play("attack");
            }
        }
    }
}