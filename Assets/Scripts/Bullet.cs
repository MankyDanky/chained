using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] GameObject destroyEffect;
    [SerializeField] GameObject splashEffect;
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
                    enemy.TakeDamage(10f, hit.point);
                }
                GameObject effect = Instantiate(destroyEffect, hit.point, Quaternion.identity);
                Destroy(effect, 2f);
                Destroy(gameObject);
            }
            else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Water"))
            {
                GameObject splash = Instantiate(splashEffect, hit.point, Quaternion.identity);
                splash.transform.forward = hit.normal;
                Color waterColor = hit.collider.GetComponent<Renderer>().material.GetColor("_BaseColor");
                splash.GetComponent<Renderer>().material.SetColor("_Color", new Color(waterColor.r, waterColor.g, waterColor.b, 1f));
                Destroy(gameObject);
            }
            else
            {
                GameObject effect = Instantiate(destroyEffect, hit.point, Quaternion.identity);
                Destroy(effect, 2f);
                Destroy(gameObject);
            }
        }
    }
}
