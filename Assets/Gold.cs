using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Gold : MonoBehaviour
{
    private Transform coinTargetPos;
    public bool isDead;
    void OnEnable()
    {
        coinTargetPos = GameObject.Find("CointargetPos").transform;
        isDead= false;
    }
    public async UniTask AwaitMoveCoinToUI(ObjectPool<GameObject> CoinPool)
    {
        await MoveCoinToUI(CoinPool);
        if (isDead)
        {
            transform.gameObject.SetActive(false);
            CoinPool.Release(transform.gameObject);
            PlayInforManager.Instance.playInfor.AddCoins(1);
        }
    }
    public async UniTask MoveCoinToUI(ObjectPool<GameObject> CoinPool)
    {
        float duration = 1f;
        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = coinTargetPos.position;
        while (elapsedTime < duration && transform.gameObject.activeSelf)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            Vector3 currentPosition = Vector3.Lerp(startPosition, targetPosition, t);
            currentPosition.z = -0.1f;
            transform.position = currentPosition;
            await UniTask.Yield();
        }
        isDead = true;
    }
}
