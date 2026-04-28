using UnityEngine;
using Photon.Pun;

public class Bullet : MonoBehaviourPun
{
    public float speed = 15f;
    public int damage = 10;
    public float destroyTime = 3f;

    private bool isDestroyed = false;

    void Start()
    {
        if (photonView.IsMine)
        {
            Invoke("DestroyBullet", destroyTime);
        }
    }

    void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
       
        if (!photonView.IsMine) return;
        if (isDestroyed) return;

        if (collision.CompareTag("Player"))
        {
            PhotonView targetPhotonView = collision.GetComponent<PhotonView>();

            if (targetPhotonView != null && !targetPhotonView.IsMine)
            {
               
                targetPhotonView.RPC("TakeDamage", RpcTarget.All, damage);
                DestroyBullet();
            }
        }
       
        else if (collision.CompareTag("Wall"))
        {
            DestroyBullet();
        }
    }

    void DestroyBullet()
    {
        if (isDestroyed) return;
        isDestroyed = true;

        PhotonNetwork.Destroy(gameObject);
    }
}