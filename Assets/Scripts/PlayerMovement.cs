using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // Movement speed
    private Rigidbody rb; // Reference to the Rigidbody component

    void Start()
    {
        // Get reference to the Rigidbody component attached to the player GameObject
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Get input from WASD keys
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Calculate movement direction based on input
        Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput) * moveSpeed * Time.deltaTime;

        // Apply movement to the player's position
        rb.MovePosition(transform.position + movement);
    }
}

