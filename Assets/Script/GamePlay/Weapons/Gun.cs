using UnityEngine;
using Photon.Pun;

public class Gun : Weapon
{

    public override void Use()
    {
        if (firePoint == null) return;
        PhotonNetwork.Instantiate("Bullet", firePoint.position, weaponPivot.transform.rotation);
    }
}
