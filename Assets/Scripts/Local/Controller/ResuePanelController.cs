using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class ResuePanelController : UIBase
{
    [Header("UIԪ��")]
    public Text ResueCoinNumText_F;      // ��ʾ����������ı�
    public Button ResueResueBtn_F;       // ���ť
    public Transform CoinTarget_F;       // ����ƶ���Ŀ��λ��
    public GameObject CoinPrefab;        // ���Ԥ���壬�������Ƕ���

    // �����������
    private int coinCount = 10;
    void Start()
    {
        GetAllChild(transform);
        ResueCoinNumText_F = childDic["ResueCoinNumText_F"].GetComponent<Text>();
        ResueResueBtn_F = childDic["ResueResueBtn_F"].GetComponent<Button>();
        CoinTarget_F = childDic["CoinTarget_F"].transform;
        // ��Ӱ�ť����¼�����
        if (ResueResueBtn_F != null)
        {
            ResueResueBtn_F.onClick.AddListener(OnResueBtnClicked);
        }
        else
        {
            Debug.LogError("ResueResueBtn_F δ���ã�");
        }
        //TTOD1������ʾ�Ľ����������
        ResueCoinNumText_F.text = "9789261e8";
    }

    /// <summary>
    /// ���ť�������
    /// </summary>
    private void OnResueBtnClicked()
    {
        // ���ð�ť����ֹ�ظ����
        ResueResueBtn_F.interactable = false;
        // ��ʼ���ɲ��ƶ����
        //StartCoroutine(GenerateAndMoveCoins());
        PlayerController playerController = FindObjectOfType<PlayerController>();
        playerController.Init();
        GameManage.Instance.SwitchState(GameState.Running);
        Time.timeScale = 1;
        Destroy(gameObject);
    }

    /// <summary>
    /// ���ɽ�Ҳ��ƶ���Э��
    /// </summary>
    private IEnumerator GenerateAndMoveCoins()
    {
        // ��ȡ��ʼλ�ã�ResueCoinNumText_F��λ�ã�
        Vector3 startPos = ResueCoinNumText_F.transform.position;

        // ��ȡĿ��λ�ã�CoinTarget_F��λ�ã�
        Vector3 targetPos = CoinTarget_F.position;

        // ��¼����ɵĽ������
        int completedCoins = 0;

        for (int i = 0; i < coinCount; i++)
        {
            string CoinName = "gold";
            if (PreController.Instance.CoinPools.TryGetValue(CoinName, out var selectedCoinPool))
            {
                GameObject coinObj = selectedCoinPool.Get();
                coinObj.SetActive(true);
                coinObj.transform.SetParent(transform.parent);
                coinObj.transform.position = startPos;
                var dragonBonesComponent = coinObj.transform.GetChild(0).GetComponent<DragonBones.UnityArmatureComponent>();
                if (dragonBonesComponent != null)
                {
                    dragonBonesComponent.animation.Play("newAnimation", -1);
                }
                //// ���ɽ��
                //GameObject coin = Instantiate(CoinPrefab, startPos, Quaternion.identity, transform.parent);

                // ȷ��CoinPrefab�������Ƕ��������Զ�����
                // �����Ҫ�ֶ����������������ڴ˴������ش���
                // ��ʼ�ƶ���ҵ�Э��
                StartCoroutine(MoveCoin(coinObj, targetPos, () =>
                {
                    // ����ƶ���ɺ�Ļص�
                    completedCoins++;
                    // ���ٽ�Ҷ���
                    Destroy(coinObj);
                    // ������н�Ҷ�����ƶ����������
                }));

                // Ϊ���ý�ҷ������ɣ�����һ�������ɹ��ർ���������⣬��������ӳ�
                yield return new WaitForSeconds(0.05f); // ÿ��0.05������һ�����
            }
        }
        if (completedCoins >= coinCount)
        {
            GameManage.Instance.SwitchState(GameState.Running);
            Time.timeScale = 1;
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// �ƶ���ҵ�Э��
    /// </summary>
    /// <param name="coin">Ҫ�ƶ��Ľ�Ҷ���</param>
    /// <param name="target">Ŀ��λ��</param>
    /// <param name="onComplete">�ƶ���ɺ�Ļص�</param>
    private IEnumerator MoveCoin(GameObject coin, Vector3 target, System.Action onComplete)
    {
        // ���г���ʱ��
        float duration = 0.5f; // 1��
        // ��¼������ʱ��
        float elapsed = 0f;
        // ��ʼλ��
        Vector3 start = coin.transform.position;
        // �˶����ߣ����Ը������������
        AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float curveValue = curve.Evaluate(t);
            // ʹ�ñ��������߻����������ӵ�·������ʵ�ָ���Ȼ�ķ��й켣
            // �����ʹ�����Բ�ֵ
            coin.transform.position = Vector3.Lerp(start, target, curveValue);
            yield return null;
        }
        // ȷ����ҵ���Ŀ��λ��
        coin.transform.position = target;
        // ������ɻص�
        onComplete?.Invoke();
    }
}
