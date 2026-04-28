using Photon.Pun;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement2D : MonoBehaviourPun, IPunObservable
{
    public float speed = 5f;
    public float jumpForce = 2f;
    public float hp = 100;

    public InputAction moveAction;
    public InputAction jumpAction;
    public InputAction attackAction;

    public bool hasItem = false;

    public GameObject weaponPivot;  
    public GameObject weaponSprite; 
    private float weaponAngle;      

    private Rigidbody2D rb;

    private void Awake()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
        attackAction = InputSystem.actions.FindAction("Attack");

        rb = GetComponent<Rigidbody2D>();

    }
    void Update()
    {
        if (weaponSprite != null)
        {
            weaponSprite.SetActive(hasItem);
        }

        if (!photonView.IsMine)
        {
            if (hasItem && weaponPivot != null)
            {
                weaponPivot.transform.rotation = Quaternion.Euler(0, 0, weaponAngle);
            }
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

        if (hasItem && weaponPivot != null)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            mousePos.z = 0f;
            Vector3 aimDirection = (mousePos - transform.position).normalized;
            weaponAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
            weaponPivot.transform.rotation = Quaternion.Euler(0, 0, weaponAngle);
        }

    }


    [PunRPC]
    void TakeDamage(int damage)
    {
        if (!photonView.IsMine) return; 

        hp -= damage;

        Debug.Log(
            "Local: " + PhotonNetwork.NickName +
            " | Owner: " + photonView.Owner.NickName +
            " | HP: " + hp
        );


    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Owner send

            //Debug.Log("SEND HP from " + PhotonNetwork.NickName + ": " + hp);
            stream.SendNext(hp);
            stream.SendNext(hasItem);
            stream.SendNext(weaponAngle);
        }
        else
        {
            // oter recive

            hp = (float)stream.ReceiveNext();
            hasItem = (bool)stream.ReceiveNext();
            weaponAngle = (float)stream.ReceiveNext();

            Debug.Log(
                "Local: " + PhotonNetwork.NickName +
                " | Owner: " + photonView.Owner.NickName +
                " | HP: " + hp +
                " | ItemHold: " + hasItem
            );
        }
    }
}