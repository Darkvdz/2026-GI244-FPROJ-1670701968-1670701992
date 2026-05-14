using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealthBar : MonoBehaviour
{
    
    public GameObject healthBarCanvas; 
    public Slider hpSlider;            
    public Image fillImage;

    public float showTime = 2f;        

    private PlayerMovement2D player;
    private float currentShowTimer = 0f;
    private float maxHp = 100f;       

    private bool isBlinking = false;
    private Coroutine blinkCoroutine;

    private Color originalFillColor;

    private void Awake()
    {
        player = GetComponent<PlayerMovement2D>();
    }

    private void Start()
    {
        if (healthBarCanvas != null) healthBarCanvas.SetActive(false);
        if (player != null) maxHp = player.hp;

        originalFillColor = fillImage.color;
    }

    private void Update()
    {
        if (player == null || healthBarCanvas == null) 
            return;

        hpSlider.value = player.hp / maxHp;

        float hpPercent = (player.hp / maxHp) * 100f;

        if (player.hp <= 0)
        {
            healthBarCanvas.SetActive(false);
            return;
        }

        if (hpPercent <= 25f)
        {
            healthBarCanvas.SetActive(true); 

            if (!isBlinking)
            {
                blinkCoroutine = StartCoroutine(BlinkBlood());
            }
        }
        else
        {
            
            if (isBlinking)
            {
                StopCoroutine(blinkCoroutine);
                isBlinking = false;
                fillImage.color = originalFillColor; 
            }

            if (currentShowTimer > 0)
            {
                healthBarCanvas.SetActive(true);
                currentShowTimer -= Time.deltaTime;
            }
            else
            {
                healthBarCanvas.SetActive(false); 
            }
        }
    }

    public void TriggerShowHealthBar()
    {
        currentShowTimer = showTime;
    }

    private IEnumerator BlinkBlood()
    {
        isBlinking = true;
        while (true)
        {
            fillImage.color = Color.white; 
            yield return new WaitForSeconds(0.15f);

            fillImage.color = Color.red;
            yield return new WaitForSeconds(0.15f);
        }
    }
}