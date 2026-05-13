using UnityEngine;

public class OutOfBound : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) 
        {
            PlayerMovement2D playerScript = collision.gameObject.GetComponent<PlayerMovement2D>();

            if (playerScript)
            {
                playerScript.Die();
            }
            else 
            {
                print("error can't find player script from out of bound script");
            }

        }
    }

}
