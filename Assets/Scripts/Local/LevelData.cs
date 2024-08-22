using UnityEngine;

[CreateAssetMenu(fileName = "NewLevelData", menuName = "Level Data", order = 51)]
public class LevelData : ScriptableObject
{
    public string[] backgroundAddress = new string[]{"ditu"};  // ����ͼƬ��Addressables��ַ
    public string[] enemyAddresses = new string[] { "Enemy1" };  // ����Ԥ�Ƽ���Addressables��ַ
    public Vector3 enemySpawnPoints = new Vector3(0, 5, 0);
    public float enemySpawnInterval = 1f;  // �������ɵ�ʱ����
}