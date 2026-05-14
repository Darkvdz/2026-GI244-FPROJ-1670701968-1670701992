using UnityEngine;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    public static SettingManager instance;

    public float musicVolume = 1f;
    public float sfxVolume = 1f;
    public bool isFullscreen = true;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        LoadSettings();
    }

    void Start()
    {
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);

        ApplySettings();
    }

    public void LoadSettings()
    {
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        isFullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;

        ApplySettings();
    }


    public void ApplySettings()
    {

        MusicManager.instance.musicSource.volume = musicVolume;
        SFXManager.instance.sfxSource.volume = sfxVolume;

        Screen.fullScreen = isFullscreen;
    }


}
