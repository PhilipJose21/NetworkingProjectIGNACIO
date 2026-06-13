using UnityEngine;
using Unity.Netcode;

public class GameManager : NetworkBehaviour
{
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
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players)
        {
            SpawnPointManager spawnManager = player.GetComponent<SpawnPointManager>();
            if (spawnManager != null)
            {
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
                NetworkPlayerHealth health = player.GetComponent<NetworkPlayerHealth>();
                if (health != null)
                {
                    health.Respawn();
                }
            }
        }
    }
}