using UnityEngine;
using UnityEngine.UI;
using DragonBones;
using Transform = UnityEngine.Transform;
using TMPro;
using Cysharp.Threading.Tasks;
using System;

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
    public TextMeshProUGUI DeCoinMonText;      // ��Ҽ����ı�
    public TextMeshProUGUI BuffText;           // Buff�ı�

    // �ӵ������
    public Transform firePoint;

    // Buff�������
    public float buffAnimationDuration = 0.5f; // ��������ʱ�䣨�룩
    public Vector3 buffStartScale = Vector3.zero; // Buff�ı���ʼ����
    public Vector3 buffEndScale = Vector3.one;    // Buff�ı���������
    public float buffDisplayDuration = 2f;        // Buff�ı���ʾ����ʱ�䣨�룩

    // �ڲ�����
    private Camera mainCamera;                         // �������
    private UnityArmatureComponent armatureComponent;   // DragonBones Armature ���

    private void Start()
    {
        // ��ʼ�������
        mainCamera = Camera.main;

        // ��ʼ�����Ѫ����Ѫ��
        Init();

        // ��ȡ DragonBones Armature ���
        armatureComponent = GameObject.Find("Player/player_003").GetComponent<UnityArmatureComponent>();

        // ���Ų�ѭ��ָ���Ķ���
        if (armatureComponent != null)
        {
            PlayDragonAnimation();
        }

        // ע���¼�������
        EventDispatcher.instance.Regist(EventNameDef.ShowBuyBulletText, (v) => ShowDeclineMoney());
    }

    private void Init()
    {
        // ��ʼ��Ѫ��
        currentValue = PlayInforManager.Instance.playInfor.health;
        MaxValue = PlayInforManager.Instance.playInfor.health;
        healthSlider.maxValue = MaxValue;
        healthSlider.value = currentValue;

        // ��ʼ���ƶ��ٶ�
        moveSpeed = 2f;
        buffEndScale *= 1.5f;
        // ��ȡ BuffText ���������Ϊ���غ�����Ϊ��
        BuffText = transform.Find("PlaySliderCav/BuffText").GetComponent<TextMeshProUGUI>();
        BuffText.gameObject.SetActive(false);
        BuffText.transform.localScale = buffStartScale;
    }

    void Update()
    {
        // �����Ϸ״̬���������У���������
        if (GameManage.Instance.gameState != GameState.Running)
            return;

        // ʹ����������������ƶ�
        ControlMovementWithMouse();

        // ����Ѫ����λ�ã�ʹ���������ƶ�
        UpdateHealthBarPosition();
    }

    // ʹ�����X��λ�ÿ�����������ƶ�
    void ControlMovementWithMouse()
    {
        // ��ȡ�������Ļ�ϵ�X��λ��
        Vector3 mousePosition = Input.mousePosition;

        // ����Ļ����ת��Ϊ��������
        // ����Zֵ����ҵ�Zֵ��ͬ����ȷ��ת����ȷ
        mousePosition.z = mainCamera.WorldToScreenPoint(transform.position).z;
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);

        // ��ʹ��X���λ�ø������λ��
        Vector3 newPosition = new Vector3(worldPosition.x, transform.position.y, transform.position.z);

        // ��������ƶ���Χ�����ұ߽�֮��
        newPosition.x = Mathf.Clamp(newPosition.x, leftBoundary, rightBoundary);

        // ������ҵ�λ��
        transform.position = newPosition;
    }

    // ���Ų�ѭ��"DragonAnimation"
    void PlayDragonAnimation()
    {
        if (armatureComponent != null)
        {
            armatureComponent.animation.Play("shoot+walk", 0);  // ����ѭ������
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
            DeCoinMonText.text = $"-{PlayInforManager.Instance.playInfor.currentGun.bulletValue}";
            await ShowDeCoinMonText();
        }
    }

    // ��ʾ�����ؽ�Ҽ����ı�
    private async UniTask ShowDeCoinMonText()
    {
        DeCoinMonText.gameObject.SetActive(true); // ��ʾ�ı�
        await UniTask.Delay(500);                  // �ȴ�0.5��
        DeCoinMonText.gameObject.SetActive(false); // �����ı�
    }

    // ��������ܵ��˺�
    public void TakeDamage(float damageAmount)
    {
        currentValue = Mathf.Max(currentValue - damageAmount, 0);
        healthSlider.value = currentValue;

        if (currentValue <= 0)
        {
            PlayerDie();
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            EnemyController enemy = collider.GetComponent<EnemyController>();
            if (enemy != null)
            {
                TakeDamage(enemy.damage);
            }
        }
    }

    // �������ʱ�Ĵ���
    private void PlayerDie()
    {
        Debug.Log("Player has died");
        UIManager.Instance.ChangeState(GameState.GameOver);
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
    public void ShowBuff(string buffDescription)
    {
        if (BuffText != null)
        {
            BuffText.text = buffDescription;
            ActivateBuffText().Forget(); // ʹ�� Forget() �����Է��ص� UniTask
        }
    }
  
}
