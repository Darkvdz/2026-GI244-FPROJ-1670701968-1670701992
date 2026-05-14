using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISetting : MonoBehaviour
{
    public Toggle fullscreenButton;

    public Slider musicSlider;
    public Slider sfxSlider;

    public TextMeshProUGUI musicPoint;
    public TextMeshProUGUI sfxPoint;

    void Start()
    {

        musicSlider.onValueChanged.AddListener(OnMusicChanged);

        sfxSlider.onValueChanged.AddListener(OnSFXChanged);

        print(fullscreenButton);
        print(SettingManager.instance.isFullscreen);
        fullscreenButton.isOn = SettingManager.instance.isFullscreen;

        fullscreenButton.onValueChanged.AddListener(SetFullscreen);

        musicSlider.value = SettingManager.instance.musicVolume;
        musicPoint.text = $"{musicSlider.value * 100:0}%";

        sfxSlider.value = SettingManager.instance.sfxVolume;
        sfxPoint.text = $"{sfxSlider.value * 100:0}%";
    }

    void OnMusicChanged(float value)
    {
        SettingManager.instance.musicVolume = value;
        musicPoint.text = $"{value * 100:0}%";

        SettingManager.instance.SaveSettings();
    }

    void OnSFXChanged(float value)
    {
        SettingManager.instance.sfxVolume = value;
        sfxPoint.text = $"{value * 100:0}%";

        SettingManager.instance.SaveSettings();
    }

    void SetFullscreen(bool isfull)
    {
        SettingManager.instance.isFullscreen = isfull;
        SettingManager.instance.ApplySettings();
        SettingManager.instance.SaveSettings();
    }

}
