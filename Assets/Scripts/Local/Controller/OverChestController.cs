using UnityEngine;
using DG.Tweening;
using DragonBones;
using Spine.Unity;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;


public class OverChestController : MonoBehaviour
{
    public UnityArmatureComponent armatureComponent;
    public SuccessPanelController successPanelController;
    public int TypeIndex;
    public int CoinBase;
    void Start()
    {
        successPanelController = transform.parent.GetComponent<SuccessPanelController>();
        // ��ʼ����Ϊ0��ʵ�ֵ���Ч��
        transform.localScale = Vector3.zero;
        TypeIndex = GetCoinIndex();
        transform.GetChild(TypeIndex - 1).gameObject.SetActive(true);
        armatureComponent = transform.GetChild(TypeIndex - 1).GetComponent<UnityArmatureComponent>();
        CoinBase = (int)(ConfigManager.Instance.Tables.TablePlayerConfig.Get(GameFlowManager.Instance.currentLevelIndex).Total * ConfigManager.Instance.Tables.TableBoxcontent.Get(TypeIndex).Rewardres);
        // ��������
        transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).OnComplete(PlayStayAnimation);
    }

    async void PlayStayAnimation()
    {
        // ����stay����
        await PlayAndWaitForAnimation(armatureComponent, "breath", 1);
        await PlayAndWaitForAnimation(armatureComponent, "open", 1);
        PlayAndWaitForAnimation(armatureComponent, "open_stay", -1);
        successPanelController.OnChestOpenComplete(CoinBase, TypeIndex);
      
    }
    public int GetCoinIndex()
    {
        int randomNum = Random.Range(1, 100);
        Debug.Log("����������" + randomNum);
        var coinindexConfig = ConfigManager.Instance.Tables.TableBoxcontent;
        if (randomNum < coinindexConfig.Get(1).Probability)
            return 1;
        else if (randomNum > coinindexConfig.Get(1).Probability && randomNum < coinindexConfig.Get(1).Probability + coinindexConfig.Get(2).Probability) // 71.45 + 23
            return 2;
        else if (randomNum > coinindexConfig.Get(1).Probability + coinindexConfig.Get(2).Probability && randomNum < coinindexConfig.Get(1).Probability + coinindexConfig.Get(2).Probability + coinindexConfig.Get(3).Probability) // 94.45 + 5
            return 3;
        else if (randomNum > coinindexConfig.Get(1).Probability + coinindexConfig.Get(2).Probability + coinindexConfig.Get(3).Probability && randomNum < coinindexConfig.Get(1).Probability + coinindexConfig.Get(2).Probability + coinindexConfig.Get(3).Probability + coinindexConfig.Get(4).Probability) // 99.45 + 0.5
            return 4;
        else // ���С��100������5
            return 5;
    }

    private async UniTask PlayAndWaitForAnimation(UnityArmatureComponent armature, string animationName, int playTimes)
    {
        var tcs = new UniTaskCompletionSource();

        // �����¼��������
        void OnAnimationComplete(string type, EventObject eventObject)
        {
            if (eventObject.animationState.name == animationName)
            {
                armature.RemoveDBEventListener(EventObject.COMPLETE, OnAnimationComplete);  // �Ƴ�������
                tcs.TrySetResult(); // �������
            }
        }
        // ����¼�������
        armature.AddDBEventListener(EventObject.COMPLETE, OnAnimationComplete);
        // ����ָ����������ָ�����Ŵ���
        armature.animation.Play(animationName, playTimes);
        // �ȴ��������
        await tcs.Task;
    }
}
