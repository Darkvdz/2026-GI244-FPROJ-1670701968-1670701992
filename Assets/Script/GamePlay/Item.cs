using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Item : MonoBehaviourPun
{
    public Transform returnSpawn ;
    public WeaponType weaponType;

    public GameObject uiPickup;

    private bool isCollectedLocal = false;
    private bool isDestroyedOnServer = false;

    private bool isLocalPlayerNear = false;
    private PlayerMovement2D localPlayerScript;
    void Start()
    {
        if (uiPickup != null)
        {
            uiPickup.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isCollectedLocal) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerMovement2D PL2DScript = collision.gameObject.GetComponent<PlayerMovement2D>();

            if (PL2DScript != null && PL2DScript.photonView.IsMine)
            {
                isLocalPlayerNear = true;
                localPlayerScript = PL2DScript;

                if (uiPickup != null && !isCollectedLocal)
                {
                    uiPickup.SetActive(true);
                }
            }  
        }

    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerMovement2D PL2DScript = collision.gameObject.GetComponentInParent<PlayerMovement2D>();

            if (PL2DScript != null && PL2DScript.photonView.IsMine)
            {
                isLocalPlayerNear = false;
                localPlayerScript = null;

                if (uiPickup != null)
                {
                    uiPickup.SetActive(false);
                }

            }
        }
    }
    private void Update()
    {
        if (!isLocalPlayerNear || isCollectedLocal) return;

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            isCollectedLocal = true;
            if (uiPickup != null) uiPickup.SetActive(false);

            localPlayerScript.PickupWeapon(weaponType);
            Debug.Log("Collected by " + PhotonNetwork.NickName);
            photonView.RPC("RequestDestroyItem", RpcTarget.MasterClient);
        }
    }
    [PunRPC]
    void RequestDestroyItem()
    {
        if (isDestroyedOnServer) return;
        isDestroyedOnServer = true;

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
