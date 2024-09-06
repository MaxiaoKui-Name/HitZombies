using DragonBones;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Transform = UnityEngine.Transform;

public class EnemyController : MonoBehaviour
{
    public float moveSpeed = 1f;  // �����ƶ��ٶ�
    public float attackRange = 0.05f;  // ���˹�������
    public float detectionRange = 2f;  // ���˿�ʼ������ƶ��ľ���
    public float damage = 10f;  // ���˹���ʱ�������ɵ��˺�
    public float attackCooldown = 1f;  // ������ȴʱ��
    public float health = 100f;  // ���˵ĳ�ʼѪ��
    private float maxHealth;    // ���˵����Ѫ�������ڼ���Ѫ������
    private Transform HitTarget;  // ��Ҷ��������
    public EnemyType enemyType;
    public int Enemycoins;
    public List<int> coinProbilityList;

    public Slider healthSlider;  // Ѫ����ʾ��Slider
    public Image redImage;  // ������ʾ��ǰѪ���ĺ�ɫͼƬ
    public Image blackBackground; // ��ɫ����
    public Transform healthBarCanvas; // Ѫ�����ڵ�Canvas

    public UnityArmatureComponent armatureComponent; // ���ڿ���DragonBones����

    private bool isAttacking = false;  // ��־λ����ʾ�����Ƿ����ڹ���
    private bool hasStartedMovingTowardsPlayer = false; // ��־λ����ʾ�����Ƿ��Ѿ���ʼ������ƶ�

    void OnEnable()
    {
        // �ҵ���Ҷ��󣨼�����ҵ�Tag��"HitTarget"��
        HitTarget = GameObject.FindGameObjectWithTag("HitTarget").transform;
        armatureComponent = transform.GetChild(0).GetComponent<UnityArmatureComponent>();

        // ��ֵ��ʼ��
        Init();
        // ��ʼ��Ѫ��UI
        if (healthSlider != null)
        {
            healthSlider.maxValue = health;
            healthSlider.value = health;
        }

        StartCoroutine(Start());
    }

    IEnumerator Start()
    {
        yield return new WaitForSeconds(1f); // �ȴ�1�룬ȷ����������ѳ�ʼ��

        // �������䶯��
        if (armatureComponent != null)
        {
            armatureComponent.animation.Play("walk");
        }

    }

    private void Init()
    {
        coinProbilityList = new List<int>();
        Enemycoins = 0;
        moveSpeed = 1f; // ��ʼ���ƶ��ٶ�
        health = 100f; // ��ʼ��Ѫ��
        damage = 10f; // ��ʼ���˺�
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

        // �����������ֵ
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
        UpdateHealthBarPosition(); // ÿ֡����Ѫ��λ��
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

    // ����Ѫ��λ��
    void UpdateHealthBarPosition()
    {
        if (healthBarCanvas != null)
        {
            healthBarCanvas.position = transform.position + new Vector3(0, 1f, 0);
            healthBarCanvas.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        }
    }

    // ��������ܵ��˺�
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

    // ����Ѫ��UI
    void UpdateHealthUI()
    {
        if (healthSlider != null)
        {
            healthSlider.value = health;
        }
    }

    // ��������ʱ���õķ���
    void Die(GameObject enemyObj)
    {
        var enemyPool = PreController.Instance.GetEnemyPoolMethod(enemyObj);
        enemyPool.Release(enemyObj);
        PreController.Instance.KillEnemyNun++;
        PlayInforManager.Instance.playInfor.AddCoins(Enemycoins);
    }
}
