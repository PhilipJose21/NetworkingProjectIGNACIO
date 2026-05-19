using UnityEngine;
using Unity.Netcode;

public class PlayerCameraManager : NetworkBehaviour
{
    public static PlayerCameraManager Instance { get; private set; }

    [SerializeField] private Camera menuCamera;
    [SerializeField] private AudioListener menuAudioListener;

    void Awake()
    {
        Instance = this;
        ShowMenuCam();
    }

    public void ShowMenuCam()
    {
        menuCamera.enabled = true;
        menuAudioListener.enabled = true;
    }

    public void HideMenuCam()
    {
        menuCamera.enabled = false;
        menuAudioListener.enabled = false;
    }
}
