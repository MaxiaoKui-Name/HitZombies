using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewLevelData", menuName = "Level Data", order = 51)]
public class LevelData : ScriptableObject
{
    public List<string>backgroundAddress = new List<string>();// 背景图片的Addressables地址
   // public List<int> GunAddresses = new List<int>();// 枪预制件的Addressables地址
    public List<string> GunBulletList= new List<string>();//子弹预制体列表
    //public List<string>bulletAddresses= new List<string>();// 子弹预制件的Addressables地址
    public List<int> Monsterwaves = new List<int>(); // 记录怪物波次
    public Dictionary<int,List<List<int>>> WavesenEmiesDic = new Dictionary<int,List<List<int>>>();//将每一波对应五个怪物类型存入
    public int WavesEnemyNun;//本关卡所有的波次共产生的敌人
    public Vector3 enemySpawnPoints;
    public float enemySpawnInterval;// 敌人生成的时间间隔
    public float BulletInterval;
    public int[] enemyNum;
    public List<string> CoinList = new List<string>();
    public List<GameObject> ChestList = new List<GameObject>();
}