using System.Collections.Generic;
using UnityEngine;

public class ZagBullet : MonoBehaviour
{
    [SerializeField] GameObject destroyEffect;
    [SerializeField] GameObject hitEffect;
    [SerializeField] GameObject splashEffect;
    Rigidbody rb;
    public GameObject target;
    public float speed = 20f;
    public float damage = 20f;
    private HashSet<GameObject> hitEnemies = new HashSet<GameObject>();
    [SerializeField] float lifespan = 5f;
    float timeLived = 0.0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        timeLived += Time.deltaTime;
        if (timeLived >= lifespan)
        {
            GameObject effectDestroy = Instantiate(destroyEffect, transform.position, Quaternion.identity);
            Destroy(effectDestroy, 2f);
            Destroy(gameObject);
            return;
        }
        if (target != null)
        {
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, (target.transform.position - transform.position).normalized * speed, Time.deltaTime * 10f);
        }
        
        if (Physics.Raycast(transform.position, rb.linearVelocity, out RaycastHit hit, rb.linearVelocity.magnitude * Time.deltaTime * 10.0f))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                Enemy enemy = hit.collider.GetComponent<Enemy>();
                if (enemy != null && !hitEnemies.Contains(hit.collider.gameObject))
                {
                    enemy.TakeDamage(damage, hit.point);
                    GameObject effect = Instantiate(hitEffect, hit.point, Quaternion.identity);
                    Destroy(effect, 2f);
                    hitEnemies.Add(hit.collider.gameObject);

                }
                if (hit.collider.gameObject == target)
                {
                    GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
                    GameObject closestEnemy = null;
                    float closestDistance = Mathf.Infinity;
                    foreach (GameObject enemyObj in enemies)
                    {
                        if (hitEnemies.Contains(enemyObj)) continue;
                        float distance = Vector3.Distance(hit.point, enemyObj.transform.position);
                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            closestEnemy = enemyObj;
                        }
                    }
                    if (closestEnemy != null)
                    {
                        target = closestEnemy;
                    }
                    else
                    {
                        GameObject effectDestroy = Instantiate(destroyEffect, hit.point, Quaternion.identity);
                        Destroy(effectDestroy, 2f);
                        Destroy(gameObject);
                    }
                }
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
