using Cysharp.Threading.Tasks;
using DragonBones;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI; // ȷ��������UI�����ռ�
using TMPro;
using System.Threading;
using Unity.VisualScripting.Antlr3.Runtime; // ȷ��������TextMeshPro

public class Gold : MonoBehaviour
{
    private CancellationTokenSource _cts;
    private ObjectPool<GameObject> _coinPool;
    private Vector2 _initialPos;
    private Vector2 _targetPos;
    private Vector2 _uiTargetPos;
    private int  coinNum;

    // ���� RectTransform ���������
    private RectTransform _rectTransform;
    private UnityArmatureComponent _armature;

    void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _armature = GetComponentInChildren<UnityArmatureComponent>();
    }

    void OnEnable()
    {
        _cts = new CancellationTokenSource();
    }

    private void OnDisable()
    {
        if (_cts != null)
        {
            _cts.Cancel();
            _cts.Dispose();
            _cts = null;
        }
    }

    /// <summary>
    /// ��ʼ����ҵ��ƶ��Ͷ���
    /// </summary>
    /// <param name="coinPool">��Ҷ����</param>
    /// <param name="initialPos">��ʼλ��</param>
    /// <param name="targetPos">�Ϸ�Ŀ��λ��</param>
    /// <param name="uiTargetPos">UIĿ��λ��</param>
    /// <param name="isSpecialEnemy">�Ƿ�Ϊ�������</param>
    public void InitializeCoin(ObjectPool<GameObject> coinPool, Vector2 initialPos, Vector2 targetPos, Vector2 uiTargetPos, GameObject SpecialEnemy,int i)
    {
        _coinPool = coinPool;
        _initialPos = initialPos;
        _targetPos = targetPos;
        _uiTargetPos = uiTargetPos;
        coinNum = i;
        // ����Э�̴����ҵ��ƶ��Ͷ���
        StartCoroutine(CoInitializeCoin(SpecialEnemy));
    }

    private IEnumerator CoInitializeCoin(GameObject SpecialEnemy)
    {
        // ���� "stop" ����
        PlayAnimation("stop");

        // �ƶ����Ϸ�Ŀ��λ��
        Debug.Log($"��� {gameObject.name} ��ʼ�ƶ����Ϸ�Ŀ��λ��: {_targetPos}");
        yield return StartCoroutine(MoveToCoroutine(_targetPos, 0.3f, SpecialEnemy));

        // �ƶ��س�ʼλ�õ�y�߶ȣ�x�ᱣ�ֲ���
        Vector2 backPos = new Vector2(_targetPos.x, _initialPos.y + UnityEngine.Random.Range(-10f, 10f));
        Debug.Log($"��� {gameObject.name} ��ʼ���ƶ��س�ʼλ��: {backPos}");
        yield return StartCoroutine(MoveToCoroutine(backPos, 0.3f, SpecialEnemy));
        if (coinNum == 1 && SpecialEnemy.transform.GetComponent<EnemyController>().isSpecialHealth)
        {
            // ��ʾ��ʾ�ı�
            yield return StartCoroutine(PreController.Instance.HandleBeginnerLevelTwo(backPos));
            // �ָ���Ϸʱ��
            Time.timeScale = 1f;
            //TTOD1��������
            GameMainPanelController gameMainPanelController = FindObjectOfType<GameMainPanelController>();
            PanelThree panelThree = gameMainPanelController.PanelThree_F.GetComponent<PanelThree>();
            Vector2 newPos = gameMainPanelController.coinText.GetComponent<RectTransform>().anchoredPosition;
            panelThree.UpdateHole(new Vector2(newPos.x - 28, newPos.y - 12), new Vector2(546f, 108f));
            gameMainPanelController.PlayHight(gameMainPanelController.coinHightAmature);
            //���ӽ���Լ�������ַŴ�
            // ���� ������һ�о����� coinText �� coinspattern_F ��ʼ����ѭ����ʾ�� ����
            gameMainPanelController.StartCoinEffectBlink();
        }
            // ���� "revolve" ����һ��
        PlayAnimation("revolve");
        float resolveDuration = GetAnimationDuration("revolve");
        Debug.Log($"��� {gameObject.name} ���� resolve ����������ʱ��: {resolveDuration} ��");
        yield return new WaitForSeconds(resolveDuration);

        // �л��� "stop" ����
        PlayAnimation("stop");
        yield return StartCoroutine(MoveToCoroutine(_uiTargetPos, 0.7f, SpecialEnemy));
        // ���ս��
        RecycleGold(SpecialEnemy);
    }

    /// <summary>
    /// ��ȡָ�������ĳ���ʱ��
    /// </summary>
    /// <param name="animationName">��������</param>
    /// <returns>��������ʱ�䣬Ĭ��1��</returns>
    private float GetAnimationDuration(string animationName)
    {
        if (_armature != null)
        {
            var state = _armature.animation.GetState(animationName);
            if (state != null)
            {
                return state._duration / _armature.animation.timeScale;
            }
        }
        // ����޷���ȡ��������ʱ�䣬Ĭ��1��
        return 1f;
    }

    /// <summary>
    /// ����ָ���Ķ���
    /// </summary>
    /// <param name="animationName">��������</param>
    private void PlayAnimation(string animationName)
    {
        if (_armature != null)
        {
            _armature.animation.Play(animationName, 1);
        }
    }

    /// <summary>
    /// �ƶ���Ŀ��λ�õ�Э��
    /// </summary>
    /// <param name="targetPos">Ŀ��λ��</param>
    /// <param name="duration">�ƶ�����ʱ��</param>
    private IEnumerator MoveToCoroutine(Vector2 targetPos, float duration,GameObject SpecialEnemy)
    {
        float elapsedTime = 0f;
        Vector2 startPosition = _rectTransform.anchoredPosition;

        Debug.Log($"��� {gameObject.name} �� {startPosition} �ƶ��� {targetPos}������ʱ��: {duration} ��");

        while (elapsedTime < duration)
        {
            elapsedTime += Time.unscaledDeltaTime; // ʹ�� unscaledDeltaTime �Ա��� Time.timeScale Ӱ��
            float t = Mathf.Clamp01(elapsedTime / duration);
            _rectTransform.anchoredPosition = Vector2.Lerp(startPosition, targetPos, t);
            yield return null;
        }
        _rectTransform.anchoredPosition = targetPos;
        Debug.Log($"��� {gameObject.name} �ѵ���Ŀ��λ��: {targetPos}");
    }

    /// <summary>
    /// ���ս�ҵ������
    /// </summary>
    private void RecycleGold(GameObject SpecialEnemy)
    {
        if (_coinPool != null)
        {
            _coinPool.Release(gameObject);
            Debug.Log($"��� {gameObject.name} �ѻ��յ������");
        }
        else
        {
            gameObject.SetActive(false);
            Debug.Log($"��� {gameObject.name} �ѱ�����");
        }
         // ֪ͨ��������������Ŀ����+1
    GameMainPanelController gmpc = FindObjectOfType<GameMainPanelController>();
    if (gmpc != null)
    {
        gmpc.OnCoinArrived(SpecialEnemy.GetComponent<EnemyController>());
    }
    }

    /// <summary>
    /// ���¼��л��ս�ң����������ط����ã�
    /// </summary>
    private void RecycleGoldFromEvent()
    {
        // ȡ�����е��첽����
        if (_cts != null)
        {
            _cts.Cancel();
            _cts.Dispose();
            _cts = null;
        }
        RecycleGold(null);
    }
}