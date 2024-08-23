using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    public float scrollSpeed = 2f;  // 背景滚动的速度
    public float backgroundHeight;  // 背景的高度（根据背景图片的实际高度调整）

    private Transform[] backgrounds;  // 用来存储所有的背景对象

    void OnEnable()
    {
        // 获取所有的子背景对象
        int backgroundCount = transform.childCount;
        backgrounds = new Transform[backgroundCount];
        for (int i = 0; i < backgroundCount; i++)
        {
            backgrounds[i] = transform.GetChild(i);
            //设置背景
        }
        backgroundHeight = backgrounds[0].GetComponent<SpriteRenderer>().bounds.size.y;
    }

    void Update()
    {
        if (!LevelManager.Instance.isLoadBack)
            return;
        // 移动每个背景对象
        foreach (Transform background in backgrounds)
        {
            background.Translate(Vector2.down * scrollSpeed * Time.deltaTime);

            // 如果背景移出视野（在Y轴上超出屏幕下方），将其重置到队列的最上方
            if (background.position.y <= -backgroundHeight)
            {
                MoveToTop(background);
            }
        }
    }

    void MoveToTop(Transform background)
    {
        // 找到所有背景中的最高点，将当前背景移到该位置上方
        float highestY = backgrounds[0].position.y;
        foreach (Transform bg in backgrounds)
        {
            if (bg.position.y > highestY)
            {
                highestY = bg.position.y;
            }
        }

        // 重置背景位置到最高点上方
        background.position = new Vector2(background.position.x, highestY + backgroundHeight);
    }
    // 新增的方法，用于设置背景
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
        // 更新背景高度
        backgroundHeight = backgrounds[0].GetComponent<SpriteRenderer>().bounds.size.y;
    }
}
