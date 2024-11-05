using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Gold : MonoBehaviour
{
    //private Transform coinTargetPos;
    void OnEnable()
    {
        //coinTargetPos = GameObject.Find("CointargetPos").transform;
    }
    public async UniTask AwaitMove(ObjectPool<GameObject> CoinPool,Transform coinTargetPos)
    {
        await MoveCoinToUI(CoinPool, coinTargetPos);
    }
    public async UniTask MoveCoinToUI(ObjectPool<GameObject> CoinPool, Transform coinTargetPos)
    {
        float duration = 1f;
        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = coinTargetPos.position;

        while (elapsedTime < duration)
        {
            // ��������Ѿ������ٻ���ã�ֱ���˳�
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

    //ǩ���ϵĽ����ui�ƶ�
    public async UniTask StartMoveCoin(RectTransform coinRect, Vector2 targetPosition, float duration)
    {
        Vector2 startPos = coinRect.anchoredPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            coinRect.anchoredPosition = Vector2.Lerp(startPos, targetPosition, t);
            await UniTask.Yield();
        }
        // ��ѡ���ƶ���ɺ�Ĵ����������ٽ��
        Destroy(gameObject.transform.parent.gameObject);
    }


    //UI��ʵ�ֽ�Ҷ���
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
    //    while (elapsed < duration)
    //    {
    //        elapsed += Time.deltaTime;
    //        float t = Mathf.Clamp01(elapsed / duration);
    //        coinRect.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, t);
    //        await UniTask.Yield();
    //    }
    //    coinRect.anchoredPosition = targetPosition;
    //    // �������������ٽ�Ҷ���
    //    Destroy(coinObj);
    //}
}
