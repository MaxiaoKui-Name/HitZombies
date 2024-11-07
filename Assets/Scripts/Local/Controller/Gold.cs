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

        //coinTargetPos = GameObject.Find("CointargetPos").transform;
    }

    private void Update()
    {
        if (GameManage.Instance.gameState != GameState.Running)
        {
            // 取消所有的异步任务
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
        // 确保在销毁时取消所有任务并释放 CTS
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
                // 检查取消请求
                token.ThrowIfCancellationRequested();

                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / duration);
                Vector3 currentPosition = Vector3.Lerp(startPosition, targetPosition, t);
                currentPosition.z = -0.1f;
                transform.position = currentPosition;
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
    public async UniTask StartMoveCoin(RectTransform coinRect, Vector2 targetPosition, float duration)
    {
        Vector2 startPos = coinRect.anchoredPosition;
        float elapsed = 0f;

        try
        {
            while (elapsed < duration)
            {
                // 检查取消请求
                _cts.Token.ThrowIfCancellationRequested();

                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                coinRect.anchoredPosition = Vector2.Lerp(startPos, targetPosition, t);
                await UniTask.Yield(_cts.Token);
            }
            // 可选：移动完成后的处理，比如销毁金币
            Destroy(gameObject.transform.parent.gameObject);
        }
        catch (OperationCanceledException)
        {
            // 异步任务被取消，可以在这里处理清理逻辑
            Debug.Log($"StartMoveCoin canceled for {gameObject.name}");
        }
    }

    // UI上实现金币动画
    public async UniTask DragonMoveCoin(GameObject coin, Vector3 targetPosition, float duration)
    {
        // await DragonCoinMove(coin, targetPosition, duration);
    }

    //public async UniTask DragonCoinMove(GameObject coinObj, Vector3 endPosition, float duration)
    //{
    //    RectTransform coinRect = coinObj.GetComponent<RectTransform>();
    //    if (coinRect == null)
    //    {
    //        Debug.LogError("GoldCanvas 预制体缺少 RectTransform 组件！");
    //    }
    //    // 将世界坐标转换为画布的局部坐标
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
    //            // 检查取消请求
    //            _cts.Token.ThrowIfCancellationRequested();

    //            elapsed += Time.deltaTime;
    //            float t = Mathf.Clamp01(elapsed / duration);
    //            coinRect.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, t);
    //            await UniTask.Yield(_cts.Token);
    //        }
    //        coinRect.anchoredPosition = targetPosition;
    //        // 动画结束后销毁金币对象
    //        Destroy(coinObj);
    //    }
    //    catch (OperationCanceledException)
    //    {
    //        Debug.Log($"DragonCoinMove canceled for {coinObj.name}");
    //    }
    //}
}
