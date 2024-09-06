using DragonBones;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    void OnEnable()
    {
        // 找到玩家对象（假设玩家的Tag是"HitTarget"）
        HitTarget = GameObject.FindGameObjectWithTag("HitTarget").transform;
        armatureComponent = transform.GetChild(0).GetComponent<UnityArmatureComponent>();

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
        GetTypeValue(enemyType);
    }

    public void GetTypeValue(EnemyType enemyType)
    {
        switch (enemyType)
        {
            case EnemyType.CuipiMonster1:
                damage = ConfigManager.Instance.Tables.TablePhysiqueReslevelConfig.Get(1).CuipiAtk;
                moveSpeed = 2f; // ConfigManager.Instance.Tables.TablePhysiqueReslevelConfig.Get(1).CuipiSpd;
                health = ConfigManager.Instance.Tables.TablePhysiqueReslevelConfig.Get(1).CuipiHp;
                coinProbilityList = ConfigManager.Instance.Tables.TablePhysiqueReslevelConfig.Get(1).CuipiMoney;
                Enemycoins = Random.Range(coinProbilityList[0], coinProbilityList[1]);
                break;
            case EnemyType.CuipiMonster2:
                damage = ConfigManager.Instance.Tables.TablePhysiqueReslevelConfig.Get(1).CuipiAtk;
                moveSpeed =2f; // ConfigManager.Instance.Tables.TablePhysiqueReslevelConfig.Get(1).CuipiSpd;
                health = ConfigManager.Instance.Tables.TablePhysiqueReslevelConfig.Get(1).CuipiHp;
                coinProbilityList = ConfigManager.Instance.Tables.TablePhysiqueReslevelConfig.Get(1).CuipiMoney;
                Enemycoins = Random.Range(coinProbilityList[0], coinProbilityList[1]);
                break;
            case EnemyType.ShortMonster:
                damage = ConfigManager.Instance.Tables.TablePhysiqueReslevelConfig.Get(1).JinAtk;
                moveSpeed = 3f; // ConfigManager.Instance.Tables.TablePhysiqueReslevelConfig.Get(1).JinSpd;
                health = ConfigManager.Instance.Tables.TablePhysiqueReslevelConfig.Get(1).JinHp;
                coinProbilityList = ConfigManager.Instance.Tables.TablePhysiqueReslevelConfig.Get(1).JinMoney;
                Enemycoins = Random.Range(coinProbilityList[0], coinProbilityList[1]);
                break;
            case EnemyType.DisMonster:
                damage = ConfigManager.Instance.Tables.TablePhysiqueReslevelConfig.Get(1).YuanAtk;
                moveSpeed = 3f; // ConfigManager.Instance.Tables.TablePhysiqueReslevelConfig.Get(1).YuanSpd;
                health = ConfigManager.Instance.Tables.TablePhysiqueReslevelConfig.Get(1).YuanHp;
                coinProbilityList = ConfigManager.Instance.Tables.TablePhysiqueReslevelConfig.Get(1).YuanMoney;
                Enemycoins = Random.Range(coinProbilityList[0], coinProbilityList[1]);
                break;
            case EnemyType.ElitesMonster:
                damage = ConfigManager.Instance.Tables.TablePhysiqueReslevelConfig.Get(1).JingAtk;
                moveSpeed = 3f; // ConfigManager.Instance.Tables.TablePhysiqueReslevelConfig.Get(1).JingSpd;
                health = ConfigManager.Instance.Tables.TablePhysiqueReslevelConfig.Get(1).JingHp;
                coinProbilityList = ConfigManager.Instance.Tables.TablePhysiqueReslevelConfig.Get(1).JingMoney;
                Enemycoins = Random.Range(coinProbilityList[0], coinProbilityList[1]);
                break;
            case EnemyType.Boss:
                damage = ConfigManager.Instance.Tables.TablePhysiqueReslevelConfig.Get(1).BossAtk;
                moveSpeed = 0f; // ConfigManager.Instance.Tables.TablePhysiqueReslevelConfig.Get(1).BossSpd;
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
            MoveVerticallyDown();
        }
        UpdateHealthBarPosition(); // 每帧更新血条位置
    }

    void MoveVerticallyDown()
    {
        // Move the enemy down the screen
        transform.position += Vector3.down * moveSpeed * Time.deltaTime;

        // Check distance to player to start moving towards the player
        float distanceToPlayer = Vector3.Distance(transform.position, HitTarget.position);
        if (distanceToPlayer <= detectionRange)
        {
            hasStartedMovingTowardsPlayer = true;
            if (armatureComponent != null)
            {
                armatureComponent.animation.Play("walk");
            }
        }
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

    // 更新血条位置
    void UpdateHealthBarPosition()
    {
        if (healthBarCanvas != null)
        {
            healthBarCanvas.position = transform.position + new Vector3(0, 1f, 0);
            healthBarCanvas.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        }
    }

    // 处理敌人受到伤害
    public void TakeDamage(float damageAmount, GameObject enemyObj)
    {
        if (healthSlider != null)
        {
            health -= damageAmount;
            health = Mathf.Max(health, 0);

            UpdateHealthUI();
            if (health <= 0)
            {
                Die(enemyObj);
            }
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

    // 敌人死亡时调用的方法
    void Die(GameObject enemyObj)
    {
        var enemyPool = PreController.Instance.GetEnemyPoolMethod(enemyObj);
        enemyPool.Release(enemyObj);
        PreController.Instance.KillEnemyNun++;
        PlayInforManager.Instance.playInfor.AddCoins(Enemycoins);
    }
}
