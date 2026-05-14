using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    public AudioSource musicSource;

    [SerializeField] private AudioClip mainmenuBG;
    [SerializeField] private AudioClip gameBG;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        musicSource = GetComponent<AudioSource>();
    }

    public void RunMainScene() 
    {
        if (musicSource.clip == mainmenuBG) return;

        musicSource.clip = mainmenuBG;
        musicSource.Play();
    }

    public void RunGameScene()
    {
        if (musicSource.clip == gameBG) return;

        musicSource.clip = gameBG;
        musicSource.Play();
    }

}
