using Cysharp.Threading.Tasks;
using DragonBones;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI; // 确保引用了UI命名空间
using TMPro;
using System.Threading;
using Unity.VisualScripting.Antlr3.Runtime; // 确保引用了TextMeshPro

public class Gold : MonoBehaviour
{
    private CancellationTokenSource _cts;
    private ObjectPool<GameObject> _coinPool;
    private Vector2 _initialPos;
    private Vector2 _targetPos;
    private Vector2 _uiTargetPos;
    private int  coinNum;

    // 缓存 RectTransform 以提高性能
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
    /// 初始化金币的移动和动画
    /// </summary>
    /// <param name="coinPool">金币对象池</param>
    /// <param name="initialPos">初始位置</param>
    /// <param name="targetPos">上方目标位置</param>
    /// <param name="uiTargetPos">UI目标位置</param>
    /// <param name="isSpecialEnemy">是否为特殊敌人</param>
    public void InitializeCoin(ObjectPool<GameObject> coinPool, Vector2 initialPos, Vector2 targetPos, Vector2 uiTargetPos, GameObject SpecialEnemy,int i)
    {
        _coinPool = coinPool;
        _initialPos = initialPos;
        _targetPos = targetPos;
        _uiTargetPos = uiTargetPos;
        coinNum = i;
        // 启动协程处理金币的移动和动画
        StartCoroutine(CoInitializeCoin(SpecialEnemy));
    }

    private IEnumerator CoInitializeCoin(GameObject SpecialEnemy)
    {
        // 播放 "stop" 动画
        PlayAnimation("stop");

        // 移动到上方目标位置
        Debug.Log($"金币 {gameObject.name} 开始移动到上方目标位置: {_targetPos}");
        yield return StartCoroutine(MoveToCoroutine(_targetPos, 0.3f, SpecialEnemy));

        // 移动回初始位置的y高度，x轴保持不变
        Vector2 backPos = new Vector2(_targetPos.x, _initialPos.y + UnityEngine.Random.Range(-10f, 10f));
        Debug.Log($"金币 {gameObject.name} 开始回移动回初始位置: {backPos}");
        yield return StartCoroutine(MoveToCoroutine(backPos, 0.3f, SpecialEnemy));
        if (coinNum == 1 && SpecialEnemy.transform.GetComponent<EnemyController>().isSpecialHealth)
        {
            // 显示提示文本
            yield return StartCoroutine(PreController.Instance.HandleBeginnerLevelTwo(backPos));
            // 恢复游戏时间
            Time.timeScale = 1f;
            //TTOD1顶部高亮
            GameMainPanelController gameMainPanelController = FindObjectOfType<GameMainPanelController>();
            PanelThree panelThree = gameMainPanelController.PanelThree_F.GetComponent<PanelThree>();
            Vector2 newPos = gameMainPanelController.coinText.GetComponent<RectTransform>().anchoredPosition;
            panelThree.UpdateHole(new Vector2(newPos.x - 28, newPos.y - 12), new Vector2(546f, 108f));
            gameMainPanelController.PlayHight(gameMainPanelController.coinHightAmature);
            //增加金币以及金币数字放大
            // —— 下面这一行就是让 coinText 和 coinspattern_F 开始缩放循环的示例 ——
            gameMainPanelController.StartCoinEffectBlink();
        }
            // 播放 "revolve" 动画一次
        PlayAnimation("revolve");
        float resolveDuration = GetAnimationDuration("revolve");
        Debug.Log($"金币 {gameObject.name} 播放 resolve 动画，持续时间: {resolveDuration} 秒");
        yield return new WaitForSeconds(resolveDuration);

        // 切换回 "stop" 动画
        PlayAnimation("stop");
        yield return StartCoroutine(MoveToCoroutine(_uiTargetPos, 0.7f, SpecialEnemy));
        // 回收金币
        RecycleGold(SpecialEnemy);
    }

    /// <summary>
    /// 获取指定动画的持续时间
    /// </summary>
    /// <param name="animationName">动画名称</param>
    /// <returns>动画持续时间，默认1秒</returns>
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
        // 如果无法获取动画持续时间，默认1秒
        return 1f;
    }

    /// <summary>
    /// 播放指定的动画
    /// </summary>
    /// <param name="animationName">动画名称</param>
    private void PlayAnimation(string animationName)
    {
        if (_armature != null)
        {
            _armature.animation.Play(animationName, 1);
        }
    }

    /// <summary>
    /// 移动到目标位置的协程
    /// </summary>
    /// <param name="targetPos">目标位置</param>
    /// <param name="duration">移动持续时间</param>
    private IEnumerator MoveToCoroutine(Vector2 targetPos, float duration,GameObject SpecialEnemy)
    {
        float elapsedTime = 0f;
        Vector2 startPosition = _rectTransform.anchoredPosition;

        Debug.Log($"金币 {gameObject.name} 从 {startPosition} 移动到 {targetPos}，持续时间: {duration} 秒");

        while (elapsedTime < duration)
        {
            elapsedTime += Time.unscaledDeltaTime; // 使用 unscaledDeltaTime 以避免 Time.timeScale 影响
            float t = Mathf.Clamp01(elapsedTime / duration);
            _rectTransform.anchoredPosition = Vector2.Lerp(startPosition, targetPos, t);
            yield return null;
        }
        _rectTransform.anchoredPosition = targetPos;
        Debug.Log($"金币 {gameObject.name} 已到达目标位置: {targetPos}");
    }

    /// <summary>
    /// 回收金币到对象池
    /// </summary>
    private void RecycleGold(GameObject SpecialEnemy)
    {
        if (_coinPool != null)
        {
            _coinPool.Release(gameObject);
            Debug.Log($"金币 {gameObject.name} 已回收到对象池");
        }
        else
        {
            gameObject.SetActive(false);
            Debug.Log($"金币 {gameObject.name} 已被禁用");
        }
         // 通知面板控制器，到达目标金币+1
    GameMainPanelController gmpc = FindObjectOfType<GameMainPanelController>();
    if (gmpc != null)
    {
        gmpc.OnCoinArrived(SpecialEnemy.GetComponent<EnemyController>());
    }
    }

    /// <summary>
    /// 从事件中回收金币（可在其他地方调用）
    /// </summary>
    private void RecycleGoldFromEvent()
    {
        // 取消所有的异步任务
        if (_cts != null)
        {
            _cts.Cancel();
            _cts.Dispose();
            _cts = null;
        }
        RecycleGold(null);
    }
}