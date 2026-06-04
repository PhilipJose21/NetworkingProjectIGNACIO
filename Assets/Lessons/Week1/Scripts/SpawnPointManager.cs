using UnityEngine;
using Unity.Netcode;
using System;
using UnityEngine.UI;
using TMPro;
// removed invalid using Unity.Random; Random is in UnityEngine

public class SpawnPointManager : NetworkBehaviour
{
    //stores which spawn point should be used next
    //static means all player objects share this value
    private static int nextSpawnIndex;

    //runs when the player object is spawned by netcode
    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            return;
        }

        if (IsOwner)
        {
            GameObject player1SpawnPoint = GameObject.FindGameObjectWithTag("PlayerSpawn");
            setSpawn(player1SpawnPoint);
        }
        else if (IsClient)
        {
            GameObject player2SpawnPoint = GameObject.FindGameObjectWithTag("Player2Spawn");
            setSpawn(player2SpawnPoint);
        }

    }

    public void setSpawn(GameObject spawnPoints)
    {
        Transform selectedSpawnPoint = spawnPoints.transform;
        CharacterController characterController = GetComponent<CharacterController>();

        //temporarily disables the characterController
        //before teleporting the player;
        if (characterController != null)
        {
            characterController.enabled = false;
        }
        transform.position = selectedSpawnPoint.position; //moves the player to the selected spawn point
        transform.rotation = selectedSpawnPoint.rotation; //rotates the player to match the spawn point

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
