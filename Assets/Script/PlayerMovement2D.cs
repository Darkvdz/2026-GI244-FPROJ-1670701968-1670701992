using Photon.Pun;
using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerMovement2D : MonoBehaviourPun, IPunObservable
{
    public float speed = 5f;
    public float jumpForce = 2f;
    public float hp = 100;

    public InputAction moveAction;
    public InputAction jumpAction;
    public InputAction attackAction;

    private Rigidbody2D rb;

    private void Awake()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");

        rb = GetComponent<Rigidbody2D>();

    }
    void Update()
    {

        if (!photonView.IsMine)
        {
            return;
        }


        if (jumpAction.WasPressedThisFrame())
        {
            rb.AddForce(jumpForce * Vector2.up, ForceMode2D.Impulse);
        }

        if (attackAction.WasPressedThisFrame())
        {
            photonView.RPC("TakeDamage", RpcTarget.All, 10);
        }

        var holizontalInput = moveAction.ReadValue<Vector2>().x;
        transform.Translate(Vector2.right * speed * holizontalInput * Time.deltaTime);

    }


    [PunRPC]
    void TakeDamage(int damage)
    {
        Debug.Log("RPC called on: " + PhotonNetwork.NickName);
        if (!photonView.IsMine) return; 

        hp -= damage;

        Debug.Log("Player: " + PhotonNetwork.NickName + " HP: " + hp);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Owner send

            Debug.Log("SEND HP from " + PhotonNetwork.NickName + ": " + hp);
            stream.SendNext(hp);
        }
        else
        {
            // oter recive

            hp = (int)stream.ReceiveNext();
            Debug.Log("RECEIVE HP on " + PhotonNetwork.NickName + ": " + hp);
        }
    }
}