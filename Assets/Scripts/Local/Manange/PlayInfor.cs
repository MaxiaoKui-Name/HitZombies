using System;
using UnityEngine;

[Serializable]
public class Gun
{
    public string gunName;           // ǹе����
    public string bulletType;        // �ӵ�����
    public long bulletValue;        // �ӵ�����

    //public Gun(string name, string bullet, float rate)
    public Gun(string gun, string bullet, long value)
    {
        gunName = gun;
        bulletType = bullet;
        bulletValue = value;
    }
}
public class PlayerInfo : IComparable<PlayerInfo>
{
    public long coinNum;         // ��ҽ����
    public long level;           // ��ҵȼ�
    public long health;          // �������ֵ
    public string playerName;   // �������
    public int FrozenBuffCount;
    public int BalstBuffCount;
    public double attackFac;//�ӵ�����ϵ��
    public double attackSpFac;//

    //ǹе����
    public Gun currentGun;                // ��ǰѡ�е�ǹе

    // TTOD1���������ݴӷ������ù��캯��
    public void SetPlayerInfo(string name, long initialCoins, long initialLevel, long initialHealth)
    {
        playerName = name;
        coinNum = initialCoins;
        level = initialLevel;
        health = initialHealth;
        BalstBuffCount = 0;
        FrozenBuffCount = 0;
    }
    public void SetGun(Gun gun)
    {
        currentGun = gun;
    }

    // �ȽϽӿ�ʵ�֣���������������ݣ����簴�÷�����
    public int CompareTo(PlayerInfo other)
    {
        if (other == null) return 1;

        // �Ƚ���ҵ÷�
        return coinNum.CompareTo(other.coinNum);
    }

    // ��ӽ��
    public void AddCoins(int amount)
    {
        coinNum += amount;
        //Debug.Log($"Added {amount} coins. Total coins: {coinNum}");
    }

    // �۳����
    public bool SpendCoins(long amount)
    {
        if (coinNum >= amount)
        {
            coinNum -= amount;
            //Debug.Log($"{playerName} spent {amount} coins. Remaining coins: {coinNum}");
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
