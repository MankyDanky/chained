using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField] private float explosionRadius = 8f;
    [SerializeField] private float explosionDamage = 100f;
    [SerializeField] private GameObject explosionEffect;

    void Start()
    {
        Destroy(gameObject, 3f);
    }

    void OnDestroy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Shake shake = GameObject.FindWithTag("MainCamera").GetComponent<Shake>();
        shake.start = true;
        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance <= explosionRadius)
            {
                Enemy enemyScript = enemy.GetComponent<Enemy>();
                if (enemyScript != null)
                {
                    enemyScript.TakeDamage(explosionDamage, enemy.transform.position);
                }
            }
        }
        Instantiate(explosionEffect, transform.position, Quaternion.identity);
    }
}
