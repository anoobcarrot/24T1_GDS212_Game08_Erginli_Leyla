using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BatterySystem : MonoBehaviour
{
    public float batteryLevel = 100f; // Initial battery level
    public float normalModeDrainRate = 1f / 3f; // Battery drainage rate in normal mode (1% every 3 seconds)
    public float spectralModeDrainRate = 1f / 1.5f; // Battery drainage rate in spectral mode (1% every 1.5 seconds)
    public TextMeshProUGUI batteryLevelText; // Reference to the UI Text displaying battery level
    public GameObject objectToHide; // Reference to the GameObject to hide when battery is depleted

    private float timeSinceLastDrain; // Time since last battery drainage

    private void Update()
    {
        // Update battery level based on drain rate
        timeSinceLastDrain += Time.deltaTime;
        if (timeSinceLastDrain >= 1f)
        {
            timeSinceLastDrain = 0f;
            DrainBattery();
        }
    }

    private void DrainBattery()
    {
        // Drain battery based on drain rate
        float drainRate = CameraController.spectralMode ? spectralModeDrainRate : normalModeDrainRate;
        batteryLevel -= drainRate;

        // Clamp battery level between 0 and 100
        batteryLevel = Mathf.Clamp(batteryLevel, 0f, 100f);

        // Update UI text
        if (batteryLevelText != null)
        {
            batteryLevelText.text = "Battery Level: " + Mathf.RoundToInt(batteryLevel) + "%";
        }

        // Check if battery is depleted
        if (batteryLevel <= 0f)
        {
            // Perform actions when battery is depleted (e.g., game over)
            Debug.Log("Battery depleted!");

            // Hide the specified GameObject
            if (objectToHide != null)
            {
                objectToHide.SetActive(false);
            }
        }
    }
}



