using UnityEngine;
using Photon.Pun;

public enum BuffType
{
    Heal,
    Invincibility
}

public class BuffItem : MonoBehaviourPun
{
    public BuffType buffType;
    public float effectAmount = 50f;
    public float duration = 0f;

    private bool isPickedUp = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isPickedUp) return;

        if (collision.CompareTag("Player"))
        {
            PhotonView playerView = collision.GetComponent<PhotonView>();

            if (playerView != null && playerView.IsMine)
            {
                playerView.RPC("RPC_ApplyBuff", RpcTarget.All, (int)buffType, effectAmount, duration);

                photonView.RPC("RPC_DestroyItem", RpcTarget.AllBuffered);
            }
        }
    }

    [PunRPC]
    void RPC_DestroyItem()
    {
        isPickedUp = true;
        gameObject.SetActive(false);

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}