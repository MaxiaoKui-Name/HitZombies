using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    public float scrollSpeed = 2f;  // �����������ٶ�
    public float backgroundHeight;  // �����ĸ߶ȣ����ݱ���ͼƬ��ʵ�ʸ߶ȵ�����

    private Transform[] backgrounds;  // �����洢���еı�������

    void OnEnable()
    {
        // ��ȡ���е��ӱ�������
        int backgroundCount = transform.childCount;
        backgrounds = new Transform[backgroundCount];
        for (int i = 0; i < backgroundCount; i++)
        {
            backgrounds[i] = transform.GetChild(i);
            //���ñ���
        }
        backgroundHeight = backgrounds[0].GetComponent<SpriteRenderer>().bounds.size.y;
    }

    void Update()
    {
        if (!LevelManager.Instance.isLoadBack)
            return;
        // �ƶ�ÿ����������
        foreach (Transform background in backgrounds)
        {
            background.Translate(Vector2.down * scrollSpeed * Time.deltaTime);

            // ��������Ƴ���Ұ����Y���ϳ�����Ļ�·������������õ����е����Ϸ�
            if (background.position.y <= -backgroundHeight)
            {
                MoveToTop(background);
            }
        }
    }

    void MoveToTop(Transform background)
    {
        // �ҵ����б����е���ߵ㣬����ǰ�����Ƶ���λ���Ϸ�
        float highestY = backgrounds[0].position.y;
        foreach (Transform bg in backgrounds)
        {
            if (bg.position.y > highestY)
            {
                highestY = bg.position.y;
            }
        }

        // ���ñ���λ�õ���ߵ��Ϸ�
        background.position = new Vector2(background.position.x, highestY + backgroundHeight);
    }
    // �����ķ������������ñ���
    public void SetBackground(Sprite backgroundSprite)
    {
        foreach (Transform background in backgrounds)
        {
            SpriteRenderer sr = background.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sprite = backgroundSprite;
            }
        }
        // ���±����߶�
        backgroundHeight = backgrounds[0].GetComponent<SpriteRenderer>().bounds.size.y;
    }
}
