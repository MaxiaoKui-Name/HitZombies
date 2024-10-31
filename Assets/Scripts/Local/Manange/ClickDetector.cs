using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// ���������ࣺClickDetector
public class ClickDetector : MonoBehaviour
{
    public event Action OnClick;
    void Update()
    {
        // �����
        if (Input.GetMouseButtonDown(0))
        {
            OnClick?.Invoke();
        }

        // �������
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            OnClick?.Invoke();
        }
    }
}

