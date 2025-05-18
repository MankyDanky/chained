using UnityEngine;

public class Remains : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in rigidbodies)
        {
            rb.AddExplosionForce(40f, transform.position, 5f);
        }
        Destroy(gameObject, 5f);
    }
}
