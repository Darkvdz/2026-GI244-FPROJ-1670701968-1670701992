using Photon.Pun;
using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerMovement2D : MonoBehaviourPun
{
    public float speed = 5f;
    public float jumpForce = 2f;
    public float hp = 100;

    public InputAction moveAction;
    public InputAction jumpAction;

    private Rigidbody2D rb;

    private void Awake()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");

        rb = GetComponent<Rigidbody2D>();

    }
    void Update()
    {
        
        // เช็คว่าตัวละครนี้เป็นของเรารึป่าว
        if (!photonView.IsMine)
        {
            return;
        }


        if (jumpAction.WasPressedThisFrame()) 
        {
            rb.AddForce(jumpForce * Vector2.up, ForceMode2D.Impulse);
        }

        var holizontalInput = moveAction.ReadValue<Vector2>().x;
        transform.Translate(Vector2.right * speed * holizontalInput * Time.deltaTime);

    }

    [PunRPC]
    void TakeDamage(int damage)
    {
        if (!photonView.IsMine) return; // 🔥 กันคนอื่นแก้

        hp -= damage;
    }

}