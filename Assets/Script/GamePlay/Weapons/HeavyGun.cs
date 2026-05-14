using UnityEngine;
using Photon.Pun;

public class HeavyGun : Weapon
{
    public string bulletPrefabName = "HeavyBullet"; 
    public float fireRate = 2.0f;

    private float nextFireTime = 0f;

    public AudioClip heavyGunSFX;
    public AudioClip heavyGunOutAmmo;


    public override void Use()
    {
        if (firePoint == null) return;

        if (Time.time >= nextFireTime)
        {
            if (currentBullet > 0)
            {
                currentBullet--; 
                nextFireTime = Time.time + fireRate;

                owner.photonView.RPC("RPC_PlayWeaponSound", RpcTarget.All, "Shoot");

                PhotonNetwork.Instantiate(bulletPrefabName, firePoint.position, weaponPivot.transform.rotation);
            }
            else
            {
                owner.photonView.RPC("RPC_PlayWeaponSound", RpcTarget.All, "Empty");
                Debug.Log("Heavy Gun Out of Bullet");
            }
        }
    }
}