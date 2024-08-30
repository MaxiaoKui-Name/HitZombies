using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewLevelData", menuName = "Level Data", order = 51)]
public class LevelData : ScriptableObject
{
    public List<string>backgroundAddress = new List<string>();// ����ͼƬ��Addressables��ַ
    public List<int> GunAddresses = new List<int>();// ǹԤ�Ƽ���Addressables��ַ
    public Dictionary<string, List<int>> GunBulletDic= new Dictionary<string, List<int>>();//��ǹ��id���Ӧ������Դ����
    //public List<string>bulletAddresses= new List<string>();// �ӵ�Ԥ�Ƽ���Addressables��ַ
    public List<int> Monsterwaves = new List<int>(); // ��¼���ﲨ����������id
    public Dictionary<int,List<int>> WavesenEmiesDic = new Dictionary<int,List<int>>();//������������������ʹ���
    public Vector3 enemySpawnPoints;
    public float enemySpawnInterval;// �������ɵ�ʱ����
    public float BulletInterval;
    public int[] enemyNum;
}