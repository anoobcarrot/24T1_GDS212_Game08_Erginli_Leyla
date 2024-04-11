using UnityEngine;
using StarterAssets;

public class OptionsMenuController : MonoBehaviour
{
    public KeyCode optionsMenuKey = KeyCode.Escape;
    public GameObject optionsMenuCanvas;
    public FirstPersonController firstPersonController; // Reference to your player movement script

    private bool isOptionsMenuOpen = false;
    private bool isPlayerFrozen = false;
    private float previousTimeScale;
    private CursorLockMode previousCursorLockMode;

    private void Start()
    {
        if (optionsMenuCanvas != null)
        {
            optionsMenuCanvas.SetActive(false); // Ensure options menu is initially closed
        }
        previousCursorLockMode = Cursor.lockState;
    }

    private void Update()
    {
        // Check for input to toggle options menu
        if (Input.GetKeyDown(optionsMenuKey))
        {
            if (!isOptionsMenuOpen)
            {
                OpenOptions();
            }
            else
            {
                CloseOptions();
            }
        }
    }

    public void OpenOptions()
    {
        isOptionsMenuOpen = true;

        // Open options menu
        Time.timeScale = 0f; // Pause time
        previousTimeScale = Time.timeScale; // Store previous time scale
        Cursor.lockState = CursorLockMode.None; // Enable mouse cursor
        Cursor.visible = true;
        if (firstPersonController != null)
        {
            firstPersonController.enabled = false; // Freeze player movement
            isPlayerFrozen = true;
        }
        optionsMenuCanvas.SetActive(true); // Show options menu
    }

    public void CloseOptions()
    {
        isOptionsMenuOpen = false;

        // Close options menu
        Time.timeScale = 1f; // Restore previous time scale
        Cursor.lockState = previousCursorLockMode; // Restore previous cursor lock mode
        if (isPlayerFrozen && firstPersonController != null)
        {
            firstPersonController.enabled = true; // Unfreeze player movement
            isPlayerFrozen = false;
        }
        optionsMenuCanvas.SetActive(false); // Hide options menu
    }
}

