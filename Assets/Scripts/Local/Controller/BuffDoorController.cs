using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class BuffDoorController : MonoBehaviour
{
    public Text buffText;
    public Text debuffText;
    public float moveSpeed; // �������������ƶ����ٶ�
    public float hideYPosition = -10f; // ������Ļ��Y����
    public bool isMove = false;
    private bool hasTriggered = false; // �����ظ���������
    private float MiddleX = 0f;

    public Transform healthBarCanvas; // Ѫ�����ڵ�Canvas (World Space Canvas)
    public GameObject buffDoor;
    public GameObject debuffDoor;
    void OnEnable()
    {

        hasTriggered = false;
        isMove = true;
        moveSpeed = ConfigManager.Instance.Tables.TableGlobal.Get(6).IntValue;
        transform.localScale = Vector3.one * initialScale;

        // �����������ܵ�λ��
        Vector3 position1 = new Vector3(0.87f, 0, 0);
        Vector3 position2 = new Vector3(-0.97f, 0, 0);

        // ������� Buff �� Debuff ��λ��
        if (Random.value > 0.5f)
        {
            buffDoor.transform.localPosition = position1;
            debuffDoor.transform.localPosition = position2;
            buffText.transform.GetComponent<RectTransform>().localPosition = new Vector3(83, 52, 0);
            debuffText.transform.GetComponent<RectTransform>().localPosition = new Vector3(-96, 52, 0);
        }
        else
        {
            buffDoor.transform.localPosition = position2;
            debuffDoor.transform.localPosition = position1;
            buffText.transform.GetComponent<RectTransform>().localPosition = new Vector3(-96, 52, 0);
            debuffText.transform.GetComponent<RectTransform>().localPosition = new Vector3(83, 52, 0);
        }

        FollowParentObject();
    }


    void Update()
    {
        if (PreController.Instance.isFrozen) return;

        if (isMove)
        {
            MoveDown();
            FollowParentObject();
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
    private void FollowParentObject()
    {
        if (healthBarCanvas != null)
        {
            // ��Ѫ������Ϊ���ͷ����λ�� (��������)
            healthBarCanvas.position = transform.position;  // 1f ΪY���ƫ����
            healthBarCanvas.localScale = new Vector3(0.01f, 0.01f, 0.01f);  // ����Ѫ�������ţ�ʹ����Ӧ����
        }
    }
    public int randomBuffId = 0;
    public int randomDeBuffId = 0;
    // ���� Buff �ŵķ���
    public void GetBuffDoorIDAndText(GameObject BuffDoorobj)
    {
        if (GameFlowManager.Instance.currentLevelIndex == 0)
        {
            if (PreController.Instance.DoorNumWave == 4)
                randomBuffId = 5;
            if (PreController.Instance.DoorNumWave == 5)
                randomBuffId = 6;
        }
        else
        {
            randomBuffId = GetBuffIndex();
        }
        //�����ŵĳ�ʼ�ı�
        randomDeBuffId = GetDeBuffIndex();
        string randomBuff = ConfigManager.Instance.Tables.TableDoorcontent.Get(randomBuffId).Name;
        string randomDeBuff = ConfigManager.Instance.Tables.TableDoorcontent.Get(randomDeBuffId).Name;
        buffText.text = randomBuff;
        debuffText.text = randomDeBuff;
    }
    public Font[] fonts;

    // �������ܵ��߼�
    public void HandleDoorCollision(GameObject player, bool isBuffDoor)
    {
        if (!hasTriggered)
        {
            hasTriggered = true;
            transform.gameObject.SetActive(false);
            PlayerController playerController = player.GetComponent<PlayerController>();
            if (isBuffDoor)
            {
                AudioManage.Instance.PlaySFX("buff", null);
                ApplyBuff(player, randomBuffId); // Ӧ������Ч��
                Font font = fonts[0];
                playerController.ShowBuff(ConfigManager.Instance.Tables.TableDoorcontent.Get(randomBuffId).Name, fonts[0]);
            }
            else
            {
                AudioManage.Instance.PlaySFX("debuff", null);
                ApplyDebuff(player, randomDeBuffId); // Ӧ�ü���Ч��
                Font font = fonts[1];
                playerController.ShowBuff(ConfigManager.Instance.Tables.TableDoorcontent.Get(randomDeBuffId).Name, fonts[1]);
            }

            Destroy(gameObject, 1f);
        }
    }


    public int GetBuffIndex()
    {
        var coinindexConfig = ConfigManager.Instance.Tables.TableDoorcontent;
        int WeightAll = 0;
        for (int i = 1; i <= 6; i++)
        {
            WeightAll += coinindexConfig.Get(i).Weight;
        }
        float randomNum = Random.Range(1, WeightAll);
        if (randomNum <= coinindexConfig.Get(1).Weight)
            return 1;
        else if (randomNum > coinindexConfig.Get(1).Weight && randomNum <= (coinindexConfig.Get(1).Weight + coinindexConfig.Get(2).Weight))
            return 2;
        else if (randomNum > (coinindexConfig.Get(1).Weight + coinindexConfig.Get(2).Weight) && randomNum <= coinindexConfig.Get(1).Weight + (coinindexConfig.Get(2).Weight + coinindexConfig.Get(3).Weight)) // 94.45 + 5
            return 3;
        else if (randomNum > (coinindexConfig.Get(1).Weight + (coinindexConfig.Get(2).Weight + coinindexConfig.Get(3).Weight)) && randomNum <= (coinindexConfig.Get(1).Weight + coinindexConfig.Get(2).Weight + coinindexConfig.Get(3).Weight + coinindexConfig.Get(4).Weight)) // 94.45 + 5
            return 4;
        else if (randomNum >= (coinindexConfig.Get(1).Weight + coinindexConfig.Get(2).Weight + coinindexConfig.Get(3).Weight + coinindexConfig.Get(4).Weight) && randomNum <= (coinindexConfig.Get(1).Weight + coinindexConfig.Get(2).Weight + coinindexConfig.Get(3).Weight + coinindexConfig.Get(4).Weight + coinindexConfig.Get(5).Weight))
            return 5;
        else // If it's less than 100, return 5
            return 6;
    }
    public int GetDeBuffIndex()
    {
        var coinindexConfig = ConfigManager.Instance.Tables.TableDoorcontent;
        int WeightAll = 0;
        for (int i = 7; i <= 10; i++)
        {
            WeightAll += coinindexConfig.Get(i).Weight;
        }
        float randomNum = Random.Range(1, WeightAll);
        if (randomNum <= coinindexConfig.Get(7).Weight)
            return 7;
        else if (randomNum > coinindexConfig.Get(7).Weight && randomNum <= (coinindexConfig.Get(7).Weight + coinindexConfig.Get(8).Weight)) // 94.45 + 5
            return 8;
        else if (randomNum > (coinindexConfig.Get(7).Weight + coinindexConfig.Get(8).Weight) && randomNum <= (coinindexConfig.Get(7).Weight + coinindexConfig.Get(8).Weight + coinindexConfig.Get(9).Weight)) // 94.45 + 5
            return 9;
        else
            return 10;
    }
    // Ӧ������Ч�����߼�
    private void ApplyBuff(GameObject player, int buffId)
    {
        switch (buffId)
        {
            case 1:
                BuffManager.Instance.ApplyCoinFacBuff(ConfigManager.Instance.Tables.TableDoorcontent.Get(buffId).Time, (float)(ConfigManager.Instance.Tables.TableDoorcontent.Get(buffId).GenusScale));
                break;
            case 2:
                BuffManager.Instance.ApplyCoinFacBuff(ConfigManager.Instance.Tables.TableDoorcontent.Get(buffId).Time, (float)(ConfigManager.Instance.Tables.TableDoorcontent.Get(buffId).GenusScale));
                break;
            case 3:
                BuffManager.Instance.ApplyCoinFacBuff(ConfigManager.Instance.Tables.TableDoorcontent.Get(buffId).Time, (float)(ConfigManager.Instance.Tables.TableDoorcontent.Get(buffId).GenusScale));
                break;
            case 4:
                // TTOD1��6����������ӵ�������Ǯ
                BuffManager.Instance.ApplyBulletCostBuff(ConfigManager.Instance.Tables.TableDoorcontent.Get(buffId).Time);
                break;
            case 5:
                SummonSoldiers(player, (int)(ConfigManager.Instance.Tables.TableDoorcontent.Get(buffId).GenusValue));
                break;
            case 6:
                SummonSoldiers(player, (int)(ConfigManager.Instance.Tables.TableDoorcontent.Get(buffId).GenusValue));
                break;
        }
        //PreController.Instance.GenerationIntervalBullet = (float)(PreController.Instance.GenerationIntervalBullet * (1 + (float)PlayInforManager.Instance.playInfor.attackSpFac));
        //PreController.Instance.GenerationIntervalBullet = Mathf.Max(0f, PreController.Instance.GenerationIntervalBullet);
        // TTOD1��GenerationIntervalBullet�иı�ʱ����Э��
        // PreController.Instance.RestartIEPlayBullet();
    }

    // Ӧ�ü���Ч�����߼�
    private void ApplyDebuff(GameObject player, int debuff)
    {
        PlayerController playerController = player.GetComponent<PlayerController>();

        switch (debuff)
        {
            case 7:
                BuffManager.Instance.ApplyGenerationIntervalBulletDebuff(ConfigManager.Instance.Tables.TableDoorcontent.Get(debuff).Time, (float)(ConfigManager.Instance.Tables.TableDoorcontent.Get(debuff).GenusScale));
                break;
            case 8:
                BuffManager.Instance.ApplyGenerationIntervalBulletDebuff(ConfigManager.Instance.Tables.TableDoorcontent.Get(debuff).Time, (float)(ConfigManager.Instance.Tables.TableDoorcontent.Get(debuff).GenusScale));
                break;
            case 9:
                BuffManager.Instance.ApplyAttackFacDebuff(ConfigManager.Instance.Tables.TableDoorcontent.Get(debuff).Time, (float)(ConfigManager.Instance.Tables.TableDoorcontent.Get(debuff).GenusScale));
                break;
            case 10:
                BuffManager.Instance.ApplyAttackFacDebuff(ConfigManager.Instance.Tables.TableDoorcontent.Get(debuff).Time, (float)(ConfigManager.Instance.Tables.TableDoorcontent.Get(debuff).GenusScale));
                break;
        }
        if (debuff == 7 || debuff == 8)
        {
            PreController.Instance.GenerationIntervalBullet = (float)(PreController.Instance.GenerationIntervalBullet * (1 + (float)PlayInforManager.Instance.playInfor.attackSpFac));
            PreController.Instance.GenerationIntervalBullet = Mathf.Max(0f, PreController.Instance.GenerationIntervalBullet);
            // TTOD1��GenerationIntervalBullet�иı�ʱ����Э��
            PreController.Instance.RestartIEPlayBullet();
        }
    }

    #region//�¸�buff���߼�
    private IEnumerator ApplyCoinFacBuff(float duration, float coinFacValue)
    {
        PlayInforManager.Instance.playInfor.coinFac = coinFacValue;
        yield return new WaitForSeconds(duration);
        PlayInforManager.Instance.playInfor.coinFac = 0f;
    }

    // Э�̣�Ӧ���ӵ��⻨��Ч��
    //private IEnumerator ApplyBulletCostBuff(float duration)
    //{
    //    PreController.Instance.isBulletCostZero = true;
    //    yield return new WaitForSeconds(duration);
    //    PreController.Instance.isBulletCostZero = false;
    //}














    #endregion
    // �ٻ�ʿ�����߼�
    private void SummonSoldiers(GameObject player, int soldierCount)
    {
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
                        existingSoldier.SetLifetime(ConfigManager.Instance.Tables.TableDoorcontent.Get(randomBuffId).Time); // ��������ʿ���Ĵ��ʱ��
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
                soldierController.SetLifetime(ConfigManager.Instance.Tables.TableDoorcontent.Get(randomBuffId).Time); // ����ʿ�����ʱ��
                generatedSoldiers++;
            }
        }
    }


    private float initialScale = 0.6f; // Initial chest scale
    private float targetScale = 1.1f; // Target chest scale
                                       // ���������ƶ�
    private void MoveDown()
    {
        transform.Translate(Vector3.down * 0.5f * Time.deltaTime);
        float currentScale = transform.localScale.x; // Assuming uniform scaling on all axes
        if (currentScale < targetScale)
        {
            float scaleFactor = InfiniteScroll.Instance.growthRate * 2 * Time.deltaTime;
            float newScale = Mathf.Min(currentScale + scaleFactor, targetScale); // Ensure the scale doesn't exceed the target scale
            if (transform.localScale.x < newScale)
            {
                transform.localScale = new Vector3(newScale, newScale, newScale);
            }
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
