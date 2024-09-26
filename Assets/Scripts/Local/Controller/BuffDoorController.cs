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
    public float moveSpeed = 1f; // 设置物体向下移动的速度
    public float hideYPosition = -10f; // 超出屏幕的Y坐标
    public bool isMove = false;
    private bool hasTriggered = false; // 避免重复触发技能
    private float MiddleX = 0f;
    public double attackFac;
    public double attackSpFac;
    public double coinFac;
    public Transform healthBarCanvas; // 血条所在的Canvas (World Space Canvas)
   
    void Start()
    {
        debuffText = GameObject.Find("BuffDoor/Canvas/DebuffdoorText").GetComponent<TextMeshProUGUI>();
        buffText = GameObject.Find("BuffDoor/Canvas/buffdoorText").GetComponent<TextMeshProUGUI>();
        hasTriggered = false;
        isMove = false;
        // 遍历并激活所有子对象
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
            // 将血条设置为玩家头顶的位置 (世界坐标)
            healthBarCanvas.position = transform.position;  // 1f 为Y轴的偏移量
            healthBarCanvas.localScale = new Vector3(0.01f, 0.01f, 0.01f);  // 调整血条的缩放，使其适应场景
        }
     }


    private void OnTriggerEnter2D(Collider2D other)
    {
        // 检查子弹是否碰到了玩家
        if (other.gameObject.layer == 8 && !hasTriggered)
        {
            hasTriggered = true;
            // transform.GetComponent<SortingGroup>().sortingLayerName = "Partical";
            Debug.Log("触发技能！！！！");
            TriggerSkill(other.gameObject); // 触发技能
        }
    }
    public int randomBuffId = 0;
    public int randomDeBuffId = 0;
    // 生成 Buff 门的方法
    public void SpawnBuffDoor()
    {
        transform.position = new Vector3(0, 5.8f, 0f);
        // 遍历并激活所有子对象
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
            isMove = true;
        }
        PreController.Instance.FixSortLayer(transform.gameObject);
        //设置门的初始文本
        randomBuffId = GetBuffIndex();
        randomDeBuffId  = GetDeBuffIndex();
        string randomBuff = ConfigManager.Instance.Tables.TableDoorcontent.Get(randomBuffId).Name;
        string randomDeBuff = ConfigManager.Instance.Tables.TableDoorcontent.Get(randomDeBuffId).Name;
        buffText.text = randomBuff;
        debuffText.text = randomDeBuff;
    }

    // 触发技能的逻辑
    private void TriggerSkill(GameObject player)
    {
        // 获取玩家的X轴位置
        float playerXPosition = player.transform.position.x;

        if (playerXPosition > MiddleX) // 假设正X轴是增益门
        {
            ApplyBuff(player, randomBuffId); // 应用增益效果
        }
        else // 否则是减益门
        {
            // 玩家进入减益门，随机选择一个减益效果
            ApplyDebuff(player, randomDeBuffId); // 应用减益效果
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
    // 应用增益效果的逻辑
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

    // 应用减益效果的逻辑
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

    // 召唤士兵的逻辑
    private void SummonSoldiers(GameObject player, int soldierCount)
    {
         List<Vector3> offsets = new List<Vector3>
    {
        new Vector3(0.5f, -0.5f, 0), // 右后1位
        new Vector3(-0.5f, -0.5f, 0), // 左后1位
        new Vector3(1f, -0.75f, 0),   // 右后
        new Vector3(-1f, -0.75f, 0)  // 左后
    };

        for (int i = 0; i < soldierCount; i++)
        {
            Vector3 spawnPosition = player.transform.position + offsets[i];
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(spawnPosition, 0.1f); // 根据需要调整半径

            bool soldierExists = false;

            foreach (var hitCollider in hitColliders)
            {
                SoldierController existingSoldier = hitCollider.GetComponent<SoldierController>();
                if (existingSoldier != null)
                {
                    existingSoldier.SetLifetime(20f); // 延长现有士兵的存在时间
                    soldierExists = true;
                    break; // 找到现有士兵后退出循环
                }
            }
            // 如果没有现有士兵，则实例化新的士兵
            if (!soldierExists)
            {
                GameObject soldier = Instantiate(Resources.Load<GameObject>("Prefabs/soldier_001"), spawnPosition, Quaternion.identity);
                soldier.transform.position = spawnPosition;
                SoldierController soldierController = soldier.GetComponent<SoldierController>();
                soldierController.SetPlayer(player);
                soldierController.SetLifetime(20f); // 设置士兵存活时间ConfigManager.Instance.Tables.TableGlobal.Get(13).IntValue
            }
        }
    }


    // 物体向下移动
    private void MoveDown()
    {
        transform.Translate(Vector3.down * moveSpeed * Time.deltaTime);
    }

    // 隐藏所有子对象
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
