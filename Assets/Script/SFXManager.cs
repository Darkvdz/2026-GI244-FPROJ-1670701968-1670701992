using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager instance;

    public AudioSource sfxSource;

    [SerializeField] private AudioClip shootSFX;
    [SerializeField] private AudioClip slashSFX;
    [SerializeField] private AudioClip outOfAmmoSFX;
    [SerializeField] private AudioClip collectSFX;
    [SerializeField] private AudioClip buffSFX;


    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        sfxSource = GetComponent<AudioSource>();
    }

    public void playSound(AudioClip clip) 
    {
        sfxSource.PlayOneShot(clip);
    }


}
