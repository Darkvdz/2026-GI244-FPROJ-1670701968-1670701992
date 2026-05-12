using UnityEngine;
using Photon.Pun;

public class BoomerangBullet : MonoBehaviourPun, IPunInstantiateMagicCallback
{
    public int damage = 15;
    public float speed = 15f;
    public float returnSpeed = 20f;
    public float flyTime = 0.5f;
    public float rotationSpeed = 360f; 
    public float curveSmoothness = 5f;

    public Transform visualsTransform;

    private bool isReturning = false;
    private float timer = 0f;

    private Transform ownerTransform;
    private PlayerMovement2D ownerScript;
    private int ownerViewID;

    private Vector3 currentVelocity;

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        object[] data = info.photonView.InstantiationData;
        if (data != null && data.Length > 0)
        {
            ownerViewID = (int)data[0];

            PhotonView pv = PhotonView.Find(ownerViewID);
            if (pv != null)
            {
                ownerTransform = pv.transform;
                ownerScript = pv.GetComponent<PlayerMovement2D>();
            }
        }

        currentVelocity = transform.right * speed;
    }

    void Update()
    {
        if (visualsTransform != null)
        {
            visualsTransform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
        }

        if (!photonView.IsMine) return;

        if (!isReturning)
        {
            transform.position += currentVelocity * Time.deltaTime;

            timer += Time.deltaTime;
            if (timer >= flyTime)
            {
                isReturning = true;
            }
        }
        else
        {
            
            if (ownerTransform != null)
            {
                
                Vector3 targetPos = ownerTransform.position; 

                if (ownerScript != null && ownerScript.currentWeapon != null && ownerScript.currentWeapon.firePoint != null)
                {
                    targetPos = ownerScript.currentWeapon.firePoint.position;
                }

                Vector3 targetDirection = (targetPos - transform.position).normalized;
                Vector3 desiredVelocity = targetDirection * returnSpeed;

                currentVelocity = Vector3.Lerp(currentVelocity, desiredVelocity, curveSmoothness * Time.deltaTime);
                transform.position += currentVelocity * Time.deltaTime;
            }
            else
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!photonView.IsMine) return;

        if (collision.CompareTag("Wall"))
        {
            isReturning = true;
            Debug.Log("Boomerang hit Wall");
            return;
        }

        if (collision.CompareTag("Player"))
        {
            PhotonView targetPV = collision.GetComponent<PhotonView>();
            if (targetPV == null) return;

            if (targetPV.ViewID != ownerViewID)
            {
                targetPV.RPC("TakeDamage", RpcTarget.All, damage);
                Debug.Log("Boomerang hit Enemy");
            }

            if (targetPV.ViewID == ownerViewID && isReturning)
            {
                if (ownerScript != null && ownerScript.currentWeapon is Boomerang)
                {
                    ((Boomerang)ownerScript.currentWeapon).ReturnBoomerang();
                }

                PhotonNetwork.Destroy(gameObject);
            }
        }
    }
}