using UnityEngine;
using UnityEngine.UI;
using DragonBones;
using Transform = UnityEngine.Transform;
using TMPro;
using Cysharp.Threading.Tasks;
using System;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 2f;   // ��������ƶ��ٶ�
    public Transform firePoint;    // �ӵ������
    public float currentValue;     // ��ǰѪ��
    public float MaxValue;         // ���Ѫ��
    public Slider healthSlider;    // Ѫ����ʾ��Slider
    public TextMeshProUGUI DeCoinMonText; 
    public Transform healthBarCanvas; // Ѫ�����ڵ�Canvas (World Space Canvas)
    public float leftBoundary = -1.5f;  // ��߽�����
    public float rightBoundary = 1.5f;  // �ұ߽�����

    private Camera mainCamera; // ����������ڽ���������תΪ��Ļ����
    private UnityArmatureComponent armatureComponent; // DragonBones Armature component

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
        EventDispatcher.instance.Regist(EventNameDef.ShowBuyBulletText, (v) => ShowDeclineMoney());
    }

    private void Init()
    {
        currentValue = PlayInforManager.Instance.playInfor.health;
        MaxValue = PlayInforManager.Instance.playInfor.health;
        healthSlider.maxValue = MaxValue;
        healthSlider.value = currentValue;
        moveSpeed = 2f;
    }

    void Update()
    {
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
    async void ShowDeclineMoney()
    {
        if (DeCoinMonText != null)
        {
            DeCoinMonText.text = $"-{ConfigManager.Instance.Tables.TablePlayer.Get(0).Total}";
            ShowDeCoinMonText();
        }
    }
    private async UniTask ShowDeCoinMonText()
    {
        DeCoinMonText.gameObject.SetActive(true); // ��ʾ�ı�
        await UniTask.Delay(500);
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
}
