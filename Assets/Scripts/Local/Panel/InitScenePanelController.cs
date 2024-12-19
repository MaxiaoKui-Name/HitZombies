using System.Collections;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using DragonBones;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class InitScenePanelController : UIBase
{
    public Slider progressBar; // 进度条
    ///public TextMeshProUGUI progressText;  // 进度百分比文本
    //public TextMeshProUGUI updateText;
    public TextMeshProUGUI StartText_F; // 开始的文本
    public GameObject Backlogo_F; // 游戏初始logo
    public GameObject ClickAnim_F; // 点击的动画效果
    public UnityArmatureComponent LoadAnim_F;
    public GameObject Back_F;

    private bool isClickTriggered = false; // 判断是否触发点击动画
    private bool isScalingUp = true; // 判断是否正在放大
    void Start()
    {
        GetAllChild(transform);
        //progressText = childDic["Percent_F"].GetComponent<TextMeshProUGUI>();
        StartText_F = childDic["StartText_F"].GetComponent<TextMeshProUGUI>();
        progressBar.gameObject.SetActive(false);
        //updateText = childDic["UpdateText_F"].GetComponent<TextMeshProUGUI>();
        Backlogo_F = childDic["Backlogo_F"].gameObject;
        Back_F = childDic["Back_F"].gameObject;
        LoadAnim_F = childDic["LoadAnim_F"].GetComponent<UnityArmatureComponent>(); 
        ClickAnim_F = childDic["ClickAnim_F"].gameObject;
        StartCoroutine(ShowLogoAndStartText());
        LoadConfig().Forget(); ;
    }
    private async UniTask LoadConfig()
    {
        await ConfigManager.Instance.Init();
        AudioManage.Instance.Init();
        AudioManage.Instance.PlayMusic("beijing", true);
    }
    // 显示 Backlogo_F 并执行动画
    private IEnumerator ShowLogoAndStartText()
    {
        // 显示 Backlogo_F 1秒
        Backlogo_F.SetActive(true);
        yield return new WaitForSeconds(1f);
        Backlogo_F.SetActive(false);
        // 显示 Back_F 和 StartText_F，并逐渐放大 StartText_F
        Back_F.SetActive(true);
        StartText_F.gameObject.SetActive(true);
        // 启动大小变化的循环
        StartCoroutine(ScaleStartText());
    }

    // StartText_F的大小变化循环
    private IEnumerator ScaleStartText()
    {
        float scaleTime = 0.5f; // 每次放大或缩小的时间
        Vector3 initialScale = StartText_F.transform.localScale;
        Vector3 targetScale = initialScale * 1.4f;
        float timer = 0f;

        while (!isClickTriggered)
        {
            if (isScalingUp)
            {
                // 放大
                while (timer < scaleTime)
                {
                    StartText_F.transform.localScale = Vector3.Lerp(initialScale, targetScale, timer / scaleTime);
                    timer += Time.deltaTime;
                    yield return null;
                }
                StartText_F.transform.localScale = targetScale; // 确保最终大小为目标大小
            }
            else
            {
                // 缩小
                while (timer < scaleTime)
                {
                    StartText_F.transform.localScale = Vector3.Lerp(targetScale, initialScale, timer / scaleTime);
                    timer += Time.deltaTime;
                    yield return null;
                }
                StartText_F.transform.localScale = initialScale; // 确保最终大小为初始大小
            }

            // 切换状态，反向缩放
            isScalingUp = !isScalingUp;
            timer = 0f; // 重置计时器
        }
    }

    // 鼠标点击触发
    void Update()
    {
        if (!isClickTriggered && (Input.GetMouseButtonDown(0) || Input.touchCount > 0))
        {
            Vector3 clickPosition = Input.GetMouseButtonDown(0) ? Input.mousePosition : Input.GetTouch(0).position;
            // 设置 ClickAnim_F 的位置为点击位置，并播放动画
            ClickAnim_F.transform.position = clickPosition;
            ClickAnim_F.SetActive(true);
            isClickTriggered = true;
            UnityArmatureComponent animator = ClickAnim_F.GetComponent<UnityArmatureComponent>();
            animator.animation.Play("click",1);  // 假设动画名为 "click"
            // 播放完动画后隐藏 StartText_F 和 ClickAnim_F
            StartCoroutine(WaitForClickAnim(animator.animation.GetState("click")._duration));
        }
    }

    // 等待 ClickAnim_F 动画完成
    private IEnumerator WaitForClickAnim(float animationLength)
    {
        yield return new WaitForSeconds(animationLength);
        // 动画播放完成后隐藏 StartText_F 和 ClickAnim_F
        StartText_F.gameObject.SetActive(false);
        ClickAnim_F.SetActive(false);
        // 触发资源加载
        LoadDll.Instance.InitAddressable();
    }

    // 更新进度条
    public void UpdateProgressBar(float progress)
    {
        if (!progressBar.gameObject.activeSelf)
        {
            progressBar.gameObject.SetActive(true);
        }

        if (progressBar != null)
        {
            progressBar.value = progress;
            if (progress == 1)
            {
                LoadAnim_F.animation.Play("<None>", -1);  // 假设动画名为 "click"
            }
        }

        //if (progressText != null)
        //{
        //    progressText.text = $"{(progress * 100):0.00}%";
        //}
    }

    // 更新文本
    //public void UpdateText(string sentance)
    //{
    //    updateText.text = sentance;
    //}
}
