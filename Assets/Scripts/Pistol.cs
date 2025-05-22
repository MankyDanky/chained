using UnityEngine;

public class Pistol : MonoBehaviour
{
    public float grenadeCooldown = 10f;
    public float grenadeTimer = 0f;
    public bool canThrowGrenade = true;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletSpawnPoint;
    [SerializeField] private float bulletSpeed = 20f;
    [SerializeField] private float fireRate = 0.5f;
    [SerializeField] private GameObject muzzleFlashPrefab;
    [SerializeField] private GameObject gunPivot;
    [SerializeField] private GameObject grenade;
    [SerializeField] private GameObject grenadeSpawnPoint;
    private GameObject grenadeInstance;
    private Animator animator;
    private float targetRecoil = 0.0f;
    private float recoil = 0.0f;
    private float lastFireTime = 0f;
    private float nextFireTime = 0f;


    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        lastFireTime += Time.deltaTime;
        recoil = Mathf.Lerp(recoil, targetRecoil, lastFireTime * 20f);
        gunPivot.transform.localRotation = Quaternion.Euler(-recoil, 0f, 0f);

        if (targetRecoil > 0f)
        {
            targetRecoil -= Time.deltaTime * 10f;
        }
        else
        {
            targetRecoil = 0f;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Fire();
        }

        if (Input.GetKeyDown(KeyCode.G) && canThrowGrenade)
        {
            grenadeInstance = null;
            animator.SetTrigger("grenade");
            grenadeTimer = 0f;
            canThrowGrenade = false;
        }

        if (grenadeTimer < grenadeCooldown)
        {
            grenadeTimer += Time.deltaTime;
        }
        else
        {
            canThrowGrenade = true;
        }
    }

    void Fire()
    {
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        bulletRb.linearVelocity = bulletSpawnPoint.forward * bulletSpeed;
        float scale = Random.Range(0.015f, 0.03f);
        float rotation = Random.Range(0f, 360f);
        GameObject muzzleFlash = Instantiate(muzzleFlashPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation * Quaternion.Euler(rotation, -90f, 0f));
        muzzleFlash.transform.localScale = new Vector3(scale, scale, scale);
        muzzleFlash.transform.SetParent(this.transform);
        lastFireTime = 0f;
        targetRecoil += 3f;
    }

    public void SpawnGrenade()
    {
        if (grenadeInstance != null)
        {
            return;
        }

        grenadeInstance = Instantiate(grenade, grenadeSpawnPoint.transform.position, grenadeSpawnPoint.transform.rotation);
        grenadeInstance.transform.SetParent(grenadeSpawnPoint.transform);
        grenadeInstance.transform.localPosition = Vector3.zero;
        grenadeInstance.transform.localRotation = Quaternion.identity;

        grenadeInstance.GetComponent<Rigidbody>().isKinematic = true;
        grenadeInstance.GetComponent<Collider>().enabled = false;
    }

    public void ThrowGrenade()
    {
        grenadeInstance.GetComponent<Rigidbody>().isKinematic = false;
        grenadeInstance.GetComponent<Collider>().enabled = true;
        grenadeInstance.transform.SetParent(null);
        grenadeInstance.GetComponent<Rigidbody>().AddForce(bulletSpawnPoint.transform.forward * 10f + bulletSpawnPoint.transform.right * 5f, ForceMode.Impulse);
        grenadeInstance.GetComponent<Rigidbody>().AddTorque(new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * 10f, ForceMode.Impulse);
    }
}
