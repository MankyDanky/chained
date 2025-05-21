using UnityEngine;

public class Remains : MonoBehaviour
{
    Rigidbody[] rigidbodies;
    MeshRenderer[] meshRenderers;
    float timer = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidbodies = GetComponentsInChildren<Rigidbody>();
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        foreach (Rigidbody rb in rigidbodies)
        {
            rb.AddExplosionForce(100f, transform.position, 5f);
        }
        
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= 3f)
        {
            Destroy(gameObject);
        }
        foreach (MeshRenderer meshRenderer in meshRenderers)
        {
            foreach (Material material in meshRenderer.materials)
            {
                material.SetFloat("_CutoffHeight", Mathf.Lerp(transform.position.y + 5, transform.position.y - 5, timer / 3f));
            }
        }
    }
}
