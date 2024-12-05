using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class RedBoxButtonHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private ChooseGunPanelController chooseGunPanelController;
    private bool isPointerDown = false;
    private float pointerDownTimer = 0f;
    public float requiredHoldTime = 1f; // 需要长按的时间

    public void Initialize(ChooseGunPanelController controller)
    {
        chooseGunPanelController = controller;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPointerDown = true;
        pointerDownTimer = 0f;
        StartCoroutine(HandleLongPress());
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPointerDown = false;
    }

    private IEnumerator HandleLongPress()
    {
        while (isPointerDown)
        {
            pointerDownTimer += Time.deltaTime;
            if (pointerDownTimer >= requiredHoldTime)
            {
                chooseGunPanelController.OnRedBoxBtnLongPressed();
                yield break;
            }
            yield return null;
        }
    }
}
