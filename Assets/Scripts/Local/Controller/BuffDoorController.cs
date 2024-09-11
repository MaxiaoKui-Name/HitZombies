using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class BuffDoorController : Singleton<BuffDoorController>
{
    public TextMeshProUGUI buffText;
    public TextMeshProUGUI debuffText;
    public float moveSpeed = 1f; // 设置物体向下移动的速度
    public float hideYPosition = -40.01f; // 超出屏幕的Y坐标
    public bool isMove = false;
    private bool hasTriggered = false; // 避免重复触发技能
    private float MiddleX = 0f;
    public double attackFac;
    public double attackSpFac;
    public double coinFac;
    public Transform healthBarCanvas; // 血条所在的Canvas (World Space Canvas)

    // 增益效果列表
    private List<string> buffs = new List<string> {
        "攻击力+15%",
        "攻击力+5%",
        "攻速+10%",
        "攻速+20%",
        "攻速+5%",
        "金币掉落+30%",
        "金币掉落+50%",
        "召唤2名士兵",
        "召唤4名士兵"
    };

    // 减益效果列表
    private List<string> debuffs = new List<string> {
        "攻击力-10%",
        "攻击力-5%",
        "攻速-10%",
        "攻速-5%",
        "金币掉落-30%",
        "金币掉落-50%"
    };

    void Start()
    {
        debuffText = GameObject.Find("BuffDoor/Canvas/DebuffdoorText").GetComponent<TextMeshProUGUI>();
        buffText = GameObject.Find("BuffDoor/Canvas/buffdoorText").GetComponent<TextMeshProUGUI>();
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
        //设置门的初始文本
        randomBuffId = Random.Range(0, buffs.Count);
        randomDeBuffId  = Random.Range(0, debuffs.Count);
        string randomBuff = buffs[randomBuffId];
        string randomDeBuff = debuffs[randomDeBuffId];
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
                attackFac = 0.15; //TTOD1使用表格
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

    // 应用减益效果的逻辑
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
