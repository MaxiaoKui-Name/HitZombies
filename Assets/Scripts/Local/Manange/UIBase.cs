using DragonBones;
using Spine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Transform = UnityEngine.Transform;

public class UIBase : MonoBehaviour
{
    public Dictionary<string, Transform> childDic = new Dictionary<string, Transform>();
    public bool isPlayWorldRankNew = false;
    public PlayerInfo playerInfo;
    //��嵯��Ч��
    public float popUpDuration = 0.5f; // ������������ʱ��
    public Vector3 popUpStartScale = new Vector3(0.8f, 0.8f, 0.8f); // ��ʼ����
    public Vector3 popUpEndScale = Vector3.one; // Ŀ������
    public float popUpOvershoot = 0.1f; // ��������

    //�������
    // ���������� ClickAMature ���䶯�����
    private GameObject clickAMature;
    private UnityArmatureComponent clickArmature;
    // ���ڸ��ٶ����Ƿ����
    private bool animationFinished = false;
    void Awake()
    {

    }
    protected void GetAllChild(Transform obj)
    {
        foreach (Transform item in obj)
        {
            if (item.name.Contains("_F"))
            {
                childDic.Add(item.name, item);
            }
            if (item.childCount > 0)
            {
                GetAllChild(item);
            }
        }
    }
    protected IEnumerator PopUpAnimation(RectTransform panelRect)
    {
        float elapsedTime = 0f;
        Vector3 startScale = popUpStartScale;
        Vector3 overshootScale = popUpEndScale * (1 + popUpOvershoot);
        Vector3 endScale = popUpEndScale;

        // ��һ��������ʼ���ŷŴ󵽳���Ŀ�����ţ������׶Σ�
        while (elapsedTime < popUpDuration / 2)
        {
            float t = elapsedTime / (popUpDuration / 2);
            panelRect.localScale = Vector3.Lerp(startScale, overshootScale, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        panelRect.localScale = overshootScale;

        // �ڶ������ӵ������Żص�Ŀ������
        elapsedTime = 0f;
        while (elapsedTime < popUpDuration / 2)
        {
            float t = elapsedTime / (popUpDuration / 2);
            panelRect.localScale = Vector3.Lerp(overshootScale, endScale, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        panelRect.localScale = endScale;
    }

    /// <summary>
    /// ��ť��������Э��
    /// </summary>
    /// <param name="buttonRect">��ť�� RectTransform</param>
    /// <param name="onComplete">������ɺ�Ļص�</param>
    protected IEnumerator ButtonBounceAnimation(RectTransform buttonRect, Action onComplete)
    {
        Vector3 originalScale = buttonRect.localScale;
        Vector3 targetScale = originalScale * 0.6f; // ��С��90%

        // ��С����
        float elapsedTime = 0f;
        float animationDuration = 0.1f;
        while (elapsedTime < animationDuration)
        {
            buttonRect.localScale = Vector3.Lerp(originalScale, targetScale, (elapsedTime / animationDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        buttonRect.localScale = targetScale;

        // �Ŵ��ԭʼ��С
        elapsedTime = 0f;
        while (elapsedTime < animationDuration)
        {
            buttonRect.localScale = Vector3.Lerp(targetScale, originalScale, (elapsedTime / animationDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        buttonRect.localScale = originalScale;

        // ִ�е���߼�
        onComplete?.Invoke();
    }

    protected void GetClickAnim(Transform uiobj)
    {
        // ��ȡ ClickAMature �����䶯�����
        Transform parentTransform = uiobj.parent;
        if (parentTransform != null)
        {
            Transform clickAMatureTransform = parentTransform.Find("ClickAMature");
            if (clickAMatureTransform != null)
            {
                clickAMature = clickAMatureTransform.gameObject;
                clickArmature = clickAMature.GetComponent<UnityArmatureComponent>();
                if (clickArmature == null)
                {
                    Debug.LogError("ClickAMature ����ȱ�� UnityArmatureComponent �����");
                }
            }
            else
            {
                Debug.LogError("δ�ҵ���Ϊ ClickAMature ���Ӷ���");
            }
        }
        else
        {
            Debug.LogError("CheckUIPanelController û�и����壡");
        }
    }


    /// <summary>
    /// ����ť����ĵ������
    /// </summary>
    /// <param name="transformObj">UI Transform ����</param>
    /// <returns></returns>
    protected IEnumerator HandleButtonClickAnimation(Transform transformObj)
    {
        // ��ȡ���λ��
        Vector3 clickPosition = Input.mousePosition;
        // ����Ļ����ת��Ϊ Canvas ��������
        Canvas canvas = transformObj.parent.GetComponent<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("�Ҳ����� Canvas��");
            yield break;
        }

        // ���� ClickAMature ��λ��
        if (clickAMature != null)
        {
            //RectTransform clickRect = clickAMature.GetComponent<RectTransform>();
            if (clickAMature != null)
            {
                clickAMature.transform.position = clickPosition;
                clickAMature.SetActive(true);
                // �� clickAMature ����Ϊ����������һ�������壬ȷ������ǰ��
                clickAMature.transform.SetAsLastSibling();
            }
            else
            {
                Debug.LogError("ClickAMature ȱ�� RectTransform �����");
            }
        }

        // ���� "click" ����
        if (clickArmature != null)
        {
            animationFinished = false;
            clickArmature.animation.Play("click", 1);  // ���趯����Ϊ "click"
            // �����궯�������� ClickAMature
            StartCoroutine(WaitForClickAnim(clickArmature.animation.GetState("click")._duration));
        }
    }

    // �ȴ� ClickAnim_F �������
    private IEnumerator WaitForClickAnim(float animationLength)
    {
        yield return new WaitForSeconds(animationLength);
        if (clickAMature != null)
        {
            clickAMature.SetActive(false);
        }
        animationFinished = true;
    }
}
    
