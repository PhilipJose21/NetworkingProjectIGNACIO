using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    public float timeToDestroy = 2f;
    private void Start()
    {
        Destroy(this.gameObject, timeToDestroy);
    }
}
