using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] GameObject[] enemies;
    [SerializeField] GameObject[] miniBosses;
    [SerializeField] Bounds spawnArea;
    [SerializeField] TMP_Text waveText;
    [SerializeField] GameObject upgradeStationPrefab;
    [SerializeField] Transform upgradeStationSpawnPoint;
    [SerializeField] Upgrade[] upgrades;
    int secondsPassed = 0;
    int wave = 1;
    bool upgrading = false;

    IEnumerator SpawnEnemy()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            if (upgrading) continue;
            secondsPassed++;
            if (secondsPassed % 20 == 0)
            {
                while (GameObject.FindGameObjectsWithTag("Enemy").Length > 0)
                {
                    yield return new WaitForSeconds(1f);
                }
                wave++;
                waveText.text = $"WAVE: {wave}";
                upgrading = true;
                secondsPassed = 0;
                UpgradeStation upgradeStation = Instantiate(upgradeStationPrefab, upgradeStationSpawnPoint.position, upgradeStationSpawnPoint.rotation).GetComponent<UpgradeStation>();
                upgradeStation.upgrades[0] = upgrades[Random.Range(0, upgrades.Length)];
                upgradeStation.upgrades[1] = upgrades[Random.Range(0, upgrades.Length)];
                upgradeStation.upgrades[2] = upgrades[Random.Range(0, upgrades.Length)];
            }
            else if (secondsPassed % 10 == 0)
            {
                GameObject prefab = miniBosses[Random.Range(0, miniBosses.Length)];
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
                        waveText.text = $"Wave: {secondsPassed / 20}";
                    }
                }
            }
            else
            {
                if (Random.Range(0f, 1f) < 0.3f)
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
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.LogWarning("Multiple instances of GameManager detected. Destroying the new instance.");
            Destroy(gameObject);
        }
    }

    void Start()
    {
        StartCoroutine(SpawnEnemy());
    }

    void Update()
    {

    }

    public void StopUpgrading()
    {
        upgrading = false;
        Pistol pistol = FindAnyObjectByType<Pistol>();
        if (pistol.grenadeWaveDamageDone > pistol.bulletWaveDamageDone && pistol.grenadeWaveDamageDone > pistol.zagWaveDamageDone)
        {
            pistol.SetChainedAttack(Pistol.AttackType.Grenade);
        }
        else if (pistol.bulletWaveDamageDone > pistol.grenadeWaveDamageDone && pistol.bulletWaveDamageDone > pistol.zagWaveDamageDone)
        {
            pistol.SetChainedAttack(Pistol.AttackType.Bullet);
        }
        else
        {
            pistol.SetChainedAttack(Pistol.AttackType.Zag);
        }
        pistol.grenadeWaveDamageDone = 0;
        pistol.bulletWaveDamageDone = 0;
        pistol.zagWaveDamageDone = 0;
    }
}
