using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBase : MonoBehaviour
{
    protected Dictionary<string, Transform> childDic = new Dictionary<string, Transform>();
    public bool isPlayWorldRankNew = false;
    public PlayerInfo playerInfo;
    protected void GetAllChild(Transform obj)
    {
        foreach (Transform item in obj)
        {
            if (item.name.Contains("_F"))
            {
                childDic.Add(item.name, item);
            }
            if (item.childCount > 0)
            {
                GetAllChild(item);
            }
        }
    }
}
