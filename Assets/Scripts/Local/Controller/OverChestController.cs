using UnityEngine;
using DG.Tweening;
using DragonBones;
using Spine.Unity;
using Cysharp.Threading.Tasks;


public class OverChestController : MonoBehaviour
{
    public UnityArmatureComponent chestSkeleton;
    public SuccessPanelController successPanelController;

    void Start()
    {
        chestSkeleton = transform.GetChild(0).GetComponent<UnityArmatureComponent>();
        // ��ʼ����Ϊ0��ʵ�ֵ���Ч��
        transform.localScale = Vector3.zero;
        // ��������
        transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).OnComplete(PlayStayAnimation);
    }

    async void PlayStayAnimation()
    {
        // ����stay����
        await PlayAndWaitForAnimation(chestSkeleton, "breath",1);
        await PlayAndWaitForAnimation(chestSkeleton, "open", 1);
        await PlayAndWaitForAnimation(chestSkeleton, "open_stay", 1);
        successPanelController.OnChestOpenComplete(100);
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
