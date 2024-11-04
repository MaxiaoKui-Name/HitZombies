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
        // 初始缩放为0，实现弹跳效果
        transform.localScale = Vector3.zero;
        // 弹跳进入
        transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).OnComplete(PlayStayAnimation);
    }

    async void PlayStayAnimation()
    {
        // 播放stay动画
        await PlayAndWaitForAnimation(chestSkeleton, "breath",1);
        await PlayAndWaitForAnimation(chestSkeleton, "open", 1);
        await PlayAndWaitForAnimation(chestSkeleton, "open_stay", 1);
        successPanelController.OnChestOpenComplete(100);
    }

   
    private async UniTask PlayAndWaitForAnimation(UnityArmatureComponent armature, string animationName, int playTimes)
    {
        var tcs = new UniTaskCompletionSource();

        // 定义事件处理程序
        void OnAnimationComplete(string type, EventObject eventObject)
        {
            if (eventObject.animationState.name == animationName)
            {
                armature.RemoveDBEventListener(EventObject.COMPLETE, OnAnimationComplete);  // 移除监听器
                tcs.TrySetResult(); // 完成任务
            }
        }
        // 添加事件监听器
        armature.AddDBEventListener(EventObject.COMPLETE, OnAnimationComplete);
        // 播放指定动画，并指定播放次数
        armature.animation.Play(animationName, playTimes);
        // 等待任务完成
        await tcs.Task;
    }
}
