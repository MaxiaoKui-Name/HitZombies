using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PlayInforManager : Singleton<PlayInforManager>
{
    public PlayerInfo playInfor;
    public List<Sprite> AllstarGunImg = new List<Sprite>();// 存储各种星级武器图片
    public List<string> AllGunName = new List<string>();// 存储玩家的武器名字 索引值越小武器越牛逼，
                                                        // 添加一个字典来存储gunType到bulletType的映射
    public Dictionary<string, string> GunToBulletMap = new Dictionary<string, string>();
    public void Init()
    {
        playInfor = new PlayerInfo();
        playInfor.SetPlayerInfo("Maxiaokui", ConfigManager.Instance.Tables.TablePlayerConfig.Get(0).Blood);
        LoadWeaponImages();
        LoadGunToBulletMap();
        playInfor.hasCompletedGunSelectionTutorial = false; // 默认未完成
    }
    // 加载所有武器的图片
    private void LoadWeaponImages()
    {
        // 假设图片存放在Resources文件夹下的"Sprites/Weapons"路径中
        string resourcePath = "Sprites/Weapons/";

        // 遍历武器的星级和编号，加载Sprite
        for (int i = 1; i <= 4; i++) // 星级1到5
        {
            for (int j = 1; j <= 4; j++) // 每个星级下有5个武器
            {
                string weaponName = $"weapon{i}-{j}";  // 根据命名规则生成图片名称
                Sprite weaponSprite = Resources.Load<Sprite>(resourcePath + weaponName); // 加载对应的Sprite

                if (weaponSprite != null)
                {
                    AllstarGunImg.Add(weaponSprite); // 添加到列表中
                }
                else
                {
                    Debug.LogWarning($"未找到资源：{resourcePath + weaponName}");
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
