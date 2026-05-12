using UnityEngine;
using Photon.Pun;

public class Axe : Weapon
{
    public float attackRange = 1.5f;

    public float damageRateTime = 1.5f;
    private float nextDamageTime = 0f;

    public float swingSpeed = 3f; 

    public override void Use()
    {
        Debug.Log("ATK by Axe!");
    }

    void Update()
    {
        if (owner != null && owner.photonView.IsMine)
        {
            if (Time.time >= nextDamageTime)
            {
                AxeAttack();
            }
        }
    }

    void AxeAttack()
    {
        bool hitSomeone = false;
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(firePoint.position, attackRange);

        foreach (Collider2D hit in hitObjects)
        {
            if (hit.CompareTag("Player"))
            {
                PhotonView targetPV = hit.GetComponent<PhotonView>();

                if (targetPV != null && !targetPV.IsMine)
                {
                    targetPV.RPC("TakeDamage", RpcTarget.All, damage);
                    
                    hitSomeone = true;
                }
            }
        }

        if (hitSomeone)
        {
            nextDamageTime = Time.time + damageRateTime;
        }
    }

    public override void UpdateAim(float angle)
    {
        if (weaponPivot != null)
        {
            Quaternion targetRotation = Quaternion.Euler(0, 0, angle);

            weaponPivot.transform.rotation = Quaternion.Lerp(weaponPivot.transform.rotation, targetRotation, Time.deltaTime * swingSpeed);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (firePoint == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(firePoint.position, attackRange);
    }
}