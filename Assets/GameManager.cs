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
    [SerializeField] GameObject weaponCamera;
    [SerializeField] GameObject boss;
    [SerializeField] GameObject[] HUDs;
    Animator canvasAnimator;
    int secondsPassed = 0;
    public int wave = 1;
    bool upgrading = false;
    bool bossing = false;
    [SerializeField] Transform cameraCutsceneTransform;
    [SerializeField] GameObject electricFences;
    [SerializeField] Transform playerCutsceneTransform;
    [SerializeField] GameObject portal;
    [SerializeField] bool lastMap = false;
    [SerializeField] int finalSceneIndex = 3;
    public float bulletDamageDone;
    public float zagDamageDone;
    public float grenadeDamageDone;

    IEnumerator SpawnEnemy()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            if (upgrading || bossing) continue;
            secondsPassed++;
            if (secondsPassed % 20 == 0)
            {
                while (GameObject.FindGameObjectsWithTag("Enemy").Length > 0)
                {
                    yield return new WaitForSeconds(1f);
                }
                wave++;
                waveText.text = $"WAVE: {wave}";
                if (wave == 3)
                {
                    canvasAnimator.SetTrigger("Load");
                    yield return new WaitForSeconds(1f);
                    bossing = true;
                    secondsPassed = 0;
                    foreach (GameObject HUD in HUDs)
                    {
                        HUD.SetActive(false);
                    }
                    FirstPersonController.Instance.position = playerCutsceneTransform.position;
                    FirstPersonController.Instance.rotation = playerCutsceneTransform.rotation;
                    FirstPersonController.Instance.GetComponent<FirstPersonController>().inCutscene = true;
                    weaponCamera.SetActive(false);
                    boss.SetActive(true);
                    Vector3 originalPosition = Camera.main.transform.position;
                    Quaternion originalRotation = Camera.main.transform.rotation;
                    Camera.main.transform.position = cameraCutsceneTransform.position;
                    Camera.main.transform.rotation = cameraCutsceneTransform.rotation;
                    yield return new WaitForSeconds(3f);
                    canvasAnimator.SetTrigger("Load");
                    yield return new WaitForSeconds(1f);
                    electricFences.SetActive(true);
                    weaponCamera.SetActive(true);
                    Camera.main.transform.position = originalPosition;
                    Camera.main.transform.rotation = originalRotation;
                    FirstPersonController.Instance.GetComponent<FirstPersonController>().inCutscene = false;
                    foreach (GameObject HUD in HUDs)
                    {
                        HUD.SetActive(true);
                    }
                    while (boss != null)
                    {
                        yield return new WaitForSeconds(1f);
                    }
                    Pistol pistol = FindAnyObjectByType<Pistol>();
                    bulletDamageDone += pistol.bulletDamageDone;
                    zagDamageDone += pistol.zagDamageDone;
                    grenadeDamageDone += pistol.grenadeDamageDone;
                    if (lastMap)
                    {
                        canvasAnimator.SetTrigger("Disappear");
                        yield return new WaitForSeconds(1f);
                        UnityEngine.SceneManagement.SceneManager.LoadScene(finalSceneIndex);
                        break;
                    }
                    else
                    {
                        portal.SetActive(true);
                    }
                }
                else
                {
                    upgrading = true;
                    secondsPassed = 0;
                    UpgradeStation upgradeStation = Instantiate(upgradeStationPrefab, upgradeStationSpawnPoint.position, upgradeStationSpawnPoint.rotation).GetComponent<UpgradeStation>();
                    upgradeStation.upgrades[0] = upgrades[Random.Range(0, upgrades.Length)];
                    upgradeStation.upgrades[1] = upgrades[Random.Range(0, upgrades.Length)];
                    upgradeStation.upgrades[2] = upgrades[Random.Range(0, upgrades.Length)];
                }
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
        canvasAnimator = GameObject.Find("Canvas").GetComponent<Animator>();
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
