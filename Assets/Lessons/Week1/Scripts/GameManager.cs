using UnityEngine;
using Unity.Netcode;

public class GameManager : NetworkBehaviour
{
    // Singleton pattern so players can easily reference it
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void ReportPlayerDeath()
    {
        if (!IsServer) return;

        Debug.Log("A player has died! Resetting all player positions...");
        ResetAllPlayersClientRpc();
    }

    [ClientRpc]
    private void ResetAllPlayersClientRpc()
    {
        // Find all player objects currently in the match
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players)
        {
            // Try to use the SpawnPointManager attached to the player to handle the teleport safely
            SpawnPointManager spawnManager = player.GetComponent<SpawnPointManager>();
            if (spawnManager != null)
            {
                // Re-evaluate their spawn based on whether they are Player 1 (Host/Server Owner) or Player 2 (Client)
                if (spawnManager.IsOwner && spawnManager.IsServer)
                {
                    GameObject p1Spawn = GameObject.FindGameObjectWithTag("PlayerSpawn");
                    if (p1Spawn != null) spawnManager.setSpawn(p1Spawn);
                }
                else
                {
                    GameObject p2Spawn = GameObject.FindGameObjectWithTag("Player2Spawn");
                    if (p2Spawn != null) spawnManager.setSpawn(p2Spawn);
                }
            }
            else
            {
                // Fallback if SpawnPointManager is missing or you prefer a random fallback spawn point
                NetworkPlayerHealth health = player.GetComponent<NetworkPlayerHealth>();
                if (health != null)
                {
                    health.Respawn();
                }
            }
        }
    }
}