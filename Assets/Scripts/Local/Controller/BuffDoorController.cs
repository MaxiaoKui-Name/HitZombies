using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class BuffDoorController : Singleton<BuffDoorController>
{
    public TextMeshProUGUI buffText;
    public TextMeshProUGUI debuffText;
    public float moveSpeed = 1f; // �������������ƶ����ٶ�
    public float hideYPosition = -40.01f; // ������Ļ��Y����
    public bool isMove = false;
    private bool hasTriggered = false; // �����ظ���������
    private float MiddleX = 0f;
    public double attackFac;
    public double attackSpFac;
    public double coinFac;
    public Transform healthBarCanvas; // Ѫ�����ڵ�Canvas (World Space Canvas)

    // ����Ч���б�
    private List<string> buffs = new List<string> {
        "������+15%",
        "������+5%",
        "����+10%",
        "����+20%",
        "����+5%",
        "��ҵ���+30%",
        "��ҵ���+50%",
        "�ٻ�2��ʿ��",
        "�ٻ�4��ʿ��"
    };

    // ����Ч���б�
    private List<string> debuffs = new List<string> {
        "������-10%",
        "������-5%",
        "����-10%",
        "����-5%",
        "��ҵ���-30%",
        "��ҵ���-50%"
    };

    void Start()
    {
        debuffText = GameObject.Find("BuffDoor/Canvas/DebuffdoorText").GetComponent<TextMeshProUGUI>();
        buffText = GameObject.Find("BuffDoor/Canvas/buffdoorText").GetComponent<TextMeshProUGUI>();
        // ���������������Ӷ���
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
        FollowParentObject();
    }

    void Update()
    {
        if (isMove)
        {
            MoveDown();
            FollowParentObject();
        }

        if (transform.position.y < hideYPosition)
        {
            HideAllChildren();
        }
    }
    private void FollowParentObject()
    {
        if (healthBarCanvas != null)
        {
            // ��Ѫ������Ϊ���ͷ����λ�� (��������)
            healthBarCanvas.position = transform.position;  // 1f ΪY���ƫ����
            healthBarCanvas.localScale = new Vector3(0.01f, 0.01f, 0.01f);  // ����Ѫ�������ţ�ʹ����Ӧ����
        }
     }


private void OnTriggerEnter2D(Collider2D other)
    {
        // ����ӵ��Ƿ����������
        if (other.gameObject.layer == 8 && !hasTriggered)
        {
            hasTriggered = true;
           // transform.GetComponent<SortingGroup>().sortingLayerName = "Partical";
            Debug.Log("�������ܣ�������");
            TriggerSkill(other.gameObject); // ��������
        }
    }
    public int randomBuffId = 0;
    public int randomDeBuffId = 0;
    // ���� Buff �ŵķ���
    public void SpawnBuffDoor()
    {
        transform.position = new Vector3(0, 5.8f, 0f);
        // ���������������Ӷ���
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
            isMove = true;
        }
        //�����ŵĳ�ʼ�ı�
        randomBuffId = Random.Range(0, buffs.Count);
        randomDeBuffId  = Random.Range(0, debuffs.Count);
        string randomBuff = buffs[randomBuffId];
        string randomDeBuff = debuffs[randomDeBuffId];
        buffText.text = randomBuff;
        debuffText.text = randomDeBuff;
    }

    // �������ܵ��߼�
    private void TriggerSkill(GameObject player)
    {
        // ��ȡ��ҵ�X��λ��
        float playerXPosition = player.transform.position.x;

        if (playerXPosition > MiddleX) // ������X����������
        {
            ApplyBuff(player, randomBuffId); // Ӧ������Ч��
        }
        else // �����Ǽ�����
        {
            // ��ҽ�������ţ����ѡ��һ������Ч��
            ApplyDebuff(player, randomDeBuffId); // Ӧ�ü���Ч��
        }

    }

    // Ӧ������Ч�����߼�
    private void ApplyBuff(GameObject player, int buffId)
    {
        switch (buffId +1)
        {
            case 1:
                attackFac = 0.15; //TTOD1ʹ�ñ��
                break;
            case 2:
                attackFac = 0.05;
                break;
            case 3:
                attackSpFac = -0.1;
                break;
            case 4:
                attackSpFac = -0.2;
                break;
            case 5:
                attackSpFac = -0.05;
                break;
            case 6:
                coinFac = 0.3;
                break;
            case 7:
                coinFac = 0.5;
                break;
            case 8:
                SummonSoldiers(player, 2);
                break;
            case 9:
                SummonSoldiers(player, 4);
                break;
        }
    }

    // Ӧ�ü���Ч�����߼�
    private void ApplyDebuff(GameObject player, int debuff)
    {
        PlayerController playerController = player.GetComponent<PlayerController>();

        switch (debuff + 1 )
        {
            case 10:
                attackFac = 0.1;
                break;
            case 11:
                attackFac = -0.05;
                break;
            case 12:
                attackSpFac = 0.1;
                break;
            case 13:
                attackSpFac = 0.05;
                break;
            case 14:
                coinFac = -0.3;
                break;
            case 15:
                coinFac = -0.5;
                break;
        }
    }

    // �ٻ�ʿ�����߼�
    private void SummonSoldiers(GameObject player, int soldierCount)
    {
        for (int i = 0; i < soldierCount; i++)
        {
            // �������Χ�ٻ�ʿ��
            Vector3 spawnPosition = player.transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
            // ������һ��SoldierԤ���壬������ڴ˴�ʵ����ʿ��
            Instantiate(Resources.Load("Prefabs/soldier_001"), spawnPosition, Quaternion.identity);
        }
    }

    // ���������ƶ�
    private void MoveDown()
    {
        transform.Translate(Vector3.down * moveSpeed * Time.deltaTime);
    }

    // ���������Ӷ���
    public void HideAllChildren()
    {
        GameManage.Instance.isPlaydoor = true;
        hasTriggered = false;
        //transform.GetComponent<SortingGroup>().sortingLayerName = "Default";
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }
}
