using Photon.Pun;
using UnityEngine;

public class OutOfBound : MonoBehaviourPunCallbacks
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) 
        {
            PlayerMovement2D playerScript = collision.gameObject.GetComponent<PlayerMovement2D>();
            PhotonView playerPV = collision.gameObject.GetComponent<PhotonView>();

            if (playerScript && playerPV.IsMine)
            {
                playerScript.Die();
            }
            else 
            {
                print("error can't find player script from out of bound script");
            }

        }
    }

}
