using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerDoorCollisionHandler : MonoBehaviour
{
    public SpecialBuffDoor specialBuffDoor;
    public bool isBuffDoor; // 标识是 Buff 门还是 Debuff 门

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player")) // 确保玩家的 Layer 设置正确
        {
            specialBuffDoor.HandleDoorCollision(other.gameObject, isBuffDoor,1);
            specialBuffDoor.HandleDoorCollision(other.gameObject, true, 2);
        }
    }
}
