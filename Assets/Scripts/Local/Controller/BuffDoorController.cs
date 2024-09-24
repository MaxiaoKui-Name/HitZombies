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
        randomBuffId = Random.Range(0, 8);
        randomDeBuffId  = Random.Range(9, 14);
        string randomBuff = ConfigManager.Instance.Tables.TableDoorcontent.Get(randomBuffId + 1).Name;
        string randomDeBuff = ConfigManager.Instance.Tables.TableDoorcontent.Get(randomDeBuffId + 1).Name;
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

    // 应用增益效果的逻辑
    private void ApplyBuff(GameObject player, int buffId)
    {
        switch (buffId +1)
        {
            case 1:
                attackFac = ConfigManager.Instance.Tables.TableDoorcontent.Get(buffId + 1).GenusScale; 
                break;
            case 2:
                attackFac = ConfigManager.Instance.Tables.TableDoorcontent.Get(buffId + 1).GenusScale;
                break;
            case 3:
                attackSpFac = ConfigManager.Instance.Tables.TableDoorcontent.Get(buffId + 1).GenusScale;
                break;
            case 4:
                attackSpFac = ConfigManager.Instance.Tables.TableDoorcontent.Get(buffId + 1).GenusScale;
                break;
            case 5:
                attackSpFac = ConfigManager.Instance.Tables.TableDoorcontent.Get(buffId + 1).GenusScale;
                break;
            case 6:
                coinFac = ConfigManager.Instance.Tables.TableDoorcontent.Get(buffId + 1).GenusScale;
                break;
            case 7:
                coinFac = ConfigManager.Instance.Tables.TableDoorcontent.Get(buffId + 1).GenusScale;
                break;
            case 8:
                //SummonSoldiers(player, (int)(ConfigManager.Instance.Tables.TableDoorcontent.Get(buffId + 1).GenusValue));
                break;
            case 9:
                //SummonSoldiers(player, (int)(ConfigManager.Instance.Tables.TableDoorcontent.Get(buffId + 1).GenusValue));
                break;
        }
    }

    // 应用减益效果的逻辑
    private void ApplyDebuff(GameObject player, int debuff)
    {
        PlayerController playerController = player.GetComponent<PlayerController>();

        switch (debuff + 1 )
        {
            case 10:
                attackFac = ConfigManager.Instance.Tables.TableDoorcontent.Get(debuff + 1).GenusScale;
                break;
            case 11:
                attackFac = ConfigManager.Instance.Tables.TableDoorcontent.Get(debuff + 1).GenusScale;
                break;
            case 12:
                attackSpFac = ConfigManager.Instance.Tables.TableDoorcontent.Get(debuff + 1).GenusScale;
                break;
            case 13:
                attackSpFac = ConfigManager.Instance.Tables.TableDoorcontent.Get(debuff + 1).GenusScale;
                break;
            case 14:
                coinFac = ConfigManager.Instance.Tables.TableDoorcontent.Get(debuff + 1).GenusScale;
                break;
            case 15:
                coinFac = ConfigManager.Instance.Tables.TableDoorcontent.Get(debuff + 1).GenusScale;
                break;
        }
    }

    // 召唤士兵的逻辑
    private void SummonSoldiers(GameObject player, int soldierCount)
    {
        for (int i = 0; i < soldierCount; i++)
        {
            // 在玩家周围召唤士兵
            Vector3 spawnPosition = player.transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
            // 假设有一个Soldier预制体，你可以在此处实例化士兵
            Instantiate(Resources.Load("Prefabs/soldier_001"), spawnPosition, Quaternion.identity);
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
