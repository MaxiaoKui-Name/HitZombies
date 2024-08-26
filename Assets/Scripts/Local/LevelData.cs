using UnityEngine;

[CreateAssetMenu(fileName = "NewLevelData", menuName = "Level Data", order = 51)]
public class LevelData : ScriptableObject
{
    public string[] backgroundAddress;// ����ͼƬ��Addressables��ַ
    public string[] enemyAddresses;// ����Ԥ�Ƽ���Addressables��ַ
    public string[] bulletAddresses;// ����Ԥ�Ƽ���Addressables��ַ
    public Vector3 enemySpawnPoints;
    public float enemySpawnInterval;// �������ɵ�ʱ����
    public float BulletInterval;
    public int[] enemyNum;
   
}