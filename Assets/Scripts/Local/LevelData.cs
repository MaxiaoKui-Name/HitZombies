using UnityEngine;

[CreateAssetMenu(fileName = "NewLevelData", menuName = "Level Data", order = 51)]
public class LevelData : ScriptableObject
{
    public string[] backgroundAddress;// 背景图片的Addressables地址
    public string[] enemyAddresses;// 敌人预制件的Addressables地址
    public string[] bulletAddresses;// 敌人预制件的Addressables地址
    public Vector3 enemySpawnPoints;
    public float enemySpawnInterval;// 敌人生成的时间间隔
    public float BulletInterval;
    public int[] enemyNum;
   
}