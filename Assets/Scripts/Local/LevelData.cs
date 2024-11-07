using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewLevelData", menuName = "Level Data", order = 51)]
public class LevelData : ScriptableObject
{
    public List<string>backgroundAddress = new List<string>();// ����ͼƬ��Addressables��ַ
    public List<GameObject> ChestUIList = new List<GameObject>();// ǹԤ�Ƽ���Addressables��ַ
    public List<Gun> GunBulletList= new List<Gun>();//�ӵ�Ԥ�����б�
    //public List<string>bulletAddresses= new List<string>();// �ӵ�Ԥ�Ƽ���Addressables��ַ
    public List<int> Monsterwaves = new List<int>(); // ��¼���ﲨ��
    public Dictionary<int, List<int>> WaveEnemyCountDic = new Dictionary<int, List<int>>(); // ��¼���ﲨ��
    public Dictionary<int,List<List<int>>> WavesenEmiesDic = new Dictionary<int,List<List<int>>>();//��ÿһ����Ӧ����������ʹ���
    public int WavesEnemyNun;//���ؿ����еĲ��ι������ĵ���
    public Vector3 enemySpawnPoints;
    public Vector3 ChestPoints;
    public float enemySpawnInterval;// �������ɵ�ʱ����
    public float BulletInterval;
    public List<int> WaveEnemyAllNumList = new List<int>();
    public int LevelIndex;
    public List<string> CoinList = new List<string>();//�����������
    public List<GameObject> ChestList = new List<GameObject>();//����Ԥ����List
    public GameObject PowbuffDoor;
    public GameObject buffDoor;
    public int resureNum;
}