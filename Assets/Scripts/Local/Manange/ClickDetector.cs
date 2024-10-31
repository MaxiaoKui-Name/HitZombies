using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// 新增辅助类：ClickDetector
public class ClickDetector : MonoBehaviour
{
    public event Action OnClick;
    void Update()
    {
        // 鼠标点击
        if (Input.GetMouseButtonDown(0))
        {
            OnClick?.Invoke();
        }

        // 触摸点击
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            OnClick?.Invoke();
        }
    }
}

