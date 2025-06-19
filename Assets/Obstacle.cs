using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] private GameObject destroyEffectPrefab;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnDestroy()
    {
        Instantiate(destroyEffectPrefab, transform.position, Quaternion.identity);
    }
}
