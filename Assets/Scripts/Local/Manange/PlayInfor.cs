using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class Gun
{
    public string gunName;           // ǹе����
    public string bulletType;        // �ӵ�����

    //public Gun(string name, string bullet, float rate)
    public Gun(string gun, string bullet)
    {
        gunName = gun;
        bulletType = bullet;
    }
}
public class PlayerInfo : IComparable<PlayerInfo>
{
    // �˻������Ϣ
    public string accountID;
    public string creationDate;
    public DateTime lastSignInDate;
    public DateTime lastSpinDate;
    public int consecutiveDays;
    public int totalCoins;
    // ��һ�����Ϣ
    public string playerName;          // �������
    public long coinNum;            // ��ҽ����
    public int level;                  // ��ҵȼ�
    public long health;                // �������ֵ
    public long experiences;           // ��Ҿ���ֵ

    // �������
    public int FrozenBuffCount;        // ����buff����
    public int BalstBuffCount;         // ����buff����
    public float attackFac;           // �ӵ�����ϵ��
    public float coinFac;           // �ӵ�����ϵ��
    public float attackSpFac;         // �����ٶ�ϵ��
    public int ResueeCount;        // ����buff����

    public bool isFirstSpecial = false;
    public bool isFirstReplaceGun = false;
    public bool FirstZeroToOne = false; 
    // ǹе����
    public Gun currentGun;             // ��ǰѡ�е�ǹе


    // �����ֶ�
    public bool hasCompletedGunSelectionTutorial;


    //���ֹص�Ǯ����
    public long EarMoney;
     public long SpendMoney;
    // TTOD1���������ݴӷ������ù��캯��
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
        lastSpinDate = DateTime.MinValue; // ��ʼ��Ϊ��Сֵ
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
    // �ȽϽӿ�ʵ�֣���������������ݣ����簴�÷�����
    public int CompareTo(PlayerInfo other)
    {
        if (other == null) return 1;

        // �Ƚ���ҵ÷�
        return coinNum.CompareTo(other.coinNum);
    }
    // �۳����
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
    // ���ӵ÷�
    //public void AddScore(int amount)
    //{
    //    score += amount;
    //    Debug.Log($"Added {amount} points. Total score: {score}");
    //}

    // ���ӵȼ�
    public void LevelUp()
    {
        level += 1;
        Debug.Log($"Leveled up! Current level: {level}");
    }

    // �۳�����ֵ
    public void TakeDamage(int damage)
    {
        health = (long)(Mathf.Max(health - damage, 0));
        Debug.Log($"Took {damage} damage. Remaining health: {health}");

        if (health <= 0)
        {
            Die();
        }
    }

    // �ָ�����ֵ
    public void Heal(int amount)
    {
        health += amount;
        Debug.Log($"Healed {amount} health. Current health: {health}");
    }

    // �������
    private void Die()
    {
        Debug.Log($"{playerName} has died.");
        // ������������Ϸ�����򸴻��߼�
    }

    // ����������ݣ��������л���JSON��
    public string SaveData()
    {
        return JsonUtility.ToJson(this);
    }

    // ����������ݣ������JSON�����л���
    public static PlayerInfo LoadData(string jsonData)
    {
        return JsonUtility.FromJson<PlayerInfo>(jsonData);
    }
}
