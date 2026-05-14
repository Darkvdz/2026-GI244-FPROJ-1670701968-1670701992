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

    [Header("Weapons")]
    public Gun gun;
    public Sword sword;
    public HeavyGun heavyGun;
    public Axe axe;
    public Boomerang boomerang;

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
        if (CameraControll.instance != null)
        {
            CameraControll.instance.AddPlayer(transform);
        }

        currentWeapon = null;
        hasItem = false;
        foreach (Weapon weapon in GetComponentsInChildren<Weapon>())
        {
            weapon.Deactivate();
        }

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
               
                //FlipWeaponMTP
                bool isAimingLeft = Mathf.Abs(weaponAngle) > 90f;
                if (currentWeapon.weaponPivot != null)
                {
                    float flipY = isAimingLeft ? -1f : 1f;
                    currentWeapon.weaponPivot.transform.localScale = new Vector3(1, flipY, 1);
                }
            }

            return;
        }

        //Walk
        var horizontalInput = moveAction.ReadValue<Vector2>().x;
        transform.Translate(Vector2.right * speed * horizontalInput * Time.deltaTime);

        //Jump
        if (jumpAction.WasPressedThisFrame())
        {
            rb.AddForce(jumpForce * Vector2.up, ForceMode2D.Impulse);
        }

        //Attack 
        if (attackAction.WasPressedThisFrame() && hasItem && currentWeapon != null)
        {
            currentWeapon.Use();
        }

        // Rotateweapon
        if (hasItem && currentWeapon != null)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            mousePos.z = 0f;
            Vector3 aimDirection = (mousePos - transform.position).normalized;
            weaponAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;

            currentWeapon.UpdateAim(weaponAngle);

            // FlipWeapons
            bool isAimingLeft = Mathf.Abs(weaponAngle) > 90f;
            if (currentWeapon.weaponPivot != null)
            {
                float flipY = isAimingLeft ? -1f : 1f;
                currentWeapon.weaponPivot.transform.localScale = new Vector3(1, flipY, 1);
            }
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

        /*Debug.Log(
            "Local: " + PhotonNetwork.NickName +
            " | Owner: " + photonView.Owner.NickName +
            " | HP: " + hp
        );*/

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
            case WeaponType.Sword: 
                currentWeapon = sword;
                break;
            case WeaponType.HeavyGun: 
                currentWeapon = heavyGun;
                break;
            case WeaponType.Axe:
                currentWeapon = axe;
                break;
            case WeaponType.Boomerang: 
                currentWeapon = boomerang; 
                break;
        }

        if (currentWeapon != null)
        {
            currentWeapon.Init(this);
            currentWeapon.Activate();
            hasItem = true;
        }
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        //Debug.Log("Dead: " + PhotonNetwork.NickName);

        PlayerManager.instance.PlayerDied(); 

        photonView.RPC("DisablePlayer", RpcTarget.All);
    }

    [PunRPC]
    void DisablePlayer()
    {
        if (CameraControll.instance != null)
        {
            CameraControll.instance.RemovePlayer(transform);
        }

        UIManager.Instance.CallUpdateUI();
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

            /*Debug.Log(
                "Local: " + PhotonNetwork.NickName +
                " | Owner: " + photonView.Owner.NickName +
                " | HP: " + hp +
                " | ItemHold: " + hasItem
            );*/
        }
    }


    private void OnDestroy()
    {
        if (CameraControll.instance != null)
        {
            CameraControll.instance.RemovePlayer(transform);
        }
    }

}