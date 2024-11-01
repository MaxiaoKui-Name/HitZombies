using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

public class BuffDoorController :MonoBehaviour
{
    public TextMeshProUGUI buffText;
    public TextMeshProUGUI debuffText;
    public float moveSpeed; // �������������ƶ����ٶ�
    public float hideYPosition = -10f; // ������Ļ��Y����
    public bool isMove = false;
    private bool hasTriggered = false; // �����ظ���������
    private float MiddleX = 0f;
 
    public Transform healthBarCanvas; // Ѫ�����ڵ�Canvas (World Space Canvas)
   
    void OnEnable()
    {
        debuffText = GameObject.Find("BuffDoor(Clone)/Canvas/DebuffdoorText").GetComponent<TextMeshProUGUI>();
        buffText = GameObject.Find("BuffDoor(Clone)/Canvas/buffdoorText").GetComponent<TextMeshProUGUI>();
        hasTriggered = false;
        isMove = true;
        moveSpeed = ConfigManager.Instance.Tables.TableGlobal.Get(6).IntValue;
        transform.localScale = Vector3.one * initialScale;
        FollowParentObject();
    }

    void Update()
    {
        if (PreController.Instance.isFrozen) return;
        if (GameManage.Instance.gameState != GameState.Running)
        {
            Destroy(gameObject);
            return; // ����ʱ��ִ���κ��߼�
        }
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
            transform.gameObject.SetActive(false);
            TriggerSkill(other.gameObject); // ��������
        }
    }
    public int randomBuffId = 0;
    public int randomDeBuffId = 0;
    // ���� Buff �ŵķ���
    public void GetBuffDoorIDAndText(GameObject BuffDoorobj)
    {
        if(GameFlowManager.Instance.currentLevelIndex == 0)
        {
            if(PreController.Instance.DoorNumWave == 4)
                randomBuffId = 3;
            if (PreController.Instance.DoorNumWave == 5)
                randomBuffId = 4;
            if (PreController.Instance.DoorNumWave == 7)
                randomBuffId = 1;
        }
        else
        {
            randomBuffId = GetBuffIndex();
        }
        //�����ŵĳ�ʼ�ı�
        randomDeBuffId  = GetDeBuffIndex();
        string randomBuff = ConfigManager.Instance.Tables.TableDoorcontent.Get(randomBuffId).Name;
        string randomDeBuff = ConfigManager.Instance.Tables.TableDoorcontent.Get(randomDeBuffId).Name;
        debuffText = transform.Find("Canvas/DebuffdoorText").GetComponent<TextMeshProUGUI>();
        buffText = transform.Find("Canvas/buffdoorText").GetComponent<TextMeshProUGUI>();
        buffText.text = randomBuff;
        debuffText.text = randomDeBuff;
    }

    // �������ܵ��߼�
    private async UniTask TriggerSkill(GameObject player)
    {
        // ��ȡ��ҵ�X��λ��
        float playerXPosition = player.transform.position.x;
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerXPosition > MiddleX) // ������X����������
        {
            ApplyBuff(player, randomBuffId); // Ӧ������Ч��
            playerController.ShowBuff(ConfigManager.Instance.Tables.TableDoorcontent.Get(randomBuffId).Name);
        }
        else // �����Ǽ�����
        {
            // ��ҽ�������ţ����ѡ��һ������Ч��
            ApplyDebuff(player, randomDeBuffId); // Ӧ�ü���Ч��
            Debug.Log("���е�buff�����ֵ======================ApplyDebuff" + PlayInforManager.Instance.playInfor.attackSpFac);
            playerController.ShowBuff(ConfigManager.Instance.Tables.TableDoorcontent.Get(randomDeBuffId).Name);
        }
        Destroy(transform.gameObject, 1f);
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
                PlayInforManager.Instance.playInfor.attackSpFac += ConfigManager.Instance.Tables.TableDoorcontent.Get(buffId).GenusScale;
                break;
            case 2:
                PlayInforManager.Instance.playInfor.attackSpFac += ConfigManager.Instance.Tables.TableDoorcontent.Get(buffId).GenusScale;
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
                PlayInforManager.Instance.playInfor.attackSpFac += ConfigManager.Instance.Tables.TableDoorcontent.Get(debuff).GenusScale;
                break;
            case 6:
                PlayInforManager.Instance.playInfor.attackSpFac += ConfigManager.Instance.Tables.TableDoorcontent.Get(debuff).GenusScale;
                break;
        }
    }

    // �ٻ�ʿ�����߼�
    private void SummonSoldiers(GameObject player, int soldierCount)
    {
        List<Vector3> offsets = new List<Vector3>
    {
        new Vector3(0.5f, -0.2f, 0), // �Һ�1λ
        new Vector3(-0.5f, -0.2f, 0), // ���1λ
        new Vector3(1f, -0.5f, 0),   // �Һ�
        new Vector3(-1f, -0.5f, 0)  // ���
    };

        int generatedSoldiers = 0;
        for (int i = 0; i < offsets.Count && generatedSoldiers < soldierCount; i++)
        {
            Vector3 spawnPosition = player.transform.position + offsets[i];
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(spawnPosition, 0.05f); // ������Ҫ�����뾶
            bool soldierExists = false;

            foreach (var hitCollider in hitColliders)
            {
                SoldierController existingSoldier = hitCollider.GetComponent<SoldierController>();
                if (existingSoldier != null)
                {
                    soldierExists = true;
                    // �ж� soldierCount �� soldiers �������߼�
                    if (soldierCount == 4 || generatedSoldiers >= 2)
                    {
                        existingSoldier.SetLifetime(20f); // ��������ʿ���Ĵ��ʱ��
                    }
                    break;
                }
            }

            // �����λ��û��ʿ��������Ҫ�����µ�ʿ��
            if (!soldierExists)
            {
                GameObject soldier = Instantiate(Resources.Load<GameObject>("Prefabs/soldier_001"), spawnPosition, Quaternion.identity);
                SoldierController soldierController = soldier.GetComponent<SoldierController>();
                soldierController.SetPlayer(player);
                soldierController.SetLifetime(20f); // ����ʿ�����ʱ��
                generatedSoldiers++;
            }
        }
    }


    private float initialScale = 0.6f; // Initial chest scale
    private float targetScale = 1.1f; // Target chest scale
                                       // ���������ƶ�
    private void MoveDown()
    {
        transform.Translate(Vector3.down * InfiniteScroll.Instance.scrollSpeed * Time.deltaTime);
        float currentScale = transform.localScale.x; // Assuming uniform scaling on all axes
        if (currentScale < targetScale)
        {
            float scaleFactor = InfiniteScroll.Instance.growthRate *2 * Time.deltaTime;
            float newScale = Mathf.Min(currentScale + scaleFactor, targetScale); // Ensure the scale doesn't exceed the target scale
                                                                                 // Apply the new scale uniformly
            transform.localScale = new Vector3(newScale, newScale, newScale);
        }
    }
    // ���������Ӷ���
    public void HideAllChildren()
    {
        hasTriggered = false;
        isMove = false;
        //transform.GetComponent<SortingGroup>().sortingLayerName = "Default";
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }
}
