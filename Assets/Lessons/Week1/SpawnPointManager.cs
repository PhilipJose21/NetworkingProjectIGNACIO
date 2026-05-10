using UnityEngine;
using Unity.Netcode;
using System;

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

        GameObject[] spawnPointObjects = GameObject.FindGameObjectsWithTag("SpawnPoint");
        if (spawnPointObjects.Length == 0)
        {
            Debug.Log("No Sppawnpoint detected");
            return;
        }

        Transform selectedSpawnPoint = spawnPointObjects[nextSpawnIndex].transform;
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

        nextSpawnIndex++;
        if (nextSpawnIndex >= spawnPointObjects.Length)
        {
            nextSpawnIndex = 0;
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
