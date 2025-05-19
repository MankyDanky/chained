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
        Debug.Log("Found " + rigidbodies.Length + " rigidbodies and " + meshRenderers.Length + " mesh renderers.");
        foreach (Rigidbody rb in rigidbodies)
        {
            rb.AddExplosionForce(40f, transform.position, 5f);
        }
        
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= 5f)
        {
            Destroy(gameObject);
        }
        foreach (MeshRenderer meshRenderer in meshRenderers)
        {
            foreach (Material material in meshRenderer.materials)
            {
                material.SetFloat("_CutoffHeight", Mathf.Lerp(5, -5, timer / 5f));
                Debug.Log("Setting cutoff height to " + material.GetFloat("_CutoffHeight"));
            }
        }
    }
}
