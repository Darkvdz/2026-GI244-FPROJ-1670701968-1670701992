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
            SFXManager.instance.playSound(gunSFX);
            PhotonNetwork.Instantiate("Bullet", firePoint.position, weaponPivot.transform.rotation);
        }
        else
        {
            SFXManager.instance.playSound(gunOutAmmo);
            Debug.Log("Out of Bullet");
        }
    }
}
