using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class SpecialBuffDoor : MonoBehaviour
{
    public Vector3 PowermaxImgStartScale = Vector3.zero; // Buff�ı���ʼ����
    public Vector3 PowerImgEndScale = Vector3.one;    // Buff�ı���������
    public float moveSpeed; // �������������ƶ����ٶ�
    public float hideYPosition = -10f; // ������Ļ��Y����
    public bool isMove = false;
    private bool hasTriggered = false; // �����ظ���������
    private float MiddleX = 0f;

    public Transform healthBarCanvas; // Ѫ�����ڵ�Canvas (World Space Canvas)
    void OnEnable()
    {
        hasTriggered = false;
        isMove = true;
        moveSpeed = 0.5f;// ConfigManager.Instance.Tables.TableGlobal.Get(6).IntValue;
        transform.localScale = Vector3.one * initialScale;
    }


    void Update()
    {
        if (PreController.Instance.isFrozen) return;

        if (isMove)
        {
            MoveDown();
        }
        if (transform.position.y < hideYPosition)
        {
            HideAllChildren();
        }
        if (GameManage.Instance.gameState != GameState.Running)
        {
            if (GameManage.Instance.gameState == GameState.Resue)
                return; // ����ʱ��ִ���κ��߼�
            else
                Destroy(gameObject);
        }
    }
    public int randomBuffId = 0;
    public int randomDeBuffId = 0;
    public Font[] fonts;

    // �������ܵ��߼�
    public void HandleDoorCollision(GameObject player, bool isBuffDoor,int randomBuffId)
    {
        if (!hasTriggered)
        {
            hasTriggered = true;
            PlayerController playerController = player.GetComponent<PlayerController>();
            if (isBuffDoor)
            {
                ApplyBuff(player, randomBuffId); // Ӧ������Ч��
                Font font = fonts[0];
                playerController.ShowBuff("+ POWER MAX +", fonts[0]);
            }
            transform.gameObject.SetActive(false);
            Destroy(gameObject, 1f);
        }
    }
    // Ӧ������Ч�����߼�
    private void ApplyBuff(GameObject player, int buffId)
    {
        switch (buffId)
        {
            case 1:
                PlayInforManager.Instance.playInfor.SetGun(ConfigManager.Instance.Tables.TablePlayerConfig.Get(GameFlowManager.Instance.currentLevelIndex).Animation, "bullet_04special");
                AccountManager.Instance.SaveAccountData();
                SummonSoldiers(player, 4);
                break;
            case 2:
                BuffManager.Instance.ApplyGenerationIntervalBulletDebuff(100f, -0.4f);
                break;
        }
    }

    #region//�¸�buff���߼�
    private IEnumerator ApplyCoinFacBuff(float duration, float coinFacValue)
    {
        PlayInforManager.Instance.playInfor.coinFac = coinFacValue;
        yield return new WaitForSeconds(duration);
        PlayInforManager.Instance.playInfor.coinFac = 0f;
    }

    #endregion
    // �ٻ�ʿ�����߼�
    private void SummonSoldiers(GameObject player, int soldierCount)
    {
        hasTriggered = false;

        List<Vector3> offsets = new List<Vector3>
    {
        new Vector3(0.5f, -0.2f, 0), // �Һ�1λ
        new Vector3(-0.5f, -0.2f, 0), // ���1λ
        new Vector3(1f, -0.5f, 0),   // �Һ�
        new Vector3(-1f, -0.5f, 0)  // ���
    };

        int generatedSoldiers = 0;
        for (int i = 0; i < offsets.Count && generatedSoldiers < soldierCount; i++)
        {
            Vector3 spawnPosition = player.transform.position + offsets[i];
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(spawnPosition, 0.05f); // ������Ҫ�����뾶
            bool soldierExists = false;

            foreach (var hitCollider in hitColliders)
            {
                SoldierController existingSoldier = hitCollider.GetComponent<SoldierController>();
                if (existingSoldier != null)
                {
                    soldierExists = true;
                    // �ж� soldierCount �� soldiers �������߼�
                    if (soldierCount == 4 || generatedSoldiers >= 2)
                    {
                        existingSoldier.SetLifetime(60); // ��������ʿ���Ĵ��ʱ��
                    }
                    break;
                }
            }

            // �����λ��û��ʿ��������Ҫ�����µ�ʿ��
            if (!soldierExists)
            {
                GameObject soldier = Instantiate(Resources.Load<GameObject>("Prefabs/soldier_001"), spawnPosition, Quaternion.identity);
                SoldierController soldierController = soldier.GetComponent<SoldierController>();
                soldierController.SetPlayer(player);
                soldierController.SetLifetime(60); // ����ʿ�����ʱ��
                generatedSoldiers++;
            }
        }
    }
    private float initialScale = 0.6f; // Initial chest scale
    private float targetScale = 1.2f; // Target chest scale
                                      // ���������ƶ�
    private void MoveDown()
    {
        transform.Translate(Vector3.down * moveSpeed * Time.deltaTime);
        float currentScale = transform.localScale.x; // Assuming uniform scaling on all axes
        if (currentScale < targetScale)
        {
            float scaleFactor = InfiniteScroll.Instance.growthRate * 3 * Time.deltaTime;
            float newScale = Mathf.Min(currentScale + scaleFactor, targetScale); // Ensure the scale doesn't exceed the target scale
            transform.localScale = new Vector3(newScale, newScale, newScale);
        }
    }
    // ���������Ӷ���
    public void HideAllChildren()
    {
        hasTriggered = false;
        isMove = false;
        //transform.GetComponent<SortingGroup>().sortingLayerName = "Default";
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }
   
}

