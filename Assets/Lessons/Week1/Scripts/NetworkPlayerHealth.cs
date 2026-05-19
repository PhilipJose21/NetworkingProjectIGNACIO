using UnityEngine;
using Unity.Netcode;

public class NetworkPlayerHealth : NetworkBehaviour
{
    [SerializeField] private int maxHealth = 100;
    private NetworkVariable<int> currentHealth = new NetworkVariable<int>(
        100,
        NetworkVariableReadPermission.Everyone, //The host, client and server can read this variable
        NetworkVariableWritePermission.Server //Only the server can write to this variable
    );

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            currentHealth.Value = maxHealth;
        }
        currentHealth.OnValueChanged += onHealthChange;
    }

    public override void OnNetworkDespawn()
    {
        currentHealth.OnValueChanged -= onHealthChange;
    }

    private void onHealthChange(int previousValue, int newValue)
    {
        Debug.Log($"{gameObject.name} health change: {previousValue} -> {newValue}");
    }

    public void TakeDamage(int damageAmount)
    {
        if (!IsServer) { return; }
        currentHealth.Value -= damageAmount;
        currentHealth.Value = Mathf.Clamp(currentHealth.Value, 0, maxHealth);
        if (currentHealth.Value <= 0)
        {
            Respawn();
        }
    }

    public void Respawn()
    {
        currentHealth.Value = maxHealth;
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
        int randomIndex = Random.Range(0, spawnPoints.Length);
        Transform selectedSpawn = spawnPoints[randomIndex].transform;
        CharacterController characterController = GetComponent<CharacterController>();

        if (characterController != null)
        {
            characterController.enabled = false;
        }

        transform.position = selectedSpawn.position;
        transform.rotation = selectedSpawn.rotation;
        if (characterController != null)
        {
            characterController.enabled = true;
        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
