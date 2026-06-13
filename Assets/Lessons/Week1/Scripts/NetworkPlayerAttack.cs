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
        if (!IsOwner) return;
        if (Input.GetKeyDown(playerAttackKey))
        {
            RequestAttackServerRpc();
        }
    }

    [ServerRpc]
    public void RequestAttackServerRpc()
    {
        Vector3 attackCenter = transform.position + transform.forward * attackRange;
        Collider[] hits = Physics.OverlapSphere(attackCenter, attackRange, damageableLayers);
        foreach (Collider hit in hits)
        {
            if (hit.gameObject == gameObject) continue; 
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
