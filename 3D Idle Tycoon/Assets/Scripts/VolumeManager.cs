using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider musicVolumeSlider;

    private const string exposedVolumeName = "music";

    public float CurrentVolume => musicVolumeSlider.value;

    private void Start()
    {
        SetMusicVolume(musicVolumeSlider.value);
    }

    public void SetMusicVolume()
    {
        SetMusicVolume(musicVolumeSlider.value);
    }

    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat(exposedVolumeName, Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20);
        musicVolumeSlider.value = volume;
    }
}
