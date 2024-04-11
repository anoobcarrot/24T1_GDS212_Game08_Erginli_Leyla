using UnityEngine;

public class LightCulling : MonoBehaviour
{
    public LayerMask spectralLayer; // The layer the light source belongs to
    private Light lightSource;

    void Start()
    {
        lightSource = GetComponent<Light>();
    }

    void Update()
    {
        // Check if the light's culling mask includes the light layer
        if ((spectralLayer & (1 << lightSource.gameObject.layer)) != 0)
        {
            // Enable the light source if the layer is allowed
            lightSource.enabled = true;
        }
        else
        {
            // Disable the light source if the layer is not allowed
            lightSource.enabled = false;
        }
    }
}
