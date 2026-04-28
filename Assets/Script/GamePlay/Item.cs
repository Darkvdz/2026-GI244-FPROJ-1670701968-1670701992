using UnityEngine;

public class Item : MonoBehaviour
{
    public Transform returnSpawn ;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) 
        {
            ItemManager.instance.ReturnSpawnPointItem(returnSpawn);

            PlayerMovement2D PL2DScript = collision.gameObject.GetComponent<PlayerMovement2D>();
            if (PL2DScript)
            {
                PL2DScript.hasItem = true;
            }
            else 
            {
                Debug.Log("error Item not found Player script");
            }

            Destroy(gameObject);
        }
    }
}
