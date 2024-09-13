using Cysharp.Threading.Tasks;
using DragonBones;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;
using Transform = UnityEngine.Transform;

public class EnemyController : MonoBehaviour
{
    public float moveSpeed = 1f;  // 敌人移动速度
    public float attackRange = 0.05f;  // 敌人攻击距离
    public float detectionRange = 2f;  // 敌人开始向玩家移动的距离
    public float damage = 10f;  // 敌人攻击时对玩家造成的伤害
    public float attackCooldown = 1f;  // 攻击冷却时间
    public float health = 100f;  // 敌人的初始血量
    private float maxHealth;    // 敌人的最大血量，用于计算血条比例
    private Transform HitTarget;  // 玩家对象的引用
    public EnemyType enemyType;
    public int Enemycoins;
    public List<int> coinProbilityList;

    public Slider healthSlider;  // 血量显示的Slider
    public Image redImage;  // 用于显示当前血量的红色图片
    public Image blackBackground; // 黑色背景
    public Transform healthBarCanvas; // 血条所在的Canvas

    public UnityArmatureComponent armatureComponent; // 用于控制DragonBones动画

    private bool isAttacking = false;  // 标志位，表示敌人是否正在攻击
    private bool hasStartedMovingTowardsPlayer = false; // 标志位，表示敌人是否已经开始朝玩家移动
    public Renderer enemyRenderer; // 用于控制材质球
    //public Color emissionColor = new Color(255, 0, 0); // 敌人受击时发光的颜色
    private Color originalEmissionColor; // 敌人材质的原始发光颜色
    private bool isStopped = false; // 是否停止移动

    public GameMainPanelController gameMainPanelController;
    private Transform coinTargetPos;
    void OnEnable()
    {
        // 找到玩家对象（假设玩家的Tag是"HitTarget"）
        HitTarget = GameObject.FindGameObjectWithTag("HitTarget").transform;
        armatureComponent = transform.GetChild(0).GetComponent<UnityArmatureComponent>();
        enemyRenderer = transform.GetChild(0).transform.GetChild(0).GetComponent<MeshRenderer>();
        gameMainPanelController = GameObject.Find("UICanvas/GameMainPanel(Clone)").GetComponent<GameMainPanelController>();
        coinTargetPos = GameObject.Find("CointargetPos").transform;
        // 数值初始化
        Init();
        // 初始化血条UI
        if (healthSlider != null)
        {
            healthSlider.maxValue = health;
            healthSlider.value = health;
        }

        StartCoroutine(Start());
    }

    IEnumerator Start()
    {
        yield return new WaitForSeconds(1f); // 等待1秒，确保动画组件已初始化

        // 播放下落动画
        if (armatureComponent != null)
        {
            armatureComponent.animation.Play("walk");
        }

    }

    private void Init()
    {
        coinProbilityList = new List<int>();
        Enemycoins = 0;
        moveSpeed = 1f; // 初始化移动速度
        health = 100f; // 初始化血量
        damage = 10f; // 初始化伤害
        attackRange = 0.05f;
        detectionRange = 2f;
        GetTypeValue(enemyType);
        // 获取材质的原始发光颜色（如果存在Emission属性）
        if (enemyRenderer != null && enemyRenderer.material.HasProperty("_EmissionColor"))
        {
            originalEmissionColor = enemyRenderer.material.GetColor("_EmissionColor");
        }
    }
    public float speed1 = 1;
    public float speed2 = 1.1f;
    public void GetTypeValue(EnemyType enemyType)
    {
        switch (enemyType)
        {
            case EnemyType.CuipiMonster1:
                damage = ConfigManager.Instance.Tables.TablePhysiqueReslevelConfig.Get(1).CuipiAtk;
                moveSpeed = speed1;// ConfigManager.Instance.Tables.TablePhysiqueReslevelConfig.Get(1).CuipiSpd;
                health = ConfigManager.Instance.Tables.TablePhysiqueReslevelConfig.Get(1).CuipiHp;
                coinProbilityList = ConfigManager.Instance.Tables.TablePhysiqueReslevelConfig.Get(1).CuipiMoney;
                Enemycoins = Random.Range(coinProbilityList[0], coinProbilityList[1]);
                break;
            case EnemyType.CuipiMonster2:
                damage = ConfigManager.Instance.Tables.TablePhysiqueReslevelConfig.Get(1).CuipiAtk;
                moveSpeed = speed1;// ConfigManager.Instance.Tables.TablePhysiqueReslevelConfig.Get(1).CuipiSpd;
                health = ConfigManager.Instance.Tables.TablePhysiqueReslevelConfig.Get(1).CuipiHp;
                coinProbilityList = ConfigManager.Instance.Tables.TablePhysiqueReslevelConfig.Get(1).CuipiMoney;
                Enemycoins = Random.Range(coinProbilityList[0], coinProbilityList[1]);
                break;
            case EnemyType.ShortMonster:
                damage = ConfigManager.Instance.Tables.TablePhysiqueReslevelConfig.Get(1).JinAtk;
                moveSpeed = speed2;// ConfigManager.Instance.Tables.TablePhysiqueReslevelConfig.Get(1).JinSpd;
                health = ConfigManager.Instance.Tables.TablePhysiqueReslevelConfig.Get(1).JinHp;
                coinProbilityList = ConfigManager.Instance.Tables.TablePhysiqueReslevelConfig.Get(1).JinMoney;
                Enemycoins = Random.Range(coinProbilityList[0], coinProbilityList[1]);
                break;
            case EnemyType.DisMonster:
                damage = ConfigManager.Instance.Tables.TablePhysiqueReslevelConfig.Get(1).YuanAtk;
                moveSpeed = speed2;// ConfigManager.Instance.Tables.TablePhysiqueReslevelConfig.Get(1).YuanSpd;
                health = ConfigManager.Instance.Tables.TablePhysiqueReslevelConfig.Get(1).YuanHp;
                coinProbilityList = ConfigManager.Instance.Tables.TablePhysiqueReslevelConfig.Get(1).YuanMoney;
                Enemycoins = Random.Range(coinProbilityList[0], coinProbilityList[1]);
                break;
            case EnemyType.ElitesMonster:
                damage = ConfigManager.Instance.Tables.TablePhysiqueReslevelConfig.Get(1).JingAtk;
                moveSpeed = speed2;// ConfigManager.Instance.Tables.TablePhysiqueReslevelConfig.Get(1).JingSpd;
                health = ConfigManager.Instance.Tables.TablePhysiqueReslevelConfig.Get(1).JingHp;
                coinProbilityList = ConfigManager.Instance.Tables.TablePhysiqueReslevelConfig.Get(1).JingMoney;
                Enemycoins = Random.Range(coinProbilityList[0], coinProbilityList[1]);
                break;
            case EnemyType.Boss:
                damage = ConfigManager.Instance.Tables.TablePhysiqueReslevelConfig.Get(1).BossAtk;
                moveSpeed = ConfigManager.Instance.Tables.TablePhysiqueReslevelConfig.Get(1).BossSpd;
                health = ConfigManager.Instance.Tables.TablePhysiqueReslevelConfig.Get(1).BossHp;
                coinProbilityList = ConfigManager.Instance.Tables.TablePhysiqueReslevelConfig.Get(1).BossMoney;
                Enemycoins = Random.Range(coinProbilityList[0], coinProbilityList[1]);
                break;
        }

        // 设置最大生命值
        maxHealth = health;
    }

    void Update()
    {
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
                armatureComponent.animation.Play("attack");
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
                    armatureComponent.animation.Play("walk"); // Resume walk animation
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
            healthBarCanvas.position = transform.position + new Vector3(0, 0.3f, 0);
            healthBarCanvas.localScale = new Vector3(0.01f, 0.01f, 0.01f);
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
        health -= damageAmount;
        health = Mathf.Max(health, 0);
        if (healthSlider != null)
        {
            UpdateHealthUI();
        }
        StartCoroutine(FlashEmission(enemyObj)); // 执行发光效果
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
        if (enemyRenderer != null)
        {
            // 启用发光效果
            enemyRenderer.material.SetFloat("_EmissionToggle", 1.0f);
            enemyRenderer.material.SetFloat("_EmissionIntensity", 2f);
            //enemyRenderer.material.SetColor("_EmissionColor", emissionColor);
            // 等待一小段时间
            yield return new WaitForSeconds(0.3f);

            // 恢复原始发光颜色
            //enemyRenderer.material.SetColor("_EmissionColor", originalEmissionColor);

            // 禁用发光效果
            enemyRenderer.material.SetFloat("_EmissionToggle", 0f);

            if (health <= 0)
            {
                Die(enemyObj); // 敌人死亡
            }
        }
    }



    public async UniTask  Die(GameObject enemyObj)
    {
        //// 播放死亡动画
        //if (armatureComponent != null)
        //{
        //    armatureComponent.animation.Play("die");
        //}
        Vector3 deathPosition = transform.position;
        //掉落金币概率
        await GetProbability(deathPosition);
        if (enemyObj.activeSelf)
        {
            var enemyPool = PreController.Instance.GetEnemyPoolMethod(enemyObj);
            enemyPool.Release(enemyObj);
        }
        //StartCoroutine(DelayedReturnToPool(enemyObj));
    }
   
    public async UniTask GetProbability(Vector3 deathPosition)
    {
        //TTOD1修改使用表格数据
        float probability = (float)(0.1 * (1 + BuffDoorController.Instance.coinFac));
        int randomNum = Random.Range(1, 100);
        Debug.Log(probability * 100 + "获得金币的概率" + randomNum + "在1-100随机抽取的数===========");
        if(randomNum < probability * 100)
        {
            Debug.Log(Enemycoins+ "获得金币!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            //执行金币出现逻辑
            await SpawnAndMoveCoins(Enemycoins, deathPosition);
        }
    }
    // 在调用 SpawnAndMoveCoins 时，确保使用 await
    public async UniTask SpawnAndMoveCoins(int coinCount, Vector3 deathPosition)
    {
        for (int i = 0; i < coinCount; i++)
        {
            // 从对象池中获取金币对象
            string CoinName = "gold";
            if (PreController.Instance.CoinPools.TryGetValue(CoinName, out var selectedCoinPool))
            {
                GameObject coinObj = selectedCoinPool.Get();
                coinObj.transform.position = deathPosition;  // 设置金币位置为敌人死亡的位置
                coinObj.SetActive(true);

                // 播放金币动画（使用 UnityArmatureComponent）
                UnityArmatureComponent coinArmature = coinObj.transform.GetChild(0).GetComponent<UnityArmatureComponent>();
                if (coinArmature != null)
                {
                    coinArmature.animation.Play("newAnimation");
                }

                // 异步移动金币到UI标识
                await MoveCoinToUI(coinObj, selectedCoinPool);
            }
        }
    }
    // 将 MoveCoinToUI 改为异步方法
    public async UniTask MoveCoinToUI(GameObject coinObj, ObjectPool<GameObject> CoinPool)
    {
        // 设置飞行的持续时间和初始位置
        float duration = 0.5f;  // 增加持续时间
        float elapsedTime = 0f;
        Vector3 startPosition = coinObj.transform.position;
        Vector3 targetPosition = coinTargetPos.position;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);

            // 通过Lerp函数平滑移动金币
            Vector3 currentPosition = Vector3.Lerp(startPosition, targetPosition, t);
            currentPosition.z = -0.1f;
            coinObj.transform.position = currentPosition;

            // 等待下一帧
            await UniTask.Yield();
        }

        // 当金币到达目标位置后，将金币返回对象池，并增加玩家的金币数量
        CoinPool.Release(coinObj);
        PlayInforManager.Instance.playInfor.AddCoins(1);  // 增加玩家的金币数量
    }




    IEnumerator MoveOffScreenWithParabola(GameObject enemyObj)
    {
        // 确定屏幕宽度的边界
        float screenWidth = Camera.main.orthographicSize * Camera.main.aspect;

        // 获取敌人当前位置
        Vector3 startPosition = transform.position;

        // 计算屏幕左边和右边的目标位置
        Vector3 leftEdge = new Vector3(-screenWidth - 1f, transform.position.y, transform.position.z);
        Vector3 rightEdge = new Vector3(screenWidth + 1f, transform.position.y, transform.position.z);

        // 选择飞向屏幕的哪一边（随机）
        Vector3 targetPosition = Random.value > 0.5f ? leftEdge : rightEdge;

        // 设置抛物线高度
        float height = 2f; // 这个值可以调整以改变抛物线的高度
        float duration = 1f; // 飞行时间
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;

            // 计算抛物线位置
            float x = Mathf.Lerp(startPosition.x, targetPosition.x, t);
            float y = Mathf.Lerp(startPosition.y, targetPosition.y, t);
            float parabolaY = Mathf.Lerp(startPosition.y, targetPosition.y, t) + Mathf.Sin(t * Mathf.PI) * height;

            transform.position = new Vector3(x, parabolaY, transform.position.z);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 确保敌人完全离开屏幕后销毁对象
        transform.position = targetPosition; // 确保目标位置是最后的位置
    }
  
    private void StopMovement()
    {
        isStopped = true;
        moveSpeed = 0f;
        if (armatureComponent != null)
        {
            armatureComponent.animation.Play("attack"); // Resume walk animation
        }
    }
    
}