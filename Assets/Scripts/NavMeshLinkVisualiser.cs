using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

[RequireComponent(typeof(NavMeshLink))]
public class NavMeshLinkVisualiser : MonoBehaviour
{
    private NavMeshLink navMeshLink;

    private void Awake()
    {
        navMeshLink = GetComponent<NavMeshLink>();
    }

    private void OnDrawGizmosSelected()
    {
        if (navMeshLink == null)
            return;

        Gizmos.color = Color.cyan;

        // Calculate center position
        Vector3 center = (navMeshLink.startPoint + navMeshLink.endPoint) * 0.5f;

        // Calculate height as the vertical distance between start and end points
        float height = Mathf.Abs(navMeshLink.startPoint.y - navMeshLink.endPoint.y);

        // Calculate size
        Vector3 size = new Vector3(navMeshLink.width, height, Vector3.Distance(navMeshLink.startPoint, navMeshLink.endPoint));

        // Draw wire cube representing the area affected by the NavMesh link
        Gizmos.DrawWireCube(center, size);
    }
}