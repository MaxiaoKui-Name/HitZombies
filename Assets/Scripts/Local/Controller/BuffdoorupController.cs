using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffdoorupController : MonoBehaviour
{
    public bool hasTriggered;
    public float moveSpeed = 1f; // �������������ƶ����ٶ�
    public float hideYPosition = -10f; // ������Ļ��Y����
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
    //        // ��Ѫ������Ϊ���ͷ����λ�� (��������)
    //        healthBarCanvas.position = transform.position;  // 1f ΪY���ƫ����
    //        healthBarCanvas.localScale = new Vector3(0.01f, 0.01f, 0.01f);  // ����Ѫ�������ţ�ʹ����Ӧ����
    //    }
    //}
    private void OnTriggerEnter2D(Collider2D other)
    {
        // ����ӵ��Ƿ����������
        if (other.gameObject.layer == 8 && !hasTriggered)
        {
            hasTriggered = true;
            // transform.GetComponent<SortingGroup>().sortingLayerName = "Partical";
            Debug.Log("���ͨ��ǿ���ţ�������");
            transform.position = new Vector3(transform.position.x, transform.position.y, -0.1f) ;
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
        //transform.GetComponent<SortingGroup>().sortingLayerName = "Default";
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
            isMove = false;
        }
        Destroy(gameObject);
    }
}
