using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class NetworkPlayerHealth : NetworkBehaviour
{
    [SerializeField] private int maxHealth = 100;
    private NetworkVariable<int> currentHealth = new NetworkVariable<int>(
        100,
        NetworkVariableReadPermission.Everyone, //The host, client and server can read this variable
        NetworkVariableWritePermission.Server //Only the server can write to this variable
    );
    public Transform damagePopupSpawnPoint;
    public GameObject damagePopupPrefab;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            currentHealth.Value = maxHealth;
        }
        currentHealth.OnValueChanged += onHealthChange;
        if (IsOwner)
        {
            HealthBar();
        }

    }

    public override void OnNetworkDespawn()
    {
        currentHealth.OnValueChanged -= onHealthChange;
    }

    private void onHealthChange(int previousValue, int newValue)
    {
        Debug.Log($"{gameObject.name} health change: {previousValue} -> {newValue}");
        if (IsOwner)
        {
            HealthBar();
            if (newValue < previousValue)
            {
                if (damagePopupPrefab == null)
                {
                    Debug.LogWarning("[NetworkPlayerHealth] damagePopupPrefab is not assigned. Assign it in the inspector.");
                }
                else
                {
                    if (damagePopupSpawnPoint != null)
                    {
                        GameObject popup = Instantiate(damagePopupPrefab, damagePopupSpawnPoint);
                        if (popup != null)
                        {
                            RectTransform rt = popup.GetComponent<RectTransform>();
                            if (rt != null)
                            {
                                rt.localPosition = Vector3.zero;
                                rt.localRotation = Quaternion.identity;
                                rt.localScale = Vector3.one;
                            }
                            else
                            {
                                popup.transform.localPosition = Vector3.zero;
                                popup.transform.localRotation = Quaternion.identity;
                                popup.transform.localScale = Vector3.one;
                            }
                        }
                        popup.GetComponent<Text>().text = (previousValue - newValue).ToString();
                    }
                    else
                    {
                        Instantiate(damagePopupPrefab);
                    }
                }
            }
        }
    }

    public void TakeDamage(int damageAmount)
    {

        if (!IsServer) { return; }
        currentHealth.Value -= damageAmount;
        currentHealth.Value = Mathf.Clamp(currentHealth.Value, 0, maxHealth);
        HealthBar();
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
        HealthBar();
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

    public void HealthBar()
    {
        if (!IsOwner) { return; }

        GameObject healthBar = GameObject.FindGameObjectWithTag("HealthBar")?.gameObject;
        if (healthBar == null) { return; }
        Image img = healthBar.GetComponent<Image>();
        if (img == null) { return; }
        img.fillAmount = (float)currentHealth.Value / maxHealth;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
