using UnityEngine;
using Unity.Netcode;

public class NetworkPlayerShooter : NetworkBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform bulletSpawnPoint;
    [SerializeField] float fireCooldown = 0.5f;
    private float lastFireTime;
    [SerializeField] KeyCode fireKey = KeyCode.Mouse1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) {return;}
        if (Input.GetKeyDown(fireKey) && Time.time >= lastFireTime + fireCooldown)
        {
            lastFireTime = Time.time + fireCooldown;
            RequestShootServerRpc(bulletSpawnPoint.position, bulletSpawnPoint.forward);
        }
    }
    [ServerRpc]
    private void RequestShootServerRpc(Vector3 spawnPosition, Vector3 spawnDirection)
    {
        GameObject projectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.LookRotation(spawnDirection));
        NetworkObject networkObject = projectile.GetComponent<NetworkObject>();
        networkObject.Spawn();
    }
}
