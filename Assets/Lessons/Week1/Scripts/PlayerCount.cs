using UnityEngine;
using TMPro;

public class PlayerCount : MonoBehaviour
{
    public int playerCount = 0;
    public TextMeshProUGUI playerCountText;
    void Start()
    {
        playerCountText = GetComponent<TextMeshProUGUI>();
        playerCount = GameObject.FindGameObjectsWithTag("Player").Length;
    }

    // Update is called once per frame
    void Update()
    {
        playerCount = GameObject.FindGameObjectsWithTag("Player").Length;
        playerCountText.text = playerCount.ToString();
    }
}
