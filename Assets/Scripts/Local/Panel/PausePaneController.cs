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

    private Button confirmButton;
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
        confirmButton = childDic["Confirm_F"].GetComponent<Button>();
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
        confirmButton.onClick.AddListener(() => StartCoroutine(OnconfirmButtonClicked()));
        closeButton.onClick.AddListener(() => StartCoroutine(OncloseButtonClicked()));

        musicSlider.onValueChanged.AddListener((value) => StartCoroutine(OnmusicSliderClicked()));
        soundSlider.onValueChanged.AddListener((value) => StartCoroutine(OnsoundSliderClicked()));
        shockSlider.onValueChanged.AddListener((value) => StartCoroutine(OnshockSliderClicked()));
    }

    /// <summary>
    /// ����ǩ����ť����¼���Э��
    /// </summary>
    private IEnumerator OnconfirmButtonClicked()
    {
        // ���ŵ������
        yield return StartCoroutine(HandleButtonClickAnimation(transform));

        // ִ�а�ť�������������ú����߼�
        yield return StartCoroutine(ButtonBounceAnimation(confirmButton.GetComponent<RectTransform>(), OnConfirmButtonClicked));
    }
    private IEnumerator OncloseButtonClicked()
    {
        // ���ŵ������
        yield return StartCoroutine(HandleButtonClickAnimation(transform));

        // ִ�а�ť�������������ú����߼�
        yield return StartCoroutine(ButtonBounceAnimation(closeButton.GetComponent<RectTransform>(), OnCloseButtonClicked));
    }


    private IEnumerator OnmusicSliderClicked()
    {
        // ��ȡ����� RectTransform
        RectTransform handleRect = musicSlider.handleRect;
        if (handleRect == null)
        {
            Debug.LogError("musicSlider ȱ�� handleRect��");
            yield break;
        }
        // ��ȡ����������ռ��λ��
        Vector3 handleWorldPosition = handleRect.position;
        // ������ռ�λ��ת��Ϊ��Ļ�ռ�λ��
        Vector3 clickPosition = handleWorldPosition;
        // ���ŵ������
        yield return StartCoroutine(HandleButtonClickAnimationSlider(clickPosition));
    }
    private IEnumerator OnsoundSliderClicked()
    {
        // ��ȡ����� RectTransform
        RectTransform handleRect = soundSlider.handleRect;
        if (handleRect == null)
        {
            Debug.LogError("musicSlider ȱ�� handleRect��");
            yield break;
        }
        // ��ȡ����������ռ��λ��
        Vector3 handleWorldPosition = handleRect.position;
        // ������ռ�λ��ת��Ϊ��Ļ�ռ�λ��
        Vector3 clickPosition = handleWorldPosition;
        // ���ŵ������
        yield return StartCoroutine(HandleButtonClickAnimationSlider(clickPosition));
    }


    private IEnumerator OnshockSliderClicked()
    {
        // ��ȡ����� RectTransform
        RectTransform handleRect = soundSlider.handleRect;
        if (handleRect == null)
        {
            Debug.LogError("musicSlider ȱ�� handleRect��");
            yield break;
        }
        // ��ȡ����������ռ��λ��
        Vector3 handleWorldPosition = handleRect.position;
        // ������ռ�λ��ת��Ϊ��Ļ�ռ�λ��
        Vector3 clickPosition = handleWorldPosition;
        // ���ŵ������
        yield return StartCoroutine(HandleButtonClickAnimationSlider(clickPosition));
    }

    /// <summary>
    /// ��ʼ��UI�����ݵ�ǰ��������ֵ���û�����ı�
    /// </summary>
    void InitializeUI()
    {
        // ���ó�ʼֵ����AudioManage��ȡ��ǰ������ֵ
        musicSlider.value = AudioManage.Instance.musicSource.volume;
        soundSlider.value = AudioManage.Instance.GetSFXVolume();
        //shockSlider.value = VibrationManager.Instance.GetVibrationValue();
        // �����ı�
        UpdateMusicStateText(musicSlider.value);
        UpdateSoundStateText(soundSlider.value);
        UpdateShockStateText(shockSlider.value);
    }

    /// <summary>
    /// ���ֻ���ֵ�仯ʱ�����ı�
    /// </summary>
    /// <param name="value">��ǰ����ֵ</param>
    void MusicSliderValueChanged(float value)
    {
        UpdateMusicStateText(value);
    }

    /// <summary>
    /// ��������״̬�ı�
    /// </summary>
    /// <param name="value">��ǰ��������ֵ</param>
    void UpdateMusicStateText(float value)
    {
        musicStateText.text = value > 0 ? "ON" : "OFF";
    }

    /// <summary>
    /// ��Ч����ֵ�仯ʱ�����ı�
    /// </summary>
    /// <param name="value">��ǰ����ֵ</param>
    void SoundSliderValueChanged(float value)
    {
        UpdateSoundStateText(value);
    }

    /// <summary>
    /// ������Ч״̬�ı�
    /// </summary>
    /// <param name="value">��ǰ��Ч����ֵ</param>
    void UpdateSoundStateText(float value)
    {
        soundStateText.text = value > 0 ? "ON" : "OFF";
    }

    /// <summary>
    /// �𶯻���ֵ�仯ʱ�����ı�
    /// </summary>
    /// <param name="value">��ǰ����ֵ</param>
    void ShockSliderValueChanged(float value)
    {
        UpdateShockStateText(value);
    }

    /// <summary>
    /// ������״̬�ı�
    /// </summary>
    /// <param name="value">��ǰ��ֵ</param>
    void UpdateShockStateText(float value)
    {
        shockStateText.text = value > 0 ? "ON" : "OFF";
    }

    /// <summary>
    /// ȷ�ϰ�ť����¼�
    /// </summary>
    void OnConfirmButtonClicked()
    {
        // �������ֵӦ�õ�AudioManage��VibrationManager

        // ���ñ�����������
        AudioManage.Instance.musicSource.volume = musicSlider.value;

        // ������Ч����
        AudioManage.Instance.SetSFXVolume(soundSlider.value);

        // ������ֵ
        //VibrationManager.Instance.SetVibrationValue(shockSlider.value);
        Time.timeScale = 1f; // ������Ϸ

        // ����PausePanel���
        Destroy(gameObject);
    }

    /// <summary>
    /// �رհ�ť����¼�
    /// </summary>
    void OnCloseButtonClicked()
    {
        Time.timeScale = 1f; // ������Ϸ
        // ����PausePanel���
        Destroy(gameObject);
    }
}
