using Cysharp.Threading.Tasks;
using DragonBones;
using Hitzb;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameMainPanelController : UIBase
{
    public Button pauseButton;   // 引用暂停按钮
    public TextMeshProUGUI coinText;        // 引用显示金币的文本框
    private bool isPaused = false;
    public TextMeshProUGUI buffFrozenText;        
    public Button buffFrozenBtn;       
    public TextMeshProUGUI buffBlastText;      
    public Button buffBlastBtn;


    [Header("Spawn Properties")]
    public float bombDropInterval;  // 炸弹投掷间隔时间
    public float planeSpeed = 2f;   // 飞机移动速度
    void Start()
    {
        GetAllChild(transform);

        // 找到子对象中的按钮和文本框
        pauseButton = childDic["pause_Btn_F"].GetComponent<Button>();
        coinText = childDic["valueText_F"].GetComponent<TextMeshProUGUI>();
        buffFrozenText = childDic["Frozentimes_F"].GetComponent<TextMeshProUGUI>();
        buffFrozenBtn = childDic["FrozenBtn_F"].GetComponent<Button>();
        buffBlastText = childDic["Blastimes_F"].GetComponent<TextMeshProUGUI>();
        buffBlastBtn = childDic["BlastBtn_F"].GetComponent<Button>();
        // 添加暂停按钮的点击事件监听器
        pauseButton.onClick.AddListener(TogglePause);
        buffFrozenBtn.onClick.AddListener(ToggleFrozen);
        buffBlastBtn.onClick.AddListener(ToggleBlast);
    }

    void Update()
    {
        // 实时更新显示的金币数量
        coinText.text = $"{PlayInforManager.Instance.playInfor.coinNum}";
    }

    // 切换暂停和继续游戏的状态
    void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f; // 暂停游戏
        }
        else
        {
            Time.timeScale = 1f; // 继续游戏
        }
    }
    public void UpdateBuffText(int FrozenBuffCount,int BalstBuffCount)
    {
        buffFrozenText.text = $"{FrozenBuffCount}";
        buffBlastText.text = $"{BalstBuffCount}";
    }
    void ToggleBlast()
    {
        //执行全屏爆炸功能
        if(PlayInforManager.Instance.playInfor.BalstBuffCount > 0)
        {
            PlayInforManager.Instance.playInfor.BalstBuffCount--;
            UpdateBuffText(PlayInforManager.Instance.playInfor.FrozenBuffCount, PlayInforManager.Instance.playInfor.BalstBuffCount);
            SpawnPlane().Forget();
        }
    }
    void ToggleFrozen()
    {
        //执行全屏冰冻功能
        if (PlayInforManager.Instance.playInfor.FrozenBuffCount > 0)
        {
            PlayInforManager.Instance.playInfor.FrozenBuffCount--;
            UpdateBuffText(PlayInforManager.Instance.playInfor.FrozenBuffCount, PlayInforManager.Instance.playInfor.BalstBuffCount);
            SpawnFrozenPlane().Forget();
        }
    }
    #region 全屏爆炸逻辑
    public async UniTask SpawnPlane()
    {
        GameObject plane = Instantiate(Resources.Load<GameObject>("Prefabs/explode_bomber"), new Vector3(0, -7f, 0), Quaternion.identity);  // 生成飞机在屏幕底部
        Debug.Log("Plane spawned!");
        await MovePlaneAndDropBombs(plane);
        if (plane != null)
        {
            Destroy(plane);
        }
    }
    // 飞机移动并投放炸弹的异步方法
    private async UniTask MovePlaneAndDropBombs(GameObject plane)
    {
        float dropTime = 0f;
        bool isThrow = false;
        while (plane != null && plane.activeSelf && plane.transform.position.y < 6f)
        {
            // Move the plane upwards
            plane.transform.Translate(Vector3.up * planeSpeed * Time.deltaTime);
            //dropTime += Time.deltaTime;

            //// TTOD1投放炸弹逻辑
            //if (dropTime >= bombDropInterval)
            //{
            //    dropTime = 0f;
            //    Vector3 bombPosition = PreController.Instance.RandomPosition(plane.transform.position);
            //    DropBomb(bombPosition).Forget();
            //}
            //// Yield control to allow other operations
            //await UniTask.Yield();
            // TTOD1投放炸弹逻辑
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
        // 投放炸弹（异步）
        private async UniTask DropBomb(Vector3 planePosition)
        {
            GameObject bomb = Instantiate(Resources.Load<GameObject>("Prefabs/explode_01"), planePosition, Quaternion.identity);
            await BombExplosion(bomb);
        }

        // 炸弹爆炸动画，并消灭敌人（异步）
        private async UniTask BombExplosion(GameObject bomb)
        {
            UnityArmatureComponent bombArmature = bomb.GetComponentInChildren<UnityArmatureComponent>();
            if (bombArmature != null)
            {
                bombArmature.animation.Play("fly", 1); // 播放一次飞行动画
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
    #region 全屏冰冻逻辑

    public async UniTask SpawnFrozenPlane()
    {
        GameObject plane = Instantiate(Resources.Load<GameObject>("Prefabs/explode_bomber"), new Vector3(0, -7f, 0), Quaternion.identity);  // 生成冰冻飞机在屏幕底部
        Debug.Log("Frozen Plane spawned!");
        await MoveFrozenPlaneAndDropBombs(plane);
        if (plane != null)
        {
            Destroy(plane);
        }
    }

    // 飞机移动并投放冰冻炸弹的异步方法
    private async UniTask MoveFrozenPlaneAndDropBombs(GameObject plane)
    {
        while (plane != null && plane.activeSelf && plane.transform.position.y < 3f) // 假设 3 是场景中间的 Y 位置
        {
            plane.transform.Translate(Vector3.up * planeSpeed * Time.deltaTime);
            await UniTask.Yield();
        }

        // 在飞机到达场景中间时投放冰冻炸弹
        if (plane != null)
        {
            Vector3 bombPosition = PreController.Instance.RandomPosition(plane.transform.position);
            DropFrozenBomb(bombPosition).Forget();
        }
    }

    // 投放冰冻炸弹（异步）
    private async UniTask DropFrozenBomb(Vector3 planePosition)
    {
        GameObject frozenBomb = Instantiate(Resources.Load<GameObject>("Prefabs/explode_01"), planePosition, Quaternion.identity);
        await FrozenBombEffect(frozenBomb);
    }

    // 冰冻炸弹效果
    private async UniTask FrozenBombEffect(GameObject bomb)
    {
        // 播放冰冻效果动画
        UnityArmatureComponent bombArmature = bomb.GetComponentInChildren<UnityArmatureComponent>();
        if (bombArmature != null)
        {
            bombArmature.animation.Play("fly", 1); // 播放冰冻动画
        }
        // 暂停所有敌人和宝箱
        FreezeAllEnemiesAndChests();
        // 等待 5 秒
        await UniTask.Delay(5000);
        // 解除冰冻效果
        UnfreezeAllEnemiesAndChests();
        Destroy(bomb);
    }

    // 冻结所有敌人和宝箱
    private void FreezeAllEnemiesAndChests()
    {
        GameManage.Instance.isFrozen = true;
        PreController.Instance.isFrozen = true;
        Debug.Log("开始冰冻=========================！!！");
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            EnemyController enemyController = enemy.GetComponent<EnemyController>();
            if (enemyController != null && !enemyController.isDead)
            {
                enemyController.isFrozen = true; // 添加一个 isFrozen 属性来控制敌人状态
            }
        }

        GameObject[] chests = GameObject.FindGameObjectsWithTag("Chest");
        foreach (GameObject chest in chests)
        {
            // 暂停宝箱逻辑
            // 例如可以设置一个 isFrozen 属性
            ChestController chestController = chest.GetComponent<ChestController>();
            if (chestController != null && chestController.isVise)
            {
                chestController.isFrozen = true; // 添加一个 isFrozen 属性来控制敌人状态
            }
        }
    }

    // 解除冻结效果
    private void UnfreezeAllEnemiesAndChests()
    {
        GameManage.Instance.SetFrozenState(false);
        PreController.Instance.isFrozen = false;
        Debug.Log("解除冰冻=========================！!！");
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            EnemyController enemyController = enemy.GetComponent<EnemyController>();
            if (enemyController != null && !enemyController.isDead)
            {
                enemyController.isFrozen = false; // 解除冰冻
            }
        }

        GameObject[] chests = GameObject.FindGameObjectsWithTag("Chest");
        foreach (GameObject chest in chests)
        {
            // 恢复宝箱逻辑
            ChestController chestController = chest.GetComponent<ChestController>();
            if (chestController != null && chestController.isVise)
            {
                chestController.isFrozen = false; // 添加一个 isFrozen 属性来控制敌人状态
            }
        }
    }
    #endregion
}
