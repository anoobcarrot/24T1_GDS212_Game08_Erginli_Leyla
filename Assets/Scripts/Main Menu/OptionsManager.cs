using UnityEngine;
using UnityEngine.UI;
using StarterAssets;

public class OptionsManager : MonoBehaviour
{
    public Slider volumeSlider;
    public Slider sensitivitySlider;
    public FirstPersonController firstPersonController; // Reference to the FirstPersonController script

    public AudioSource[] allAudioSources; // Assign the audio sources in the inspector

    private void Start()
    {

        // Set the volume slider value
        float savedVolume = PlayerPrefs.GetFloat("Volume", 1f);
        volumeSlider.value = savedVolume;

        // Adjust the volume based on the loaded settings
        AdjustVolume();

        // Set the sensitivity slider value
        float savedSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 2f);
        sensitivitySlider.value = savedSensitivity;

        // Adjust volume and mouse sensitivity based on loaded settings
        AdjustMouseSensitivity();
    }

    public void AdjustVolume()
    {
        float volume = volumeSlider.value;
        Debug.Log("Volume Adjusted: " + volume);

        // Adjust volume for all assigned audio sources
        foreach (var audioSource in allAudioSources)
        {
            audioSource.volume = volume;
        }

        // Save volume
        PlayerPrefs.SetFloat("Volume", volume);
    }

    public void AdjustMouseSensitivity()
    {
        float sensitivity = sensitivitySlider.value;
        int roundedSensitivity = Mathf.RoundToInt(sensitivity); // Round sensitivity to the nearest whole number
        firstPersonController.RotationSpeed = roundedSensitivity; // Update player's rotation speed
        PlayerPrefs.SetFloat("MouseSensitivity", sensitivity);
    }

    public void ApplyChanges()
    {
        // Save settings when apply button is clicked
        PlayerPrefs.Save();
    }
}

