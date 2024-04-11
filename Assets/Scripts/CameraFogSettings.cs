using UnityEngine;

[ExecuteAlways]
public class CameraFogSettings : MonoBehaviour
{
    public bool fog;
    public Color fogColor;
    public float fogDensity;
    public float fogStartDistance;
    public float fogEndDistance;
    public Color ambientLight;
    public float haloStrength;
    public float flareStrength;

    private bool previousFog;
    private Color previousFogColor;
    private float previousFogDensity;
    private float previousFogStartDistance;
    private float previousFogEndDistance;
    private Color previousAmbientLight;
    private float previousHaloStrength;
    private float previousFlareStrength;

    private void OnPreRender()
    {
        previousFog = RenderSettings.fog;
        previousFogColor = RenderSettings.fogColor;
        previousFogDensity = RenderSettings.fogDensity;
        previousFogStartDistance = RenderSettings.fogStartDistance;
        previousFogEndDistance = RenderSettings.fogEndDistance;
        previousAmbientLight = RenderSettings.ambientLight;
        previousHaloStrength = RenderSettings.haloStrength;
        previousFlareStrength = RenderSettings.flareStrength;
        if (fog)
        {
            RenderSettings.fog = fog;
            RenderSettings.fogColor = fogColor;
            RenderSettings.fogDensity = fogDensity;
            RenderSettings.fogStartDistance = fogStartDistance;
            RenderSettings.fogEndDistance = fogEndDistance;
            RenderSettings.ambientLight = ambientLight;
            RenderSettings.haloStrength = haloStrength;
            RenderSettings.flareStrength = flareStrength;
        }
    }

    private void OnPostRender()
    {
        RenderSettings.fog = previousFog;
        RenderSettings.fogColor = previousFogColor;
        RenderSettings.fogDensity = previousFogDensity;
        RenderSettings.fogStartDistance = previousFogStartDistance;
        RenderSettings.fogEndDistance = previousFogEndDistance;
        RenderSettings.ambientLight = previousAmbientLight;
        RenderSettings.haloStrength = previousHaloStrength;
        RenderSettings.flareStrength = previousFlareStrength;
    }
}
