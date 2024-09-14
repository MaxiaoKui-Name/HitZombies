using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayInforManager : Singleton<PlayInforManager>
{
    public PlayerInfo playInfor;
    public void Init()
    {
        playInfor = new PlayerInfo();
        playInfor.SetPlayerInfo("Maxiaokui", 400000000,
        ConfigManager.Instance.Tables.TableGlobal.Get(4).IntValue, ConfigManager.Instance.Tables.TablePlayer.Get(0).Blood);
    }
    //ConfigManager.Instance.Tables.TableGlobal.Get(1).IntValue
    // Update is called once per frame
    void Update()
    {
        
    }
}
