using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] GameObject destroyEffect;
    [SerializeField] GameObject splashEffect;
    Rigidbody rb;
    public bool isEnemyBullet = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Physics.Raycast(transform.position, rb.linearVelocity, out RaycastHit hit, rb.linearVelocity.magnitude * Time.deltaTime * 10.0f))
        {
            if ((hit.collider.CompareTag("Enemy") && !isEnemyBullet) || hit.collider.CompareTag("Player") && isEnemyBullet)
            {
                if (!isEnemyBullet)
                {
                    Enemy enemy = hit.collider.GetComponent<Enemy>();
                    if (enemy != null)
                    {
                        enemy.TakeDamage(10f, hit.point);
                    }
                }
                else
                {
                    FirstPersonController player = hit.collider.GetComponent<FirstPersonController>();
                    if (player != null)
                    {
                        player.TakeDamage(10f);
                    }
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
            else if (!hit.collider.gameObject.CompareTag("Fence"))
            {
                GameObject effect = Instantiate(destroyEffect, hit.point, Quaternion.identity);
                Destroy(effect, 2f);
                Destroy(gameObject);
            }
        }
    }
}
