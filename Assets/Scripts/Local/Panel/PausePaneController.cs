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

    private Button ConfirmBtn;//继续游戏按钮
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
        ConfirmBtn = childDic["Confirm_F"].GetComponent<Button>();
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
        ConfirmBtn.onClick.AddListener(() => StartCoroutine(OnconfirmButtonClicked()));
        closeButton.onClick.AddListener(() => StartCoroutine(OncloseButtonClicked()));

        // 添加滑块Handle点击监听
        musicSlider.handleRect.GetComponent<Button>().onClick.AddListener(() => StartCoroutine(OnSliderHandleClicked(musicSlider, musicStateText)));
        soundSlider.handleRect.GetComponent<Button>().onClick.AddListener(() => StartCoroutine(OnSliderHandleClicked(soundSlider, soundStateText)));
        shockSlider.handleRect.GetComponent<Button>().onClick.AddListener(() => StartCoroutine(OnSliderHandleClicked(shockSlider, shockStateText)));
    }

    /// <summary>
    /// 处理签到按钮点击事件的协程
    /// </summary>
    private IEnumerator OnconfirmButtonClicked()
    {
        AudioManage.Instance.PlaySFX("button", null);
        // 播放点击动画
        yield return StartCoroutine(HandleButtonClickAnimation(transform));

        // 执行按钮弹跳动画并调用后续逻辑
        yield return StartCoroutine(ButtonBounceAnimation(ConfirmBtn.GetComponent<RectTransform>(), OnConfirmButtonClicked));
    }
    private IEnumerator OncloseButtonClicked()
    {
        AudioManage.Instance.PlaySFX("button", null);
        // 播放点击动画
        yield return StartCoroutine(HandleButtonClickAnimation(transform));

        // 执行按钮弹跳动画并调用后续逻辑
        yield return StartCoroutine(ButtonBounceAnimation(closeButton.GetComponent<RectTransform>(), OnCloseButtonClicked));
    }

    /// <summary>
    /// 初始化UI，根据当前音量和震动值设置滑块和文本
    /// </summary>
    void InitializeUI()
    {
        musicSlider.value = 1f;
        soundSlider.value = 1f;
        shockSlider.value = 1f; // 假设震动初始值为1
        UpdateMusicStateText(musicSlider.value);
        UpdateSoundStateText(soundSlider.value);
        UpdateShockStateText(shockSlider.value);
    }

    /// <summary>
    /// 音乐滑块值变化时更新文本
    /// </summary>
    /// <param name="value">当前滑块值</param>
    private IEnumerator OnSliderHandleClicked(Slider slider, TextMeshProUGUI stateText)
    {
        AudioManage.Instance.PlaySFX("button", null);
        float targetValue = slider.value > 0 ? 0f : 1f;
        string targetState = targetValue > 0 ? "ON" : "OFF";

        float startValue = slider.value;
        float elapsedTime = 0f;
        float duration = 0.2f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.unscaledTime;
            slider.value = Mathf.Lerp(startValue, targetValue, elapsedTime / duration);
            yield return null;
        }

        slider.value = targetValue;
        stateText.text = targetState;
        ApplySliderValues();
    }

    private void ApplySliderValues()
    {
        AudioManage.Instance.musicSource.volume = musicSlider.value;
        AudioManage.Instance.SetSFXVolume(soundSlider.value);
        // VibrationManager.Instance.SetVibrationValue(shockSlider.value); 假设有震动管理器
    }

    void UpdateMusicStateText(float value)
    {
        musicStateText.text = value > 0 ? "ON" : "OFF";
    }

    void UpdateSoundStateText(float value)
    {
        soundStateText.text = value > 0 ? "ON" : "OFF";
    }

    void UpdateShockStateText(float value)
    {
        shockStateText.text = value > 0 ? "ON" : "OFF";
    }
    /// <summary>
    /// 关闭按钮点击事件
    /// </summary>
    void OnCloseButtonClicked()
    {
        Destroy(gameObject);
    }

    void OnConfirmButtonClicked()
    {
        // 设置背景音乐音量
        AudioManage.Instance.musicSource.volume = musicSlider.value;
        // 设置音效音量
        AudioManage.Instance.SetSFXVolume(soundSlider.value);
        // 销毁PausePanel面板
        Destroy(gameObject);
    }
}
