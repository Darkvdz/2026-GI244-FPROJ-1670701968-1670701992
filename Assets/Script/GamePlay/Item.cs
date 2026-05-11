using Photon.Pun;
using UnityEngine;

public class Item : MonoBehaviourPun
{
    public Transform returnSpawn ;
    public WeaponType weaponType;

    private bool isCollectedLocal = false;
    private bool isDestroyedOnServer = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isCollectedLocal) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerMovement2D PL2DScript = collision.gameObject.GetComponent<PlayerMovement2D>();

            if (PL2DScript != null)
            {
                isCollectedLocal = true;

                PL2DScript.PickupWeapon(weaponType);

                Debug.Log("Collected by " + collision.gameObject.name);
                photonView.RPC("RequestDestroyItem", RpcTarget.MasterClient);
            }
            else
            {
                Debug.Log("error: Item not found PlayerMovement2D script");
            }
        }

    }

    [PunRPC]
    void RequestDestroyItem()
    {
        if (isDestroyedOnServer) return;
        isDestroyedOnServer = true;

        if (ItemManager.instance != null)
        {
            ItemManager.instance.ReturnSpawnPointItem(returnSpawn);
        }

        //Hide Before Dts
        foreach (SpriteRenderer sr in GetComponentsInChildren<SpriteRenderer>())
        {
            sr.enabled = false;
        }
        foreach (Collider2D col in GetComponentsInChildren<Collider2D>())
        {
            col.enabled = false;
        }
            
        PhotonNetwork.Destroy(gameObject);
    }


}
