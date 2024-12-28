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
            yield return null; // �ȴ���һ֡
            timer += Time.unscaledDeltaTime; // ʹ�� unscaledDeltaTime
        }
        PlayInforManager.Instance.playInfor.coinFac = 0f;
        coinFacBuffCoroutine = null;
    }

    // Buff���ӵ�������
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

    // Debuff���޸��ӵ�������
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
        // �洢��ʼֵ
       // originalGenerationIntervalBullet = (float)(ConfigManager.Instance.Tables.TablePlayerConfig.Get(GameFlowManager.Instance.currentLevelIndex).Cd / 1000f);
        // �޸� GenerationIntervalBullet
        PreController.Instance.GenerationIntervalBullet *= (1 + genusScale);
        PreController.Instance.GenerationIntervalBullet = Mathf.Max(0f, PreController.Instance.GenerationIntervalBullet);
        PreController.Instance.RestartIEPlayBullet();
        float timer = 0f;
        while (timer < duration)
        {
            yield return null;
            timer += Time.unscaledDeltaTime;
        }
        // �ָ���ʼֵ
        PreController.Instance.GenerationIntervalBullet = originalGenerationIntervalBullet;
        PreController.Instance.RestartIEPlayBullet();
        generationIntervalBulletDebuffCoroutine = null;
    }
    /// ��ǰ�Ƴ��ӵ����ɼ���ļ���Ч��
    public void RemoveGenerationIntervalBulletDebuff()
    {
        if (generationIntervalBulletDebuffCoroutine != null)
        {
            // ֹͣЭ��
            StopCoroutine(generationIntervalBulletDebuffCoroutine);
            generationIntervalBulletDebuffCoroutine = null;
            // �ָ���ʼֵ
            PreController.Instance.GenerationIntervalBullet = originalGenerationIntervalBullet;
            PreController.Instance.RestartIEPlayBullet();
        }
    }
    // Debuff���޸Ĺ�����
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
        // �洢��ʼֵ
        originalAttackFac = PlayInforManager.Instance.playInfor.attackFac;
        // �޸� attackFac
        PlayInforManager.Instance.playInfor.attackFac = genusScale;
        float timer = 0f;
        while (timer < duration)
        {
            yield return null;
            timer += Time.unscaledDeltaTime;
        }
        // �ָ���ʼֵ
        PlayInforManager.Instance.playInfor.attackFac = originalAttackFac;
        attackFacDebuffCoroutine = null;
    }
    
}
