using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffManager : Singleton<BuffManager>
{
    private Coroutine coinFacBuffCoroutine;
    public void ApplyCoinFacBuff(float duration, float coinFacValue)
    {
        if (coinFacBuffCoroutine != null)
        {
            StopCoroutine(coinFacBuffCoroutine);
        }
        coinFacBuffCoroutine = StartCoroutine(CoinFacBuffCoroutine(duration, coinFacValue));
    }

    private IEnumerator CoinFacBuffCoroutine(float duration, float coinFacValue)
    {
        PlayInforManager.Instance.playInfor.coinFac = coinFacValue;
        float timer = 0f;
        while (timer < duration)
        {
            yield return null; // 等待下一帧
            timer += Time.unscaledDeltaTime; // 使用 unscaledDeltaTime
        }
        PlayInforManager.Instance.playInfor.coinFac = 0f;
        coinFacBuffCoroutine = null;
    }

    // Buff：子弹免消耗
    private Coroutine bulletCostBuffCoroutine;
    public void ApplyBulletCostBuff(float duration)
    {
        if (bulletCostBuffCoroutine != null)
        {
            StopCoroutine(bulletCostBuffCoroutine);
        }
        bulletCostBuffCoroutine = StartCoroutine(BulletCostBuffCoroutine(duration));
    }

    private IEnumerator BulletCostBuffCoroutine(float duration)
    {
        PreController.Instance.isBulletCostZero = true;
        float timer = 0f;
        while (timer < duration)
        {
            yield return null;
            timer += Time.unscaledDeltaTime;
        }
        PreController.Instance.isBulletCostZero = false;
        bulletCostBuffCoroutine = null;
    }

    // Debuff：修改子弹发射间隔
    private Coroutine generationIntervalBulletDebuffCoroutine;
    public float originalGenerationIntervalBullet;
    public void ApplyGenerationIntervalBulletDebuff(float duration, float genusScale)
    {
        if (generationIntervalBulletDebuffCoroutine != null)
        {
            StopCoroutine(generationIntervalBulletDebuffCoroutine);
            PreController.Instance.GenerationIntervalBullet = originalGenerationIntervalBullet;
            PreController.Instance.RestartIEPlayBullet();
        }
        else
        {
            generationIntervalBulletDebuffCoroutine = StartCoroutine(GenerationIntervalBulletDebuffCoroutine(duration, genusScale));

        }
    }

    private IEnumerator GenerationIntervalBulletDebuffCoroutine(float duration, float genusScale)
    {
        // 存储初始值
       // originalGenerationIntervalBullet = (float)(ConfigManager.Instance.Tables.TablePlayerConfig.Get(GameFlowManager.Instance.currentLevelIndex).Cd / 1000f);
        // 修改 GenerationIntervalBullet
        PreController.Instance.GenerationIntervalBullet *= (1 + genusScale);
        PreController.Instance.GenerationIntervalBullet = Mathf.Max(0f, PreController.Instance.GenerationIntervalBullet);
        PreController.Instance.RestartIEPlayBullet();
        float timer = 0f;
        while (timer < duration)
        {
            yield return null;
            timer += Time.unscaledDeltaTime;
        }
        // 恢复初始值
        PreController.Instance.GenerationIntervalBullet = originalGenerationIntervalBullet;
        PreController.Instance.RestartIEPlayBullet();
        generationIntervalBulletDebuffCoroutine = null;
    }
    /// 提前移除子弹生成间隔的减益效果
    public void RemoveGenerationIntervalBulletDebuff()
    {
        if (generationIntervalBulletDebuffCoroutine != null)
        {
            // 停止协程
            StopCoroutine(generationIntervalBulletDebuffCoroutine);
            generationIntervalBulletDebuffCoroutine = null;
            // 恢复初始值
            PreController.Instance.GenerationIntervalBullet = originalGenerationIntervalBullet;
            PreController.Instance.RestartIEPlayBullet();
        }
    }
    // Debuff：修改攻击力
    private Coroutine attackFacDebuffCoroutine;
    private float originalAttackFac;
    public void ApplyAttackFacDebuff(float duration, float genusScale)
    {
        if (attackFacDebuffCoroutine != null)
        {
            StopCoroutine(attackFacDebuffCoroutine);
            PlayInforManager.Instance.playInfor.attackFac = originalAttackFac;
        }
        attackFacDebuffCoroutine = StartCoroutine(AttackFacDebuffCoroutine(duration, genusScale));
    }

    private IEnumerator AttackFacDebuffCoroutine(float duration, float genusScale)
    {
        // 存储初始值
        originalAttackFac = PlayInforManager.Instance.playInfor.attackFac;
        // 修改 attackFac
        PlayInforManager.Instance.playInfor.attackFac = genusScale;
        float timer = 0f;
        while (timer < duration)
        {
            yield return null;
            timer += Time.unscaledDeltaTime;
        }
        // 恢复初始值
        PlayInforManager.Instance.playInfor.attackFac = originalAttackFac;
        attackFacDebuffCoroutine = null;
    }
    
}
