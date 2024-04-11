using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private void Start()
    {
        // Show the cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
