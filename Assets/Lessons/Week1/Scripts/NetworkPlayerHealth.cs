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
    private int takenDamage;

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
        if (IsOwner)
        {
            HealthBar();
            if (newValue < previousValue)
            {
                int damageAmount = Mathf.Max(0, previousValue - newValue);

                if (damagePopupPrefab == null)
                {
                    Debug.LogWarning("[NetworkPlayerHealth] damagePopupPrefab is not assigned. Assign it in the inspector.");
                }
                else
                {
                    GameObject popup = null;

                    // Instantiate under spawn point if provided, otherwise standalone
                    if (damagePopupSpawnPoint != null)
                    {
                        popup = Instantiate(damagePopupPrefab, damagePopupSpawnPoint);
                    }
                    else
                    {
                        popup = Instantiate(damagePopupPrefab);
                    }

                    if (popup != null)
                    {
                        // normalize RectTransform for UI prefabs
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

                        // Set the popup text: support both legacy `Text` and TextMeshPro (`TextMeshProUGUI`)
                        Text uiText = popup.GetComponentInChildren<Text>(true);
                        if (uiText != null)
                        {
                            uiText.text = damageAmount.ToString();
                        }
                        else
                        {
                            TMPro.TextMeshProUGUI tmp = popup.GetComponentInChildren<TMPro.TextMeshProUGUI>(true);
                            if (tmp != null)
                            {
                                tmp.text = damageAmount.ToString();
                            }
                        }
                    }
                }
            }
        }
    }

    public void TakeDamage(int damageAmount, GameObject damageSource)
    {
        if (!IsServer) { return; } // Confirmed: We are running on the Server!
        
        currentHealth.Value -= damageAmount;
        takenDamage = damageAmount;
        currentHealth.Value = Mathf.Clamp(currentHealth.Value, 0, maxHealth);
        HealthBar();
        
        if (currentHealth.Value <= 0)
        {
            // FIX: Directly award the point on the server instead of invoking an RPC
            if (damageSource != null)
            {
                NetworkPlayerController attacker = damageSource.GetComponent<NetworkPlayerController>();
                
                // If the script is on a child object of the player (like a weapon/projectile), 
                // check the parent components as a fallback
                if (attacker == null)
                {
                    attacker = damageSource.GetComponentInParent<NetworkPlayerController>();
                }

                if (attacker != null)
                {
                    attacker.AddPointDirect(); 
                    Debug.Log($"Point awarded directly to: {damageSource.name}");
                }
                else
                {
                    Debug.LogWarning($"Could not find NetworkPlayerController on {damageSource.name} to award point!");
                }
            }

            // Reset health for the next round
            currentHealth.Value = maxHealth;
            HealthBar();

            // Tell the global GameManager to reset everyone across the network
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ReportPlayerDeath();
            }
            else
            {
                Respawn(); 
            }
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
