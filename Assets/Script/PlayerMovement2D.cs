using Photon.Pun;
using System.IO;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement2D : MonoBehaviourPun, IPunObservable
{
    public float speed = 5f;
    public float jumpForce = 2f;
    public float hp = 100;
    private Vector3 networkPosition;

    public InputAction moveAction;
    public InputAction jumpAction;
    public InputAction attackAction;

    public bool hasItem = false;

    //public GameObject weaponPivot;  
    //public GameObject weaponSprite; 
    //public Transform firePoint;

    public Gun gun;      
   
    public Weapon currentWeapon;
    private float weaponAngle;
 

    private bool isDead = false;

    private Rigidbody2D rb;

    private void Awake()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
        attackAction = InputSystem.actions.FindAction("Attack");

        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        currentWeapon = null;
        hasItem = false;
        foreach (Weapon weapon in GetComponentsInChildren<Weapon>())
        {
            weapon.Deactivate();
        }

        print("self");
        print(PhotonNetwork.LocalPlayer.ActorNumber);
    }

    void Update()
    {

        if (!photonView.IsMine)
        {
            if (hasItem && currentWeapon != null)
            {
                transform.position = Vector3.Lerp(
                    transform.position,
                    networkPosition,
                    Time.deltaTime * 10f
                );

                currentWeapon.UpdateAim(weaponAngle);
            }
            return;
        }

        var horizontalInput = moveAction.ReadValue<Vector2>().x;
        transform.Translate(Vector2.right * speed * horizontalInput * Time.deltaTime);

        if (jumpAction.WasPressedThisFrame())
        {
            rb.AddForce(jumpForce * Vector2.up, ForceMode2D.Impulse);
        }

        if (attackAction.WasPressedThisFrame() && hasItem && currentWeapon != null)
        {
            currentWeapon.Use();
        }

        if (hasItem && currentWeapon != null)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            mousePos.z = 0f;
            Vector3 aimDirection = (mousePos - transform.position).normalized;
            weaponAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
            currentWeapon.UpdateAim(weaponAngle);
        }
    }
    public void PickupWeapon(WeaponType type)
    {
        photonView.RPC("RPC_PickupWeapon", RpcTarget.All, (int)type);
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

        if (hp <= 0)
        {
            Die(); 
        }

    }

    [PunRPC]
    void RPC_PickupWeapon(int typeInt)
    {
        WeaponType type = (WeaponType)typeInt;

        if (currentWeapon != null)
            currentWeapon.Deactivate();

        switch (type)
        {
            case WeaponType.Gun:
                currentWeapon = gun;
                break;
        }

        if (currentWeapon != null)
        {
            currentWeapon.Init(this);
            currentWeapon.Activate();
            hasItem = true;
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("Dead: " + PhotonNetwork.NickName);

        PlayerManager.instance.PlayerDied(); 

        photonView.RPC("DisablePlayer", RpcTarget.All);
    }

    [PunRPC]
    void DisablePlayer()
    {
        gameObject.SetActive(false);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Owner send

            stream.SendNext(hp);
            stream.SendNext(hasItem);
            stream.SendNext(weaponAngle);

            stream.SendNext(transform.position);

        }
        else
        {
            // oter recive

            hp = (float)stream.ReceiveNext();
            hasItem = (bool)stream.ReceiveNext();
            weaponAngle = (float)stream.ReceiveNext();

            networkPosition = (Vector3)stream.ReceiveNext();

            Debug.Log(
                "Local: " + PhotonNetwork.NickName +
                " | Owner: " + photonView.Owner.NickName +
                " | HP: " + hp +
                " | ItemHold: " + hasItem
            );
        }
    }
}