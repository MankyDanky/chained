using UnityEngine;

public class Rocket : MonoBehaviour
{

    Transform player;
    Rigidbody rb;
    [SerializeField] float speed = 10f;
    [SerializeField] float damage = 50f;
    [SerializeField] float explosionRadius = 5f;
    [SerializeField] GameObject explosionEffectPrefab;
    [SerializeField] GameObject destroyEffectPrefab;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = FirstPersonController.Instance.transform;
        rb = GetComponent<Rigidbody>();
        rb.linearVelocity = transform.forward * speed;
        transform.LookAt(transform.position + rb.linearVelocity);
    }

    // Update is called once per frame
    void Update()
    {
        rb.AddForce((player.position - transform.position).normalized * speed * Time.deltaTime * 200, ForceMode.Force);
        transform.LookAt(transform.position + rb.linearVelocity);
        transform.Rotate(-90, 180, 0);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (transform.childCount == 0) return; 
        Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        if ((player.transform.position - transform.position).magnitude < explosionRadius)
        {
            FirstPersonController playerScript = player.GetComponent<FirstPersonController>();
            playerScript.TakeDamage(damage);
            playerScript.ApplyImpulse((player.transform.position - transform.position).normalized * 10f + Vector3.up * 5f);
        }
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            Destroy(collision.gameObject);
        }
        Transform child = transform.GetChild(0);
        child.SetParent(null, true); // Unparent and keep world position/scale
        Destroy(child.gameObject, 5f);
        child.GetComponent<ParticleSystem>().Stop();
        child.localScale = Vector3.one * 0.4f;
        Instantiate(destroyEffectPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
