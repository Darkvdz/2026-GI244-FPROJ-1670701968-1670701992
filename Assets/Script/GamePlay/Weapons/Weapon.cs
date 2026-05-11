using UnityEngine;
using Photon.Pun;

public abstract class Weapon : MonoBehaviourPun
{
    public int damage = 10;
    public Transform firePoint;
    public GameObject weaponPivot;
    public GameObject weaponSprite;

    protected PlayerMovement2D owner;

    public void Init(PlayerMovement2D player)
    {
        owner = player;
    }

    public void Activate()
    {
        if (weaponSprite != null)
        {
            weaponSprite.SetActive(true);
        }      
    }

    public void Deactivate()
    {
        if (weaponSprite != null)
        {
            weaponSprite.SetActive(false);
        }
    }

    public abstract void Use();

    public virtual void UpdateAim(float angle)
    {
        if (weaponPivot != null)
        {
            weaponPivot.transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
}
