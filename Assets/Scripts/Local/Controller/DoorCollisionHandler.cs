using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorCollisionHandler : MonoBehaviour
{
    public BuffDoorController buffDoorController;
    public bool isBuffDoor; // ��ʶ�� Buff �Ż��� Debuff ��

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player")) // ȷ����ҵ� Layer ������ȷ
        {
            buffDoorController.HandleDoorCollision(other.gameObject, isBuffDoor);
        }
    }
}
