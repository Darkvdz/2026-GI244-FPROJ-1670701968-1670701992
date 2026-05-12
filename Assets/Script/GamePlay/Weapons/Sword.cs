using UnityEngine;
using Photon.Pun;

public class Sword : Weapon
{
    public float attackRange = 1.5f;   

    public override void Use()
    {
        Debug.Log("ATK by Sword!");
        MeleeAttack();
    }

    void MeleeAttack()
    {

        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(firePoint.position, attackRange);

        foreach (Collider2D hit in hitObjects)
        {
            
            if (hit.CompareTag("Player"))
            {
                PhotonView targetPV = hit.GetComponent<PhotonView>();

                if (targetPV != null && !targetPV.IsMine)
                {
                    targetPV.RPC("TakeDamage", RpcTarget.All, damage);
                    Debug.Log("Sword Hit : " + hit.name);
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (firePoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(firePoint.position, attackRange);
    }
}