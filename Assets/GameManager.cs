using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject[] enemies;
    [SerializeField] GameObject[] miniBosses;
    [SerializeField] Bounds spawnArea;

    IEnumerator SpawnEnemy()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            if (Random.Range(0f, 1f) < 0.25f)
            {
                GameObject prefab = enemies[Random.Range(0, enemies.Length)];
                Vector3 randomPos = new Vector3(
                    Random.Range(transform.position.x + spawnArea.min.x, transform.position.x + spawnArea.max.x),
                    transform.position.y + spawnArea.max.y,
                    Random.Range(transform.position.z + spawnArea.min.z, transform.position.z + spawnArea.max.z)
                );

                Ray ray = new Ray(randomPos, Vector3.down);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 100f))
                {
                    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Terrain"))
                    {
                        Instantiate(prefab, hit.point, Quaternion.identity);
                        Debug.Log($"Spawned {prefab.name} at {hit.point}");
                    }
                }
            }
        }
    }

    void Start()
    {
        StartCoroutine(SpawnEnemy());
    }

    void Update()
    {
        
    }
}
