using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FourNoteFController : MonoBehaviour
{
    // 抖动幅度，可以根据需要增大
    public float shakeMagnitude = 40f;

    // 抖动的持续时间
    public float shakeDuration = 0.3f;

    // 抖动之间的间隔时间
    public float shakeInterval = 0.2f;

    // 原始位置
    private Vector3 originalPosition;

    void OnEnable()
    {
        // 记录弹窗的原始位置
        originalPosition = transform.localPosition;
        // 开始抖动效果
        StartCoroutine(Shake());
    }

    IEnumerator Shake()
    {
        Vector3 originalPosition = transform.GetComponent<RectTransform>().anchoredPosition;
        float halfDuration = shakeDuration / 2f;
        float timer = 0f;

        // 生成一个随机方向的偏移量，确保在所有轴上都有变化
        Vector3 randomDirection = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f)
        ).normalized;

        Vector3 offset = randomDirection * shakeMagnitude;

        // 从原位置移动到偏移位置
        while (timer < halfDuration)
        {
            float t = timer / halfDuration;
            transform.GetComponent<RectTransform>().anchoredPosition = Vector3.Lerp(originalPosition, originalPosition + offset, t);
            timer += Time.unscaledDeltaTime;
            yield return null;
        }
        // 确保达到偏移位置
        transform.GetComponent<RectTransform>().anchoredPosition = originalPosition + offset;

        // 从偏移位置返回到原位置
        timer = 0f;
        while (timer < halfDuration)
        {
            float t = timer / halfDuration;
            transform.GetComponent<RectTransform>().anchoredPosition = Vector3.Lerp(originalPosition + offset, originalPosition, t);
            timer += Time.unscaledDeltaTime;
            yield return null;
        }
        // 确保回到原位置
        transform.GetComponent<RectTransform>().anchoredPosition = originalPosition;
    }
}
