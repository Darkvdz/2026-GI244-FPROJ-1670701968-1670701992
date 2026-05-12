using UnityEngine;
using Photon.Pun;

public class Boomerang : Weapon
{
   
    public string bulletName = "BoomerangPrefab"; 

    public override void Init(PlayerMovement2D player)
    {
        base.Init(player);
        currentBullet = 1;
    }

    public override void Use()
    {
        if (firePoint == null) return;

        if (currentBullet > 0)
        {
            currentBullet--; 

            object[] data = new object[] { owner.photonView.ViewID };

            PhotonNetwork.Instantiate(bulletName, firePoint.position, weaponPivot.transform.rotation, 0, data);
            Debug.Log("Throw Boomerang");
        }
        else
        {
            Debug.Log("Waiting for boomerang return");
        }
    }

    public void ReturnBoomerang()
    {
        currentBullet = 1; 
    }
}