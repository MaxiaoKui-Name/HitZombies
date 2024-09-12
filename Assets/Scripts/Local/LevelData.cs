using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewLevelData", menuName = "Level Data", order = 51)]
public class LevelData : ScriptableObject
{
    public List<string>backgroundAddress = new List<string>();// ����ͼƬ��Addressables��ַ
   // public List<int> GunAddresses = new List<int>();// ǹԤ�Ƽ���Addressables��ַ
    public List<string> GunBulletList= new List<string>();//�ӵ�Ԥ�����б�
    //public List<string>bulletAddresses= new List<string>();// �ӵ�Ԥ�Ƽ���Addressables��ַ
    public List<int> Monsterwaves = new List<int>(); // ��¼���ﲨ��
    public Dictionary<int,List<List<int>>> WavesenEmiesDic = new Dictionary<int,List<List<int>>>();//��ÿһ����Ӧ����������ʹ���
    public int WavesEnemyNun;//���ؿ����еĲ��ι������ĵ���
    public Vector3 enemySpawnPoints;
    public float enemySpawnInterval;// �������ɵ�ʱ����
    public float BulletInterval;
    public int[] enemyNum;
    public List<string> CoinList = new List<string>();
    public List<GameObject> ChestList = new List<GameObject>();
}