using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField] public float explosionRadius = 8f;
    [SerializeField] public float explosionDamage = 100f;
    [SerializeField] private GameObject explosionEffect;
    Pistol pistol;

    void Start()
    {
        Destroy(gameObject, 2f);
        pistol = FindAnyObjectByType<Pistol>();
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
                    pistol.grenadeDamageDone += explosionDamage;
                    pistol.grenadeWaveDamageDone += explosionDamage;
                    enemyScript.TakeDamage(explosionDamage, enemy.transform.position);
                }
            }
        }
        Instantiate(explosionEffect, transform.position, Quaternion.identity);
    }
}
