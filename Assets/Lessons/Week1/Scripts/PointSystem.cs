using UnityEngine;
using System.Collections;
using Unity.Netcode;
using TMPro;

public class PointSystem : MonoBehaviour
{
    public GameObject player1, player2;
    public PlayerCount playerCount;
    public TextMeshProUGUI player1PointsText, player2PointsText;
    public GameObject player1WinText, player2WinText;

    private bool isMatchOver = false;
    
    void Start()
    {
        player1WinText.SetActive(false);
        player2WinText.SetActive(false);
    }

    void OnEnable()
    {
        // Subscribe to scene loading events so we know when the new scene is ready
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.SceneManager != null)
        {
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += OnSceneLoadComplete;
        }
    }

    void OnDisable()
    {
        // Clean up our listener when destroyed or disabled
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.SceneManager != null)
        {
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= OnSceneLoadComplete;
        }
    }

    void Update()
    {
        if (isMatchOver) return;

        if (playerCount.playerCount >= 2)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            if (players.Length >= 2)
            {
                player1 = null;
                player2 = null;

                for (int i = 0; i < players.Length; i++)
                {
                    var netObj = players[i].GetComponent<NetworkObject>();
                    if (netObj != null)
                    {
                        if (netObj.OwnerClientId == 0)
                        {
                            player1 = players[i];
                        }
                        else
                        {
                            player2 = players[i];
                        }
                    }
                }
            }

            if (player1 != null && player2 != null)
            {
                var p1Controller = player1.GetComponent<NetworkPlayerController>();
                var p2Controller = player2.GetComponent<NetworkPlayerController>();

                if (p1Controller != null && p2Controller != null)
                {
                    player1PointsText.text = p1Controller.points.Value.ToString();
                    player2PointsText.text = p2Controller.points.Value.ToString();

                    if (p1Controller.points.Value >= 10)
                    {
                        Debug.Log("Player 1 (Host) Wins!");
                        isMatchOver = true;
                        player1WinText.SetActive(true);
                        StartCoroutine(returnToLobby());
                    }
                    else if (p2Controller.points.Value >= 10)
                    {
                        Debug.Log("Player 2 (Client) Wins!");
                        isMatchOver = true;
                        player2WinText.SetActive(true);
                        StartCoroutine(returnToLobby());
                    }
                }
            }
        }
    }

    IEnumerator returnToLobby()
    {
        yield return new WaitForSeconds(5f);

        if (NetworkManager.Singleton.IsServer)
        {
            // Just trigger the scene load. DO NOT reset positions here yet, 
            // because the spawn points don't exist in the old scene state!
            NetworkManager.Singleton.SceneManager.LoadScene("Week1", UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
    }

    // This runs automatically on both server and client AFTER the scene reloads completely
    private void OnSceneLoadComplete(string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode, System.Collections.Generic.List<ulong> clientsCompleted, System.Collections.Generic.List<ulong> clientsTimedOut)
    {
        // Make sure we are responding to our intended lobby scene
        if (sceneName == "Week1")
        {
            // Clean up the local UI elements completely
            if (player1WinText != null) player1WinText.SetActive(false);
            if (player2WinText != null) player2WinText.SetActive(false);
            isMatchOver = false;

            // Only the server should modify network variables and move player components
            if (NetworkManager.Singleton.IsServer)
            {
                ResetAndPositionPlayers();
            }
        }
    }

    private void ResetAndPositionPlayers()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        
        foreach (GameObject player in players)
        {
            var netObj = player.GetComponent<NetworkObject>();
            if (netObj == null) continue;

            // 1. Clear their points back to zero
            var controller = player.GetComponent<NetworkPlayerController>();
            if (controller != null)
            {
                controller.points.Value = 0;
            }

            // 2. Refill health back to maximum
            var health = player.GetComponent<NetworkPlayerHealth>();
            if (health != null)
            {
                // We access the health system directly to reset parameters safely
                health.Respawn(); 
            }

            // 3. Force them to update positions based on fresh spawn tags in the clean scene
            var spawnManager = player.GetComponent<SpawnPointManager>();
            if (spawnManager != null)
            {
                if (netObj.OwnerClientId == 0)
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
        }
    }
}