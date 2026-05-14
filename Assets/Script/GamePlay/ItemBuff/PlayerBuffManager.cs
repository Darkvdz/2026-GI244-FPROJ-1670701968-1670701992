using UnityEngine;
using Photon.Pun;
using System.Collections;
public class PlayerBuffManager : MonoBehaviourPun
{
    private PlayerMovement2D playerMovement;

    public bool isInvincible = false;

    private PlayerColorManager colormanager;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement2D>();
       
        colormanager = GetComponent<PlayerColorManager>();
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

        StartCoroutine(HealColor());
        Debug.Log("Healed! HP: " + playerMovement.hp);
    }

    private IEnumerator Invincibility(float duration)
    {
        isInvincible = true;

        if (colormanager != null)
        {
            colormanager.ApplyColor(Color.goldenRod); 
        }

        yield return new WaitForSeconds(duration);

        isInvincible = false;

        if (colormanager != null)
        {
            colormanager.SetDefault();
        }
    }

    private IEnumerator HealColor()
    {
        if (colormanager != null)
        {
            colormanager.ApplyColor(new Color(0.5f, 1f, 0.5f));
        }

        yield return new WaitForSeconds(0.55f); 

        if (colormanager != null && !isInvincible)
        {
            colormanager.SetDefault();
        }
    }
}