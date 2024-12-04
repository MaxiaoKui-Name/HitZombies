using DG.Tweening;
using DragonBones;
using Spine;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;
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
#region[��������]
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
    #endregion
    /// <summary>
    /// ��ť��������Э��
    /// </summary>
    /// <param name="buttonRect">��ť�� RectTransform</param>
    /// <param name="onComplete">������ɺ�Ļص�</param>
 #region[��ť��������]
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
    #endregion

 #region[�����ť�������]
    protected void GetClickAnim(Transform uiobj)
    {
        if(clickAMature == null && clickArmature == null)
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
    #endregion

 #region[��ҵ���Ч��]
    protected IEnumerator AnimateCoins(Transform spwanPos, Transform target,GameObject DesObj)
    {
        // ���ؽ�� Prefab
        GameObject coinPrefab = Resources.Load<GameObject>("Prefabs/Coin/newgold");
        if (coinPrefab == null)
        {
            Debug.LogError("�Ҳ������Prefab��");
            yield break;
        }

        int coinCount = 5;
        for (int i = 0; i < coinCount; i++)
        {
            // ���ƫ������ʹ��ҴӲ�ͬλ�õ���
            Vector3 randomOffset = new Vector3(Random.Range(-50f, 50f), Random.Range(-50f, 50f), 0);
            Vector3 spawnPosition = spwanPos.position + randomOffset;

            GameObject coin = Instantiate(coinPrefab, spawnPosition, Quaternion.identity, transform.parent);
            // ���Ŷ���
            UnityArmatureComponent coinArmature = coin.transform.GetChild(0).GetComponent<UnityArmatureComponent>();
            if (coinArmature != null)
            {
                coinArmature.animation.Play("newAnimation", -1);
            }
            // ʹ�� DOTween �ƶ���ҵ�Сƫ��λ�ã�Ȼ�����Ŀ��λ��
            Vector3 intermediatePosition = spawnPosition + new Vector3(Random.Range(-80f, 80f), Random.Range(-80f, 80f), 0);

            float moveDuration1 = 0.25f;
            float moveDuration2 = 1f;

            Sequence sequence = DOTween.Sequence();
            sequence.Append(coin.transform.DOMove(intermediatePosition, moveDuration1).SetEase(Ease.OutQuad));
            sequence.Append(coin.transform.DOMove(target.position, moveDuration2).SetEase(Ease.InOutQuad));
            sequence.OnComplete(() =>
            {
                Destroy(coin);
            });

            yield return new WaitForSeconds(0.2f);
        }

        // ������ɺ����ٽ������
        yield return new WaitForSeconds(2f);
        Destroy(DesObj);
    }
    #endregion

#region[��ҵ���Ч��]
    // �Զ�������ֹ��������������� Slot ��
    private float displayedValue = 0f;
    protected IEnumerator AnimateRewardText(float targetValue, float displayregionValue, float duration, Text PlayText)
    {
        // ������ʾֵ
        displayedValue = displayregionValue;

        // ʹ��DOTween����һ����0��targetValue�Ķ���
        Tween tween = DOTween.To(() => displayedValue, x => {
            displayedValue = x;
            PlayText.text = Mathf.FloorToInt(displayedValue).ToString();
        }, targetValue, duration)
        .SetEase(Ease.OutCubic) // ʹ�û���������ʹ�����
        .SetUpdate(true); // ȷ��������Time.timeScaleΪ0ʱ��Ȼ���У������Ҫ��

        yield return tween.WaitForCompletion();
        // ȷ������ֵ׼ȷ
        PlayText.text = targetValue.ToString("N0");
    }
    private float displayedValueUGUI = 0f;
    protected IEnumerator AnimateRewardTextUGUI(float targetValue, float displayregionValue, float duration, TextMeshProUGUI PlayText)
    {
        // ������ʾֵ
        displayedValueUGUI = displayregionValue;

        // ʹ��DOTween����һ����0��targetValue�Ķ���
        Tween tween = DOTween.To(() => displayedValueUGUI, x => {
            displayedValueUGUI = x;
            PlayText.text = Mathf.FloorToInt(displayedValueUGUI).ToString();
        }, targetValue, duration)
        .SetEase(Ease.OutCubic) // ʹ�û���������ʹ�����
        .SetUpdate(true); // ȷ��������Time.timeScaleΪ0ʱ��Ȼ���У������Ҫ��

        yield return tween.WaitForCompletion();
        // ȷ������ֵ׼ȷ
        PlayText.text = targetValue.ToString("N0");
    }
    #endregion
}

