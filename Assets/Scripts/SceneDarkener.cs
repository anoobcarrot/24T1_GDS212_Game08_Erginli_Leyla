using UnityEngine;

public class SceneDarkener : MonoBehaviour
{
    public Color ambientLightColor = new Color(0.1f, 0.1f, 0.1f); // Dark ambient light color
    public float fogDensity = 0.03f; // Density of the fog
    public Color fogColor = new Color(0.05f, 0.05f, 0.05f); // Color of the fog

    private void Start()
    {
        // Set ambient light color
        RenderSettings.ambientLight = ambientLightColor;

        // Enable fog
        RenderSettings.fog = true;
        RenderSettings.fogDensity = fogDensity;
        RenderSettings.fogColor = fogColor;
    }
}

