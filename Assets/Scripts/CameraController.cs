using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CameraController : MonoBehaviour
{
    public KeyCode switchModeKey = KeyCode.Tab;
    public KeyCode captureKey = KeyCode.Space;
    public KeyCode moveCameraKey = KeyCode.Mouse1; // Change this to the desired keycode for right-click
    public Camera mainCamera; // Reference to the main camera
    public Camera playerCamera;
    public GameObject cameraModel; // Reference to the camera model object
    // public RawImage renderTextureImage; // Reference to the RawImage component displaying the render texture
    public VolumeProfile spectralVolumeProfile; // Reference to the Volume Profile for spectral mode

    public Light cameraFlashLight; // Reference to the light source for the camera flash

    public float flashIntensity = 5f; // Intensity of the camera flash
    public float flashDuration = 0.1f; // Duration of the camera flash

    // public Vector3 renderTextureOffset; // Offset for the render texture image position
    // public Vector3 renderTextureResetPosition; // Offset for resetting the render texture image position

    public Color spectralBackgroundColor = Color.white; // Background color when spectral mode is enabled
    public Color defaultBackgroundColor = Color.black; // Background color when spectral mode is disabled

    public static bool spectralMode = false;
    private List<Volume> volumes = new List<Volume>(); // List to store all Volume components
    private Vector3 originalModelPosition; // Original position of the camera model object
    private Vector3 initialOffset; // Initial offset between camera model and player
    private bool isRightClicking = false; // Flag to track right-click state
    private GameObject enemyObject; // Reference to the enemy object
    private Renderer enemyRenderer; // Reference to the renderer of the enemy object

    public float minZoomFOV = 20f; // Minimum FOV for zooming out
    public float maxZoomFOV = 60f; // Maximum FOV for zooming in
    public float zoomSpeed = 1f; // Speed of zooming
    private bool isZoomingIn = false;
    private bool isZoomingOut = false;

    private void Start()
    {
        // Find all Volume components in the hierarchy
        FindAllVolumes();

        // Disable volume initially
        DisableAllVolumes();

        // Store the original position of the camera model object
        originalModelPosition = cameraModel.transform.localPosition;

        // Store the initial offset between camera model and player
        initialOffset = cameraModel.transform.position - playerCamera.transform.position;

        // Find the enemy object with the "Enemy" tag
        enemyObject = GameObject.FindGameObjectWithTag("Enemy");

        // Get the renderer component of the enemy object
        enemyRenderer = enemyObject.GetComponent<Renderer>();
    }

    private void Update()
    {
        // Switch camera modes
        if (Input.GetKeyDown(switchModeKey))
        {
            spectralMode = !spectralMode;

            // Enable/disable volume based on spectral mode
            if (spectralMode)
            {
                EnableSpectralMode();
            }
            else
            {
                DisableSpectralMode();
            }

        // Toggle enemy visibility based on spectral mode
        ToggleEnemyVisibility();
        }

        // Capture photo
        if (Input.GetKeyDown(captureKey) && spectralMode)
        {
            // Perform camera flash effect
            StartCoroutine(CameraFlashEffect());
        }

        // Handle right-click movement
        if (Input.GetKeyDown(moveCameraKey))
        {
            isRightClicking = true;
            MoveCameraModelToCenter();
        }
        else if (Input.GetKeyUp(moveCameraKey))
        {
            isRightClicking = false;
            MoveCameraModelToOriginalPosition();
        }

        // Get mouse scroll wheel input
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        // Zoom in
        if (scrollInput > 0f && !isZoomingOut)
        {
            isZoomingIn = true;
            ZoomIn();
        }
        else
        {
            isZoomingIn = false;
        }

        // Zoom out
        if (scrollInput < 0f && !isZoomingIn)
        {
            isZoomingOut = true;
            ZoomOut();
        }
        else
        {
            isZoomingOut = false;
        }

    // Update render texture image position
    // if (isRightClicking)
    // {
    // renderTextureImage.rectTransform.localPosition = cameraModel.transform.position + renderTextureOffset;
    // }
}

    void ZoomIn()
    {
        // Calculate new FOV for zooming in
        float newFOV = mainCamera.fieldOfView - zoomSpeed;

        // Clamp FOV within the specified range
        newFOV = Mathf.Clamp(newFOV, minZoomFOV, maxZoomFOV);

        // Apply the new FOV to the camera
        mainCamera.fieldOfView = newFOV;
    }

    void ZoomOut()
    {
        // Calculate new FOV for zooming out
        float newFOV = mainCamera.fieldOfView + zoomSpeed;

        // Clamp FOV within the specified range
        newFOV = Mathf.Clamp(newFOV, minZoomFOV, maxZoomFOV);

        // Apply the new FOV to the camera
        mainCamera.fieldOfView = newFOV;
    }

    private void FindAllVolumes()
    {
        // Clear the list of volumes
        volumes.Clear();

        // Find all Volume components in the hierarchy
        Volume[] allVolumes = FindObjectsOfType<Volume>();

        // Add them to the list
        volumes.AddRange(allVolumes);
    }

    private void DisableAllVolumes()
    {
        // Disable all Volume components
        foreach (Volume volume in volumes)
        {
            volume.enabled = false;
        }
    }

    private void EnableSpectralMode()
    {
        // Enable all Volume components and assign the Spectral Volume Profile
        foreach (Volume volume in volumes)
        {
            volume.profile = spectralVolumeProfile;
            volume.enabled = true;
        }
    }

    private void DisableSpectralMode()
    {
        // Disable all Volume components
        DisableAllVolumes();
    }

    private void CapturePhoto()
    {
        // Store the previous render texture
        RenderTexture previousRenderTexture = mainCamera.targetTexture;

        // Create a RenderTexture with the same dimensions as the main camera's viewport
        RenderTexture renderTexture = new RenderTexture(mainCamera.pixelWidth, mainCamera.pixelHeight, 24);

        // Set the target texture of the main camera to the RenderTexture
        mainCamera.targetTexture = renderTexture;

        // Create a Texture2D and read pixels from the RenderTexture
        Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        mainCamera.Render();
        RenderTexture.active = renderTexture;
        texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture.Apply();

        // Reset the target texture of the main camera to the previous render texture
        mainCamera.targetTexture = previousRenderTexture;

        // Release the RenderTexture
        RenderTexture.active = null;
        Destroy(renderTexture);

        // Encode the Texture2D to a PNG file
        byte[] bytes = texture.EncodeToPNG();

        // Write the PNG file
        string fileName = "photo_" + System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".png";
        System.IO.File.WriteAllBytes(fileName, bytes);
        Debug.Log("Photo captured: " + fileName);
    }

    private IEnumerator CameraFlashEffect()
    {
        // Enable the light source
        cameraFlashLight.intensity = flashIntensity;
        CapturePhoto();

        // Wait for the flash duration
        yield return new WaitForSeconds(flashDuration);

        // Disable the light source
        cameraFlashLight.intensity = 0f;
    }

    private void MoveCameraModelToCenter()
    {
        // Set the center position as per your requirement
        Vector3 centerPosition = new Vector3(0f, -0.25f, 1.132f);

        // Convert center position to world point
        Vector3 worldCenter = playerCamera.transform.TransformPoint(centerPosition);

        // Keep the z coordinate of the camera model and update x and y coordinates
        worldCenter.z = cameraModel.transform.position.z;

        // Move camera model to the specified world center position
        cameraModel.transform.position = worldCenter;
    }

    private void MoveCameraModelToOriginalPosition()
    {
        // Reset camera model position to original
        cameraModel.transform.localPosition = originalModelPosition;

        // Reset render texture image position to its original position
        // renderTextureImage.rectTransform.localPosition = renderTextureResetPosition;
    }

    private void ToggleEnemyVisibility()
    {
        // Get the layer mask for the "Spectral" layer
        int spectralLayerMask = 1 << LayerMask.NameToLayer("Spectral");

        // Toggle enemy visibility based on spectral mode for the main camera
        if (spectralMode)
        {
            // If spectral mode is enabled, enable the "Spectral" layer in the main camera's culling mask
            mainCamera.cullingMask |= spectralLayerMask;
            mainCamera.backgroundColor = spectralBackgroundColor;
        }
        else
        {
            // If spectral mode is disabled, disable the "Spectral" layer in the main camera's culling mask
            mainCamera.cullingMask &= ~spectralLayerMask;
            mainCamera.backgroundColor = defaultBackgroundColor;
        }
    }
}