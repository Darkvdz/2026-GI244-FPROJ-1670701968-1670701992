using UnityEngine;
using Photon.Pun;

public class Gun : Weapon
{

    public override void Use()
    {
        if (firePoint == null) return;

        if (currentBullet > 0)
        {
            currentBullet--;
            PhotonNetwork.Instantiate("Bullet", firePoint.position, weaponPivot.transform.rotation);
        }
        else
        {
            Debug.Log("Out of Bullet");
        }
    }
}
