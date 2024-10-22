using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Gold : MonoBehaviour
{
    private Transform coinTargetPos;
    void OnEnable()
    {
        coinTargetPos = GameObject.Find("CointargetPos").transform;
    }
    public async UniTask AwaitMove(ObjectPool<GameObject> CoinPool)
    {
        await MoveCoinToUI(CoinPool);
        
    }
    public async void StartMoveCoin(ObjectPool<GameObject> CoinPool, GameObject coin, Vector3 targetPosition, float duration)
    {
        await MoveCoin(CoinPool, coin, targetPosition, duration);
    }
    public async UniTask MoveCoinToUI(ObjectPool<GameObject> CoinPool)
    {
        float duration = 1f;
        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = coinTargetPos.position;

        while (elapsedTime < duration)
        {
            // 如果对象已经被销毁或禁用，直接退出
            if (this == null || !gameObject.activeSelf)
            {
                return;
            }

            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            Vector3 currentPosition = Vector3.Lerp(startPosition, targetPosition, t);
            currentPosition.z = -0.1f;
            transform.position = currentPosition;
            await UniTask.Yield();
        }

        // Ensure the object is still active before deactivating it
        if (gameObject.activeSelf && elapsedTime >= duration)
        {
            transform.gameObject.SetActive(false);
            CoinPool.Release(transform.gameObject);
            PlayInforManager.Instance.playInfor.AddCoins(1);
        }
    }
    public async UniTask MoveCoin(ObjectPool<GameObject> CoinPool,GameObject coin, Vector3 targetPosition, float duration)
    {
        Vector3 startPos = coin.transform.position;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            coin.transform.position = Vector3.Lerp(startPos, targetPosition, t);
            await UniTask.Yield();
        }
        if (gameObject.activeSelf && elapsed >= duration)
        {
            transform.gameObject.SetActive(false);
            CoinPool.Release(transform.gameObject);
        }
    }
}
