using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BuffDoorController : MonoBehaviour
{
    public TextMeshProUGUI buffText;
    public TextMeshProUGUI debuffText;
    public float moveSpeed = 1f; // �������������ƶ����ٶ�
    public float hideYPosition = -40.01f; // ������Ļ��Y����
    public bool isMove = false;
    void Start()
    {
        debuffText = transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>();
        buffText = transform.GetChild(1).GetComponentInChildren<TextMeshProUGUI>();
        // ���������������Ӷ���
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
    //    // ����ӵ��Ƿ������˵���
    //    if (other.gameObject.layer == 8)  // ������˴���Layer 6
    //    {
    //        TriggerSkill(); // ��������
    //    }
    //}
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
        //����TTOD1buff����
        buffText.text = "���ӹ�����1";
        debuffText.text = "���ٹ�����";
        GameManage.Instance.isPlaydoor = true;
    }
    private void TriggerSkill()
    {
        // �������������Ӧ�ü���Ч�����߼�

    }
    private void MoveDown()
    {
        transform.Translate(Vector3.down * moveSpeed * Time.deltaTime);
    }

    // ���������Ӷ���ķ���
    public void HideAllChildren()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }
}
