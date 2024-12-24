using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class Gun
{
    public string gunName;           // 枪械名称
    public string bulletType;        // 子弹类型

    //public Gun(string name, string bullet, float rate)
    public Gun(string gun, string bullet)
    {
        gunName = gun;
        bulletType = bullet;
    }
}
public class PlayerInfo : IComparable<PlayerInfo>
{
    // 账户相关信息
    public string accountID;
    public string creationDate;
    public DateTime lastSignInDate;
    public DateTime lastSpinDate;
    public int consecutiveDays;
    public int totalCoins;
    // 玩家基本信息
    public string playerName;          // 玩家姓名
    public long coinNum;            // 玩家金币数
    public int level;                  // 玩家等级
    public long health;                // 玩家生命值
    public long experiences;           // 玩家经验值

    // 玩家属性
    public int FrozenBuffCount;        // 冻结buff数量
    public int BalstBuffCount;         // 爆破buff数量
    public float attackFac;           // 子弹攻击系数
    public float coinFac;           // 子弹攻击系数
    public float attackSpFac;         // 攻击速度系数
    public int ResueeCount;        // 冻结buff数量

    public bool isFirstSpecial = false;
    public bool isFirstReplaceGun = false;
    public bool FirstZeroToOne = false; 
    // 枪械管理
    public Gun currentGun;             // 当前选中的枪械


    // 新增字段
    public bool hasCompletedGunSelectionTutorial;


    //新手关的钱数；
    public long EarMoney;
     public long SpendMoney;
    // TTOD1永久性数据从服务器拿构造函数
    public void SetPlayerInfo(string name,long initialHealth)
    {
        playerName = name;
        health = initialHealth;
        attackSpFac = 0;
        coinFac = 0;
        attackFac = 0;
    }
    public void SetGun(string gunname,string bulletname)
    {
        if (currentGun == null)
        {
            currentGun = new Gun(gunname, bulletname);
        }
        else
        {
            currentGun.gunName = gunname;
            currentGun.bulletType = bulletname;
        }

    }
    public void SetPlayerAccount(string id, string creation, DateTime lastSignIn, int consecutive, long coinnum, int initiallevel,
        long exceperice,int balstBuffCount, int frozenBuffCount,string gunName, string bulletName, int rageSkill,int replaceGun,
        int zeroToOne)
    {
        accountID = id;
        creationDate = creation;
        lastSignInDate = lastSignIn;
        consecutiveDays = consecutive;
        totalCoins = 0; // Initialize coins
        coinNum = coinnum;
        level = initiallevel;
        experiences = exceperice;
        BalstBuffCount = balstBuffCount;
        FrozenBuffCount = frozenBuffCount;
        lastSpinDate = DateTime.MinValue; // 初始化为最小值
        if (currentGun == null)
        {
            currentGun = new Gun(gunName, bulletName);
        }
        else
        {
            currentGun.gunName = gunName;
            currentGun.bulletType = bulletName;
        }
        isFirstSpecial = rageSkill == 1?true:false;
        isFirstReplaceGun = replaceGun == 1 ? true : false;
        FirstZeroToOne =  zeroToOne == 1 ? true : false;
    }

    // Method to add coins
    public void AddCoins(long amount)
    {
        coinNum += amount;
        if(GameFlowManager.Instance.currentLevelIndex == 0)
        {
            EarMoney += amount;
        }
        AudioManage.Instance.PlaySFX("coin", null);
        EventDispatcher.instance.DispatchEvent(EventNameDef.UPDATECOIN);
    }
    // 比较接口实现，用于排序玩家数据（例如按得分排序）
    public int CompareTo(PlayerInfo other)
    {
        if (other == null) return 1;

        // 比较玩家得分
        return coinNum.CompareTo(other.coinNum);
    }
    // 扣除金币
    private Coroutine insufficientCoinsCoroutine;

    public bool SpendCoins(long amount)
    {
        if (coinNum >= amount)
        {
            coinNum -= amount;
            if (GameFlowManager.Instance.currentLevelIndex == 0)
            {
                SpendMoney += amount;
            }
            EventDispatcher.instance.DispatchEvent(EventNameDef.UPDATECOIN);
            return true;
        }
        else
        {
            // Start playing the sound effect repeatedly every 0.25 seconds
            if (insufficientCoinsCoroutine == null)
            {
                insufficientCoinsCoroutine = GameFlowManager.Instance.StartCoroutine(PlayBulletJammingRepeatedly(amount));
            }
            return false;
        }
    }

    private IEnumerator PlayBulletJammingRepeatedly(long amount)
    {
        while (coinNum < amount)
        {
            AudioManage.Instance.PlaySFX("bulletjamming", null);
            yield return new WaitForSeconds(0.25f);
        }
        // Stop the coroutine once the player has enough coins
        insufficientCoinsCoroutine = null;
    }
    // 增加得分
    //public void AddScore(int amount)
    //{
    //    score += amount;
    //    Debug.Log($"Added {amount} points. Total score: {score}");
    //}

    // 增加等级
    public void LevelUp()
    {
        level += 1;
        Debug.Log($"Leveled up! Current level: {level}");
    }

    // 扣除生命值
    public void TakeDamage(int damage)
    {
        health = (long)(Mathf.Max(health - damage, 0));
        Debug.Log($"Took {damage} damage. Remaining health: {health}");

        if (health <= 0)
        {
            Die();
        }
    }

    // 恢复生命值
    public void Heal(int amount)
    {
        health += amount;
        Debug.Log($"Healed {amount} health. Current health: {health}");
    }

    // 玩家死亡
    private void Die()
    {
        Debug.Log($"{playerName} has died.");
        // 这里可以添加游戏结束或复活逻辑
    }

    // 保存玩家数据（例如序列化到JSON）
    public string SaveData()
    {
        return JsonUtility.ToJson(this);
    }

    // 加载玩家数据（例如从JSON反序列化）
    public static PlayerInfo LoadData(string jsonData)
    {
        return JsonUtility.FromJson<PlayerInfo>(jsonData);
    }
}
