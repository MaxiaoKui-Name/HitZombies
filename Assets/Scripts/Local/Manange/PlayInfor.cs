using System;
using UnityEngine;

[Serializable]
public class PlayerInfo : IComparable<PlayerInfo>
{
    public long coinNum;         // 玩家金币数
    public long level;           // 玩家等级
    public long health;          // 玩家生命值
    public string playerName;   // 玩家姓名

    // 构造函数
    public void SetPlayerInfo(string name, long initialCoins, long initialLevel, long initialHealth)
    {
        playerName = name;
        coinNum = initialCoins;
        level = initialLevel;
        health = initialHealth;
    }

    // 比较接口实现，用于排序玩家数据（例如按得分排序）
    public int CompareTo(PlayerInfo other)
    {
        if (other == null) return 1;

        // 比较玩家得分
        return coinNum.CompareTo(other.coinNum);
    }

    // 添加金币
    public void AddCoins(int amount)
    {
        coinNum += amount;
        //Debug.Log($"Added {amount} coins. Total coins: {coinNum}");
    }

    // 扣除金币
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
