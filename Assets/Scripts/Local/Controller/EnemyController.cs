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
    public Renderer enemyRenderer; // ���ڿ��Ʋ�����
    //public Color emissionColor = new Color(255, 0, 0); // �����ܻ�ʱ�������ɫ
    private Color originalEmissionColor; // ���˲��ʵ�ԭʼ������ɫ
    private bool isStopped = false; // �Ƿ�ֹͣ�ƶ�
    void OnEnable()
    {
        // �ҵ���Ҷ��󣨼�����ҵ�Tag��"HitTarget"��
        HitTarget = GameObject.FindGameObjectWithTag("HitTarget").transform;
        armatureComponent = transform.GetChild(0).GetComponent<UnityArmatureComponent>();
        enemyRenderer = transform.GetChild(0).transform.GetChild(0).GetComponent<MeshRenderer>();
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
        detectionRange = 2f;
        GetTypeValue(enemyType);
        // ��ȡ���ʵ�ԭʼ������ɫ���������Emission���ԣ�
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
            if (!isStopped)
            {
                MoveVerticallyDown();

            }
        }
        UpdateHealthBarPosition(); // ÿ֡����Ѫ��λ��
    }

    void MoveVerticallyDown()
    {
        // Move the enemy down the screen
        transform.position += Vector3.down * moveSpeed * Time.deltaTime;

        // Check distance to player to start moving towards the player
        //float distanceToPlayer = Vector3.Distance(transform.position, HitTarget.position);
        //if (distanceToPlayer <= detectionRange)
        //{
        //    hasStartedMovingTowardsPlayer = true;
        //    //if (armatureComponent != null)
        //    //{
        //    //    armatureComponent.animation.Play("walk");
        //    //}
        //}
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
            healthBarCanvas.position = transform.position + new Vector3(0, 0.3f, 0);
            healthBarCanvas.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        }
    }

    // ��������ܵ��˺�
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
        StartCoroutine(FlashEmission(enemyObj)); // ִ�з���Ч��

        //if (health <= 0)
        //{
        //    Die(enemyObj); // ��������
        //}
        //else
        //{
        //}

    }

    // ����Ѫ��UI
    void UpdateHealthUI()
    {
        if (healthSlider != null)
        {
            healthSlider.value = health;
        }
    }
    // Э����ʵ�ֵ����ܻ�ʱ�ķ���Ч��
    // Э����ʵ�ֵ����ܻ�ʱ�ķ���Ч��
    IEnumerator FlashEmission(GameObject enemyObj)
    {
        if (enemyRenderer != null)
        {
            // ���÷���Ч��
            enemyRenderer.material.SetFloat("_EmissionToggle", 1.0f);
            enemyRenderer.material.SetFloat("_EmissionIntensity", 2f);
            //enemyRenderer.material.SetColor("_EmissionColor", emissionColor);
            // �ȴ�һС��ʱ��
            yield return new WaitForSeconds(0.1f);

            // �ָ�ԭʼ������ɫ
            //enemyRenderer.material.SetColor("_EmissionColor", originalEmissionColor);

            // ���÷���Ч��
            enemyRenderer.material.SetFloat("_EmissionToggle", 0f);

            if (health <= 0)
            {
                Die(enemyObj); // ��������
            }
        }
    }



    void Die(GameObject enemyObj)
    {
        //// ������������
        //if (armatureComponent != null)
        //{
        //    armatureComponent.animation.Play("die");
        //}

        // ���µ��˳غͽ��
        var enemyPool = PreController.Instance.GetEnemyPoolMethod(enemyObj);
        enemyPool.Release(enemyObj);
        //�����Ҹ���
        GetProbability();
        // ����Э���õ��˷�����Ļ����
        //StartCoroutine(MoveOffScreenWithParabola(enemyObj));
    }
    public void GetProbability()
    {
        //TTOD1�޸�ʹ�ñ������
        float probability = (float)(0.1 * (1 + BuffDoorController.Instance.coinFac));
        int randomNum = Random.Range(1, 100);
        Debug.Log(probability * 100 + "��ý�ҵĸ���" + randomNum + "��1-100�����ȡ����===========");
        if(randomNum < probability * 100)
        {
            PlayInforManager.Instance.playInfor.AddCoins(Enemycoins);
            Debug.Log(Enemycoins+ "��ý��!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
        }
    }

    IEnumerator MoveOffScreenWithParabola(GameObject enemyObj)
    {
        // ȷ����Ļ��ȵı߽�
        float screenWidth = Camera.main.orthographicSize * Camera.main.aspect;

        // ��ȡ���˵�ǰλ��
        Vector3 startPosition = transform.position;

        // ������Ļ��ߺ��ұߵ�Ŀ��λ��
        Vector3 leftEdge = new Vector3(-screenWidth - 1f, transform.position.y, transform.position.z);
        Vector3 rightEdge = new Vector3(screenWidth + 1f, transform.position.y, transform.position.z);

        // ѡ�������Ļ����һ�ߣ������
        Vector3 targetPosition = Random.value > 0.5f ? leftEdge : rightEdge;

        // ���������߸߶�
        float height = 2f; // ���ֵ���Ե����Ըı������ߵĸ߶�
        float duration = 1f; // ����ʱ��
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;

            // ����������λ��
            float x = Mathf.Lerp(startPosition.x, targetPosition.x, t);
            float y = Mathf.Lerp(startPosition.y, targetPosition.y, t);
            float parabolaY = Mathf.Lerp(startPosition.y, targetPosition.y, t) + Mathf.Sin(t * Mathf.PI) * height;

            transform.position = new Vector3(x, parabolaY, transform.position.z);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // ȷ��������ȫ�뿪��Ļ�����ٶ���
        transform.position = targetPosition; // ȷ��Ŀ��λ��������λ��
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Shield")) // ��������ֵ�Tag��"Shield"
        {
            StopMovement();
        }
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