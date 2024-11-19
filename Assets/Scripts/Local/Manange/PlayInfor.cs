using System;
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
    public float attackSpFac;         // �����ٶ�ϵ��
    public int ResueeCount;        // ����buff����

    // ǹе����
    public Gun currentGun;             // ��ǰѡ�е�ǹе

    // TTOD1���������ݴӷ������ù��캯��
    public void SetPlayerInfo(string name,long initialHealth)
    {
        playerName = name;
        health = initialHealth;
        attackSpFac = 0;
    }
    public void SetGun(Gun gun)
    {
        currentGun = gun;
        currentGun.gunName = gun.gunName;
        currentGun.bulletType = gun.bulletType;
    }
    public void SetPlayerAccount(string id, string creation, DateTime lastSignIn, int consecutive, long coinnum, int initiallevel,
        long exceperice,int balstBuffCount, int frozenBuffCount,string bulletName,string gunName)
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
    }

    // Method to add coins
    public void AddCoins(int amount)
    {
        coinNum += amount;
        //AudioManage.Instance.PlaySFX("Coin", null);
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
    public bool SpendCoins(long amount)
    {
        if (coinNum >= amount)
        {
            coinNum -= amount;
            //AudioManage.Instance.PlaySFX("Gun", null);
            EventDispatcher.instance.DispatchEvent(EventNameDef.UPDATECOIN);
            return true;
        }
        else
        {
            //Debug.Log($"{playerName} does not have enough coins to spend {amount}.");
            return false;
        }
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
