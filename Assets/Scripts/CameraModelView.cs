using UnityEngine;
using UnityEngine.UI;

public class CameraModelView : MonoBehaviour
{
    public Camera cameraModelCamera; // Reference to the camera component of the camera model
    public RenderTexture renderTexture; // Reference to the render texture, assign this in the inspector

    private void Start()
    {
        // Set the render texture as the target texture of the camera
        cameraModelCamera.targetTexture = renderTexture;
    }
}


