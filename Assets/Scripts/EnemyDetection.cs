using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class EnemyDetection : MonoBehaviour
{
    public GameObject playerObject;
    public float detectionRadius = 10f;
    public List<AudioClip> detectionAudios = new List<AudioClip>();

    private AudioSource audioSource;
    private bool isInRange = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = false;

        // Check if the player object is assigned
        if (playerObject == null)
        {
            Debug.LogError("Player object is not assigned!");
            return;
        }
    }

    private void Update()
    {
        // Check if the player object is assigned
        if (playerObject == null)
        {
            return;
        }

        // Check if the player is within the detection radius
        float distanceToPlayer = Vector3.Distance(transform.position, playerObject.transform.position);
        bool newInRange = distanceToPlayer <= detectionRadius;

        // If the player enters the detection radius, play a random audio from the list
        if (newInRange && !isInRange)
        {
            // Player entered the detection radius
            Debug.Log("Player entered detection radius");
            PlayRandomAudio();
            isInRange = true;
        }
        // If the player exits the detection radius, stop the audio
        else if (!newInRange && isInRange)
        {
            // Player exited the detection radius
            Debug.Log("Player exited detection radius");
            audioSource.Stop();
            isInRange = false;
        }
        // If the player is still within the detection radius and the current audio clip has finished playing, play a new one
        else if (newInRange && isInRange && !audioSource.isPlaying)
        {
            // Play a new random audio clip
            PlayRandomAudio();
        }
    }

    private void PlayRandomAudio()
    {
        // Choose a random audio clip from the list
        if (detectionAudios.Count > 0)
        {
            int randomIndex = Random.Range(0, detectionAudios.Count);
            audioSource.clip = detectionAudios[randomIndex];
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("No audio clips assigned!");
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Draw a wire sphere to visualize the detection radius
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}




