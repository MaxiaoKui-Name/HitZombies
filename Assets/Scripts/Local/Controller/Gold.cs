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
        // 初始化 CancellationTokenSource
        _cts = new CancellationTokenSource();

        EventDispatcher.instance.Regist(EventNameDef.GAME_OVER, (v) => RecycleGold());
        //coinTargetPos = GameObject.Find("CointargetPos").transform;
    }

    private void RecycleGold()
    {
        // 取消所有的异步任务
        _cts.Cancel();
        string CoinName = "gold";
        if (PreController.Instance.CoinPools.TryGetValue(CoinName, out var selectedCoinPool))
        {
            transform.gameObject.SetActive(false);
            // 确保在销毁时取消所有任务并释放 CTS
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
                // 检查取消请求
                token.ThrowIfCancellationRequested();

                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / duration);
                Vector3 currentPosition = Vector3.Lerp(startPosition, targetPosition, t);
                transform.GetComponent<RectTransform>().anchoredPosition = currentPosition;
                await UniTask.Yield(token);
            }
            // 确保对象仍然激活并且时间已达到
            if (gameObject.activeSelf && elapsedTime >= duration)
            {
                transform.gameObject.SetActive(false);
                CoinPool.Release(transform.gameObject);
                PlayInforManager.Instance.playInfor.AddCoins(1);
            }
        }
        catch (OperationCanceledException)
        {
            // 异步任务被取消，可以在这里处理清理逻辑
            Debug.Log($"MoveCoinToUI canceled for {gameObject.name}");
        }
    }

    // 签到上的金币向UI移动
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
        // 确保对象仍然激活并且时间已达到
        if (gameObject.activeSelf && elapsedTime >= duration)
        {
            transform.gameObject.SetActive(false);
            //PlayInforManager.Instance.playInfor.AddCoins(1);
            Destroy(gameObject);
        }
    }
}
