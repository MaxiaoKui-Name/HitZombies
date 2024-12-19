// PausePaneController.cs ��ͣ��������
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PausePaneController : UIBase
{
    // ����UIԪ�صı���
    private Slider musicSlider;
    private TextMeshProUGUI musicStateText;
    private Slider soundSlider;
    private TextMeshProUGUI soundStateText;
    private Slider shockSlider;
    private TextMeshProUGUI shockStateText;

    private Button ConfirmBtn;//������Ϸ��ť
    private Button closeButton;

    void Start()
    {
        GetAllChild(transform);

        // ��ȡUIԪ��
        musicSlider = childDic["MusicSlider_F"].GetComponent<Slider>();
        musicStateText = childDic["musickOpenText_F"].GetComponent<TextMeshProUGUI>();
        soundSlider = childDic["soundSlider_F"].GetComponent<Slider>();
        soundStateText = childDic["soundOpenText_F"].GetComponent<TextMeshProUGUI>();
        shockSlider = childDic["shockSlider_F"].GetComponent<Slider>();
        shockStateText = childDic["shockOpenText_F"].GetComponent<TextMeshProUGUI>();
        ConfirmBtn = childDic["Confirm_F"].GetComponent<Button>();
        closeButton = childDic["CloseBtn_F"].GetComponent<Button>();

        // ��ʼ��������ı�
        InitializeUI();

        // ��ӻ���ֵ�仯�ļ����¼�
        //musicSlider.onValueChanged.AddListener(MusicSliderValueChanged);
        //soundSlider.onValueChanged.AddListener(SoundSliderValueChanged);
        //shockSlider.onValueChanged.AddListener(ShockSliderValueChanged);

        // ��Ӱ�ť����¼�
        //confirmButton.onClick.AddListener(OnConfirmButtonClicked);
        //closeButton.onClick.AddListener(OnCloseButtonClicked);
        // ��ʼ���������Ϊ0
        RectTransform panelRect = GetComponent<RectTransform>();
        StartCoroutine(PopUpAnimation(panelRect));
        // ��ȡ ClickAMature �����䶯�����
        GetClickAnim(transform);
        // �޸İ�ť����¼�������
        ConfirmBtn.onClick.AddListener(() => StartCoroutine(OnconfirmButtonClicked()));
        closeButton.onClick.AddListener(() => StartCoroutine(OncloseButtonClicked()));

        // ��ӻ���Handle�������
        musicSlider.handleRect.GetComponent<Button>().onClick.AddListener(() => StartCoroutine(OnSliderHandleClicked(musicSlider, musicStateText)));
        soundSlider.handleRect.GetComponent<Button>().onClick.AddListener(() => StartCoroutine(OnSliderHandleClicked(soundSlider, soundStateText)));
        shockSlider.handleRect.GetComponent<Button>().onClick.AddListener(() => StartCoroutine(OnSliderHandleClicked(shockSlider, shockStateText)));
    }

    /// <summary>
    /// ����ǩ����ť����¼���Э��
    /// </summary>
    private IEnumerator OnconfirmButtonClicked()
    {
        AudioManage.Instance.PlaySFX("button", null);
        // ���ŵ������
        yield return StartCoroutine(HandleButtonClickAnimation(transform));

        // ִ�а�ť�������������ú����߼�
        yield return StartCoroutine(ButtonBounceAnimation(ConfirmBtn.GetComponent<RectTransform>(), OnConfirmButtonClicked));
    }
    private IEnumerator OncloseButtonClicked()
    {
        AudioManage.Instance.PlaySFX("button", null);
        // ���ŵ������
        yield return StartCoroutine(HandleButtonClickAnimation(transform));

        // ִ�а�ť�������������ú����߼�
        yield return StartCoroutine(ButtonBounceAnimation(closeButton.GetComponent<RectTransform>(), OnCloseButtonClicked));
    }

    /// <summary>
    /// ��ʼ��UI�����ݵ�ǰ��������ֵ���û�����ı�
    /// </summary>
    void InitializeUI()
    {
        musicSlider.value = 1f;
        soundSlider.value = 1f;
        shockSlider.value = 1f; // �����𶯳�ʼֵΪ1
        UpdateMusicStateText(musicSlider.value);
        UpdateSoundStateText(soundSlider.value);
        UpdateShockStateText(shockSlider.value);
    }

    /// <summary>
    /// ���ֻ���ֵ�仯ʱ�����ı�
    /// </summary>
    /// <param name="value">��ǰ����ֵ</param>
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
        // VibrationManager.Instance.SetVibrationValue(shockSlider.value); �������𶯹�����
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
    /// �رհ�ť����¼�
    /// </summary>
    void OnCloseButtonClicked()
    {
        Destroy(gameObject);
    }

    void OnConfirmButtonClicked()
    {
        // ���ñ�����������
        AudioManage.Instance.musicSource.volume = musicSlider.value;
        // ������Ч����
        AudioManage.Instance.SetSFXVolume(soundSlider.value);
        // ����PausePanel���
        Destroy(gameObject);
    }
}
