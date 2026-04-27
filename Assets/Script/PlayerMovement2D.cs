using Photon.Pun;
using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerMovement2D : MonoBehaviourPun
{
    public float speed = 5f;
    public InputAction moveAction;

    private void Awake()
    {
        moveAction = InputSystem.actions.FindAction("Move");
    }
    void Update()
    {
        
        // เช็คว่าตัวละครนี้เป็นของเรารึป่าว
        if (!photonView.IsMine)
        {
            return;
        }

        var moveInput = moveAction.ReadValue<Vector2>();
        transform.Translate(new Vector3(moveInput.x, moveInput.y, 0) * speed * Time.deltaTime);
    }
}