using UnityEngine;
using Unity.Netcode;

public class NetworkPlayerAttack : NetworkBehaviour
{

    [SerializeField] public int damageAmount = 10;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private LayerMask damageableLayers;
    [SerializeField] KeyCode playerAttackKey = KeyCode.Mouse0;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return; // only let the owning client send attack requests
        if (Input.GetKeyDown(playerAttackKey))
        {
            RequestAttackServerRpc();
        }
    }

    [ServerRpc]
    public void RequestAttackServerRpc()
    {
        Debug.Log($"ServerRpc: attack requested by object {gameObject.name}");
        Vector3 attackCenter = transform.position + transform.forward * attackRange;
        Collider[] hits = Physics.OverlapSphere(attackCenter, attackRange, damageableLayers);
        Debug.Log($"ServerRpc: found {hits.Length} hits");
        foreach (Collider hit in hits)
        {
            if (hit.gameObject == gameObject) continue; // Skip self
            Debug.Log($"ServerRpc: hit {hit.gameObject.name}");
            NetworkPlayerHealth health = hit.GetComponent<NetworkPlayerHealth>();
            if (health != null)
            {
                health.TakeDamage(damageAmount, gameObject);
                break;
            }
        }


        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward * attackRange, attackRange);
    }
}
