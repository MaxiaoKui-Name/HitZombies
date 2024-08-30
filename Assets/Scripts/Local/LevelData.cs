using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewLevelData", menuName = "Level Data", order = 51)]
public class LevelData : ScriptableObject
{
    public List<string>backgroundAddress = new List<string>();// 背景图片的Addressables地址
    public List<int> GunAddresses = new List<int>();// 枪预制件的Addressables地址
    public Dictionary<string, List<int>> GunBulletDic= new Dictionary<string, List<int>>();//将枪的id与对应美术资源存入
    //public List<string>bulletAddresses= new List<string>();// 子弹预制件的Addressables地址
    public List<int> Monsterwaves = new List<int>(); // 记录怪物波次所含怪物id
    public Dictionary<int,List<int>> WavesenEmiesDic = new Dictionary<int,List<int>>();//将波数与五个怪物类型存入
    public Vector3 enemySpawnPoints;
    public float enemySpawnInterval;// 敌人生成的时间间隔
    public float BulletInterval;
    public int[] enemyNum;
}