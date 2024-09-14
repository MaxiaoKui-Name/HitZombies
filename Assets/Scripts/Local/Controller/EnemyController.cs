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
    public float moveSpeed = 1f;  // �����ƶ��ٶ�
    public float attackRange = 0.05f;  // ���˹�������
    public float detectionRange = 2f;  // ���˿�ʼ������ƶ��ľ���
    public float damage = 10f;  // ���˹���ʱ�������ɵ��˺�
    public float attackCooldown = 1f;  // ������ȴʱ��
    public Vector3 targetScale;
    public float health = 100f;  // ���˵ĳ�ʼѪ��
    private float maxHealth;    // ���˵����Ѫ�������ڼ���Ѫ������
    private Transform HitTarget;  // ��Ҷ��������
    public EnemyType enemyType;
    public int Enemycoins1;
    public int Enemycoins2 = 10;
    //public List<int> coinProbilityList;

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

    public GameMainPanelController gameMainPanelController;
    private Transform coinTargetPos;
    public float probabilityBase; 
    void OnEnable()
    {
        // �ҵ���Ҷ��󣨼�����ҵ�Tag��"HitTarget"��
        HitTarget = GameObject.FindGameObjectWithTag("HitTarget").transform;
        armatureComponent = transform.GetChild(0).GetComponent<UnityArmatureComponent>();
        enemyRenderer = transform.GetChild(0).transform.GetChild(0).GetComponent<MeshRenderer>();
        gameMainPanelController = GameObject.Find("UICanvas/GameMainPanel(Clone)").GetComponent<GameMainPanelController>();
        coinTargetPos = GameObject.Find("CointargetPos").transform;
        // ��ֵ��ʼ��
        Init();
        Enemycoins2 = 10;
        transform.localScale = targetScale;
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
            armatureComponent.animation.Play("walk",-1);
        }

    }

    private void Init()
    {
       // coinProbilityList = new List<int>();
        Enemycoins1 = 0;
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
                damage = ConfigManager.Instance.Tables.TableMonster[1].Atk;
                moveSpeed = ConfigManager.Instance.Tables.TableMonster[1].Spd;
                health = ConfigManager.Instance.Tables.TableMonster[1].Hp;
                probabilityBase = ConfigManager.Instance.Tables.TableMonster[1].MoneyProbability;
                targetScale = Vector3.one * ConfigManager.Instance.Tables.TableMonster[1].Scale;
                //coinProbilityList = ConfigManager.Instance.Tables.TablePhysiqueReslevelConfig.Get(1).CuipiMoney;
                Enemycoins1 = Random.Range(ConfigManager.Instance.Tables.TableMonster[1].MoneyMin, ConfigManager.Instance.Tables.TableMonster[1].MoneyMax);
                break;
            case EnemyType.CuipiMonster2:
                damage = ConfigManager.Instance.Tables.TableMonster[2].Atk;
                moveSpeed = ConfigManager.Instance.Tables.TableMonster[2].Spd;
                health = ConfigManager.Instance.Tables.TableMonster[2].Hp;
                probabilityBase = ConfigManager.Instance.Tables.TableMonster[2].MoneyProbability;
                targetScale = Vector3.one * ConfigManager.Instance.Tables.TableMonster[2].Scale;
                //coinProbilityList = ConfigManager.Instance.Tables.TablePhysiqueReslevelConfig.Get(1).CuipiMoney;
                Enemycoins1 = Random.Range(ConfigManager.Instance.Tables.TableMonster[2].MoneyMin, ConfigManager.Instance.Tables.TableMonster[2].MoneyMax);
                break;
            case EnemyType.ShortMonster:
                damage = ConfigManager.Instance.Tables.TableMonster[3].Atk;
                moveSpeed = ConfigManager.Instance.Tables.TableMonster[3].Spd;
                health = ConfigManager.Instance.Tables.TableMonster[3].Hp;
                probabilityBase = ConfigManager.Instance.Tables.TableMonster[3].MoneyProbability;
                targetScale = Vector3.one * ConfigManager.Instance.Tables.TableMonster[3].Scale;
                // coinProbilityList = ConfigManager.Instance.Tables.TablePhysiqueReslevelConfig.Get(1).JinMoney;
                Enemycoins1 = Random.Range(ConfigManager.Instance.Tables.TableMonster[3].MoneyMin, ConfigManager.Instance.Tables.TableMonster[3].MoneyMax);
                break;
            case EnemyType.DisMonster:
                damage = ConfigManager.Instance.Tables.TablePhysiqueReslevelConfig.Get(1).YuanAtk;
                moveSpeed = ConfigManager.Instance.Tables.TableMonster[4].Spd;
                health = ConfigManager.Instance.Tables.TableMonster[4].Hp;
                probabilityBase = ConfigManager.Instance.Tables.TableMonster[4].MoneyProbability;
                targetScale = Vector3.one * ConfigManager.Instance.Tables.TableMonster[4].Scale;
                //coinProbilityList = ConfigManager.Instance.Tables.TablePhysiqueReslevelConfig.Get(1).YuanMoney;
                Enemycoins1 = Random.Range(ConfigManager.Instance.Tables.TableMonster[4].MoneyMin, ConfigManager.Instance.Tables.TableMonster[4].MoneyMax);
                break;
            case EnemyType.ElitesMonster:
                damage = ConfigManager.Instance.Tables.TableMonster[5].Atk;
                moveSpeed = ConfigManager.Instance.Tables.TableMonster[5].Spd;
                health = ConfigManager.Instance.Tables.TableMonster[5].Hp;
                probabilityBase = ConfigManager.Instance.Tables.TableMonster[5].MoneyProbability;
                targetScale = Vector3.one * ConfigManager.Instance.Tables.TableMonster[100].Scale;
                //coinProbilityList = ConfigManager.Instance.Tables.TablePhysiqueReslevelConfig.Get(1).JingMoney;
                Enemycoins1 = Random.Range(ConfigManager.Instance.Tables.TableMonster[5].MoneyMin, ConfigManager.Instance.Tables.TableMonster[5].MoneyMax);
                break;
            case EnemyType.Boss:
                damage = ConfigManager.Instance.Tables.TableMonster[100].Atk;
                moveSpeed = ConfigManager.Instance.Tables.TableMonster[100].Spd;
                health = ConfigManager.Instance.Tables.TableMonster[100].Hp;
                probabilityBase = ConfigManager.Instance.Tables.TableMonster[100].MoneyProbability;
                //coinProbilityList = ConfigManager.Instance.Tables.TablePhysiqueReslevelConfig.Get(1).BossMoney;
                Enemycoins1 = Random.Range(ConfigManager.Instance.Tables.TableMonster[100].MoneyMin, ConfigManager.Instance.Tables.TableMonster[100].MoneyMax);
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
                    armatureComponent.animation.Play("walk"); // Resume walk animation
                }
            }

            moveSpeed = Mathf.Max(moveSpeed, 0.1f); // Ensure moveSpeed is positive
            transform.position += (HitTarget.position - transform.position).normalized * moveSpeed * Time.deltaTime;
        }
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Shield")) // ��������ֵ�Tag��"Shield"
        {
            StopMovement();
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
            //enemyRenderer.material.SetFloat("_EmissionIntensity", 2f);
            //enemyRenderer.material.SetColor("_EmissionColor", emissionColor);
            // �ȴ�һС��ʱ��
            yield return new WaitForSeconds(0.3f);

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


    public async UniTask Die(GameObject enemyObj)
    {
        if (armatureComponent != null)
        {
            Debug.Log("������������");
            armatureComponent.animation.Play("die");
            //await WaitForAnimationComplete(armatureComponent, "die");
            Vector3 deathPosition = transform.position;
            RecycleEnemy(enemyObj);
            await GetProbability(deathPosition, enemyObj);
        }
        else
        {
            Debug.Log("����Ϊ��");
        }
    }

    private async UniTask WaitForAnimationComplete(UnityArmatureComponent armature, string animationName)
    {
        var tcs = new UniTaskCompletionSource();

        void OnAnimationComplete(string type, EventObject eventObject)
        {
            if (eventObject.animationState.name == animationName)
            {
                armature.RemoveDBEventListener(EventObject.COMPLETE, OnAnimationComplete);
                tcs.TrySetResult();
            }
        }

        armature.AddDBEventListener(EventObject.COMPLETE, OnAnimationComplete);
        await tcs.Task;
    }

    public async UniTask GetProbability(Vector3 deathPosition, GameObject enemyObj)
    {
        float probability = (float)(probabilityBase * (1 + BuffDoorController.Instance.coinFac));
        int randomNum = Random.Range(1, 100);
        Debug.Log(probability * 100 + "��ý�ҵĸ���" + randomNum);

        if (randomNum < probability * 100)
        {
            Debug.Log(Enemycoins1 + "��ý��");
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
                await MoveCoinToUI(coinObj, selectedCoinPool);
            }
        }
    }

    public async UniTask MoveCoinToUI(GameObject coinObj, ObjectPool<GameObject> CoinPool)
    {
        float duration = 0.5f;
        float elapsedTime = 0f;
        Vector3 startPosition = coinObj.transform.position;
        Vector3 targetPosition = coinTargetPos.position;

        if (coinObj == null || !coinObj.activeSelf)
        {
            Debug.LogWarning("coinObj �Ѿ������ջ���ã�");
            return;
        }

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            Vector3 currentPosition = Vector3.Lerp(startPosition, targetPosition, t);
            currentPosition.z = -0.1f;

            if (coinObj == null || !coinObj.activeSelf)
            {
                Debug.LogWarning("coinObj �Ѿ������ջ���ã�");
                return;
            }

            coinObj.transform.position = currentPosition;
            await UniTask.Yield();
        }

        if (coinObj.activeSelf)
        {
            CoinPool.Release(coinObj);
            PlayInforManager.Instance.playInfor.AddCoins(1);
        }
    }

    private void RecycleEnemy(GameObject enemyObj)
    {
        var enemyPool = PreController.Instance.GetEnemyPoolMethod(enemyObj);
        Debug.Log("���˻������");
        enemyPool.Release(enemyObj);
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