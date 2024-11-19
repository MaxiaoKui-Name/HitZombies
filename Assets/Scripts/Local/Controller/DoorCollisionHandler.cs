using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorCollisionHandler : MonoBehaviour
{
    public BuffDoorController buffDoorController;
    public bool isBuffDoor; // 标识是 Buff 门还是 Debuff 门

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player")) // 确保玩家的 Layer 设置正确
        {
            buffDoorController.HandleDoorCollision(other.gameObject, isBuffDoor);
        }
    }
}
