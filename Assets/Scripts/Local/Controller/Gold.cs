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
    public async UniTask MoveCoinToUI(ObjectPool<GameObject> CoinPool)
    {
        float duration = 1f;
        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = coinTargetPos.position;

        while (elapsedTime < duration)
        {
            // Check if the GameObject is still active
            if (!gameObject.activeSelf)
            {
                return; // Exit the coroutine if the object is no longer active
            }
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            Vector3 currentPosition = Vector3.Lerp(startPosition, targetPosition, t);
            currentPosition.z = -0.1f;
            transform.position = currentPosition;
            await UniTask.Yield();
        }

        // Ensure the object is still active before deactivating it
        if (gameObject.activeSelf)
        {
            transform.gameObject.SetActive(false);
            CoinPool.Release(transform.gameObject);
            PlayInforManager.Instance.playInfor.AddCoins(1);
        }
    }

}