using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PlayInforManager : Singleton<PlayInforManager>
{
    public PlayerInfo playInfor;
    public List<Sprite> AllstarGunImg = new List<Sprite>();// �洢�����Ǽ�����ͼƬ
    public List<string> AllGunName = new List<string>();// �洢��ҵ��������� ����ֵԽС����Խţ�ƣ�
                                                        // ���һ���ֵ����洢gunType��bulletType��ӳ��
    public Dictionary<string, string> GunToBulletMap = new Dictionary<string, string>();
    public void Init()
    {
        playInfor = new PlayerInfo();
        playInfor.SetPlayerInfo("Maxiaokui", ConfigManager.Instance.Tables.TablePlayerConfig.Get(0).Blood);
        LoadWeaponImages();
        LoadGunToBulletMap();
        playInfor.hasCompletedGunSelectionTutorial = false; // Ĭ��δ���
    }
    // ��������������ͼƬ
    private void LoadWeaponImages()
    {
        // ����ͼƬ�����Resources�ļ����µ�"Sprites/Weapons"·����
        string resourcePath = "Sprites/Weapons/";

        // �����������Ǽ��ͱ�ţ�����Sprite
        for (int i = 1; i <= 4; i++) // �Ǽ�1��5
        {
            for (int j = 1; j <= 4; j++) // ÿ���Ǽ�����5������
            {
                string weaponName = $"weapon{i}-{j}";  // ����������������ͼƬ����
                Sprite weaponSprite = Resources.Load<Sprite>(resourcePath + weaponName); // ���ض�Ӧ��Sprite

                if (weaponSprite != null)
                {
                    AllstarGunImg.Add(weaponSprite); // ��ӵ��б���
                }
                else
                {
                    Debug.LogWarning($"δ�ҵ���Դ��{resourcePath + weaponName}");
                }
            }
        }
    }
    private void LoadGunToBulletMap()
    {
        var playerConfig = ConfigManager.Instance.Tables.TablePlayerConfig;
        for (int i = 0;i < 311; i ++)
        {
            string gunKey = $"{playerConfig.Get(i).Animation}-{playerConfig.Get(i).StartNum}";
            if (!GunToBulletMap.ContainsKey(gunKey))
            {
                GunToBulletMap[gunKey] = ConfigManager.Instance.Tables.TableTransmitConfig.Get(playerConfig.Get(i).Fires[0]).Resource;
            }
        }
    }
    void Update()
    {
        
    }
}
