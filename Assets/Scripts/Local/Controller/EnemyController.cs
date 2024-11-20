using Cysharp.Threading.Tasks;
using DragonBones;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Transform = UnityEngine.Transform;
using Random = UnityEngine.Random;

public class EnemyController : MonoBehaviour
{
    public float moveSpeed = 1f;  // 敌人基础移动速度
    public float attackRange = 1f;  // 敌人攻击距离
    public float detectionRange = 2f;  // 敌人开始向玩家移动的距离
    public float damage = 10f;  // 敌人攻击时对玩家造成的伤害
    public float attackCooldown = 1f;  // 攻击冷却时间
    public Vector3 targetScale = Vector3.one;  // 怪物的目标大小
    public float health = 100f;  // 敌人的初始血量
    private float maxHealth;    // 敌人的最大血量，用于计算血条比例
    private Transform playerTransform;  // 玩家对象的引用
    public EnemyType enemyType;
    public int Enemycoins1;
    public int Enemycoins2 = 10;

    private Camera mainCamera;

    public Slider healthSlider;  // 血量显示的Slider
    public Text CoinText; // 显示金币数量的文本
    public Transform healthBarCanvas; // 血条所在的Canvas
    public Vector3 addVector = Vector3.zero;
    public Vector3 ScaleVector = Vector3.one;

    public UnityArmatureComponent armatureComponent; // 用于控制DragonBones动画

    private bool isAttacking = false;  // 标志位，表示敌人是否正在攻击
    public bool hasStartedMovingTowardsPlayer = false; // 标志位，表示敌人是否已经开始朝玩家移动
    public Renderer[] enemyRenderers; // 用于控制材质球
    private Color originalEmissionColor; // 敌人材质的原始发光颜色
    public bool isStopped = false; // 是否停止移动

    public GameMainPanelController gameMainPanelController;
    public float probabilityBase;
    public bool isDead;
    public bool isFrozen;
    public bool isVise;

    public float hideYPosition = -10f; // 超出屏幕的Y坐标

    void OnEnable()
    {
        // 找到玩家对象（假设玩家的Tag是"Player"）
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        armatureComponent = transform.GetChild(0).GetComponent<UnityArmatureComponent>();
        enemyRenderers = transform.GetChild(0).GetComponentsInChildren<MeshRenderer>();
        if (GameObject.Find("UICanvas/GameMainPanel(Clone)") != null)
        {
            gameMainPanelController = GameObject.Find("UICanvas/GameMainPanel(Clone)").GetComponent<GameMainPanelController>();
        }
        CoinText = healthBarCanvas.transform.Find("CoinText").GetComponent<Text>();
        CoinText.gameObject.SetActive(false);

        isDead = false;
        isFrozen = false;
        isStopped = false;
        isVise = false;

        // 获取主摄像机
        mainCamera = Camera.main;
        probabilityBase = 0;

        // 数值初始化
        Init();
        Enemycoins2 = 1;
        transform.localScale = targetScale;

        // 初始化血条UI
        if (healthSlider != null)
        {
            healthSlider.maxValue = health;
            healthSlider.value = health;
        }

        hasStartedMovingTowardsPlayer = false; // 初始化为false

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
        Enemycoins1 = 0;
        moveSpeed = 1f; // 初始化移动速度
        health = 100f; // 初始化血量
        damage = 10f; // 初始化伤害
        attackRange = 1f;
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
                health = ConfigManager.Instance.Tables.TableMonsterConfig[1].Hp * ConfigManager.Instance.Tables.TableDanConfig[GameFlowManager.Instance.currentLevelIndex].BloodCoefficient;// ConfigManager.Instance.Tables.TableDanConfig[GameFlowManager.Instance.currentLevelIndex == 0 ? +1 : GameFlowManager.Instance.currentLevelIndex].BloodCoefficient;
                probabilityBase = ConfigManager.Instance.Tables.TableMonsterConfig[1].MoneyProbability * ConfigManager.Instance.Tables.TableDanConfig[GameFlowManager.Instance.currentLevelIndex].CoinProbabilityCoefficient;
                targetScale = Vector3.one * ConfigManager.Instance.Tables.TableMonsterConfig[1].Scale;
                //coinProbilityList = ConfigManager.Instance.Tables.TablePhysiqueReslevelConfig.Get(1).CuipiMoney;
                Enemycoins1 = (int)(Random.Range(ConfigManager.Instance.Tables.TableMonsterConfig[1].MoneyMin, ConfigManager.Instance.Tables.TableMonsterConfig[1].MoneyMax) * ConfigManager.Instance.Tables.TableDanConfig[GameFlowManager.Instance.currentLevelIndex].CoinNumberCoefficient); //ConfigManager.Instance.Tables.TableDanConfig[GameFlowManager.Instance.currentLevelIndex == 0 ? +1 : GameFlowManager.Instance.currentLevelIndex].CoinNumberCoefficient);
                break;
            case EnemyType.CuipiMonster2:
                addVector.y = 0.7f;
                ScaleVector = new Vector3(0.01f, 0.01f, 0.01f);
                damage = ConfigManager.Instance.Tables.TableMonsterConfig[2].Atk;
                moveSpeed = ConfigManager.Instance.Tables.TableMonsterConfig[2].Spd;
                health = ConfigManager.Instance.Tables.TableMonsterConfig[2].Hp * ConfigManager.Instance.Tables.TableDanConfig[GameFlowManager.Instance.currentLevelIndex].BloodCoefficient;// ConfigManager.Instance.Tables.TableDanConfig[GameFlowManager.Instance.currentLevelIndex == 0 ? +1 : GameFlowManager.Instance.currentLevelIndex].BloodCoefficient;
                probabilityBase = ConfigManager.Instance.Tables.TableMonsterConfig[2].MoneyProbability * ConfigManager.Instance.Tables.TableDanConfig[GameFlowManager.Instance.currentLevelIndex].CoinProbabilityCoefficient;
                targetScale = Vector3.one * ConfigManager.Instance.Tables.TableMonsterConfig[2].Scale;
                //coinProbilityList = ConfigManager.Instance.Tables.TablePhysiqueReslevelConfig.Get(1).CuipiMoney;
                Enemycoins1 = (int)(Random.Range(ConfigManager.Instance.Tables.TableMonsterConfig[2].MoneyMin, ConfigManager.Instance.Tables.TableMonsterConfig[2].MoneyMax) * ConfigManager.Instance.Tables.TableDanConfig[GameFlowManager.Instance.currentLevelIndex].CoinNumberCoefficient); //ConfigManager.Instance.Tables.TableDanConfig[GameFlowManager.Instance.currentLevelIndex == 0 ? +1 : GameFlowManager.Instance.currentLevelIndex].CoinNumberCoefficient);
                break;
            case EnemyType.ShortMonster:
                addVector.y = 2.4f;
                ScaleVector = new Vector3(0.01f, 0.01f, 0.01f);
                damage = ConfigManager.Instance.Tables.TableMonsterConfig[3].Atk;
                moveSpeed = ConfigManager.Instance.Tables.TableMonsterConfig[3].Spd;
                health = ConfigManager.Instance.Tables.TableMonsterConfig[3].Hp * ConfigManager.Instance.Tables.TableDanConfig[GameFlowManager.Instance.currentLevelIndex].BloodCoefficient; //ConfigManager.Instance.Tables.TableDanConfig[GameFlowManager.Instance.currentLevelIndex == 0 ? +1 : GameFlowManager.Instance.currentLevelIndex].BloodCoefficient;
                probabilityBase = ConfigManager.Instance.Tables.TableMonsterConfig[3].MoneyProbability * ConfigManager.Instance.Tables.TableDanConfig[GameFlowManager.Instance.currentLevelIndex].CoinProbabilityCoefficient;
                targetScale = Vector3.one * ConfigManager.Instance.Tables.TableMonsterConfig[3].Scale;
                // coinProbilityList = ConfigManager.Instance.Tables.TablePhysiqueReslevelConfig.Get(1).JinMoney;
                Enemycoins1 = (int)(Random.Range(ConfigManager.Instance.Tables.TableMonsterConfig[3].MoneyMin, ConfigManager.Instance.Tables.TableMonsterConfig[3].MoneyMax) * ConfigManager.Instance.Tables.TableDanConfig[GameFlowManager.Instance.currentLevelIndex].CoinNumberCoefficient); //ConfigManager.Instance.Tables.TableDanConfig[GameFlowManager.Instance.currentLevelIndex == 0 ? +1 : GameFlowManager.Instance.currentLevelIndex].CoinNumberCoefficient);
                break;
            case EnemyType.DisMonster:
                addVector.y = 0.7f;
                ScaleVector = new Vector3(0.007f, 0.007f, 0.007f); ;
                damage = ConfigManager.Instance.Tables.TableMonsterConfig.Get(4).Atk;
                moveSpeed = ConfigManager.Instance.Tables.TableMonsterConfig[4].Spd;
                health = ConfigManager.Instance.Tables.TableMonsterConfig[4].Hp * ConfigManager.Instance.Tables.TableDanConfig[GameFlowManager.Instance.currentLevelIndex].BloodCoefficient; //ConfigManager.Instance.Tables.TableDanConfig[GameFlowManager.Instance.currentLevelIndex == 0 ? +1 : GameFlowManager.Instance.currentLevelIndex].BloodCoefficient;
                probabilityBase = ConfigManager.Instance.Tables.TableMonsterConfig[4].MoneyProbability * ConfigManager.Instance.Tables.TableDanConfig[GameFlowManager.Instance.currentLevelIndex].CoinProbabilityCoefficient;
                targetScale = Vector3.one * ConfigManager.Instance.Tables.TableMonsterConfig[4].Scale;
                //coinProbilityList = ConfigManager.Instance.Tables.TablePhysiqueReslevelConfig.Get(1).YuanMoney;
                Enemycoins1 = (int)(Random.Range(ConfigManager.Instance.Tables.TableMonsterConfig[4].MoneyMin, ConfigManager.Instance.Tables.TableMonsterConfig[4].MoneyMax) * ConfigManager.Instance.Tables.TableDanConfig[GameFlowManager.Instance.currentLevelIndex].CoinNumberCoefficient); //ConfigManager.Instance.Tables.TableDanConfig[GameFlowManager.Instance.currentLevelIndex == 0 ? +1 : GameFlowManager.Instance.currentLevelIndex].CoinNumberCoefficient);
                break;
            case EnemyType.ElitesMonster:
                addVector.y = 2f;
                ScaleVector = new Vector3(0.01f, 0.01f, 0.01f);
                damage = ConfigManager.Instance.Tables.TableMonsterConfig[5].Atk;
                moveSpeed = ConfigManager.Instance.Tables.TableMonsterConfig[5].Spd;
                health = ConfigManager.Instance.Tables.TableMonsterConfig[5].Hp * ConfigManager.Instance.Tables.TableDanConfig[GameFlowManager.Instance.currentLevelIndex].BloodCoefficient; //ConfigManager.Instance.Tables.TableDanConfig[GameFlowManager.Instance.currentLevelIndex == 0 ? +1 : GameFlowManager.Instance.currentLevelIndex].BloodCoefficient; 
                probabilityBase = ConfigManager.Instance.Tables.TableMonsterConfig[5].MoneyProbability * ConfigManager.Instance.Tables.TableDanConfig[GameFlowManager.Instance.currentLevelIndex].CoinProbabilityCoefficient;
                targetScale = Vector3.one * ConfigManager.Instance.Tables.TableMonsterConfig[5].Scale;
                //coinProbilityList = ConfigManager.Instance.Tables.TablePhysiqueReslevelConfig.Get(1).JingMoney;
                Enemycoins1 = (int)(Random.Range(ConfigManager.Instance.Tables.TableMonsterConfig[5].MoneyMin, ConfigManager.Instance.Tables.TableMonsterConfig[5].MoneyMax) * ConfigManager.Instance.Tables.TableDanConfig[GameFlowManager.Instance.currentLevelIndex].CoinNumberCoefficient); //ConfigManager.Instance.Tables.TableDanConfig[GameFlowManager.Instance.currentLevelIndex == 0 ? +1 : GameFlowManager.Instance.currentLevelIndex].CoinNumberCoefficient);
                break;
            case EnemyType.Boss:
                addVector.y = 2f;
                ScaleVector = new Vector3(0.01f, 0.01f, 0.01f);
                damage = ConfigManager.Instance.Tables.TableMonsterConfig[100].Atk;
                moveSpeed = ConfigManager.Instance.Tables.TableMonsterConfig[100].Spd;
                health = ConfigManager.Instance.Tables.TableMonsterConfig[100].Hp * ConfigManager.Instance.Tables.TableDanConfig[GameFlowManager.Instance.currentLevelIndex].BloodCoefficient; //ConfigManager.Instance.Tables.TableDanConfig[GameFlowManager.Instance.currentLevelIndex == 0 ? +1 : GameFlowManager.Instance.currentLevelIndex].BloodCoefficient;
                probabilityBase = ConfigManager.Instance.Tables.TableMonsterConfig[100].MoneyProbability * ConfigManager.Instance.Tables.TableDanConfig[GameFlowManager.Instance.currentLevelIndex].CoinProbabilityCoefficient;
                //coinProbilityList = ConfigManager.Instance.Tables.TablePhysiqueReslevelConfig.Get(1).BossMoney;
                Enemycoins1 = (int)(Random.Range(ConfigManager.Instance.Tables.TableMonsterConfig[100].MoneyMin, ConfigManager.Instance.Tables.TableMonsterConfig[100].MoneyMax) * ConfigManager.Instance.Tables.TableDanConfig[GameFlowManager.Instance.currentLevelIndex].CoinNumberCoefficient); //ConfigManager.Instance.Tables.TableDanConfig[GameFlowManager.Instance.currentLevelIndex == 0 ? +1 : GameFlowManager.Instance.currentLevelIndex].CoinNumberCoefficient);
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
            return; // 游戏未运行时不执行任何逻辑
        }


        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        // 检测是否开始朝玩家移动
        if (!hasStartedMovingTowardsPlayer && distanceToPlayer <= detectionRange)
        {
            hasStartedMovingTowardsPlayer = true;
        }

        if (hasStartedMovingTowardsPlayer)
        {
            // 朝玩家移动
            MoveTowardsPlayer();
        }
        else
        {
            // 向下移动
            MoveDownward();
        }

        if (transform.position.y < hideYPosition)
        {
            isStopped = true;
            RecycleEnemy(gameObject);
        }
        UpdateHealthBarPosition(); // 每帧更新血条位置
    }

    void MoveDownward()
    {
        // 敌人以 moveSpeed 向下移动
        transform.Translate(Vector3.down * moveSpeed * Time.deltaTime, Space.World);
    }

    void MoveTowardsPlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer <= attackRange)
        {
            if (!isAttacking && armatureComponent != null)
            {
                armatureComponent.animation.Play("hit", -1); // 重复播放"hit"动画
                isAttacking = true;
            }
        }
        // 朝玩家移动
        transform.Translate((playerTransform.position - transform.position).normalized * moveSpeed * Time.deltaTime, Space.World);

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
        health -= damageAmount;
        health = Mathf.Max(health, 0);

        if (health <= 0 && !isDead)
        {
            isDead = true;
            PreController.Instance.DecrementActiveEnemy();

            if (gameObject.activeSelf)
            {
                StartCoroutine(FlashEmission(enemyObj)); // 执行发光效果
            }
        }

        if (healthSlider != null)
        {
            UpdateHealthUI();
        }
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
            await PlayAndWaitForAnimation(armatureComponent, "die", 1);  // 播放一次"die"动画

            Vector3 deathPosition = transform.position;
            if (enemyObj.activeSelf)
            {
                await GetProbability(deathPosition, enemyObj);
                RecycleEnemy(enemyObj);
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
        Debug.Log(probability * 100 + "获得金币的概率" + randomNum + "金币数" + Enemycoins1);
        if (randomNum > (100 - probability))
        {
            Debug.Log(Enemycoins1 + "获得金币");
            await SpawnAndMoveCoins(Enemycoins2, deathPosition, enemyObj);
            PlayInforManager.Instance.playInfor.AddCoins(Enemycoins1 - Enemycoins2);
            CoinText.gameObject.SetActive(true);
            CoinText.text = $"+{FormatCoinCount(Enemycoins1)}";
            await UniTask.Delay(1000);
            CoinText.gameObject.SetActive(false);

        }
    }

    public async UniTask SpawnAndMoveCoins(int coinCount, Vector3 deathPosition, GameObject enemyObj)
    {
        for (int i = 1; i <= coinCount; i++)
        {
            string CoinName = "NewGold";
            if (PreController.Instance.CoinPools.TryGetValue(CoinName, out var selectedCoinPool))
            {
                GameObject coinObj = selectedCoinPool.Get();
                coinObj.SetActive(true);

                // 1. 将父物体下的局部坐标转换为世界坐标
                Vector3 worldPos = enemyObj.transform.parent.TransformPoint(deathPosition);

                // 2. 将世界坐标转换为屏幕坐标
                Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

                // 3. 获取Canvas的RectTransform
                RectTransform canvasRect = gameMainPanelController.canvasRectTransform;

                // 4. 将屏幕坐标转换为Canvas的本地坐标
                Vector2 localPos;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, null, out localPos);

                // 5. 设置coinObj的RectTransform的锚点位置
                RectTransform coinRect = coinObj.GetComponent<RectTransform>();
                coinRect.anchoredPosition = localPos;

                // 6. 播放动画
                UnityArmatureComponent coinArmature = coinObj.transform.GetChild(0).GetComponent<UnityArmatureComponent>();
                if (coinArmature != null)
                {
                    coinArmature.animation.Play("newAnimation", -1);
                }

                // 7. 获取Gold组件并启动移动逻辑
                Gold gold = coinObj.GetComponent<Gold>();
                gold.AwaitMove(selectedCoinPool, gameMainPanelController.coinspattern_F);
            }

            // 等待0.05秒后继续生成下一个金币
            await UniTask.Delay(TimeSpan.FromSeconds(0.05f));
        }
    }


    public void RecycleEnemy(GameObject enemyObj)
    {
        if (enemyObj != null && enemyObj.activeSelf)
        {
            var enemyPool = PreController.Instance.GetEnemyPoolMethod(enemyObj);
            GameManage.Instance.KilledMonsterNun++;
            isVise = false;
            enemyObj.SetActive(false);
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
