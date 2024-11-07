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
    private Collider2D collider;

    private Camera mainCamera;
    //public List<int> coinProbilityList;

    public Slider healthSlider;  // Ѫ����ʾ��Slider
    public Image redImage;  // ������ʾ��ǰѪ���ĺ�ɫͼƬ
    public Image blackBackground; // ��ɫ����
    public Text CoinText; // Ѫ�����ڵ�Canvas
    public Transform healthBarCanvas; // Ѫ�����ڵ�Canvas
    public Vector3 addVector = Vector3.zero;
    public Vector3 ScaleVector;

    public UnityArmatureComponent armatureComponent; // ���ڿ���DragonBones����

    private bool isAttacking = false;  // ��־λ����ʾ�����Ƿ����ڹ���
    public bool hasStartedMovingTowardsPlayer = false; // ��־λ����ʾ�����Ƿ��Ѿ���ʼ������ƶ�
    public Renderer[] enemyRenderers; // ���ڿ��Ʋ�����
    //public Color emissionColor = new Color(255, 0, 0); // �����ܻ�ʱ�������ɫ
    private Color originalEmissionColor; // ���˲��ʵ�ԭʼ������ɫ
    public bool isStopped = false; // �Ƿ�ֹͣ�ƶ�

    public GameMainPanelController gameMainPanelController;
    //private Transform coinTargetPos;
    public float probabilityBase;
    public bool isDead;
    public bool isVise;
    public bool isFrozen;
    public float hideYPosition = -10f; // ������Ļ��Y����

    void OnEnable()
    {

        // �ҵ���Ҷ��󣨼�����ҵ�Tag��"HitTarget"��
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
        // ��ȡ�������
        mainCamera = Camera.main;
        probabilityBase = 0;
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

        StartCoroutine(Start1());
    }
    void Start()
    {
        EventDispatcher.instance.Regist(EventNameDef.GAME_OVER, (v) => RecycleEnemy(gameObject));
    }

    IEnumerator Start1()
    {
        yield return new WaitForSeconds(1f); // �ȴ�1�룬ȷ����������ѳ�ʼ��

        // �������䶯��
        if (armatureComponent != null)
        {
            armatureComponent.animation.Play("walk", -1);
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
        // �����������ֵ
        maxHealth = health;
    }

    void Update()
    {
        if (isFrozen || GameManage.Instance.isFrozen)
        {
            return; // ����ʱ��ִ���κ��߼�
        }
        if (GameManage.Instance.gameState != GameState.Running)
        {
            RecycleEnemy(gameObject);
            return; // ����ʱ��ִ���κ��߼�
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
                    armatureComponent.animation.Play("walk",-1); // Resume walk animation
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
           
            healthBarCanvas.position = transform.position + addVector;
            healthBarCanvas.localScale = ScaleVector;
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
        StartCoroutine(FlashEmission(enemyObj)); // ִ�з���Ч��
    }
    public bool IsEnemyOnScreen(GameObject enemy)
    {
        Vector3 viewportPoint = mainCamera.WorldToViewportPoint(enemy.transform.position);
        // �������Ƿ�����Ļ�Ŀɼ���Χ�ڣ�0��1�ķ�ΧΪ��Ļ�ڣ�Y��Ϊ1��ʾ��Ļ������
        return viewportPoint.x >= 0 && viewportPoint.x <= 1 && viewportPoint.y >= 0 && viewportPoint.y <= 1;
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
        for (int i = 0; i < enemyRenderers.Length; i++)
        {
            if (enemyRenderers[i] != null && enemyRenderers[i].material.HasProperty("_EmissionColor"))
            {
                // ���÷���Ч��
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
            Die(enemyObj); // ��������
        }
    }


    public async UniTask Die(GameObject enemyObj)
    {
        if (armatureComponent != null)
        {
            Debug.Log("������������");
            if(enemyObj.name != "zombieelite_005(Clone)")
            {
                await PlayAndWaitForAnimation(armatureComponent, "die", 1);  // ����һ��hit����
               
            }
            Vector3 deathPosition = transform.position;
            if (enemyObj.activeSelf)
            {
                RecycleEnemy(enemyObj);
                await GetProbability(deathPosition, enemyObj);
                // ���ٻ�Ծ��������
            }
        }
        else
        {
            Debug.Log("����Ϊ��");
        }
    }

    // �ȴ������������
    private async UniTask PlayAndWaitForAnimation(UnityArmatureComponent armature, string animationName, int playTimes = 1)
    {
        var tcs = new UniTaskCompletionSource();

        // �����¼��������
        void OnAnimationComplete(string type, EventObject eventObject)
        {
            if (eventObject.animationState.name == animationName)
            {
                armature.RemoveDBEventListener(EventObject.COMPLETE, OnAnimationComplete);  // �Ƴ�������
                tcs.TrySetResult(); // �������
            }
        }

        // ����¼�������
        armature.AddDBEventListener(EventObject.COMPLETE, OnAnimationComplete);

        // ����ָ����������ָ�����Ŵ���
        armature.animation.Play(animationName, playTimes);

        // �ȴ��������
        await tcs.Task;
        if(animationName == "die")
        {
            CoinText.gameObject.SetActive(true);
            CoinText.text = $"+{FormatCoinCount(Enemycoins1)}";
            await UniTask.Delay(200);
            CoinText.gameObject.SetActive(false);
        }
    }
    // ��ʽ���������
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
    //        Debug.LogWarning("coinObj �Ѿ������ջ���ã�");
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
    //            Debug.LogWarning("coinObj �Ѿ������ջ���ã�");
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
            Debug.Log("���˻������");
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