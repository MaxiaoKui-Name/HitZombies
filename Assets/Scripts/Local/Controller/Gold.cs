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

        EventDispatcher.instance.Regist(EventNameDef.GAME_OVER, (v) => RecycleGold());
        //coinTargetPos = GameObject.Find("CointargetPos").transform;
    }

    private void RecycleGold()
    {
        // ȡ�����е��첽����
        _cts.Cancel();
        string CoinName = "gold";
        if (PreController.Instance.CoinPools.TryGetValue(CoinName, out var selectedCoinPool))
        {
            transform.gameObject.SetActive(false);
            // ȷ��������ʱȡ�����������ͷ� CTS
            if (_cts != null)
            {
                _cts.Cancel();
                _cts.Dispose();
                _cts = null;
            }
            selectedCoinPool.Release(transform.gameObject);
            PlayInforManager.Instance.playInfor.AddCoins(1);
        }
}


    public async UniTask AwaitMove(ObjectPool<GameObject> CoinPool, Vector3 coinTargetPos)
    {
        await MoveCoinToUI(CoinPool, coinTargetPos, _cts.Token);
    }
    public async UniTask AwaitMovePanel(Vector3 coinTargetPos, float duration)
    {
        await MoveCoinToPanel(coinTargetPos, duration);
    }

    public async UniTask MoveCoinToUI(ObjectPool<GameObject> CoinPool, Vector3 coinTargetPos, CancellationToken token)
    {
        float duration = 1f;
        float elapsedTime = 0f;
        Vector3 startPosition = transform.GetComponent<RectTransform>().anchoredPosition;
        Vector3 targetPosition = coinTargetPos;

        try
        {
            while (elapsedTime < duration)
            {
                // ���ȡ������
                token.ThrowIfCancellationRequested();

                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / duration);
                Vector3 currentPosition = Vector3.Lerp(startPosition, targetPosition, t);
                transform.GetComponent<RectTransform>().anchoredPosition = currentPosition;
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
    public async UniTask MoveCoinToPanel(Vector3 coinTargetPos, float duration)
    {
        float elapsedTime = 0f;
        Vector3 startPosition = transform.GetComponent<RectTransform>().anchoredPosition;
        Vector3 targetPosition = coinTargetPos;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            Vector3 currentPosition = Vector3.Lerp(startPosition, targetPosition, t);
            transform.GetComponent<RectTransform>().anchoredPosition = currentPosition;
            await UniTask.Yield();
        }
        // ȷ��������Ȼ�����ʱ���Ѵﵽ
        if (gameObject.activeSelf && elapsedTime >= duration)
        {
            transform.gameObject.SetActive(false);
            //PlayInforManager.Instance.playInfor.AddCoins(1);
            Destroy(gameObject);
        }
    }
}
