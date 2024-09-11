using UnityEngine;
using UnityEngine.UI;
using DragonBones;  // Make sure you include the DragonBones namespace if you are using DragonBones
using Transform = UnityEngine.Transform;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 2f;   // ��������ƶ��ٶ�
    public Transform firePoint;    // �ӵ������
    public float currentValue;     // ��ǰѪ��
    public float MaxValue;         // ���Ѫ��
    public Slider healthSlider;    // Ѫ����ʾ��Slider
    public Transform healthBarCanvas; // Ѫ�����ڵ�Canvas (World Space Canvas)
    public float leftBoundary = -1.5f;  // ��߽�����
    public float rightBoundary = 1.5f;  // �ұ߽�����
    private float horizontalInput;

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
    }

    private void Init()
    {
        currentValue = 100; // �����ʼѪ��Ϊ100
        MaxValue = 100;
        healthSlider.maxValue = MaxValue;
        healthSlider.value = currentValue;
    }

    void Update()
    {
        if (GameManage.Instance.gameState != GameState.Running)
            return;

        // ��������ƶ�
        horizontalInput = Input.GetAxis("Horizontal");
        Vector3 newPosition = transform.position + new Vector3(horizontalInput * moveSpeed * Time.deltaTime, 0);
        newPosition.x = Mathf.Clamp(newPosition.x, leftBoundary, rightBoundary);
        transform.position = newPosition;

        // ����Ѫ����λ�ã�ʹ���������ƶ�
        UpdateHealthBarPosition();
    }

    // ���Ų�ѭ��"DragonAnimation"
    void PlayDragonAnimation()
    {
        if (armatureComponent != null)
        {
            // ��������Ϊ"DragonAnimation"�Ķ�����������ѭ������
            armatureComponent.animation.Play("shoot+walk", 0);  // ��2������0��ʾ����ѭ��
        }
    }

    // ����Ѫ����λ�ã�ȷ�����������
    void UpdateHealthBarPosition()
    {
        if (healthBarCanvas != null)
        {
            // ��Ѫ������Ϊ���ͷ����λ�� (��������)
            healthBarCanvas.position = transform.position + new Vector3(0, 0.8f, 0);  // 1f ΪY���ƫ����
            healthBarCanvas.localScale = new Vector3(0.01f, 0.01f, 0.01f);  // ����Ѫ�������ţ�ʹ����Ӧ����
        }
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
        // �������Ƿ������˵���
        if (collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            // ���������һ��EnemyController�ű����������˺�ֵ
            EnemyController enemy = collider.GetComponent<EnemyController>();
            if (enemy != null)
            {
                // ����ܵ����˵��˺�
                TakeDamage(enemy.damage);
            }
        }
    }

    // �������ʱ�Ĵ���
    private void PlayerDie()
    {
        // ������Լ�������������߼����粥��������������Ϸ������
        Debug.Log("Player has died");
        UIManager.Instance.ChangeState(GameState.GameOver);  // ������GameOver�Ĵ����߼�
    }
}
