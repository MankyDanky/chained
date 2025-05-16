using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    [SerializeField] float timeToLive = 0.2f; 
    float timeLived = 0.0f;

    void Update()
    {
        if (timeLived >= timeToLive)
        {
            Destroy(gameObject);
        }
        else
        {
            timeLived += Time.deltaTime;
        }
    }
}
