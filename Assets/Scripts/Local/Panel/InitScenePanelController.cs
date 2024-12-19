using System.Collections;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using DragonBones;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class InitScenePanelController : UIBase
{
    public Slider progressBar; // ������
    ///public TextMeshProUGUI progressText;  // ���Ȱٷֱ��ı�
    //public TextMeshProUGUI updateText;
    public TextMeshProUGUI StartText_F; // ��ʼ���ı�
    public GameObject Backlogo_F; // ��Ϸ��ʼlogo
    public GameObject ClickAnim_F; // ����Ķ���Ч��
    public UnityArmatureComponent LoadAnim_F;
    public GameObject Back_F;

    private bool isClickTriggered = false; // �ж��Ƿ񴥷��������
    private bool isScalingUp = true; // �ж��Ƿ����ڷŴ�
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
    // ��ʾ Backlogo_F ��ִ�ж���
    private IEnumerator ShowLogoAndStartText()
    {
        // ��ʾ Backlogo_F 1��
        Backlogo_F.SetActive(true);
        yield return new WaitForSeconds(1f);
        Backlogo_F.SetActive(false);
        // ��ʾ Back_F �� StartText_F�����𽥷Ŵ� StartText_F
        Back_F.SetActive(true);
        StartText_F.gameObject.SetActive(true);
        // ������С�仯��ѭ��
        StartCoroutine(ScaleStartText());
    }

    // StartText_F�Ĵ�С�仯ѭ��
    private IEnumerator ScaleStartText()
    {
        float scaleTime = 0.5f; // ÿ�ηŴ����С��ʱ��
        Vector3 initialScale = StartText_F.transform.localScale;
        Vector3 targetScale = initialScale * 1.4f;
        float timer = 0f;

        while (!isClickTriggered)
        {
            if (isScalingUp)
            {
                // �Ŵ�
                while (timer < scaleTime)
                {
                    StartText_F.transform.localScale = Vector3.Lerp(initialScale, targetScale, timer / scaleTime);
                    timer += Time.deltaTime;
                    yield return null;
                }
                StartText_F.transform.localScale = targetScale; // ȷ�����մ�СΪĿ���С
            }
            else
            {
                // ��С
                while (timer < scaleTime)
                {
                    StartText_F.transform.localScale = Vector3.Lerp(targetScale, initialScale, timer / scaleTime);
                    timer += Time.deltaTime;
                    yield return null;
                }
                StartText_F.transform.localScale = initialScale; // ȷ�����մ�СΪ��ʼ��С
            }

            // �л�״̬����������
            isScalingUp = !isScalingUp;
            timer = 0f; // ���ü�ʱ��
        }
    }

    // ���������
    void Update()
    {
        if (!isClickTriggered && (Input.GetMouseButtonDown(0) || Input.touchCount > 0))
        {
            Vector3 clickPosition = Input.GetMouseButtonDown(0) ? Input.mousePosition : Input.GetTouch(0).position;
            // ���� ClickAnim_F ��λ��Ϊ���λ�ã������Ŷ���
            ClickAnim_F.transform.position = clickPosition;
            ClickAnim_F.SetActive(true);
            isClickTriggered = true;
            UnityArmatureComponent animator = ClickAnim_F.GetComponent<UnityArmatureComponent>();
            animator.animation.Play("click",1);  // ���趯����Ϊ "click"
            // �����궯�������� StartText_F �� ClickAnim_F
            StartCoroutine(WaitForClickAnim(animator.animation.GetState("click")._duration));
        }
    }

    // �ȴ� ClickAnim_F �������
    private IEnumerator WaitForClickAnim(float animationLength)
    {
        yield return new WaitForSeconds(animationLength);
        // ����������ɺ����� StartText_F �� ClickAnim_F
        StartText_F.gameObject.SetActive(false);
        ClickAnim_F.SetActive(false);
        // ������Դ����
        LoadDll.Instance.InitAddressable();
    }

    // ���½�����
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
                LoadAnim_F.animation.Play("<None>", -1);  // ���趯����Ϊ "click"
            }
        }

        //if (progressText != null)
        //{
        //    progressText.text = $"{(progress * 100):0.00}%";
        //}
    }

    // �����ı�
    //public void UpdateText(string sentance)
    //{
    //    updateText.text = sentance;
    //}
}
