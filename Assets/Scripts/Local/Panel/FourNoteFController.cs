using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FourNoteFController : MonoBehaviour
{
    // �������ȣ����Ը�����Ҫ����
    public float shakeMagnitude = 40f;

    // �����ĳ���ʱ��
    public float shakeDuration = 0.3f;

    // ����֮��ļ��ʱ��
    public float shakeInterval = 0.2f;

    // ԭʼλ��
    private Vector3 originalPosition;

    void OnEnable()
    {
        // ��¼������ԭʼλ��
        originalPosition = transform.localPosition;
        // ��ʼ����Ч��
        StartCoroutine(Shake());
    }

    IEnumerator Shake()
    {
        Vector3 originalPosition = transform.GetComponent<RectTransform>().anchoredPosition;
        float halfDuration = shakeDuration / 2f;
        float timer = 0f;

        // ����һ����������ƫ������ȷ�����������϶��б仯
        Vector3 randomDirection = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f)
        ).normalized;

        Vector3 offset = randomDirection * shakeMagnitude;

        // ��ԭλ���ƶ���ƫ��λ��
        while (timer < halfDuration)
        {
            float t = timer / halfDuration;
            transform.GetComponent<RectTransform>().anchoredPosition = Vector3.Lerp(originalPosition, originalPosition + offset, t);
            timer += Time.unscaledDeltaTime;
            yield return null;
        }
        // ȷ���ﵽƫ��λ��
        transform.GetComponent<RectTransform>().anchoredPosition = originalPosition + offset;

        // ��ƫ��λ�÷��ص�ԭλ��
        timer = 0f;
        while (timer < halfDuration)
        {
            float t = timer / halfDuration;
            transform.GetComponent<RectTransform>().anchoredPosition = Vector3.Lerp(originalPosition + offset, originalPosition, t);
            timer += Time.unscaledDeltaTime;
            yield return null;
        }
        // ȷ���ص�ԭλ��
        transform.GetComponent<RectTransform>().anchoredPosition = originalPosition;
    }
}
