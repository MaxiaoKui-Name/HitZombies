using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InPausePanelController : UIBase
{
    private Slider musicSlider;
    private TextMeshProUGUI musicStateText;
    private Slider soundSlider;
    private TextMeshProUGUI soundStateText;
    private Slider shockSlider;
    private TextMeshProUGUI shockStateText;

    private Button Continue_F; // 继续游戏按钮
    private Button CloseBtn_F; // 返回主页按钮
    //private Button closeButton;

    private GameObject readyUIPanelPrefab; // ReadyUI预制体

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
        Continue_F = childDic["Continue_F"].GetComponent<Button>();
        CloseBtn_F = childDic["ReturnBtn_F"].GetComponent<Button>();
        //closeButton = childDic["CloseBtn_F"].GetComponent<Button>();

        InitializeUI();

        Continue_F.onClick.AddListener(() => StartCoroutine(OnContinueButtonClicked()));
        CloseBtn_F.onClick.AddListener(() => StartCoroutine(OnReturnButtonClicked()));
        //closeButton.onClick.AddListener(() => StartCoroutine(OnCloseButtonClicked()));

        // 添加滑块Handle点击监听
        musicSlider.handleRect.GetComponent<Button>().onClick.AddListener(() => StartCoroutine(OnSliderHandleClicked(musicSlider, musicStateText)));
        soundSlider.handleRect.GetComponent<Button>().onClick.AddListener(() => StartCoroutine(OnSliderHandleClicked(soundSlider, soundStateText)));
        shockSlider.handleRect.GetComponent<Button>().onClick.AddListener(() => StartCoroutine(OnSliderHandleClicked(shockSlider, shockStateText)));
    }

    void InitializeUI()
    {
        musicSlider.value = 1f;
        soundSlider.value = 1f;
        shockSlider.value = 1f; // 假设震动初始值为1
        UpdateMusicStateText(musicSlider.value);
        UpdateSoundStateText(soundSlider.value);
        UpdateShockStateText(shockSlider.value);
    }

    private IEnumerator OnContinueButtonClicked()
    {
        AudioManage.Instance.PlaySFX("button", null);
        yield return StartCoroutine(HandleButtonClickAnimation(transform));
        Time.timeScale = 1f;
        GameMainPanelController gameMainPanelController = FindObjectOfType<GameMainPanelController>();
        gameMainPanelController.pauseButton.interactable = true;
        Destroy(gameObject);
    }

    private IEnumerator OnReturnButtonClicked()
    {
        AudioManage.Instance.PlaySFX("button", null);
        yield return StartCoroutine(HandleButtonClickAnimation(transform));
        GameManage.Instance.GameOverReset();
        PlayInforManager.Instance.playInfor.attackSpFac = 0;
        GameMainPanelController gameMainPanelController = FindObjectOfType<GameMainPanelController>();
        Destroy(gameMainPanelController.gameObject);
        UIManager.Instance.ChangeState(GameState.Ready);
        GameManage.Instance.InitialPalyer();
        Destroy(gameObject);
        //TTOD1进行修改
        if (readyUIPanelPrefab == null)
        {
            readyUIPanelPrefab = Instantiate(Resources.Load<GameObject>("Prefabs/UIPannel/ReadyPanel"));
            readyUIPanelPrefab.transform.SetParent(transform, false);
            readyUIPanelPrefab.transform.localPosition = Vector3.zero;
        }
    }

    //private IEnumerator OnCloseButtonClicked()
    //{
    //    yield return StartCoroutine(HandleButtonClickAnimation(closeButton.transform));
    //    Time.timeScale = 1f;
    //    Destroy(gameObject);
    //}

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
}
