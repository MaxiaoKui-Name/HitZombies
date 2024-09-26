using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

public class BuffDoorController : Singleton<BuffDoorController>
{
    public TextMeshProUGUI buffText;
    public TextMeshProUGUI debuffText;
    public float moveSpeed = 1f; // �������������ƶ����ٶ�
    public float hideYPosition = -10f; // ������Ļ��Y����
    public bool isMove = false;
    private bool hasTriggered = false; // �����ظ���������
    private float MiddleX = 0f;
    public double attackFac;
    public double attackSpFac;
    public double coinFac;
    public Transform healthBarCanvas; // Ѫ�����ڵ�Canvas (World Space Canvas)
   
    void Start()
    {
        debuffText = GameObject.Find("BuffDoor/Canvas/DebuffdoorText").GetComponent<TextMeshProUGUI>();
        buffText = GameObject.Find("BuffDoor/Canvas/buffdoorText").GetComponent<TextMeshProUGUI>();
        hasTriggered = false;
        isMove = false;
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
        PreController.Instance.FixSortLayer(transform.gameObject);
        //�����ŵĳ�ʼ�ı�
        randomBuffId = GetBuffIndex();
        randomDeBuffId  = GetDeBuffIndex();
        string randomBuff = ConfigManager.Instance.Tables.TableDoorcontent.Get(randomBuffId).Name;
        string randomDeBuff = ConfigManager.Instance.Tables.TableDoorcontent.Get(randomDeBuffId).Name;
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
    public int GetBuffIndex()
    {
        var coinindexConfig = ConfigManager.Instance.Tables.TableDoorcontent;
        int WeightAll = 0;
        for (int i = 1;i<= 4; i++)
        {
            WeightAll += coinindexConfig.Get(i).Weight;
        }
        float randomNum = Random.Range(1, WeightAll);
        if (randomNum <= coinindexConfig.Get(1).Weight)
            return 1;
        else if (randomNum > coinindexConfig.Get(1).Weight && randomNum <= (coinindexConfig.Get(2).Weight + coinindexConfig.Get(3).Weight)) // 71.45 + 23
            return 2;
        else if (randomNum > (coinindexConfig.Get(2).Weight + coinindexConfig.Get(3).Weight) && randomNum <= (coinindexConfig.Get(2).Weight + coinindexConfig.Get(3).Weight + coinindexConfig.Get(4).Weight)) // 94.45 + 5
            return 3;
        else // If it's less than 100, return 5
            return 4;
    }
    public int GetDeBuffIndex()
    {
        var coinindexConfig = ConfigManager.Instance.Tables.TableDoorcontent;
        int WeightAll = 0;
        for (int i = 5 ; i <= 6; i++)
        {
            WeightAll += coinindexConfig.Get(i).Weight;
        }
        float randomNum = Random.Range(1, WeightAll);
        if (randomNum <= coinindexConfig.Get(5).Weight)
            return 5;
        else // If it's less than 100, return 5
            return 6;
    }
    // Ӧ������Ч�����߼�
    private void ApplyBuff(GameObject player, int buffId)
    {
        switch (buffId)
        {
            case 1:
                attackSpFac = ConfigManager.Instance.Tables.TableDoorcontent.Get(buffId).GenusScale;
                break;
            case 2:
                attackSpFac = ConfigManager.Instance.Tables.TableDoorcontent.Get(buffId).GenusScale;
                break;
            case 3:
                SummonSoldiers(player, (int)(ConfigManager.Instance.Tables.TableDoorcontent.Get(buffId).GenusValue));
                break;
            case 4:
                SummonSoldiers(player, (int)(ConfigManager.Instance.Tables.TableDoorcontent.Get(buffId).GenusValue));
                break;
        }
    }

    // Ӧ�ü���Ч�����߼�
    private void ApplyDebuff(GameObject player, int debuff)
    {
        PlayerController playerController = player.GetComponent<PlayerController>();

        switch (debuff)
        {
            case 5:
                attackSpFac = ConfigManager.Instance.Tables.TableDoorcontent.Get(debuff).GenusScale;
                break;
            case 6:
                coinFac = ConfigManager.Instance.Tables.TableDoorcontent.Get(debuff).GenusScale;
                break;
        }
    }

    // �ٻ�ʿ�����߼�
    private void SummonSoldiers(GameObject player, int soldierCount)
    {
         List<Vector3> offsets = new List<Vector3>
    {
        new Vector3(0.5f, -0.5f, 0), // �Һ�1λ
        new Vector3(-0.5f, -0.5f, 0), // ���1λ
        new Vector3(1f, -0.75f, 0),   // �Һ�
        new Vector3(-1f, -0.75f, 0)  // ���
    };

        for (int i = 0; i < soldierCount; i++)
        {
            Vector3 spawnPosition = player.transform.position + offsets[i];
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(spawnPosition, 0.1f); // ������Ҫ�����뾶

            bool soldierExists = false;

            foreach (var hitCollider in hitColliders)
            {
                SoldierController existingSoldier = hitCollider.GetComponent<SoldierController>();
                if (existingSoldier != null)
                {
                    existingSoldier.SetLifetime(20f); // �ӳ�����ʿ���Ĵ���ʱ��
                    soldierExists = true;
                    break; // �ҵ�����ʿ�����˳�ѭ��
                }
            }
            // ���û������ʿ������ʵ�����µ�ʿ��
            if (!soldierExists)
            {
                GameObject soldier = Instantiate(Resources.Load<GameObject>("Prefabs/soldier_001"), spawnPosition, Quaternion.identity);
                soldier.transform.position = spawnPosition;
                SoldierController soldierController = soldier.GetComponent<SoldierController>();
                soldierController.SetPlayer(player);
                soldierController.SetLifetime(20f); // ����ʿ�����ʱ��ConfigManager.Instance.Tables.TableGlobal.Get(13).IntValue
            }
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
