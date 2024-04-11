using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;
using System.Collections;
using UnityEngine.SceneManagement;

public class EnemyController : MonoBehaviour
{
    public float detectionRadius = 10f; // Radius within which the player is detected
    public float specialDetectionRadius = 0.2f; // Special radius for triggering behavior
    public string playerTag = "Player"; // Tag of the player
    public AudioClip detectionSound; // Sound to play when player is detected
    public float fadeDuration = 1f; // Duration of fade out
    public CanvasGroup fadeCanvasGroup; // CanvasGroup for fade out effect
    public string sceneToLoad; // Name of the scene to load

    private NavMeshAgent navMeshAgent; // NavMeshAgent component
    private Transform player; // Player's transform
    private bool hasDetectedPlayer = false; // Flag to track if player has been detected

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag(playerTag).transform;
    }

    void Update()
    {
        // Check if player is within special detection radius
        if (!hasDetectedPlayer && Vector3.Distance(transform.position, player.position) <= specialDetectionRadius)
        {
            // Play detection sound
            if (detectionSound != null)
            {
                AudioSource.PlayClipAtPoint(detectionSound, transform.position);
            }

            // Set flag to true
            hasDetectedPlayer = true;

            // Fade out and load scene
            StartCoroutine(FadeOutAndLoadScene());
        }
        else
        {
            // Check if player is within detection radius and in line of sight
            if (Vector3.Distance(transform.position, player.position) <= detectionRadius && InLineOfSight())
            {
                // Chase the player
                navMeshAgent.SetDestination(player.position);
            }
            else
            {
                // Roam around randomly
                Roam();
            }
        }
    }

    bool InLineOfSight()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, (player.position - transform.position).normalized, out hit, detectionRadius))
        {
            // Check if the hit object has the player tag
            if (hit.collider.CompareTag(playerTag))
            {
                return true;
            }
        }
        return false;
    }

    void Roam()
    {
        // Check if the enemy has reached its current destination
        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            // Generate a random destination within a larger radius around the enemy
            Vector3 randomDirection = Random.insideUnitSphere * detectionRadius * 2f;
            randomDirection += transform.position;
            NavMeshHit navHit;

            // Sample for a valid position within the NavMesh
            if (NavMesh.SamplePosition(randomDirection, out navHit, detectionRadius * 2f, NavMesh.AllAreas))
            {
                // Check if the destination is on another level
                if (IsOnDifferentLevel(navHit.position, transform.position))
                {
                    // Set the destination to the random position
                    navMeshAgent.SetDestination(navHit.position);
                }
                else
                {
                    // Check if there's a NavMeshLink ahead
                    NavMeshLink navLink = FindNavLinkAhead(navHit.position);
                    if (navLink != null)
                    {
                        // Traverse the NavMeshLink
                        TraverseNavLink(navLink);
                    }
                    else
                    {
                        // Set the destination to the random position
                        navMeshAgent.SetDestination(navHit.position);
                    }
                }
            }
        }
    }

    bool IsOnDifferentLevel(Vector3 position1, Vector3 position2)
    {
        // Check if the y coordinates are significantly different
        return Mathf.Abs(position1.y - position2.y) > 0.5f;
    }

    NavMeshLink FindNavLinkAhead(Vector3 position)
    {
        // Raycast forward to check for a NavMeshLink
        RaycastHit hit;
        if (Physics.Raycast(transform.position, position - transform.position, out hit, detectionRadius))
        {
            NavMeshLink navLink = hit.collider.GetComponent<NavMeshLink>();
            if (navLink != null)
            {
                return navLink;
            }
        }
        return null;
    }

    void TraverseNavLink(NavMeshLink navLink)
    {
        // Get the end position of the NavMeshLink
        Vector3 endPosition = navLink.endPoint;

        // Set the destination to the end position of the NavMeshLink
        navMeshAgent.SetDestination(endPosition);
    }

    IEnumerator FadeOutAndLoadScene()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            fadeCanvasGroup.alpha = alpha;
            yield return null;
        }

        // Load the next scene
        SceneManager.LoadScene(sceneToLoad);
    }
}