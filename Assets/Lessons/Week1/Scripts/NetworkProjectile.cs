using UnityEngine;
using Unity.Netcode;

public class NetworkProjectile : NetworkBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifetime = 5f;
    private float despawnTime;
    private int despawnTimeInitialized;
    [HideInInspector]
    public int damageAmount = 5;
    public GameObject owner;
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            despawnTime = Time.time + lifetime;
        }
        
    }

    void Update()
    {
        if (!IsServer) {return;}
        transform.position += transform.forward * speed * Time.deltaTime;
        if (Time.time >= despawnTime)
        {
            NetworkObject.Despawn();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) {return;}
        if (other.CompareTag("Player"))
        {
            NetworkPlayerHealth health = other.GetComponent<NetworkPlayerHealth>();
            
            if (health != null)
            {
                health.TakeDamage(damageAmount, owner);
            }
            NetworkObject.Despawn();
            return;
        }
        NetworkObject.Despawn();
    }
}
