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
    //面板弹跳效果
    public float popUpDuration = 0.5f; // 弹出动画持续时间
    public Vector3 popUpStartScale = new Vector3(0.8f, 0.8f, 0.8f); // 起始缩放
    public Vector3 popUpEndScale = Vector3.one; // 目标缩放
    public float popUpOvershoot = 0.1f; // 弹跳幅度

    //点击动画
    // 新增：引用 ClickAMature 及其动画组件
    private GameObject clickAMature;
    private UnityArmatureComponent clickArmature;
    // 用于跟踪动画是否完成
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
#region[弹窗动画]
    protected IEnumerator PopUpAnimation(RectTransform panelRect)
    {
        float elapsedTime = 0f;
        Vector3 startScale = popUpStartScale;
        Vector3 overshootScale = popUpEndScale * (1 + popUpOvershoot);
        Vector3 endScale = popUpEndScale;

        // 第一步：从起始缩放放大到超过目标缩放（弹跳阶段）
        while (elapsedTime < popUpDuration / 2)
        {
            float t = elapsedTime / (popUpDuration / 2);
            panelRect.localScale = Vector3.Lerp(startScale, overshootScale, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        panelRect.localScale = overshootScale;

        // 第二步：从弹跳缩放回到目标缩放
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
    /// 按钮弹跳动画协程
    /// </summary>
    /// <param name="buttonRect">按钮的 RectTransform</param>
    /// <param name="onComplete">动画完成后的回调</param>
 #region[按钮弹跳动画]
    protected IEnumerator ButtonBounceAnimation(RectTransform buttonRect, Action onComplete)
    {
        Vector3 originalScale = buttonRect.localScale;
        Vector3 targetScale = originalScale * 0.6f; // 缩小到90%

        // 缩小动画
        float elapsedTime = 0f;
        float animationDuration = 0.1f;
        while (elapsedTime < animationDuration)
        {
            buttonRect.localScale = Vector3.Lerp(originalScale, targetScale, (elapsedTime / animationDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        buttonRect.localScale = targetScale;

        // 放大回原始大小
        elapsedTime = 0f;
        while (elapsedTime < animationDuration)
        {
            buttonRect.localScale = Vector3.Lerp(targetScale, originalScale, (elapsedTime / animationDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        buttonRect.localScale = originalScale;

        // 执行点击逻辑
        onComplete?.Invoke();
    }
    #endregion

 #region[点击按钮点击动画]
    protected void GetClickAnim(Transform uiobj)
    {
        if(clickAMature == null && clickArmature == null)
        {
            // 获取 ClickAMature 对象及其动画组件
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
                        Debug.LogError("ClickAMature 对象缺少 UnityArmatureComponent 组件！");
                    }
                }
                else
                {
                    Debug.LogError("未找到名为 ClickAMature 的子对象！");
                }
            }
            else
            {
                Debug.LogError("CheckUIPanelController 没有父物体！");
            }
        }
       
    }


    /// <summary>
    /// 处理按钮点击的点击动画
    /// </summary>
    /// <param name="transformObj">UI Transform 对象</param>
    /// <returns></returns>
    protected IEnumerator HandleButtonClickAnimation(Transform transformObj)
    {
        // 获取点击位置
        Vector3 clickPosition = Input.mousePosition;
        // 将屏幕坐标转换为 Canvas 本地坐标
        Canvas canvas = transformObj.parent.GetComponent<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("找不到父 Canvas！");
            yield break;
        }

        // 设置 ClickAMature 的位置
        if (clickAMature != null)
        {
            //RectTransform clickRect = clickAMature.GetComponent<RectTransform>();
            if (clickAMature != null)
            {
                clickAMature.transform.position = clickPosition;
                clickAMature.SetActive(true);
                // 将 clickAMature 设置为父物体的最后一个子物体，确保在最前面
                clickAMature.transform.SetAsLastSibling();
            }
            else
            {
                Debug.LogError("ClickAMature 缺少 RectTransform 组件！");
            }
        }

        // 播放 "click" 动画
        if (clickArmature != null)
        {
            animationFinished = false;
            clickArmature.animation.Play("click", 1);  // 假设动画名为 "click"
            // 播放完动画后隐藏 ClickAMature
            StartCoroutine(WaitForClickAnim(clickArmature.animation.GetState("click")._duration));
        }
    }

    // 等待 ClickAnim_F 动画完成
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

 #region[金币弹跳效果]
    protected IEnumerator AnimateCoins(Transform spwanPos, Transform target,GameObject DesObj)
    {
        // 加载金币 Prefab
        GameObject coinPrefab = Resources.Load<GameObject>("Prefabs/Coin/newgold");
        if (coinPrefab == null)
        {
            Debug.LogError("找不到金币Prefab！");
            yield break;
        }

        int coinCount = 5;
        for (int i = 0; i < coinCount; i++)
        {
            // 随机偏移量，使金币从不同位置弹出
            Vector3 randomOffset = new Vector3(Random.Range(-50f, 50f), Random.Range(-50f, 50f), 0);
            Vector3 spawnPosition = spwanPos.position + randomOffset;

            GameObject coin = Instantiate(coinPrefab, spawnPosition, Quaternion.identity, transform.parent);
            // 播放动画
            UnityArmatureComponent coinArmature = coin.transform.GetChild(0).GetComponent<UnityArmatureComponent>();
            if (coinArmature != null)
            {
                coinArmature.animation.Play("newAnimation", -1);
            }
            // 使用 DOTween 移动金币到小偏移位置，然后飞向目标位置
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

        // 动画完成后销毁奖励面板
        yield return new WaitForSeconds(2f);
        Destroy(DesObj);
    }
    #endregion

#region[金币弹跳效果]
    // 自定义的数字滚动动画，类似于 Slot 机
    private float displayedValue = 0f;
    protected IEnumerator AnimateRewardText(float targetValue, float displayregionValue, float duration, Text PlayText)
    {
        // 重置显示值
        displayedValue = displayregionValue;

        // 使用DOTween创建一个从0到targetValue的动画
        Tween tween = DOTween.To(() => displayedValue, x => {
            displayedValue = x;
            PlayText.text = Mathf.FloorToInt(displayedValue).ToString();
        }, targetValue, duration)
        .SetEase(Ease.OutCubic) // 使用缓出动画，使其减速
        .SetUpdate(true); // 确保动画在Time.timeScale为0时仍然运行（如果需要）

        yield return tween.WaitForCompletion();
        // 确保最终值准确
        PlayText.text = targetValue.ToString("N0");
    }
    private float displayedValueUGUI = 0f;
    protected IEnumerator AnimateRewardTextUGUI(float targetValue, float displayregionValue, float duration, TextMeshProUGUI PlayText)
    {
        // 重置显示值
        displayedValueUGUI = displayregionValue;

        // 使用DOTween创建一个从0到targetValue的动画
        Tween tween = DOTween.To(() => displayedValueUGUI, x => {
            displayedValueUGUI = x;
            PlayText.text = Mathf.FloorToInt(displayedValueUGUI).ToString();
        }, targetValue, duration)
        .SetEase(Ease.OutCubic) // 使用缓出动画，使其减速
        .SetUpdate(true); // 确保动画在Time.timeScale为0时仍然运行（如果需要）

        yield return tween.WaitForCompletion();
        // 确保最终值准确
        PlayText.text = targetValue.ToString("N0");
    }
    #endregion
}

