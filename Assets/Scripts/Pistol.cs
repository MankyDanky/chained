using UnityEngine.UI;
using UnityEngine;

public class Pistol : MonoBehaviour
{
    public float grenadeCooldown = 10f;
    public float grenadeTimer = 0f;
    public bool canThrowGrenade = true;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject secondaryBulletPrefab;
    [SerializeField] private Transform bulletSpawnPoint;
    [SerializeField] private float bulletSpeed = 20f;
    [SerializeField] private float fireCooldown = 0.5f;
    [SerializeField] private float secondaryFireCooldown = 1.0f;
    [SerializeField] private GameObject muzzleFlashPrefab;
    [SerializeField] private GameObject secondaryMuzzleFlashPrefab;
    [SerializeField] private GameObject gunPivot;
    [SerializeField] private GameObject grenade;
    [SerializeField] private GameObject grenadeSpawnPoint;
    private GameObject grenadeInstance;
    private Animator animator;
    private float targetRecoil = 0.0f;
    private float recoil = 0.0f;
    private float lastFireTime = 0f;
    private float lastSecondaryFireTime = 0f;
    [SerializeField] GameObject fireSound;
    [SerializeField] GameObject secondaryFireSound;
    [SerializeField] Image fireCooldownImage;
    [SerializeField] Image secondaryFireCooldownImage;
    [SerializeField] Image grenadeCooldownImage;


    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (lastFireTime < fireCooldown)
        {
            lastFireTime += Time.deltaTime;
            fireCooldownImage.fillAmount = lastFireTime / fireCooldown;
        }
        else
        {
            fireCooldownImage.fillAmount = 1f;
        }

        if (lastSecondaryFireTime < secondaryFireCooldown)
        {
            lastSecondaryFireTime += Time.deltaTime;
            secondaryFireCooldownImage.fillAmount = lastSecondaryFireTime / secondaryFireCooldown;
        }
        else
        {
            secondaryFireCooldownImage.fillAmount = 1f;
        }
        
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

        if (Input.GetMouseButtonDown(0) && lastFireTime >= fireCooldown)
        {
            Fire();
        }
        if (Input.GetMouseButtonDown(1) && lastSecondaryFireTime >= secondaryFireCooldown)
        {
            FireSecondary();
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
            grenadeCooldownImage.fillAmount = grenadeTimer / grenadeCooldown;
        }
        else
        {
            canThrowGrenade = true;
            grenadeCooldownImage.fillAmount = 1f;
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
        muzzleFlash.transform.SetParent(this.transform.parent);
        lastFireTime = 0f;
        targetRecoil += 3f;
        GameObject sound = Instantiate(fireSound, bulletSpawnPoint.position, bulletSpawnPoint.rotation);

    }

    void FireSecondary()
    {
        GameObject bullet = Instantiate(secondaryBulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        bulletRb.linearVelocity = bulletSpawnPoint.forward * bulletSpeed;
        Instantiate(secondaryFireSound, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        
        // Find closest enemy to center of screen and set as target
        ZagBullet zagBullet = bullet.GetComponent<ZagBullet>();
        if (zagBullet != null)
        {
            Transform closestEnemy = FindClosestEnemyToScreenCenter();
            if (closestEnemy != null)
            {
                zagBullet.target = closestEnemy.gameObject;
            }
        }
        
        float scale = Random.Range(0.015f, 0.03f);
        float rotation = Random.Range(0f, 360f);
        GameObject muzzleFlash = Instantiate(secondaryMuzzleFlashPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation * Quaternion.Euler(rotation, -90f, 0f));
        muzzleFlash.transform.localScale = new Vector3(scale, scale, scale);
        muzzleFlash.transform.SetParent(this.transform.parent);
        lastSecondaryFireTime = 0f;
        targetRecoil += 5f;
    }

    private Transform FindClosestEnemyToScreenCenter()
    {
        Camera playerCamera = Camera.main; // Or get reference to player camera
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
        
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Transform closestEnemy = null;
        float closestDistance = float.MaxValue;
        
        foreach (GameObject enemy in enemies)
        {
            // Convert enemy world position to screen position
            Vector3 screenPos = playerCamera.WorldToScreenPoint(enemy.transform.position);
            
            // Check if enemy is in front of camera
            if (screenPos.z > 0)
            {
                // Calculate distance from screen center
                float screenDistance = Vector2.Distance(new Vector2(screenPos.x, screenPos.y), new Vector2(screenCenter.x, screenCenter.y));
                
                if (screenDistance < closestDistance)
                {
                    closestDistance = screenDistance;
                    closestEnemy = enemy.transform;
                }
            }
        }
        
        return closestEnemy;
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
