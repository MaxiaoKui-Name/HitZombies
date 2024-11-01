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
    public float moveSpeed; // 设置物体向下移动的速度
    public float hideYPosition = -10f; // 超出屏幕的Y坐标
    public bool isMove = false;
    private bool hasTriggered = false; // 避免重复触发技能
    private float MiddleX = 0f;
 
    public Transform healthBarCanvas; // 血条所在的Canvas (World Space Canvas)
   
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
            return; // 冻结时不执行任何逻辑
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
            transform.gameObject.SetActive(false);
            TriggerSkill(other.gameObject); // 触发技能
        }
    }
    public int randomBuffId = 0;
    public int randomDeBuffId = 0;
    // 生成 Buff 门的方法
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
        //设置门的初始文本
        randomDeBuffId  = GetDeBuffIndex();
        string randomBuff = ConfigManager.Instance.Tables.TableDoorcontent.Get(randomBuffId).Name;
        string randomDeBuff = ConfigManager.Instance.Tables.TableDoorcontent.Get(randomDeBuffId).Name;
        debuffText = transform.Find("Canvas/DebuffdoorText").GetComponent<TextMeshProUGUI>();
        buffText = transform.Find("Canvas/buffdoorText").GetComponent<TextMeshProUGUI>();
        buffText.text = randomBuff;
        debuffText.text = randomDeBuff;
    }

    // 触发技能的逻辑
    private async UniTask TriggerSkill(GameObject player)
    {
        // 获取玩家的X轴位置
        float playerXPosition = player.transform.position.x;
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerXPosition > MiddleX) // 假设正X轴是增益门
        {
            ApplyBuff(player, randomBuffId); // 应用增益效果
            playerController.ShowBuff(ConfigManager.Instance.Tables.TableDoorcontent.Get(randomBuffId).Name);
        }
        else // 否则是减益门
        {
            // 玩家进入减益门，随机选择一个减益效果
            ApplyDebuff(player, randomDeBuffId); // 应用减益效果
            Debug.Log("抽中的buff间隔数值======================ApplyDebuff" + PlayInforManager.Instance.playInfor.attackSpFac);
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
    // 应用增益效果的逻辑
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

    // 应用减益效果的逻辑
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

    // 召唤士兵的逻辑
    private void SummonSoldiers(GameObject player, int soldierCount)
    {
        List<Vector3> offsets = new List<Vector3>
    {
        new Vector3(0.5f, -0.2f, 0), // 右后1位
        new Vector3(-0.5f, -0.2f, 0), // 左后1位
        new Vector3(1f, -0.5f, 0),   // 右后
        new Vector3(-1f, -0.5f, 0)  // 左后
    };

        int generatedSoldiers = 0;
        for (int i = 0; i < offsets.Count && generatedSoldiers < soldierCount; i++)
        {
            Vector3 spawnPosition = player.transform.position + offsets[i];
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(spawnPosition, 0.05f); // 根据需要调整半径
            bool soldierExists = false;

            foreach (var hitCollider in hitColliders)
            {
                SoldierController existingSoldier = hitCollider.GetComponent<SoldierController>();
                if (existingSoldier != null)
                {
                    soldierExists = true;
                    // 判断 soldierCount 和 soldiers 的生成逻辑
                    if (soldierCount == 4 || generatedSoldiers >= 2)
                    {
                        existingSoldier.SetLifetime(20f); // 增加现有士兵的存活时间
                    }
                    break;
                }
            }

            // 如果该位置没有士兵，且需要生成新的士兵
            if (!soldierExists)
            {
                GameObject soldier = Instantiate(Resources.Load<GameObject>("Prefabs/soldier_001"), spawnPosition, Quaternion.identity);
                SoldierController soldierController = soldier.GetComponent<SoldierController>();
                soldierController.SetPlayer(player);
                soldierController.SetLifetime(20f); // 设置士兵存活时间
                generatedSoldiers++;
            }
        }
    }


    private float initialScale = 0.6f; // Initial chest scale
    private float targetScale = 1.1f; // Target chest scale
                                       // 物体向下移动
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
    // 隐藏所有子对象
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
