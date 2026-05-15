using Photon.Pun;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement2D : MonoBehaviourPun, IPunObservable
{
    public GameObject pointer;
    public GameObject crown;

    public float speed = 5f;
    public float jumpForce = 4f;
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

    public int maxJumps = 2;      
    private int jumpCount = 0;

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

        if (photonView.IsMine) 
        {
            pointer.SetActive(true);
        }

        int score = 0;

        if (photonView.Owner.CustomProperties.ContainsKey("score"))
        {
            score = (int)photonView.Owner.CustomProperties["score"];
        }

        if (score >= 3)
        {
            if (crown != null)
            {
                crown.SetActive(true);
            }
        }


        //print(PhotonNetwork.LocalPlayer.ActorNumber);
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
        if (jumpAction.WasPressedThisFrame() && jumpCount < maxJumps)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(jumpForce * Vector2.up, ForceMode2D.Impulse);
            jumpCount++;
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
        PlayerHealthBar healthBar = GetComponent<PlayerHealthBar>();
        if (healthBar != null)
        {
            healthBar.TriggerShowHealthBar();
        }

        if (!photonView.IsMine) return;

        PlayerBuffManager buffManager = GetComponent<PlayerBuffManager>();

        if (buffManager != null && buffManager.isInvincible)
        {
            Debug.Log("Player is Invincible");
            return;
        }

        hp -= damage;

        /*Debug.Log(
            "Local: " + PhotonNetwork.NickName +
            " | Owner: " + photonView.Owner.NickName +
            " | HP: " + hp
        );*/

        if (hp <= 0)
        {
            print("dead");
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!photonView.IsMine) return;

        if (collision.gameObject.CompareTag("Wall"))
        {
            if (collision.contacts.Length > 0 && collision.contacts[0].normal.y > 0.5f)
            {
                jumpCount = 0; 
            }
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

        if (UIManager.Instance != null)
        {
            UIManager.Instance.CallUpdateUI();
        }
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

    [PunRPC]
    public void RPC_PlayWeaponSound(string action)
    {
        if (currentWeapon == null) return;

        if (currentWeapon == gun)
        {
            if (action == "Shoot") SFXManager.instance.playSound(gun.gunSFX, 0.3f);
            else if (action == "Empty") SFXManager.instance.playSound(gun.gunOutAmmo);
        }
        else if (currentWeapon == heavyGun)
        {
            if (action == "Shoot") SFXManager.instance.playSound(heavyGun.heavyGunSFX);
            else if (action == "Empty") SFXManager.instance.playSound(heavyGun.heavyGunOutAmmo);
        }
        else if (currentWeapon == sword)
        {
            if (action == "Swing") SFXManager.instance.playSound(sword.SlashSFX);
        }
        else if (currentWeapon == axe)
        {
            if (action == "Swing") SFXManager.instance.playSound(axe.axeSFX, 0.4f);
        }
        else if (currentWeapon == boomerang)
        {
            if (action == "Throw" && boomerang.bmrSFX != null)
            {
                boomerang.bmrAudioSource.clip = boomerang.bmrSFX;
                boomerang.bmrAudioSource.Play();
            }
            else if (action == "StopThrow" && boomerang.bmrAudioSource != null)
            {
                boomerang.bmrAudioSource.Stop();
            }
        }
    }
}