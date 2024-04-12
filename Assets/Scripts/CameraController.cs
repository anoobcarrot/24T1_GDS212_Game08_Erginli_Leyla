using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using Unity.AI.Navigation;
using TMPro;

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

    public GameObject player; // Reference to the player object
    public float minDistance = 20f; // Minimum distance from the player
    public float maxDistance = 30f; // Maximum distance from the player
    public List<NavMeshSurface> navMeshSurfaces;

    public int maxPhotos = 15; // Maximum number of photos the player can take
    private int photosTaken = 0; // Number of photos already taken
    public TextMeshProUGUI remainingPhotosText; // Reference to the TMPro text displaying remaining photos
    public TextMeshProUGUI modeText;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor to the center of the screen
        Cursor.visible = false; // Hide the cursor
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

        // Populate the list of NavMeshSurfaces if not already assigned
        if (navMeshSurfaces == null || navMeshSurfaces.Count == 0)
        {
            navMeshSurfaces = new List<NavMeshSurface>(GameObject.FindObjectsOfType<NavMeshSurface>());
        }
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
                if (modeText != null)
                {
                    modeText.text = "MODE: Spectral";
                }
            }
            else
            {
                DisableSpectralMode();
                if (modeText != null)
                {
                    modeText.text = "MODE: Normal";
                }
            }

        // Toggle enemy visibility based on spectral mode
        ToggleEnemyVisibility();
        }

        // Capture photo
        if (Input.GetKeyDown(captureKey) && photosTaken < maxPhotos)
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

        // Update remaining photos text
        UpdateRemainingPhotosText();

        // Update render texture image position
        // if (isRightClicking)
        // {
        // renderTextureImage.rectTransform.localPosition = cameraModel.transform.position + renderTextureOffset;
        // }
    }

    // Method to update remaining photos text
    private void UpdateRemainingPhotosText()
    {
        if (remainingPhotosText != null)
        {
            int remainingPhotos = maxPhotos - photosTaken;
            remainingPhotosText.text = "Remaining Photos: " + remainingPhotos.ToString();
        }
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
        if (IsEnemyInView())
        {
            // Disable the enemy object for 20 seconds
            StartCoroutine(DisableEnemyForSeconds(20f));
        }

        // Increment the number of photos taken
        photosTaken++;

        // Update remaining photos text
        UpdateRemainingPhotosText();

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

    private IEnumerator DisableEnemyForSeconds(float duration)
    {
        // Disable the enemy object
        enemyObject.SetActive(false);

        // Wait for the specified duration
        yield return new WaitForSeconds(duration);

        // Re-enable the enemy object
        enemyObject.SetActive(true);

        // Teleport the enemy to the furthest point on a random NavMesh surface
        TeleportEnemyToRandomNavMesh();
    }

    private bool IsEnemyInView()
    {
        // Get the direction from the camera to the enemy
        Vector3 directionToEnemy = enemyObject.transform.position - mainCamera.transform.position;

        // Calculate the angle between the camera's forward direction and the direction to the enemy
        float angle = Vector3.Angle(mainCamera.transform.forward, directionToEnemy);

        // Check if the angle is within the camera's field of view
        if (angle <= mainCamera.fieldOfView / 2f)
        {
            // Check if the enemy is within the camera's view distance
            RaycastHit hit;
            if (Physics.Raycast(mainCamera.transform.position, directionToEnemy, out hit, Mathf.Infinity))
            {
                if (hit.collider.CompareTag("Enemy"))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void TeleportEnemyToRandomNavMesh()
    {
        if (navMeshSurfaces == null || navMeshSurfaces.Count == 0)
        {
            Debug.LogWarning("No NavMesh surfaces defined.");
            return;
        }

        // Shuffle the list of navMeshSurfaces
        Shuffle(navMeshSurfaces);

        // Get a random NavMesh surface from the list
        NavMeshSurface randomSurface = navMeshSurfaces[Random.Range(0, navMeshSurfaces.Count)];

        // Get a random point within the NavMesh surface bounds
        Vector3 randomPosition = GetRandomPointOnNavMesh(randomSurface);

        // Teleport the enemy to the random position
        enemyObject.transform.position = randomPosition;
    }

    private Vector3 GetRandomPointOnNavMesh(NavMeshSurface surface)
    {
        // Get all colliders in the children of the NavMesh surface
        Collider[] colliders = surface.GetComponentsInChildren<Collider>();
        if (colliders.Length == 0)
        {
            Debug.LogWarning("No colliders found in the children of a NavMesh surface.");
            return Vector3.zero;
        }

        // Combine bounds of all colliders
        Bounds combinedBounds = new Bounds();
        foreach (Collider collider in colliders)
        {
            combinedBounds.Encapsulate(collider.bounds);
        }

        // Generate a random point within the combined bounds
        Vector3 randomPoint = combinedBounds.center + Random.insideUnitSphere * combinedBounds.extents.magnitude;

        // Sample the point on the NavMesh
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, combinedBounds.extents.magnitude, NavMesh.AllAreas))
        {
            return hit.position;
        }
        else
        {
            Debug.LogWarning("Could not find a valid position on the NavMesh surface.");
            return Vector3.zero;
        }
    }

    private void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(i, list.Count);
            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
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
        // Set the local position of the camera model relative to the player's camera
        Vector3 localCenter = new Vector3(0f, -0.25f, 1.6f); // Adjust the local position as needed
        Vector3 targetPosition = playerCamera.transform.TransformPoint(localCenter);

        // Move the camera model to the target position
        cameraModel.transform.position = targetPosition;
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