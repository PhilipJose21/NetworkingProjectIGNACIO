using UnityEngine;
using Unity.Netcode;

public class PlayerColor : NetworkBehaviour
{
    private GameObject player;
    public Renderer playerRenderer;
    public Material hostMaterial;
    public Material clientMaterial;

    public override void OnNetworkSpawn()
    {
        player = this.gameObject;
        playerRenderer = player.GetComponent<Renderer>();

        // Only set material for the local player object
        if (!IsOwner) return;

        if (IsServer)
        {
            if (hostMaterial != null && playerRenderer != null)
                playerRenderer.material = hostMaterial;
        }
        else
        {
            if (clientMaterial != null && playerRenderer != null)
                playerRenderer.material = clientMaterial;
        }
    }
}
