using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Pool;

public class Gold : MonoBehaviour
{
    private CancellationTokenSource _cts;

    //private Transform coinTargetPos;

    void OnEnable()
    {
        // ��ʼ�� CancellationTokenSource
        _cts = new CancellationTokenSource();

        //coinTargetPos = GameObject.Find("CointargetPos").transform;
    }

    private void Update()
    {
        if (GameManage.Instance.gameState != GameState.Running)
        {
            // ȡ�����е��첽����
            _cts.Cancel();
            string CoinName = "gold";
            if (PreController.Instance.CoinPools.TryGetValue(CoinName, out var selectedCoinPool))
            {
                transform.gameObject.SetActive(false);
                selectedCoinPool.Release(transform.gameObject);
                PlayInforManager.Instance.playInfor.AddCoins(1);
            }
        }
    }

    private void OnDestroy()
    {
        // ȷ��������ʱȡ�����������ͷ� CTS
        if (_cts != null)
        {
            _cts.Cancel();
            _cts.Dispose();
            _cts = null;
        }
    }

    public async UniTask AwaitMove(ObjectPool<GameObject> CoinPool, Transform coinTargetPos)
    {
        await MoveCoinToUI(CoinPool, coinTargetPos, _cts.Token);
    }

    public async UniTask MoveCoinToUI(ObjectPool<GameObject> CoinPool, Transform coinTargetPos, CancellationToken token)
    {
        float duration = 1f;
        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = coinTargetPos.position;

        try
        {
            while (elapsedTime < duration)
            {
                // ���ȡ������
                token.ThrowIfCancellationRequested();

                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / duration);
                Vector3 currentPosition = Vector3.Lerp(startPosition, targetPosition, t);
                currentPosition.z = -0.1f;
                transform.position = currentPosition;
                await UniTask.Yield(token);
            }

            // ȷ��������Ȼ�����ʱ���Ѵﵽ
            if (gameObject.activeSelf && elapsedTime >= duration)
            {
                transform.gameObject.SetActive(false);
                CoinPool.Release(transform.gameObject);
                PlayInforManager.Instance.playInfor.AddCoins(1);
            }
        }
        catch (OperationCanceledException)
        {
            // �첽����ȡ�������������ﴦ�������߼�
            Debug.Log($"MoveCoinToUI canceled for {gameObject.name}");
        }
    }

    // ǩ���ϵĽ����UI�ƶ�
    public async UniTask StartMoveCoin(RectTransform coinRect, Vector2 targetPosition, float duration)
    {
        Vector2 startPos = coinRect.anchoredPosition;
        float elapsed = 0f;

        try
        {
            while (elapsed < duration)
            {
                // ���ȡ������
                _cts.Token.ThrowIfCancellationRequested();

                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                coinRect.anchoredPosition = Vector2.Lerp(startPos, targetPosition, t);
                await UniTask.Yield(_cts.Token);
            }
            // ��ѡ���ƶ���ɺ�Ĵ����������ٽ��
            Destroy(gameObject.transform.parent.gameObject);
        }
        catch (OperationCanceledException)
        {
            // �첽����ȡ�������������ﴦ�������߼�
            Debug.Log($"StartMoveCoin canceled for {gameObject.name}");
        }
    }

    // UI��ʵ�ֽ�Ҷ���
    public async UniTask DragonMoveCoin(GameObject coin, Vector3 targetPosition, float duration)
    {
        // await DragonCoinMove(coin, targetPosition, duration);
    }

    //public async UniTask DragonCoinMove(GameObject coinObj, Vector3 endPosition, float duration)
    //{
    //    RectTransform coinRect = coinObj.GetComponent<RectTransform>();
    //    if (coinRect == null)
    //    {
    //        Debug.LogError("GoldCanvas Ԥ����ȱ�� RectTransform �����");
    //    }
    //    // ����������ת��Ϊ�����ľֲ�����
    //    Vector2 localEndPosition;
    //    RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect,
    //        RectTransformUtility.WorldToScreenPoint(uiCamera, endPosition),
    //        uiCamera, out localEndPosition);

    //    Vector2 startPosition = coinRect.anchoredPosition;
    //    Vector2 targetPosition = localEndPosition;

    //    float elapsed = 0f;
    //    try
    //    {
    //        while (elapsed < duration)
    //        {
    //            // ���ȡ������
    //            _cts.Token.ThrowIfCancellationRequested();

    //            elapsed += Time.deltaTime;
    //            float t = Mathf.Clamp01(elapsed / duration);
    //            coinRect.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, t);
    //            await UniTask.Yield(_cts.Token);
    //        }
    //        coinRect.anchoredPosition = targetPosition;
    //        // �������������ٽ�Ҷ���
    //        Destroy(coinObj);
    //    }
    //    catch (OperationCanceledException)
    //    {
    //        Debug.Log($"DragonCoinMove canceled for {coinObj.name}");
    //    }
    //}
}
