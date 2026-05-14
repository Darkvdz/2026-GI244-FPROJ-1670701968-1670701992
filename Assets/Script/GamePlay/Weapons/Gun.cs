using UnityEngine;
using Photon.Pun;

public class Gun : Weapon
{
    public AudioClip gunSFX;
    public AudioClip gunOutAmmo;
    public override void Use()
    {
        if (firePoint == null) return;

        if (currentBullet > 0)
        {
            currentBullet--;

            owner.photonView.RPC("RPC_PlayWeaponSound", RpcTarget.All, "Shoot");

            PhotonNetwork.Instantiate("Bullet", firePoint.position, weaponPivot.transform.rotation);
        }
        else
        {
            owner.photonView.RPC("RPC_PlayWeaponSound", RpcTarget.All, "Empty");

            Debug.Log("Out of Bullet");
        }
    }
}
