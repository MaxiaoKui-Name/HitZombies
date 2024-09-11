using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BuffDoorController : MonoBehaviour
{
    public TextMeshProUGUI buffText;
    public TextMeshProUGUI debuffText;
    public float moveSpeed = 1f; // 设置物体向下移动的速度
    public float hideYPosition = -40.01f; // 超出屏幕的Y坐标
    public bool isMove = false;
    void Start()
    {
        debuffText = transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>();
        buffText = transform.GetChild(1).GetComponentInChildren<TextMeshProUGUI>();
        // 遍历并激活所有子对象
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isMove)
        {
            MoveDown();
        }
        //if(transform.position.y < hideYPosition)
        //{
        //    HideAllChildren();
        //}
    }
    //private void OnTriggerEnter2D(Collider2D other)
    //{
    //    // 检查子弹是否碰到了敌人
    //    if (other.gameObject.layer == 8)  // 假设敌人处于Layer 6
    //    {
    //        TriggerSkill(); // 触发技能
    //    }
    //}
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
        //设置TTOD1buff技能
        buffText.text = "增加攻击力1";
        debuffText.text = "减少攻击力";
        GameManage.Instance.isPlaydoor = true;
    }
    private void TriggerSkill()
    {
        // 可以在这里添加应用技能效果的逻辑

    }
    private void MoveDown()
    {
        transform.Translate(Vector3.down * moveSpeed * Time.deltaTime);
    }

    // 隐藏所有子对象的方法
    public void HideAllChildren()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }
}
