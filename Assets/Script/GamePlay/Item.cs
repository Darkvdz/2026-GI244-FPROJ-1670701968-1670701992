using Photon.Pun;
using UnityEngine;

public class Item : MonoBehaviourPun
{
    public Transform returnSpawn ;

    private bool isCollectedLocal = false;
    private bool isDestroyedOnServer = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isCollectedLocal) return;

        if (collision.gameObject.CompareTag("Player")) 
        {
            ItemManager.instance.ReturnSpawnPointItem(returnSpawn);

            PlayerMovement2D PL2DScript = collision.gameObject.GetComponent<PlayerMovement2D>();

            if (PL2DScript != null && PL2DScript.photonView.IsMine)
            {
                isCollectedLocal = true;

                Debug.Log("Collected by " + photonView.Owner.NickName);

                PL2DScript.hasItem = true;

                photonView.RPC("RequestDestroyItem", RpcTarget.MasterClient);

            }
            else 
            {
                Debug.Log("error Item not found Player script");
            }


            if (!PhotonNetwork.IsMasterClient) return;

            PhotonNetwork.Destroy(gameObject);
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
        PhotonNetwork.Destroy(gameObject);
    }

    /*private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            ItemManager.instance.ReturnSpawnPointItem(returnSpawn);

            PlayerMovement2D PL2DScript = collision.gameObject.GetComponent<PlayerMovement2D>();
            if (PL2DScript)
            {
                Debug.Log("Collected by " + photonView.Owner.NickName);
                PL2DScript.hasItem = true;

            }
            else
            {
                Debug.Log("error Item not found Player script");
            }

            PhotonNetwork.Destroy(gameObject);
        }
    }*/

}
