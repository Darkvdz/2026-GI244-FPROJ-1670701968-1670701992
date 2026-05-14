using UnityEngine;
using Photon.Pun;

public class Sword : Weapon
{
    public float attackRange = 1.5f;

    public float damageRateTime = 0.5f; 
    private float nextDamageTime = 0f;

    public AudioClip SlashSFX;

    public override void Use()
    {
        Debug.Log("ATK by Sword!");   
    }

    void Update()
    {
        if (owner != null && owner.photonView.IsMine)
        {
           
            if (Time.time >= nextDamageTime)
            {
                SFXManager.instance.playSound(SlashSFX);

                SwordAttack();
            }
        }
    }

    void SwordAttack()
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
                    Debug.Log("Sword Auto-Hit : " + hit.name);

                    hitSomeone = true; 
                }
            }
        }

        if (hitSomeone)
        {
            nextDamageTime = Time.time + damageRateTime;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (firePoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(firePoint.position, attackRange);
    }
}