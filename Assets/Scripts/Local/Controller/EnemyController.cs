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
    public float moveSpeed = 1f;  // ���˻����ƶ��ٶ�
    public float attackRange = 2f;  // ���˹�������
    public float detectionRange = 3f;  // ���˿�ʼ������ƶ��ľ���
    public float damage = 10f;  // ���˹���ʱ�������ɵ��˺�
    public float attackCooldown = 1f;  // ������ȴʱ��
    public Vector3 targetScale = Vector3.one;  // �����Ŀ���С
    public float health = 100f;  // ���˵ĳ�ʼѪ��
    public float maxHealth;    // ���˵����Ѫ�������ڼ���Ѫ������
    private Transform playerTransform;  // ��Ҷ��������
    public EnemyType enemyType;
    public int Enemycoins1;
    public int Enemycoins2 = 10;

    private Camera mainCamera;

    public Slider healthSlider;  // Ѫ����ʾ��Slider
    public Text CoinText; // ��ʾ����������ı�
    public Transform healthBarCanvas; // Ѫ�����ڵ�Canvas
    public Vector3 addVector = Vector3.zero;
    public Vector3 ScaleVector = Vector3.one;

    public UnityArmatureComponent armatureComponent; // ���ڿ���DragonBones����

    private bool isAttacking = false;  // ��־λ����ʾ�����Ƿ����ڹ���
    public bool hasStartedMovingTowardsPlayer = false; // ��־λ����ʾ�����Ƿ��Ѿ���ʼ������ƶ�
    public Renderer[] enemyRenderers; // ���ڿ��Ʋ�����
    private Color originalEmissionColor; // ���˲��ʵ�ԭʼ������ɫ
    public bool isStopped = false; // �Ƿ�ֹͣ�ƶ�

    public GameMainPanelController gameMainPanelController;
    public float probabilityBase;
    public bool isDead;
    public bool isFrozen;
    public bool isVise;

    public float hideYPosition = -10f; // ������Ļ��Y����
    public bool isSpecialHealth = false;
    void OnEnable()
    {
        // �ҵ���Ҷ��󣨼�����ҵ�Tag��"Player"��
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

        // ��ȡ�������
        mainCamera = Camera.main;
        probabilityBase = 0;

        // ��ֵ��ʼ��
        Init();
        // �����������ֵ
        Enemycoins2 = 5;
        transform.localScale = targetScale;

        hasStartedMovingTowardsPlayer = false; // ��ʼ��Ϊfalse

        StartCoroutine(Start1());
    }

    void Start()
    {
        EventDispatcher.instance.Regist(EventNameDef.GAME_OVER, (v) => RecycleEnemy(gameObject));
        if (isSpecialHealth)
        {
            health = 1000000000f;
        }
        maxHealth = health;
        // ��ʼ��Ѫ��UI
        if (healthSlider != null)
        {
            healthSlider.minValue = 0;
            healthSlider.maxValue = maxHealth;
            healthSlider.value = health;
            healthSlider.gameObject.SetActive(true);
        }
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
        Enemycoins1 = 0;
        moveSpeed = 1f; // ��ʼ���ƶ��ٶ�
        health = 100f; // ��ʼ��Ѫ��
        damage = 10f; // ��ʼ���˺�
        attackRange = 1f;
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
            case EnemyType.NormalMonster:
                addVector.y = 0.7f;
                ScaleVector = new Vector3(0.01f, 0.01f, 0.01f);
                damage = ConfigManager.Instance.Tables.TableMonsterConfig[1].Atk;
                moveSpeed = ConfigManager.Instance.Tables.TableMonsterConfig[1].Spd;
                health = ConfigManager.Instance.Tables.TableMonsterConfig[1].Hp * ConfigManager.Instance.Tables.TableDanConfig[GameFlowManager.Instance.currentLevelIndex].BloodCoefficient;// ConfigManager.Instance.Tables.TableDanConfig[GameFlowManager.Instance.currentLevelIndex == 0 ? +1 : GameFlowManager.Instance.currentLevelIndex].BloodCoefficient;
                probabilityBase = ConfigManager.Instance.Tables.TableMonsterConfig[1].MoneyProbability * ConfigManager.Instance.Tables.TableDanConfig[GameFlowManager.Instance.currentLevelIndex].CoinProbabilityCoefficient;
                targetScale = transform.localScale * ConfigManager.Instance.Tables.TableMonsterConfig[1].Scale;
                //coinProbilityList = ConfigManager.Instance.Tables.TablePhysiqueReslevelConfig.Get(1).CuipiMoney;
                Enemycoins1 = (int)(Random.Range(ConfigManager.Instance.Tables.TableMonsterConfig[1].MoneyMin, ConfigManager.Instance.Tables.TableMonsterConfig[1].MoneyMax) * ConfigManager.Instance.Tables.TableDanConfig[GameFlowManager.Instance.currentLevelIndex].CoinNumberCoefficient); //ConfigManager.Instance.Tables.TableDanConfig[GameFlowManager.Instance.currentLevelIndex == 0 ? +1 : GameFlowManager.Instance.currentLevelIndex].CoinNumberCoefficient);
                break;
            case EnemyType.BasketMonster:
                addVector.y = 0.7f;
                ScaleVector = new Vector3(0.01f, 0.01f, 0.01f);
                damage = ConfigManager.Instance.Tables.TableMonsterConfig[2].Atk;
                moveSpeed = ConfigManager.Instance.Tables.TableMonsterConfig[2].Spd;
                health = ConfigManager.Instance.Tables.TableMonsterConfig[2].Hp * ConfigManager.Instance.Tables.TableDanConfig[GameFlowManager.Instance.currentLevelIndex].BloodCoefficient;// ConfigManager.Instance.Tables.TableDanConfig[GameFlowManager.Instance.currentLevelIndex == 0 ? +1 : GameFlowManager.Instance.currentLevelIndex].BloodCoefficient;
                probabilityBase = ConfigManager.Instance.Tables.TableMonsterConfig[2].MoneyProbability * ConfigManager.Instance.Tables.TableDanConfig[GameFlowManager.Instance.currentLevelIndex].CoinProbabilityCoefficient;
                targetScale = transform.localScale * ConfigManager.Instance.Tables.TableMonsterConfig[2].Scale;
                //coinProbilityList = ConfigManager.Instance.Tables.TablePhysiqueReslevelConfig.Get(1).CuipiMoney;
                Enemycoins1 = (int)(Random.Range(ConfigManager.Instance.Tables.TableMonsterConfig[2].MoneyMin, ConfigManager.Instance.Tables.TableMonsterConfig[2].MoneyMax) * ConfigManager.Instance.Tables.TableDanConfig[GameFlowManager.Instance.currentLevelIndex].CoinNumberCoefficient); //ConfigManager.Instance.Tables.TableDanConfig[GameFlowManager.Instance.currentLevelIndex == 0 ? +1 : GameFlowManager.Instance.currentLevelIndex].CoinNumberCoefficient);
                break;
            case EnemyType.SteelMonster:
                addVector.y = 2.4f;
                ScaleVector = new Vector3(0.01f, 0.01f, 0.01f);
                damage = ConfigManager.Instance.Tables.TableMonsterConfig[3].Atk;
                moveSpeed = ConfigManager.Instance.Tables.TableMonsterConfig[3].Spd;
                health = ConfigManager.Instance.Tables.TableMonsterConfig[3].Hp * ConfigManager.Instance.Tables.TableDanConfig[GameFlowManager.Instance.currentLevelIndex].BloodCoefficient;// ConfigManager.Instance.Tables.TableDanConfig[GameFlowManager.Instance.currentLevelIndex == 0 ? +1 : GameFlowManager.Instance.currentLevelIndex].BloodCoefficient;
                probabilityBase = ConfigManager.Instance.Tables.TableMonsterConfig[3].MoneyProbability * ConfigManager.Instance.Tables.TableDanConfig[GameFlowManager.Instance.currentLevelIndex].CoinProbabilityCoefficient;
                targetScale = transform.localScale * ConfigManager.Instance.Tables.TableMonsterConfig[3].Scale;
                // coinProbilityList = ConfigManager.Instance.Tables.TablePhysiqueReslevelConfig.Get(1).JinMoney;
                Enemycoins1 = (int)(Random.Range(ConfigManager.Instance.Tables.TableMonsterConfig[3].MoneyMin, ConfigManager.Instance.Tables.TableMonsterConfig[3].MoneyMax) * ConfigManager.Instance.Tables.TableDanConfig[GameFlowManager.Instance.currentLevelIndex].CoinNumberCoefficient); //ConfigManager.Instance.Tables.TableDanConfig[GameFlowManager.Instance.currentLevelIndex == 0 ? +1 : GameFlowManager.Instance.currentLevelIndex].CoinNumberCoefficient);
                break;
            case EnemyType.HulkMonster:
                addVector.y = 0.7f;
                ScaleVector = new Vector3(0.007f, 0.007f, 0.007f); ;
                damage = ConfigManager.Instance.Tables.TableMonsterConfig.Get(4).Atk;
                moveSpeed = ConfigManager.Instance.Tables.TableMonsterConfig[4].Spd;
                health = ConfigManager.Instance.Tables.TableMonsterConfig[4].Hp * ConfigManager.Instance.Tables.TableDanConfig[GameFlowManager.Instance.currentLevelIndex].BloodCoefficient;// ConfigManager.Instance.Tables.TableDanConfig[GameFlowManager.Instance.currentLevelIndex == 0 ? +1 : GameFlowManager.Instance.currentLevelIndex].BloodCoefficient;
                probabilityBase = ConfigManager.Instance.Tables.TableMonsterConfig[4].MoneyProbability * ConfigManager.Instance.Tables.TableDanConfig[GameFlowManager.Instance.currentLevelIndex].CoinProbabilityCoefficient;
                targetScale = transform.localScale * ConfigManager.Instance.Tables.TableMonsterConfig[4].Scale;
                //coinProbilityList = ConfigManager.Instance.Tables.TablePhysiqueReslevelConfig.Get(1).YuanMoney;
                Enemycoins1 = (int)(Random.Range(ConfigManager.Instance.Tables.TableMonsterConfig[4].MoneyMin, ConfigManager.Instance.Tables.TableMonsterConfig[4].MoneyMax) * ConfigManager.Instance.Tables.TableDanConfig[GameFlowManager.Instance.currentLevelIndex].CoinNumberCoefficient); //ConfigManager.Instance.Tables.TableDanConfig[GameFlowManager.Instance.currentLevelIndex == 0 ? +1 : GameFlowManager.Instance.currentLevelIndex].CoinNumberCoefficient);
                break;
            case EnemyType.Boss:
                addVector.y = 2f;
                ScaleVector = new Vector3(0.01f, 0.01f, 0.01f);
                damage = ConfigManager.Instance.Tables.TableMonsterConfig[100].Atk;
                moveSpeed = ConfigManager.Instance.Tables.TableMonsterConfig[100].Spd;
                health = ConfigManager.Instance.Tables.TableMonsterConfig[100].Hp * ConfigManager.Instance.Tables.TableDanConfig[GameFlowManager.Instance.currentLevelIndex].BloodCoefficient;// ConfigManager.Instance.Tables.TableDanConfig[GameFlowManager.Instance.currentLevelIndex == 0 ? +1 : GameFlowManager.Instance.currentLevelIndex].BloodCoefficient;
                probabilityBase = ConfigManager.Instance.Tables.TableMonsterConfig[100].MoneyProbability * ConfigManager.Instance.Tables.TableDanConfig[GameFlowManager.Instance.currentLevelIndex].CoinProbabilityCoefficient;
                targetScale = transform.localScale * ConfigManager.Instance.Tables.TableMonsterConfig[4].Scale;
                //coinProbilityList = ConfigManager.Instance.Tables.TablePhysiqueReslevelConfig.Get(1).BossMoney;
                Enemycoins1 = (int)(Random.Range(ConfigManager.Instance.Tables.TableMonsterConfig[100].MoneyMin, ConfigManager.Instance.Tables.TableMonsterConfig[100].MoneyMax) * ConfigManager.Instance.Tables.TableDanConfig[GameFlowManager.Instance.currentLevelIndex].CoinNumberCoefficient); //ConfigManager.Instance.Tables.TableDanConfig[GameFlowManager.Instance.currentLevelIndex == 0 ? +1 : GameFlowManager.Instance.currentLevelIndex].CoinNumberCoefficient);
                break;
        }
    }


    void Update()
    {
        if (isFrozen || GameManage.Instance.isFrozen)
        {
            return; // ����ʱ��ִ���κ��߼�
        }
        if (GameManage.Instance.gameState != GameState.Running)
        {
            if(gameObject.name != "Boss(Clone)")
               RecycleEnemy(gameObject);
            else
                Destroy(gameObject);
            return; // ��Ϸδ����ʱ��ִ���κ��߼�
        }


        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        // ����Ƿ�ʼ������ƶ�
        if (!hasStartedMovingTowardsPlayer && distanceToPlayer <= detectionRange)
        {
            hasStartedMovingTowardsPlayer = true;
        }

        if (hasStartedMovingTowardsPlayer)
        {
            // ������ƶ�
            MoveTowardsPlayer();
        }
        else
        {
            // �����ƶ�
            MoveDownward();
        }

        if (transform.position.y < hideYPosition)
        {
            isStopped = true;
            RecycleEnemy(gameObject);
        }
        UpdateHealthBarPosition(); // ÿ֡����Ѫ��λ��
    }

    void MoveDownward()
    {
        // ������ moveSpeed �����ƶ�
        transform.Translate(Vector3.down * moveSpeed * Time.deltaTime, Space.World);
    }

    void MoveTowardsPlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        //if (distanceToPlayer <= attackRange)
        //{
        //    if (!isAttacking && armatureComponent != null)
        //    {
        //        armatureComponent.animation.Play("hit", -1); // �ظ�����"hit"����
        //        isAttacking = true;
        //    }
        //}
        // ������ƶ�
        transform.Translate((playerTransform.position - transform.position).normalized * moveSpeed * Time.deltaTime, Space.World);

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
        health -= damageAmount;
        health = Mathf.Max(health, 0);
        if (health <= 0 && !isDead)
        {
            isDead = true;
            PreController.Instance.DecrementActiveEnemy();
            if (gameObject.activeSelf)
            {
                StartCoroutine(FlashEmission(enemyObj)); // ִ�з���Ч��
            }
        }
        if (healthSlider != null)
        {
            UpdateHealthUI();
        }
    }

    // ����Ѫ��UI
    void UpdateHealthUI()
    {
        if (healthSlider != null)
        {
            healthSlider.value = Mathf.Max(health, 0);
        }
    }

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
            await PlayAndWaitForAnimation(armatureComponent, "die", 1);  // ����һ��"die"����
            Vector3 deathPosition = transform.position;
            if (enemyObj.activeSelf)
            {
                if (healthSlider != null)
                {
                    healthSlider.gameObject.SetActive(false);
                }
                await GetProbability(deathPosition, enemyObj);
                RecycleEnemy(enemyObj);
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
        if (isSpecialHealth)
        {
            probabilityBase = 100;
            Enemycoins1 = 50000;
        }
        float probability = probabilityBase;
        int randomNum = Random.Range(1, 100);
        if (randomNum > (100 - probability))
        {
            await SpawnAndMoveCoins(Enemycoins2, deathPosition, enemyObj);
            if(GameFlowManager.Instance.currentLevelIndex == 0 && isSpecialHealth)
            {
                PlayInforManager.Instance.playInfor.AddCoins(Enemycoins1 - Enemycoins2);
                CoinText.gameObject.SetActive(true);
                CoinText.text = $"+{FormatCoinCount((long)Enemycoins1)}";
                await UniTask.Delay(3000);
                CoinText.gameObject.SetActive(false);
            }
            else
            {
                // ���� coinFac
                float totalCoins = Enemycoins1 * (PlayInforManager.Instance.playInfor.coinFac > 0 ? PlayInforManager.Instance.playInfor.coinFac : 1);
                Debug.Log("δ����֮ǰ��ҵ�ֵ" + (Enemycoins1 - Enemycoins2) + "totalCoins��ֵ" + totalCoins + "==========��ҷ�����ֵ" + PlayInforManager.Instance.playInfor.coinFac);
                PlayInforManager.Instance.playInfor.AddCoins((int)(totalCoins - Enemycoins2));
                CoinText.gameObject.SetActive(true);
                CoinText.text = $"+{FormatCoinCount((long)totalCoins)}";
                await UniTask.Delay(3000);
                CoinText.gameObject.SetActive(false);

            }
        }
    }
    public async UniTask SpawnAndMoveCoins(int coinCount, Vector3 deathPosition, GameObject enemyObj)
    {
        for (int i = 1; i <= coinCount; i++)
        {
            string CoinName = "NewGold"; // ȷ��Ԥ����������ȷ
            if (PreController.Instance.CoinPools.TryGetValue(CoinName, out var selectedCoinPool))
            {
                GameObject coinObj = selectedCoinPool.Get();
                coinObj.SetActive(true);

                // ��ȡ Canvas �� RectTransform
                RectTransform canvasRect = gameMainPanelController.canvasRectTransform;

                // ������λ�ô���������ת��Ϊ��Ļ����
                Vector3 screenPos = Camera.main.WorldToScreenPoint(deathPosition);

                // ����Ļ����ת��Ϊ Canvas �ı�������
                Vector2 localPos;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, null, out localPos);
                int randomSide = Random.Range(0, 2);
                float xOffset = randomSide == 0 ? Random.Range(-40f, -20f) : Random.Range(20f, 40f);
                // ����������Ϸ�Ŀ��λ�ã�y��ƫ��0��30
                Vector2 targetPos = localPos + new Vector2(xOffset, Random.Range(80f, 130f));
                // ���ý�ҵ� RectTransform ��ê��λ��Ϊ��ʼλ��
                RectTransform coinRect = coinObj.GetComponent<RectTransform>();
                coinRect.anchoredPosition = localPos;

                // ��ȡ Gold ����������ƶ��Ͷ����߼�
                Gold gold = coinObj.GetComponent<Gold>();
                if (gold != null)
                {
                    // ���� Gold �ű��еķ��������ݳ�ʼλ�á�Ŀ��λ�ú�UIĿ��λ��
                    gold.InitializeCoin(
                        selectedCoinPool,
                        localPos,
                        targetPos,
                        gameMainPanelController.coinspattern_F.GetComponent<RectTransform>().anchoredPosition
                    ,transform.gameObject);
                }
            }
            // �ȴ�0.02������������һ�����
            await UniTask.Delay(TimeSpan.FromSeconds(0.02f));
        }
    }


    public void RecycleEnemy(GameObject enemyObj)
    {
        if (enemyObj != null && enemyObj.activeSelf)
        {
            var enemyPool = PreController.Instance.GetEnemyPoolMethod(enemyObj);
            GameManage.Instance.KilledMonsterNun++;
            isSpecialHealth = false;
            isVise = false;
            enemyObj.SetActive(false);
            if (enemyObj != null && enemyObj.activeSelf)
                enemyPool.Release(enemyObj);
        }
    }

    private void StopMovement()
    {
        isStopped = true;
        moveSpeed = 0f;
        if (armatureComponent != null)
        {
            if (enemyType == EnemyType.SteelMonster)
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
