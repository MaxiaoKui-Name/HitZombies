// PausePaneController.cs 暂停面板控制器
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PausePaneController : UIBase
{
    // 声明UI元素的变量
    private Slider musicSlider;
    private TextMeshProUGUI musicStateText;
    private Slider soundSlider;
    private TextMeshProUGUI soundStateText;
    private Slider shockSlider;
    private TextMeshProUGUI shockStateText;

    private Button confirmButton;
    private Button closeButton;

    void Start()
    {
        GetAllChild(transform);

        // 获取UI元素
        musicSlider = childDic["MusicSlider_F"].GetComponent<Slider>();
        musicStateText = childDic["musickOpenText_F"].GetComponent<TextMeshProUGUI>();
        soundSlider = childDic["soundSlider_F"].GetComponent<Slider>(); 
        soundStateText = childDic["soundOpenText_F"].GetComponent<TextMeshProUGUI>();
        shockSlider = childDic["shockSlider_F"].GetComponent<Slider>(); 
        shockStateText = childDic["shockOpenText_F"].GetComponent<TextMeshProUGUI>();
        confirmButton = childDic["Confirm_F"].GetComponent<Button>();
        closeButton = childDic["CloseBtn_F"].GetComponent<Button>();

        // 初始化滑块和文本
        InitializeUI();

        // 添加滑块值变化的监听事件
        //musicSlider.onValueChanged.AddListener(MusicSliderValueChanged);
        //soundSlider.onValueChanged.AddListener(SoundSliderValueChanged);
        //shockSlider.onValueChanged.AddListener(ShockSliderValueChanged);

        // 添加按钮点击事件
        //confirmButton.onClick.AddListener(OnConfirmButtonClicked);
        //closeButton.onClick.AddListener(OnCloseButtonClicked);
        // 初始化面板缩放为0
        RectTransform panelRect = GetComponent<RectTransform>();
        StartCoroutine(PopUpAnimation(panelRect));
        // 获取 ClickAMature 对象及其动画组件
        GetClickAnim(transform);
        // 修改按钮点击事件监听器
        confirmButton.onClick.AddListener(() => StartCoroutine(OnconfirmButtonClicked()));
        closeButton.onClick.AddListener(() => StartCoroutine(OncloseButtonClicked()));

        musicSlider.onValueChanged.AddListener((value) => StartCoroutine(OnmusicSliderClicked()));
        soundSlider.onValueChanged.AddListener((value) => StartCoroutine(OnsoundSliderClicked()));
        shockSlider.onValueChanged.AddListener((value) => StartCoroutine(OnshockSliderClicked()));
    }

    /// <summary>
    /// 处理签到按钮点击事件的协程
    /// </summary>
    private IEnumerator OnconfirmButtonClicked()
    {
        // 播放点击动画
        yield return StartCoroutine(HandleButtonClickAnimation(transform));

        // 执行按钮弹跳动画并调用后续逻辑
        yield return StartCoroutine(ButtonBounceAnimation(confirmButton.GetComponent<RectTransform>(), OnConfirmButtonClicked));
    }
    private IEnumerator OncloseButtonClicked()
    {
        // 播放点击动画
        yield return StartCoroutine(HandleButtonClickAnimation(transform));

        // 执行按钮弹跳动画并调用后续逻辑
        yield return StartCoroutine(ButtonBounceAnimation(closeButton.GetComponent<RectTransform>(), OnCloseButtonClicked));
    }


    private IEnumerator OnmusicSliderClicked()
    {
        // 获取滑块的 RectTransform
        RectTransform handleRect = musicSlider.handleRect;
        if (handleRect == null)
        {
            Debug.LogError("musicSlider 缺少 handleRect！");
            yield break;
        }
        // 获取滑块在世界空间的位置
        Vector3 handleWorldPosition = handleRect.position;
        // 将世界空间位置转换为屏幕空间位置
        Vector3 clickPosition = handleWorldPosition;
        // 播放点击动画
        yield return StartCoroutine(HandleButtonClickAnimationSlider(clickPosition));
    }
    private IEnumerator OnsoundSliderClicked()
    {
        // 获取滑块的 RectTransform
        RectTransform handleRect = soundSlider.handleRect;
        if (handleRect == null)
        {
            Debug.LogError("musicSlider 缺少 handleRect！");
            yield break;
        }
        // 获取滑块在世界空间的位置
        Vector3 handleWorldPosition = handleRect.position;
        // 将世界空间位置转换为屏幕空间位置
        Vector3 clickPosition = handleWorldPosition;
        // 播放点击动画
        yield return StartCoroutine(HandleButtonClickAnimationSlider(clickPosition));
    }


    private IEnumerator OnshockSliderClicked()
    {
        // 获取滑块的 RectTransform
        RectTransform handleRect = soundSlider.handleRect;
        if (handleRect == null)
        {
            Debug.LogError("musicSlider 缺少 handleRect！");
            yield break;
        }
        // 获取滑块在世界空间的位置
        Vector3 handleWorldPosition = handleRect.position;
        // 将世界空间位置转换为屏幕空间位置
        Vector3 clickPosition = handleWorldPosition;
        // 播放点击动画
        yield return StartCoroutine(HandleButtonClickAnimationSlider(clickPosition));
    }

    /// <summary>
    /// 初始化UI，根据当前音量和震动值设置滑块和文本
    /// </summary>
    void InitializeUI()
    {
        // 设置初始值，从AudioManage获取当前的音量值
        musicSlider.value = AudioManage.Instance.musicSource.volume;
        soundSlider.value = AudioManage.Instance.GetSFXVolume();
        //shockSlider.value = VibrationManager.Instance.GetVibrationValue();
        // 更新文本
        UpdateMusicStateText(musicSlider.value);
        UpdateSoundStateText(soundSlider.value);
        UpdateShockStateText(shockSlider.value);
    }

    /// <summary>
    /// 音乐滑块值变化时更新文本
    /// </summary>
    /// <param name="value">当前滑块值</param>
    void MusicSliderValueChanged(float value)
    {
        UpdateMusicStateText(value);
    }

    /// <summary>
    /// 更新音乐状态文本
    /// </summary>
    /// <param name="value">当前音乐音量值</param>
    void UpdateMusicStateText(float value)
    {
        musicStateText.text = value > 0 ? "ON" : "OFF";
    }

    /// <summary>
    /// 音效滑块值变化时更新文本
    /// </summary>
    /// <param name="value">当前滑块值</param>
    void SoundSliderValueChanged(float value)
    {
        UpdateSoundStateText(value);
    }

    /// <summary>
    /// 更新音效状态文本
    /// </summary>
    /// <param name="value">当前音效音量值</param>
    void UpdateSoundStateText(float value)
    {
        soundStateText.text = value > 0 ? "ON" : "OFF";
    }

    /// <summary>
    /// 震动滑块值变化时更新文本
    /// </summary>
    /// <param name="value">当前滑块值</param>
    void ShockSliderValueChanged(float value)
    {
        UpdateShockStateText(value);
    }

    /// <summary>
    /// 更新震动状态文本
    /// </summary>
    /// <param name="value">当前震动值</param>
    void UpdateShockStateText(float value)
    {
        shockStateText.text = value > 0 ? "ON" : "OFF";
    }

    /// <summary>
    /// 确认按钮点击事件
    /// </summary>
    void OnConfirmButtonClicked()
    {
        // 将滑块的值应用到AudioManage和VibrationManager

        // 设置背景音乐音量
        AudioManage.Instance.musicSource.volume = musicSlider.value;

        // 设置音效音量
        AudioManage.Instance.SetSFXVolume(soundSlider.value);

        // 设置震动值
        //VibrationManager.Instance.SetVibrationValue(shockSlider.value);
        Time.timeScale = 1f; // 继续游戏

        // 销毁PausePanel面板
        Destroy(gameObject);
    }

    /// <summary>
    /// 关闭按钮点击事件
    /// </summary>
    void OnCloseButtonClicked()
    {
        Time.timeScale = 1f; // 继续游戏
        // 销毁PausePanel面板
        Destroy(gameObject);
    }
}
