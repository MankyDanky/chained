using UnityEngine;

public class Pistol : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletSpawnPoint;
    [SerializeField] private float bulletSpeed = 20f;
    [SerializeField] private float fireRate = 0.5f;
    [SerializeField] private GameObject muzzleFlashPrefab;
    [SerializeField] private GameObject gunPivot;
    private float targetRecoil = 0.0f;
    private float recoil = 0.0f;
    private float lastFireTime = 0f;
    private float nextFireTime = 0f;
    

    void Start()
    {

    }

    void Update()
    {
        lastFireTime += Time.deltaTime;
        recoil = Mathf.Lerp(recoil, targetRecoil, lastFireTime * 10f);
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
    }

    void Fire()
    {
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        bulletRb.linearVelocity = bulletSpawnPoint.forward * bulletSpeed;
        GameObject muzzleFlash = Instantiate(muzzleFlashPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        muzzleFlash.transform.SetParent(this.transform);
        Destroy(muzzleFlash, 0.4f);
        lastFireTime = 0f;
        targetRecoil += 3f;
    }
}
