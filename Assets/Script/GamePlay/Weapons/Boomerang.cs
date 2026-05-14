using UnityEngine;
using Photon.Pun;

public class Boomerang : Weapon
{
    public string bulletName = "BoomerangPrefab"; 

    public AudioClip bmrSFX;
    public AudioSource bmrAudioSource;
    public override void Init(PlayerMovement2D player)
    {
        base.Init(player);
        currentBullet = 1;

        bmrAudioSource = GetComponent<AudioSource>();
    }

    public override void Use()
    {
        if (firePoint == null) return;

        if (currentBullet > 0)
        {
            currentBullet--;

            if (bmrSFX != null)
            {
                bmrAudioSource.clip = bmrSFX; // ใส่แผ่นเสียง
                bmrAudioSource.Play();        // กดปุ่ม Play
            }

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

        if (bmrAudioSource != null && bmrAudioSource.isPlaying)
        {
            bmrAudioSource.Stop();
        }
    }
}