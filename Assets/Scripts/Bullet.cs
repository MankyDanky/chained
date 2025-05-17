using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] GameObject destroyEffect;
    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Debug.DrawRay(transform.position, rb.linearVelocity * 100, Color.red);
        if (Physics.Raycast(transform.position, rb.linearVelocity, out RaycastHit hit, rb.linearVelocity.magnitude * Time.deltaTime * 10.0f))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                Enemy enemy = hit.collider.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(10f);
                }
            }
            GameObject effect = Instantiate(destroyEffect, hit.point, Quaternion.identity);
            Destroy(effect, 2f);
            Destroy(gameObject);
        }
    }
}
