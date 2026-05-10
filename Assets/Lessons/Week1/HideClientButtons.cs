using UnityEngine;

public class HideClientButtons : MonoBehaviour
{
    public GameObject buttons;
    public void hideButtons()
    {
        buttons.SetActive(false);
    }
}
