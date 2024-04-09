using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public float detectionRadius = 10f; // Radius within which the player is detected
    public string playerTag = "Player"; // Tag of the player 
    NavMeshAgent navMeshAgent; // NavMeshAgent component
    Transform player; // player's transform

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag(playerTag).transform;
    }

    void Update()
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
        // Generate a random destination within a radius around the enemy
        Vector3 randomDirection = Random.insideUnitSphere * detectionRadius;
        randomDirection += transform.position;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randomDirection, out navHit, detectionRadius, NavMesh.AllAreas);

        // Set the destination to the random position
        navMeshAgent.SetDestination(navHit.position);
    }
}
