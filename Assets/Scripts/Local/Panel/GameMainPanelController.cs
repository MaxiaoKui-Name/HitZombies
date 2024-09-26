using Cysharp.Threading.Tasks;
using DragonBones;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameMainPanelController : UIBase
{
    public Button pauseButton;   // ������ͣ��ť
    public TextMeshProUGUI coinText;        // ������ʾ��ҵ��ı���
    private bool isPaused = false;
    public TextMeshProUGUI buffFrozenText;        
    public Button buffFrozenBtn;       
    public TextMeshProUGUI buffBlastText;      
    public Button buffBlastBtn;


    [Header("Spawn Properties")]
    public float bombDropInterval;  // ը��Ͷ�����ʱ��
    public float planeSpeed = 2f;   // �ɻ��ƶ��ٶ�
    void Start()
    {
        GetAllChild(transform);

        // �ҵ��Ӷ����еİ�ť���ı���
        pauseButton = childDic["pause_Btn_F"].GetComponent<Button>();
        coinText = childDic["valueText_F"].GetComponent<TextMeshProUGUI>();
        buffFrozenText = childDic["Frozentimes_F"].GetComponent<TextMeshProUGUI>();
        buffFrozenBtn = childDic["FrozenBtn_F"].GetComponent<Button>();
        buffBlastText = childDic["Blastimes_F"].GetComponent<TextMeshProUGUI>();
        buffBlastBtn = childDic["BlastBtn_F"].GetComponent<Button>();
        // �����ͣ��ť�ĵ���¼�������
        pauseButton.onClick.AddListener(TogglePause);
        buffFrozenBtn.onClick.AddListener(ToggleFrozen);
        buffBlastBtn.onClick.AddListener(ToggleBlast);
    }

    void Update()
    {
        // ʵʱ������ʾ�Ľ������
        coinText.text = $"{PlayInforManager.Instance.playInfor.coinNum}";
    }

    // �л���ͣ�ͼ�����Ϸ��״̬
    void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f; // ��ͣ��Ϸ
        }
        else
        {
            Time.timeScale = 1f; // ������Ϸ
        }
    }
    public void UpdateBuffText(int FrozenBuffCount,int BalstBuffCount)
    {
        buffFrozenText.text = $"{FrozenBuffCount}";
        buffBlastText.text = $"{BalstBuffCount}";
    }
    void ToggleBlast()
    {
        //ִ��ȫ����ը����
        if(PlayInforManager.Instance.playInfor.BalstBuffCount > 0)
        {
            PlayInforManager.Instance.playInfor.BalstBuffCount--;
            UpdateBuffText(PlayInforManager.Instance.playInfor.FrozenBuffCount, PlayInforManager.Instance.playInfor.BalstBuffCount);
            SpawnPlane().Forget();
        }
    }
    void ToggleFrozen()
    {
        //ִ��ȫ����������
        if (PlayInforManager.Instance.playInfor.FrozenBuffCount > 0)
        {
            PlayInforManager.Instance.playInfor.FrozenBuffCount--;
            UpdateBuffText(PlayInforManager.Instance.playInfor.FrozenBuffCount, PlayInforManager.Instance.playInfor.BalstBuffCount);
           //TTOD1���ӱ����߼�

        }
    }
    #region ȫ����ը�߼�
    public async UniTask SpawnPlane()
    {
        GameObject plane = Instantiate(Resources.Load<GameObject>("Prefabs/explode_bomber"), new Vector3(0, -7f, 0), Quaternion.identity);  // ���ɷɻ�����Ļ�ײ�
        Debug.Log("Plane spawned!");
        await MovePlaneAndDropBombs(plane);
        if (plane != null)
        {
            Destroy(plane);
        }
    }
    // �ɻ��ƶ���Ͷ��ը�����첽����
    private async UniTask MovePlaneAndDropBombs(GameObject plane)
    {
        float dropTime = 0f;
        bool isThrow = false;
        while (plane != null && plane.activeSelf && plane.transform.position.y < 6f)
        {
            // Move the plane upwards
            plane.transform.Translate(Vector3.up * planeSpeed * Time.deltaTime);
            //dropTime += Time.deltaTime;

            //// TTOD1Ͷ��ը���߼�
            //if (dropTime >= bombDropInterval)
            //{
            //    dropTime = 0f;
            //    Vector3 bombPosition = PreController.Instance.RandomPosition(plane.transform.position);
            //    DropBomb(bombPosition).Forget();
            //}
            //// Yield control to allow other operations
            //await UniTask.Yield();
            // TTOD1Ͷ��ը���߼�
            if (plane.transform.position.y > 0 && !isThrow)
            {
                isThrow = true;
                Vector3 bombPosition = PreController.Instance.RandomPosition(plane.transform.position);
                DropBomb(bombPosition).Forget();
            }
            // Yield control to allow other operations
            await UniTask.Yield();
        }
    }
        // Ͷ��ը�����첽��
        private async UniTask DropBomb(Vector3 planePosition)
        {
            GameObject bomb = Instantiate(Resources.Load<GameObject>("Prefabs/explode_01"), planePosition, Quaternion.identity);
            await BombExplosion(bomb);
        }

        // ը����ը��������������ˣ��첽��
        private async UniTask BombExplosion(GameObject bomb)
        {
            UnityArmatureComponent bombArmature = bomb.GetComponentInChildren<UnityArmatureComponent>();
            if (bombArmature != null)
            {
                bombArmature.animation.Play("fly", 1); // ����һ�η��ж���
            }
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject enemy in enemies)
            {
                if (enemy.activeSelf)
                {
                    EnemyController enemyController = enemy.GetComponent<EnemyController>();
                    if (!enemyController.isDead && enemyController.isVise)
                    {
                        enemyController.Enemycoins2 = 10;
                        enemyController.TakeDamage(100000, enemy);
                    }
                }
            }
            await UniTask.Delay(1000);
            Destroy(bomb);
        }
    #endregion
    #region ȫ�������߼�
    #endregion
}
