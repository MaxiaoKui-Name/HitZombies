using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class RedBoxButtonHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public event Action OnLongPress;

    private bool isPressed = false;
    private float pressDuration = 0.5f; // 长按时间阈值
    private float timer = 0f;

    private GameMainPanelController controller;

    public void Initialize(GameMainPanelController ctrl)
    {
        controller = ctrl;
    }

    void Update()
    {
        if (isPressed)
        {
            timer += Time.unscaledDeltaTime; // 使用非缩放时间
            if (timer >= pressDuration)
            {
                isPressed = false;
                timer = 0f;
                Debug.Log("Long press detected");
                OnLongPress?.Invoke();
                // 执行长按后的操作
                gameObject.SetActive(false); // 长按后隐藏按钮
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Pointer Down on RedBoxBtn_F");
        isPressed = true;
        timer = 0f;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("Pointer Up on RedBoxBtn_F");
        isPressed = false;
        timer = 0f;
    }
}
