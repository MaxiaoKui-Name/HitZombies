using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerDoorCollisionHandler : MonoBehaviour
{
    public SpecialBuffDoor specialBuffDoor;
    public bool isBuffDoor; // ��ʶ�� Buff �Ż��� Debuff ��

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player")) // ȷ����ҵ� Layer ������ȷ
        {
            specialBuffDoor.HandleDoorCollision(other.gameObject, isBuffDoor,1);
            specialBuffDoor.HandleDoorCollision(other.gameObject, true, 2);
        }
    }
}
