using UnityEngine;
using UnityEngine.UI;
using DragonBones;
using Transform = UnityEngine.Transform;
using TMPro;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    // ����ƶ����
    public float moveSpeed = 2f;               // ��������ƶ��ٶ�
    public float leftBoundary = -1.5f;         // ��߽�����
    public float rightBoundary = 1.5f;         // �ұ߽�����

    // Ѫ�����
    public float currentValue;                 // ��ǰѪ��
    public float MaxValue;                     // ���Ѫ��
    public Slider healthSlider;                // Ѫ����ʾ��Slider
    public Transform healthBarCanvas;          // Ѫ�����ڵ�Canvas (World Space Canvas)

    // UI �ı����
    public Text DeCoinMonText;      // ��Ҽ����ı�
    public Text BuffText;           // Buff�ı�

    // �ӵ������
    public Transform firePoint;


    // �������֣������˲���ʾ DieImg_F
    [Header("���˼�����")]
    public Vector2 detectionAreaSize = new Vector2(10f, 6f); // ��������С������


    // Buff�������
    public float buffAnimationDuration = 0.5f; // ��������ʱ�䣨�룩
    public Vector3 buffStartScale = Vector3.zero; // Buff�ı���ʼ����
    public Vector3 buffEndScale = Vector3.one;    // Buff�ı���������
    public float buffDisplayDuration = 2f;        // Buff�ı���ʾ����ʱ�䣨�룩

    // �ڲ�����
    private Camera mainCamera;                         // �������
    public UnityArmatureComponent armatureComponent;   // DragonBones Armature ���

    // �����������
    private float touchStartX;      // ������ʼ��X����
    private float touchDeltaX;      // �����ƶ���Xƫ����
    public bool isTouching = false; // ��ǰ�Ƿ��д���

    public GameMainPanelController gameMainPanelController; // ���� GameMainPanelController


    private float longPressDuration = 1f; // ��������ʱ��
    private float pressTimer = 0f;
    private bool isLongPress = false;
    public GameObject chooseGunPanel;

    // �����������
    private Vector2 touchStartPos;      // ������ʼ��λ��
    private Vector2 touchCurrentPos;    // ��ǰ������λ��
    private float swipeThreshold = 50f; // �жϻ�������ֵ
    private void Start()
    {
        // ��ʼ�����Ѫ����Ѫ��
        Init();
        //TTPD1������ǹ�л������߼�
        ReplaceGunDragon();

    }

    public void Init()
    {
        // ��ʼ�������
        mainCamera = Camera.main;
        isTouching = false;
        // ��ʼ��Ѫ��
        currentValue = 10;// PlayInforManager.Instance.playInfor.health;
        MaxValue = 10;// PlayInforManager.Instance.playInfor.health;
        healthSlider.maxValue = MaxValue;
        healthSlider.value = currentValue;

        // ��ʼ���ƶ��ٶ�
        moveSpeed = ConfigManager.Instance.Tables.TableGlobal.Get(6).IntValue;
        // ��ȡ BuffText ���������Ϊ���غ�����Ϊ��
        DeCoinMonText = transform.Find("PlaySliderCav/DecoinMonText").GetComponent<Text>();
        DeCoinMonText.gameObject.SetActive(false);
        BuffText = transform.Find("PlaySliderCav/BuffText").GetComponent<Text>();
        BuffText.gameObject.SetActive(false);
        BuffText.transform.localScale = buffStartScale;
        // ��ȡ DragonBones Armature ���
        armatureComponent = GameObject.Find("Player/player1").GetComponent<UnityArmatureComponent>();
        transform.Find("cover").GetComponent<Collider2D>().isTrigger = false; // ��ȡ��ײ�����
        transform.GetComponent<Collider2D>().isTrigger = false;// ��ȡ��ײ�����
        // ���Ų�ѭ��ָ���Ķ���
        if (armatureComponent != null)
        {
            PlayDragonAnimation();
        }
        PreController.Instance.OnPlayerFiring += UpdatePlayerAnimation; // ���¼�������
        // ע���¼�������
        EventDispatcher.instance.Regist(EventNameDef.ShowBuyBulletText, (v) => ShowDeclineMoney());
        //HandleTouchInput();

    }

    public void ReplaceGunDragon()
    {
        // ���浱ǰ�Ķ���״̬�������Ҫ��
        string currentAnimation = armatureComponent?.animation?.lastAnimationName;
        string newArmatureName = PlayInforManager.Instance.playInfor.currentGun.gunName;
        // �ͷŵ�ǰ�� armature
        if (armatureComponent != null)
        {
            armatureComponent.armature.Dispose();
        }
        // ʹ���µ� armatureName ���¹����Ǽ�
        armatureComponent = UnityFactory.factory.BuildArmatureComponent(newArmatureName.Substring(0,7), "player", transform.Find("player1").gameObject.name);
        armatureComponent.transform.gameObject.name = "player1";
        armatureComponent.transform.parent = this.transform;
        armatureComponent.transform.localPosition = new Vector3(-0.037f, -0.226f, 0);
        armatureComponent.transform.localScale = Vector3.one;

        // ��� armatureComponent �Ƿ�ɹ�����
        if (armatureComponent != null)
        {
            // �ָ�֮ǰ�Ķ���״̬���򲥷��¶���
            if (!string.IsNullOrEmpty(currentAnimation) && armatureComponent.animation.HasAnimation(currentAnimation))
            {
                armatureComponent.animation.Play(currentAnimation);
            }
            else
            {
                armatureComponent.animation.Play("walk"); // ���û�б���Ķ���������Ĭ�϶���
            }
        }
        else
        {
            Debug.LogError("Failed to create armatureComponent for: " + newArmatureName);
            return; // �������ʧ�ܣ���ǰ����
        }
    }

    void Update()
    {
        // �����Ϸ״̬���������У���������
        if (GameManage.Instance.gameState != GameState.Running || Time.timeScale == 0)
            return;
        // ʹ����������������ƶ�
        HandleTouchInput();
        // ����Ѫ����λ�ã�ʹ���������ƶ�
        UpdateHealthBarPosition();

        // �������֣������˲���ʾ/���� DieImg_F
        CheckEnemiesInDetectionArea();
        //HandleInput();

    }
    /// <summary>
    /// �����������Կ�������ƶ�
    /// </summary>
    private void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    isTouching = true;
                    touchStartPos = touch.position;
                    pressTimer = 0f;
                    isLongPress = false;
                    break;

                case TouchPhase.Moved:
                    if (isTouching)
                    {
                        Vector2 delta = touch.deltaPosition;

                        // �ж��Ƿ�Ϊ����
                        if (delta.magnitude > swipeThreshold && !isLongPress)
                        {
                            float deltaX = delta.x / Screen.width * 10; ; // ���������������ƶ��ٶȵĹ�ϵ
                            float newX = Mathf.Clamp(transform.position.x + deltaX, leftBoundary, rightBoundary);
                            transform.position = new Vector3(newX, transform.position.y, 0);

                            // ���ﲻ��Ҫ���� touchStartPos����Ϊʹ�õ��� deltaPosition
                        }

                        // �������߼�
                        pressTimer += Time.deltaTime;
                        if (pressTimer >= longPressDuration && !isLongPress)
                        {
                            isLongPress = true;
                            //OnLongPress();
                        }
                    }
                    break;

                case TouchPhase.Stationary:
                    if (isTouching)
                    {
                        // �������߼�
                        pressTimer += Time.deltaTime;
                        if (pressTimer >= longPressDuration && !isLongPress)
                        {
                            isLongPress = true;
                            //OnLongPress();
                        }
                    }
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    isTouching = false;
                    break;
            }
        }
    }



    //������ǹ����
    //private void HandleInput()
    //{
    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        pressTimer = 0f;
    //        isLongPress = true;
    //    }
    //    if (Input.GetMouseButton(0))
    //    {
    //        pressTimer += Time.deltaTime;
    //        if (isLongPress && pressTimer >= longPressDuration)
    //        {
    //            isLongPress = false;
    //            // ����жϣ�ȷ����ǰû�д򿪵�chooseGunPanel
    //            if (chooseGunPanel == null || !chooseGunPanel.activeSelf)
    //            {
    //                OnPlayerLongPressed();
    //            }
    //        }
    //    }
    //    if (Input.GetMouseButtonUp(0))
    //    {
    //        isLongPress = false;
    //    }
    //}

    private void OnPlayerLongPressed()
    {
        // ��ʾChooseGunPanelUI
        // ʵ�����������
        chooseGunPanel = Instantiate(Resources.Load<GameObject>("Prefabs/UIPannel/ChooseGunPanelNew"));
        chooseGunPanel.transform.SetParent(GameObject.Find("UICanvas").transform, false);
        chooseGunPanel.transform.localPosition = Vector3.zero;
        ChooseGunPanelController chooseGunPanelController = chooseGunPanel.GetComponent<ChooseGunPanelController>();
        chooseGunPanelController.ShowChooseGunPanel(false); // ������ʾ���ǵ�һ��
    }

    // �������������ָ���������Ƿ��е���
    private void CheckEnemiesInDetectionArea()
    {
        // �������֣����� GameMainPanelController ʵ��
        if (SceneManager.GetActiveScene().name == "First")
        {
            gameMainPanelController = FindObjectOfType<GameMainPanelController>();
        }
        if (gameMainPanelController == null)
            return;

        Vector2 playerPosition = transform.position;
        // ʹ�� OverlapBoxAll ���ָ�������ڵĵ���
        int LayerEnemy = LayerMask.NameToLayer("Enemy");
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(playerPosition, detectionAreaSize,LayerEnemy);
        bool hasEnemies = false;
        foreach (Collider2D collider in hitColliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                hasEnemies = true;
                break;
            }
        }

        if (!hasEnemies)
        {
            // û�е��ˣ�����ʾ DieImg_F
            if (gameMainPanelController.DieImg_F != null && gameMainPanelController.DieImg_F.gameObject.activeSelf)
            {
                gameMainPanelController.DieImg_F.gameObject.SetActive(false);
                Debug.Log("��⵽�޵��ˣ����� DieImg_F");
            }
        }
        else
        {
            // �е��ˣ���ʾ DieImg_F
            if (gameMainPanelController.DieImg_F != null && !gameMainPanelController.DieImg_F.gameObject.activeSelf)
            {
                gameMainPanelController.DieImg_F.gameObject.SetActive(true);
                Debug.Log("��⵽�е��ˣ���ʾ DieImg_F");
            }
        }
    }

    // ���ӻ�������򣬷������
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, detectionAreaSize);
    }
    // ����������������Ҷ���
    void UpdatePlayerAnimation()
    {
        if (PreController.Instance.isFiring)
        {
            if (armatureComponent.animation.lastAnimationName != "walk+hit")
            {
                armatureComponent.animation.Play("walk+hit", 0);  // �����ӵ�ʱ���Ź�������
            }
        }
        else
        {
            if (armatureComponent.animation.lastAnimationName != "walk")
            {
                armatureComponent.animation.Play("walk", 0);  // �������ӵ�ʱ�������߶���
            }
        }
    }
    // ʹ�����X��λ�ÿ�����������ƶ�
    //void ControlMovementWithMouse()
    //{
    //    // ��ȡ�������Ļ�ϵ�X��λ��
    //    Vector3 mousePosition = Input.mousePosition;

    //    // ����Ļ����ת��Ϊ��������
    //    // ����Zֵ����ҵ�Zֵ��ͬ����ȷ��ת����ȷ
    //    mousePosition.z = mainCamera.WorldToScreenPoint(transform.position).z;
    //    Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);

    //    // ��ʹ��X���λ�ø������λ��
    //    Vector3 newPosition = new Vector3(worldPosition.x, transform.position.y, transform.position.z);

    //    // ��������ƶ���Χ�����ұ߽�֮��
    //    newPosition.x = Mathf.Clamp(newPosition.x, leftBoundary, rightBoundary);

    //    // ������ҵ�λ��
    //    transform.position = newPosition;
    //}
    //�ֿ�������ƶ�
    // ʹ�ô���������������ƶ�
    //void ControlMovementWithMouse()
    //{
    //    isTouching = false;
    //    if (Input.touchCount > 0)
    //    {
    //        Touch touch = Input.GetTouch(0);

    //        switch (touch.phase)
    //        {
    //            case TouchPhase.Began:
    //                isTouching = true;
    //                touchStartX = touch.position.x;
    //                break;

    //            case TouchPhase.Moved:
    //            case TouchPhase.Stationary:
    //                if (isTouching)
    //                {
    //                    touchDeltaX = touch.position.x - touchStartX;
    //                    float screenWidth = Screen.width;
    //                    // ���㴥��ƫ�����ı���
    //                    float deltaX = (touchDeltaX / screenWidth) * 2f; // �����ƶ�������
    //                    // �����µ�Xλ��
    //                    float newX = Mathf.Clamp(transform.position.x + deltaX * moveSpeed * Time.deltaTime, leftBoundary, rightBoundary);
    //                    transform.position = new Vector3(newX, transform.position.y, transform.position.z);
    //                    touchStartX = touch.position.x; // ������ʼ����ʵ�������ƶ�
    //                }
    //                break;

    //            case TouchPhase.Ended:
    //            case TouchPhase.Canceled:
    //                isTouching = false;
    //                break;
    //        }
    //    }
    //}


    // ���Ų�ѭ��"DragonAnimation"
    void PlayDragonAnimation()
    {
        if (armatureComponent != null)
        {
            armatureComponent.animation.Play("walk", 0);  // ����ѭ������
        }
    }

    // ����Ѫ����λ�ã�ȷ�����������
    void UpdateHealthBarPosition()
    {
        if (healthBarCanvas != null)
        {
            // ��Ѫ������Ϊ���ͷ����λ�� (��������)
            healthBarCanvas.position = transform.position + new Vector3(0, 1.6f, 0);  // Y���ƫ����
            healthBarCanvas.localScale = new Vector3(0.01f, 0.01f, 0.01f);  // ����Ѫ��������
        }
    }

    // ��ʾ��Ҽ����ı�
    async void ShowDeclineMoney()
    {
        if (DeCoinMonText != null)
        {
            if (PreController.Instance.isBulletCostZero)
            {
                DeCoinMonText.text = $"-{0}";
            }
            else
            {
                DeCoinMonText.text = $"-{ConfigManager.Instance.Tables.TablePlayerConfig.Get(GameFlowManager.Instance.currentLevelIndex).Total}";
            }
            await ShowDeCoinMonText();
        }
    }

    // ��ʾ�����ؽ�Ҽ����ı�
    private async UniTask ShowDeCoinMonText()
    {
        DeCoinMonText.gameObject.SetActive(true); // ��ʾ�ı�
        await UniTask.Delay(1000);                  // �ȴ�0.5��
        DeCoinMonText.gameObject.SetActive(false); // �����ı�
    }

    // ��������ܵ��˺�
    public void TakeDamage(float damageAmount)
    {
        transform.Find("cover").GetComponent<Collider2D>().isTrigger = true; // ��ȡ��ײ�����
        transform.GetComponent<Collider2D>().isTrigger = true;// ��ȡ��ײ�����
        PlayerDie();
        //currentValue = Mathf.Max(currentValue - damageAmount, 0);
        //healthSlider.value = currentValue;
        //if (currentValue <= 0)
        //{
        //    transform.Find("cover").GetComponent<Collider2D>().isTrigger = true; // ��ȡ��ײ�����
        //    transform.GetComponent<Collider2D>().isTrigger = true;// ��ȡ��ײ�����
        //    PlayerDie();
        //}
    }

     // �������ʱ�Ĵ���
    private void PlayerDie()
    {
        Debug.Log("Player has died");
        //TTOD1�и������
      
        if (GameFlowManager.Instance.currentLevelIndex == 0)
        {
            GameManage.Instance.GameOverReset();
            GameFlowManager.Instance.currentLevelIndex++;
            PlayInforManager.Instance.playInfor.level = GameFlowManager.Instance.currentLevelIndex;
            PlayInforManager.Instance.playInfor.SetGun(ConfigManager.Instance.Tables.TablePlayerConfig.Get(GameFlowManager.Instance.currentLevelIndex).Animation, ConfigManager.Instance.Tables.TableTransmitConfig.Get(ConfigManager.Instance.Tables.TablePlayerConfig.Get(GameFlowManager.Instance.currentLevelIndex).Fires[0]).Resource);
            AccountManager.Instance.SaveAccountData();
            PlayInforManager.Instance.playInfor.attackSpFac = 0;
            GameMainPanelController gameMainPanelController = FindObjectOfType<GameMainPanelController>();
            Destroy(gameMainPanelController.gameObject);
            UIManager.Instance.ChangeState(GameState.Ready);
            GameManage.Instance.InitialPalyer();
            //TTPD1������ǹ�л������߼�
            ReplaceGunDragon();
        }
        else
        {
            if (PlayInforManager.Instance.playInfor.ResueeCount > 0)
            {
                PlayInforManager.Instance.playInfor.ResueeCount--;
                Time.timeScale = 0;
                UIManager.Instance.ChangeState(GameState.Resue);

            }
            else
            {
                PlayInforManager.Instance.playInfor.attackSpFac = 0;
                AccountManager.Instance.SaveAccountData();
                GameManage.Instance.GameOverReset();
                if(GameManage.Instance.gameState != GameState.NextLevel)
                {
                    UIManager.Instance.ChangeState(GameState.GameOver);
                    EventDispatcher.instance.DispatchEvent(EventNameDef.GAME_OVER);
                }
            }
        }
    }

    // �������������������ʾ BuffText
    private async UniTaskVoid ActivateBuffText()
    {
        if (BuffText != null)
        {
            BuffText.gameObject.SetActive(true);
            BuffText.transform.localScale = buffStartScale;

            float elapsed = 0f;
            while (elapsed < buffAnimationDuration)
            {
                float t = elapsed / buffAnimationDuration;
                BuffText.transform.localScale = Vector3.Lerp(buffStartScale, buffEndScale, t);
                elapsed += Time.deltaTime;
                await UniTask.Yield();
            }
            BuffText.transform.localScale = buffEndScale;
            // �ȴ�һ��ʱ�������
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
            BuffText.transform.localScale = buffStartScale;
            BuffText.gameObject.SetActive(false);
        }
    }

    // �����������ⲿ��������ʾ Buff
    public void ShowBuff(string buffDescription,Font font)
    {
        if (BuffText != null)
        {
            BuffText.font = font;
            BuffText.text = buffDescription;
            ActivateBuffText().Forget(); // ʹ�� Forget() �����Է��ص� UniTask
        }
    }
  
}
