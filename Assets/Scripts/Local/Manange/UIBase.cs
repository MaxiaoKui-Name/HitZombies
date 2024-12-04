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

    // Panel Pop-Up Animation Settings
    public float popUpDuration = 0.5f; // Duration of pop-up animation
    public Vector3 popUpStartScale = new Vector3(0.8f, 0.8f, 0.8f); // Starting scale
    public Vector3 popUpEndScale = Vector3.one; // Target scale
    public float popUpOvershoot = 0.1f; // Overshoot amount

    // Click Animation References
    private GameObject clickAMature;
    private UnityArmatureComponent clickArmature;
    private bool animationFinished = false;

    void Awake()
    {
        // Initialize child dictionary if needed
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

    #region [Pop-Up Animation]
    protected IEnumerator PopUpAnimation(RectTransform panelRect)
    {
        float elapsedTime = 0f;
        Vector3 startScale = popUpStartScale;
        Vector3 overshootScale = popUpEndScale * (1 + popUpOvershoot);
        Vector3 endScale = popUpEndScale;

        // Step 1: Scale from start to overshoot
        while (elapsedTime < popUpDuration / 2)
        {
            float t = elapsedTime / (popUpDuration / 2);
            panelRect.localScale = Vector3.Lerp(startScale, overshootScale, t);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }
        panelRect.localScale = overshootScale;

        // Step 2: Scale from overshoot to end
        elapsedTime = 0f;
        while (elapsedTime < popUpDuration / 2)
        {
            float t = elapsedTime / (popUpDuration / 2);
            panelRect.localScale = Vector3.Lerp(overshootScale, endScale, t);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }
        panelRect.localScale = endScale;
    }
    #endregion

    /// <summary>
    /// Button Bounce Animation Coroutine
    /// </summary>
    /// <param name="buttonRect">Button's RectTransform</param>
    /// <param name="onComplete">Callback after animation completes</param>
    #region [Button Bounce Animation]
    protected IEnumerator ButtonBounceAnimation(RectTransform buttonRect, Action onComplete)
    {
        Vector3 originalScale = buttonRect.localScale;
        Vector3 targetScale = originalScale * 0.6f; // Scale down to 60%

        float elapsedTime = 0f;
        float animationDuration = 0.1f;

        // Scale down
        while (elapsedTime < animationDuration)
        {
            buttonRect.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / animationDuration);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }
        buttonRect.localScale = targetScale;

        // Scale up to original
        elapsedTime = 0f;
        while (elapsedTime < animationDuration)
        {
            buttonRect.localScale = Vector3.Lerp(targetScale, originalScale, elapsedTime / animationDuration);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }
        buttonRect.localScale = originalScale;

        // Execute click logic
        onComplete?.Invoke();
    }
    #endregion

    #region [Click Button Animation]
    protected void GetClickAnim(Transform uiobj)
    {
        if (clickAMature == null && clickArmature == null)
        {
            // Get ClickAMature object and its animation component
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
                        Debug.LogError("ClickAMature object is missing UnityArmatureComponent!");
                    }
                }
                else
                {
                    Debug.LogError("Child object named ClickAMature not found!");
                }
            }
            else
            {
                Debug.LogError("CheckUIPanelController has no parent object!");
            }
        }
    }

    /// <summary>
    /// Handle button click animation
    /// </summary>
    /// <param name="transformObj">UI Transform object</param>
    /// <returns></returns>
    protected IEnumerator HandleButtonClickAnimation(Transform transformObj)
    {
        // Get click position
        Vector3 clickPosition = Input.mousePosition;
        // Convert screen coordinates to Canvas local coordinates
        Canvas canvas = transformObj.parent.GetComponent<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("Parent Canvas not found!");
            yield break;
        }

        // Set ClickAMature position
        if (clickAMature != null)
        {
            clickAMature.transform.position = clickPosition;
            clickAMature.SetActive(true);
            // Ensure ClickAMature is the last sibling to appear on top
            clickAMature.transform.SetAsLastSibling();
        }

        // Play "click" animation
        if (clickArmature != null)
        {
            animationFinished = false;
            clickArmature.animation.Play("click", 1); // Assuming animation name is "click"
            // Hide ClickAMature after animation completes
            float animDuration = clickArmature.animation.GetState("click")._duration;
            StartCoroutine(WaitForClickAnim(animDuration));
        }
    }

    // For Slider Click Animation
    protected IEnumerator HandleButtonClickAnimationSlider(Vector3 clickPosition)
    {
        // Convert screen coordinates to Canvas local coordinates
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("Parent Canvas not found!");
            yield break;
        }

        // Set ClickAMature position
        if (clickAMature != null)
        {
            clickAMature.transform.position = clickPosition;
            clickAMature.SetActive(true);
            // Ensure ClickAMature is the last sibling to appear on top
            clickAMature.transform.SetAsLastSibling();
        }

        // Play "click" animation
        if (clickArmature != null)
        {
            animationFinished = false;
            clickArmature.animation.Play("click", 1); // Assuming animation name is "click"
            // Hide ClickAMature after animation completes
            float animDuration = clickArmature.animation.GetState("click")._duration;
            StartCoroutine(WaitForClickAnim(animDuration));
        }
    }

    // Wait for Click Animation to complete
    private IEnumerator WaitForClickAnim(float animationLength)
    {
        yield return new WaitForSecondsRealtime(animationLength);
        if (clickAMature != null)
        {
            clickAMature.SetActive(false);
        }
        animationFinished = true;
    }
    #endregion

    #region [Animate Coins]
    protected IEnumerator AnimateCoins(Transform spawnPos, Transform target, GameObject desObj)
    {
        // Load Coin Prefab
        GameObject coinPrefab = Resources.Load<GameObject>("Prefabs/Coin/newgold");
        if (coinPrefab == null)
        {
            Debug.LogError("Coin Prefab not found!");
            yield break;
        }

        int coinCount = 5;
        for (int i = 0; i < coinCount; i++)
        {
            // Random offset for spawning coins
            Vector3 randomOffset = new Vector3(Random.Range(-50f, 50f), Random.Range(-50f, 50f), 0);
            Vector3 spawnPosition = spawnPos.position + randomOffset;

            GameObject coin = Instantiate(coinPrefab, spawnPosition, Quaternion.identity, transform.parent);
            // Play animation
            UnityArmatureComponent coinArmature = coin.transform.GetChild(0).GetComponent<UnityArmatureComponent>();
            if (coinArmature != null)
            {
                coinArmature.animation.Play("newAnimation", -1);
            }

            // Intermediate random position
            Vector3 intermediatePosition = spawnPosition + new Vector3(Random.Range(-80f, 80f), Random.Range(-80f, 80f), 0);

            float moveDuration1 = 0.25f;
            float moveDuration2 = 1f;

            Sequence sequence = DOTween.Sequence();
            sequence.Append(coin.transform.DOMove(intermediatePosition, moveDuration1).SetEase(Ease.OutQuad).SetUpdate(true));
            sequence.Append(coin.transform.DOMove(target.position, moveDuration2).SetEase(Ease.InOutQuad).SetUpdate(true));
            sequence.OnComplete(() =>
            {
                Destroy(coin);
            });

            yield return new WaitForSecondsRealtime(0.2f);
        }

        // Wait for all coins to finish animation before destroying the reward panel
        yield return new WaitForSecondsRealtime(2f);
        Destroy(desObj);
    }
    #endregion

    #region [Animate Reward Text]
    // Custom number scrolling animation similar to a slot machine
    private float displayedValue = 0f;
    protected IEnumerator AnimateRewardText(float targetValue, float displayStartValue, float duration, Text playText)
    {
        // Reset displayed value
        displayedValue = displayStartValue;

        // Create a DOTween animation from displayStartValue to targetValue
        Tween tween = DOTween.To(() => displayedValue, x =>
        {
            displayedValue = x;
            playText.text = Mathf.FloorToInt(displayedValue).ToString();
        }, targetValue, duration)
        .SetEase(Ease.OutCubic) // Ease out for deceleration effect
        .SetUpdate(true); // Ensure animation runs regardless of time scale

        yield return tween.WaitForCompletion();

        // Ensure the final value is accurate
        playText.text = targetValue.ToString("N0");
    }

    private float displayedValueUGUI = 0f;
    protected IEnumerator AnimateRewardTextUGUI(float targetValue, float displayStartValue, float duration, TextMeshProUGUI playText)
    {
        // Reset displayed value
        displayedValueUGUI = displayStartValue;

        // Create a DOTween animation from displayStartValue to targetValue
        Tween tween = DOTween.To(() => displayedValueUGUI, x =>
        {
            displayedValueUGUI = x;
            playText.text = Mathf.FloorToInt(displayedValueUGUI).ToString();
        }, targetValue, duration)
        .SetEase(Ease.OutCubic) // Ease out for deceleration effect
        .SetUpdate(true); // Ensure animation runs regardless of time scale

        yield return tween.WaitForCompletion();

        // Ensure the final value is accurate
        playText.text = targetValue.ToString("N0");
    }
    #endregion
}
