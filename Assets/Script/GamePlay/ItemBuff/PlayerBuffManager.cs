using UnityEngine;
using Photon.Pun;
using System.Collections;
public class PlayerBuffManager : MonoBehaviourPun
{
    private PlayerMovement2D playerMovement;
    private SpriteRenderer sr;

    public bool isInvincible = false;

    Color OriColor;
    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement2D>();
        sr = GetComponent<SpriteRenderer>();
        OriColor = sr.color;
    }

    [PunRPC]
    public void RPC_ApplyBuff(int buffTypeIndex, float amount, float duration)
    {
        BuffType type = (BuffType)buffTypeIndex;

        switch (type)
        {
            case BuffType.Heal:
                Heal(amount);
                break;

            case BuffType.Invincibility:
                StartCoroutine(Invincibility(duration));
                break;
        }
    }

    private void Heal(float amount)
    {
        if (!photonView.IsMine) return;

        playerMovement.hp += amount;
        if (playerMovement.hp > 100) playerMovement.hp = 100;
        Debug.Log("Healed! HP: " + playerMovement.hp);
    }

    private IEnumerator Invincibility(float duration)
    {
        isInvincible = true;
        if (sr != null)
        {
            sr.color = Color.yellow;
        }

        yield return new WaitForSeconds(duration);

        isInvincible = false;
        if (sr != null)
        {
            sr.color = OriColor;
        }
        
    }
}