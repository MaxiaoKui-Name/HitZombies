using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffdoorupController : MonoBehaviour
{
    public bool hasTriggered;
    public float moveSpeed = 1f; // 设置物体向下移动的速度
    public float hideYPosition = -10f; // 超出屏幕的Y坐标
    public bool isMove = false;
    private void OnEnable()
    {
        hasTriggered = false;
        isMove = true;
    }
    void Update()
    {
        if (isMove)
        {
            MoveDown();
           // FollowParentObject();
        }

        if (transform.position.y < hideYPosition)
        {
            HideAllChildren();
        }
    }
    //private void FollowParentObject()
    //{
    //    if (healthBarCanvas != null)
    //    {
    //        // 将血条设置为玩家头顶的位置 (世界坐标)
    //        healthBarCanvas.position = transform.position;  // 1f 为Y轴的偏移量
    //        healthBarCanvas.localScale = new Vector3(0.01f, 0.01f, 0.01f);  // 调整血条的缩放，使其适应场景
    //    }
    //}
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 检查子弹是否碰到了玩家
        if (other.gameObject.layer == 8 && !hasTriggered)
        {
            hasTriggered = true;
            // transform.GetComponent<SortingGroup>().sortingLayerName = "Partical";
            Debug.Log("玩家通过强力门！！！！");
            transform.position = new Vector3(transform.position.x, transform.position.y, -0.1f) ;
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
        //transform.GetComponent<SortingGroup>().sortingLayerName = "Default";
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
            isMove = false;
        }
        Destroy(gameObject);
    }
}
