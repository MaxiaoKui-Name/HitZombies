using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Cysharp.Threading.Tasks;
public class ParticleManager : Singleton<ParticleManager>
{
    Dictionary<EffectType, ObjectPool<GameObject>> particlePoolInfo = new Dictionary<EffectType, ObjectPool<GameObject>>();
    const string baseParticlePrefabPath = "Prefabs/";

    public void Init()
    {
        //加载预制粒子
        foreach (EffectType _particleType in Enum.GetValues(typeof(EffectType)))
        {
            ObjectPool<GameObject> _particlePool = new ObjectPool<GameObject>(() => CreateParticle(Resources.Load($"{baseParticlePrefabPath}{_particleType}") as GameObject, transform), Get, Release, MyDestroy, true, 50, 100);
            particlePoolInfo.Add(_particleType, _particlePool);
        }
        CreatePool();
    }

    public void CreatePool()
    {
        //对象池 提前预制粒子
        ObjectPool<GameObject> boomParticlePoolflow = particlePoolInfo[EffectType.BulletEffect];
        for (int i = 0; i < 50; i++)
            boomParticlePoolflow.Release(boomParticlePoolflow.Get());

    }

    //对象池事件
    private GameObject CreateParticle(GameObject b, Transform bulletParent)
    {
        GameObject obj = Instantiate<GameObject>(b, bulletParent);
        return obj;
    }
    private void Get(GameObject go)
    {
        //go.SetActive(true);

    }

    public void Release(GameObject go)
    {
        go.SetActive(false);
    }

    private void MyDestroy(GameObject go)
    {
        Destroy(go);
    }
    public void ShowEffect(EffectType effectType, Vector3 position, Quaternion quaternion, Transform parent = null)
    {
        position.z = -0.1f;
        var eff = Get(effectType, position, quaternion, parent);
        UniTask.Create(async () =>
        {
            var parts = eff.GetComponentsInChildren<ParticleSystem>();
            while (true)
            {
                await UniTask.Delay(3000, cancellationToken: eff.GetCancellationTokenOnDestroy());
                bool playing = false;
                foreach (ParticleSystem item in parts)
                {
                    if (item.isPlaying)
                    {
                        playing = true;
                        break;
                    }
                }
                if (!playing) { Release(eff); break; }
            }
        });
    }

    //public void ShowEffect(EffectType effectType, Vector3 position, Quaternion quaternion, Transform parent = null)
    //{
    //    var eff = Get(effectType, position, quaternion, parent);
    //    StartCoroutine(EffectCoroutine(eff));
    //}

    //private IEnumerator EffectCoroutine(GameObject eff)
    //{
    //    var parts = eff.GetComponentsInChildren<ParticleSystem>();

    //    // 等待3秒钟
    //    yield return new WaitForSeconds(3f);

    //    bool playing;
    //    do
    //    {
    //        playing = false;
    //        foreach (ParticleSystem item in parts)
    //        {
    //            if (item.isPlaying)
    //            {
    //                playing = true;
    //                break;
    //            }
    //        }
    //        // 如果有粒子系统在播放，继续等待
    //        if (playing)
    //        {
    //            yield return null;
    //        }
    //    } while (playing);

    //    Release(eff);
    //}

    public GameObject Get(EffectType effectType, Vector3 position, Quaternion quaternion, Transform parent = null)
    {
        GameObject g;
        if (particlePoolInfo.TryGetValue(effectType, out ObjectPool<GameObject> pool))
        {
            g = pool.Get();
            g.SetActive(true);
            if (parent == null)
            {
                g.transform.SetPositionAndRotation(position, quaternion);
            }
            else
            {
                g.transform.SetParent(parent);
                g.transform.SetLocalPositionAndRotation(position, quaternion);
            }
            return g;
        }
        else
            XLog.LogError($"[{effectType}] 粒子不存在");
        return null;
    }
}

//public class ParticleManager : Singleton<ParticleManager>
//{
//    Dictionary<EffectType, ObjectPool<GameObject>> particlePoolInfo = new Dictionary<EffectType, ObjectPool<GameObject>>();
//    const string baseParticlePrefabPath = "Effect_Prefabs/";
//    public void Init()
//    {
//        //加载预制粒子
//        foreach (EffectType _particleType in Enum.GetValues(typeof(EffectType)))
//        {
//            ObjectPool<GameObject> _particlePool = new ObjectPool<GameObject>(() => CreateParticle(Resources.Load($"{baseParticlePrefabPath}{_particleType}") as GameObject, transform), Get, Release, MyDestroy, true, 50, 100);
//            particlePoolInfo.Add(_particleType, _particlePool);
//        }
//        CreatePool();
//    }

//    public void CreatePool()
//    {
//        //对象池 提前预制粒子
//        ObjectPool<GameObject> boomParticlePoolflow = particlePoolInfo[EffectType.flow];
//        for (int i = 0; i < 50; i++)
//            boomParticlePoolflow.Release(boomParticlePoolflow.Get());
//        ObjectPool<GameObject> boomParticlePoolt_explosion = particlePoolInfo[EffectType.t_explosion];
//        for (int i = 0; i < 50; i++)
//            boomParticlePoolt_explosion.Release(boomParticlePoolt_explosion.Get());
//        ObjectPool<GameObject> boomParticlePooldestory = particlePoolInfo[EffectType.destory];
//        for (int i = 0; i < 50; i++)
//            boomParticlePooldestory.Release(boomParticlePooldestory.Get());
//        ObjectPool<GameObject> boomParticlePooldestorydiji = particlePoolInfo[EffectType.destorydiji];
//        for (int i = 0; i < 50; i++)
//            boomParticlePooldestorydiji.Release(boomParticlePooldestorydiji.Get());
//        ObjectPool<GameObject> boomParticlePooldestorygaoji = particlePoolInfo[EffectType.destorygaoji];
//        for (int i = 0; i < 50; i++)
//            boomParticlePooldestorygaoji.Release(boomParticlePooldestorygaoji.Get());
//        ObjectPool<GameObject> boomParticlePool = particlePoolInfo[EffectType.destoryzhongji];
//        for (int i = 0; i < 50; i++)
//            boomParticlePool.Release(boomParticlePool.Get());
//        ObjectPool<GameObject> boomParticlePooldestoryzhongji = particlePoolInfo[EffectType.sp_explosion];
//        for (int i = 0; i < 50; i++)
//            boomParticlePooldestoryzhongji.Release(boomParticlePooldestoryzhongji.Get());
//        ObjectPool<GameObject> boomParticlePoolb_explosion = particlePoolInfo[EffectType.b_explosion];
//        for (int i = 0; i < 50; i++)
//            boomParticlePoolb_explosion.Release(boomParticlePoolb_explosion.Get());
//        ObjectPool<GameObject> boomParticlePooltransform = particlePoolInfo[EffectType.transform];
//        for (int i = 0; i < 50; i++)
//            boomParticlePooltransform.Release(boomParticlePooltransform.Get()); 
//        ObjectPool<GameObject> boomParticlePools_explosion = particlePoolInfo[EffectType.s_explosion];
//        for (int i = 0; i < 50; i++)
//            boomParticlePools_explosion.Release(boomParticlePools_explosion.Get()); 
//        ObjectPool<GameObject> boomParticlePooln_explosion = particlePoolInfo[EffectType.n_explosion];
//        for (int i = 0; i < 50; i++)
//            boomParticlePooln_explosion.Release(boomParticlePooln_explosion.Get());
//        ObjectPool<GameObject> boomParticlePoolwhitetransform = particlePoolInfo[EffectType.whitetransform];
//        for (int i = 0; i < 50; i++)
//            boomParticlePoolwhitetransform.Release(boomParticlePoolwhitetransform.Get());
//        ObjectPool<GameObject> boomParticlePoolyunshibaozha = particlePoolInfo[EffectType.yunshibaozha];
//        for (int i = 0; i < 50; i++)
//            boomParticlePoolyunshibaozha.Release(boomParticlePoolyunshibaozha.Get());
//    }

//    //对象池事件
//    private GameObject CreateParticle(GameObject b, Transform bulletParent)
//    {
//        GameObject obj = Instantiate<GameObject>(b, bulletParent);
//        return obj;
//    }
//    private void Get(GameObject go)
//    {
//        //go.SetActive(true);

//    }

//    public void Release(GameObject go)
//    {
//        go.SetActive(false);
//    }

//    private void MyDestroy(GameObject go)
//    {
//        Destroy(go);
//    }


//    public void ShowEffect(EffectType effectType, GameObject objPosition, Quaternion quaternion, Transform parent = null)
//    {
//        var eff = Get(effectType, objPosition.transform.position, quaternion, parent);
//        if (effectType == EffectType.whitetransform)
//        {
//            var white = eff.GetComponent<Whitetransform>();
//            if (white == null) return;
//            white.Parposition = objPosition;
//        }
//        UniTask.Create(async () =>
//        {
//            var parts = eff.GetComponentsInChildren<ParticleSystem>();
//            while (true)
//            {
//                await UniTask.Delay(3000, cancellationToken: eff.GetCancellationTokenOnDestroy());
//                bool playing = false;
//                foreach (ParticleSystem item in parts)
//                {
//                    if (item.isPlaying)
//                    {
//                        playing = true;
//                        break;
//                    }
//                }
//                if (!playing) { Release(eff); break; }
//            }
//        });
//    }
//    public void ShowEffect1(EffectType effectType, Vector3 position, Quaternion quaternion, Transform parent = null)
//    {
//        var eff = Get(effectType, position, quaternion, parent);
//        UniTask.Create(async () =>
//        {
//            var parts = eff.GetComponentsInChildren<ParticleSystem>();
//            while (true)
//            {
//                await UniTask.Delay(3000, cancellationToken: eff.GetCancellationTokenOnDestroy());
//                bool playing = false;
//                foreach (ParticleSystem item in parts)
//                {
//                    if (item.isPlaying)
//                    {
//                        playing = true;
//                        break;
//                    }
//                }
//                if (!playing) { Release(eff); break; }
//            }
//        });
//    }


//    public GameObject Get(EffectType effectType, Vector3 position, Quaternion quaternion, Transform parent = null)
//    {
//        GameObject g;
//        if (particlePoolInfo.TryGetValue(effectType, out ObjectPool<GameObject> pool))
//        {
//            g = pool.Get();
//            g.SetActive(true);
//            if (parent == null)
//            {
//                g.transform.SetPositionAndRotation(position, quaternion);
//            }
//            else
//            {
//                g.transform.SetParent(parent);
//                g.transform.SetLocalPositionAndRotation(position, quaternion);
//            }
//            return g;
//        }
//        else
//            XLog.LogError($"[{effectType}] 粒子不存在");
//        return null;
//    }
//}
public enum EffectType
{
    BulletEffect
}

