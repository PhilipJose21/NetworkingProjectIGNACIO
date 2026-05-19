using UnityEngine;
using Unity.Netcode;

public class MuliplayerMenu : MonoBehaviour
{
    public HideClientButtons hideClientButtons;

    private void Awake()
    {
        // if (hideClientButtons == null)
        // {
        //     hideClientButtons = FindObjectOfType<HideClientButtons>();
        // }
    }
    public void StartHost()
    {
        if (hideClientButtons != null) hideClientButtons.hideButtons();
        NetworkManager.Singleton.StartHost();
    }

    public void StartClient()
    {
        if (hideClientButtons != null) hideClientButtons.hideButtons();
        NetworkManager.Singleton.StartClient();
    }

    public void StartServer()
    {
        if (hideClientButtons != null) hideClientButtons.hideButtons();
        NetworkManager.Singleton.StartServer();
    }
}
