using UnityEngine;
using TMPro;

public class PlayerCount : MonoBehaviour
{
    public int playerCount = 0;
    public TextMeshProUGUI playerCountText;
    public GameObject waitingContainerUI;
    private TextMeshProUGUI countDownText;
    void Start()
    {
        playerCountText = GetComponent<TextMeshProUGUI>();
        countDownText = waitingContainerUI.GetComponentInChildren<TextMeshProUGUI>();
        playerCount = GameObject.FindGameObjectsWithTag("Player").Length;
        waitingContainerUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        playerCount = GameObject.FindGameObjectsWithTag("Player").Length;
        playerCountText.text = playerCount.ToString();

        if (playerCount >= 2)
        {
            Time.timeScale = 1f;
            // CountDown(true);
            waitingContainerUI.SetActive(false);
            Debug.Log("Game Started");
        }
        else if (playerCount == 1)
        {
            Time.timeScale = 0f;
            waitingContainerUI.SetActive(true);
            // CountDown(false);
        }
        else
        {
            Time.timeScale = 0f;
            waitingContainerUI.SetActive(false);
            Debug.Log("Waiting for players...");
        }
    }

    public void CountDown(bool startCD)
    {
        int count = 5;
        if (startCD)
        {
            while (count > 0)
            {
                countDownText.text = count.ToString();
                count--;
                System.Threading.Thread.Sleep(1000); // Wait for 1 second
            }
        }
        else
        {
            countDownText.text = "Waiting for players...";
        }
    }

}
