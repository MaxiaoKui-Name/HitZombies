using UnityEngine;

[CreateAssetMenu(fileName = "NewLevelData", menuName = "Level Data", order = 51)]
public class LevelData : ScriptableObject
{
    public string[] backgroundAddress = new string[]{"ditu"};  // 背景图片的Addressables地址
    public string[] enemyAddresses = new string[] { "Enemy1" };  // 敌人预制件的Addressables地址
    public Vector3 enemySpawnPoints = new Vector3(0, 5, 0);
    public float enemySpawnInterval = 1f;  // 敌人生成的时间间隔
}