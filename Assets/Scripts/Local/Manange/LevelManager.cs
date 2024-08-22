using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
//�йؿ��Ľű�
public class LevelManager : Singleton<LevelManager>
{
    public LevelData levelData;  // ��ǰ�ؿ�����
    //����LoadLevel������������һ������
    public void LoadLevel(string levelName)
    {
        Addressables.LoadSceneAsync(levelName, LoadSceneMode.Single);/*.Completed += OnLevelLoaded*/;
    }
    //�л�������ĳ�ʼ���ؿ�
    private void OnLevelLoaded(AsyncOperationHandle<SceneInstance> obj)
    {
        LoadLevel();
    }
   
    //���ɹؿ� 
    public void LoadLevel()
    {
        // ���ر���
        Addressables.LoadAssetAsync<Sprite>(levelData.backgroundAddress).Completed += OnBackgroundLoaded;

        // ���ز����ɵ���
        foreach (var enemyAddress in levelData.enemyAddresses)
        {
            Addressables.LoadAssetAsync<GameObject>(enemyAddress).Completed += OnEnemyLoaded;
        }
    }

    private void OnBackgroundLoaded(AsyncOperationHandle<Sprite> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            Sprite background = handle.Result;
            // ���ñ���
            SetBackground(background);
        }
    }

    private void OnEnemyLoaded(AsyncOperationHandle<GameObject> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            GameObject enemyPrefab = handle.Result;
            // ���ݹؿ��������ɵ���
            Instantiate(enemyPrefab, levelData.enemySpawnPoints, Quaternion.identity);
        }
    }

    private void SetBackground(Sprite background)
    {
        // ��������һ������ͼƬ��GameObject
        GameObject backgroundObject = GameObject.Find("Background");
        if (backgroundObject != null)
        {
            var spriteRenderer = backgroundObject.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = background;
            }
        }
    }

}

