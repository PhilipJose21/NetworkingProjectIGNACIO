using UnityEngine;

public class PlayerRotation : MonoBehaviour
{
    [Tooltip("Camera used to convert mouse position to world ray. If null, Camera.main is used.")]
    public Camera playerCamera;
    [Tooltip("Optional layer mask for raycasting (set to Ground layer)")]
    public LayerMask groundLayer = ~0;
    [Tooltip("Rotation smoothing speed. Set to 0 for instant rotation.")]
    public float rotationSpeed = 10f;

    void Awake()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }
    }

    void Update()
    {
        if (playerCamera == null) return;

        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);

        Vector3 targetPoint = Vector3.zero;

        // Raycast all and pick the first hit that isn't this player (or a child)
        RaycastHit[] hits = Physics.RaycastAll(ray, 100f, groundLayer, QueryTriggerInteraction.Ignore);
        bool found = false;
        if (hits != null && hits.Length > 0)
        {
            System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
            foreach (var h in hits)
            {
                GameObject hitGo = h.collider.gameObject;
                if (hitGo == this.gameObject) continue;
                if (hitGo.transform.IsChildOf(transform)) continue;
                targetPoint = h.point;
                found = true;
                break;
            }
        }

        if (!found)
        {
            // Fallback: project onto horizontal plane at player's y
            Plane plane = new Plane(Vector3.up, new Vector3(0f, transform.position.y, 0f));
            if (!plane.Raycast(ray, out float enter)) return;
            targetPoint = ray.GetPoint(enter);
        }

        Vector3 direction = targetPoint - transform.position;
        direction.y = 0f; // only rotate around Y
        if (direction.sqrMagnitude < 0.0001f) return;

        Quaternion targetRot = Quaternion.LookRotation(direction);
        if (rotationSpeed <= 0f)
        {
            transform.rotation = targetRot;
        }
        else
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }
    }
}
