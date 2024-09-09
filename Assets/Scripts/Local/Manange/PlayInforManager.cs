using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayInforManager : Singleton<PlayInforManager>
{
    public PlayerInfo playInfor;
    public void Init()
    {
        playInfor = new PlayerInfo();
        playInfor.SetPlayerInfo("Maxiaokui", 40000000000,
        ConfigManager.Instance.Tables.TableGlobalResConfig.Get(3).IntValue, 100);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
