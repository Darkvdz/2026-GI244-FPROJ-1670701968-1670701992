using Photon.Pun;
using UnityEngine;

public class PlayerColorManager : MonoBehaviourPun
{
    private SpriteRenderer sr;
    private Color originalColor;

    private Color[] playerColors = new Color[]
    {
        Color.darkBlue,           
        Color.yellow,           
        Color.green,            
        Color.red              
    };

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        SetJoinOrderColor();
    }

    void SetJoinOrderColor()
    {
        if (sr == null) 
            return;

        int index = (int)photonView.Owner.CustomProperties["slot"];
       // print(index);
        originalColor = playerColors[index];

        sr.color = originalColor;
    }

    public void ApplyColor(Color effectColor)
    {
        if (sr != null)
        {
            sr.color = effectColor;
        }
     
    }

    public void SetDefault()
    {
       
        if (sr != null)
        {
            sr.color = originalColor;
        }
          
    }
}
