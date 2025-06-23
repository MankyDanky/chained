using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class Pistol : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletSpawnPoint;
    [SerializeField] private float bulletSpeed = 20f;
    [SerializeField] private float fireRate = 0.5f;
    [SerializeField] private GameObject muzzleFlashPrefab;
    [SerializeField] private GameObject ChargeParti;
    [SerializeField] private GameObject gunPivot;
    [SerializeField] private GameObject LazerBulletPrefab;
    [SerializeField] private float LazerSpeed = 300f;
    private float targetRecoil = Mathf.Clamp(0, 0, 25);
    private float recoil = 0.0f;
    private float lastFireTime = 0f;
    private float nextFireTime = 0f;
    

    void Start()
    {

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
        if (Input.GetMouseButtonDown(1))
        {
            StartCoroutine(Lazer());
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
    private IEnumerator Lazer()
    {
        Debug.Log("Started Charge");
        GameObject Charge = Instantiate(ChargeParti, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        Charge.transform.SetParent(this.transform);
        yield return new WaitForSeconds(5f);
        Debug.Log("Fired");
        GameObject bullet = Instantiate(LazerBulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        bulletRb.linearVelocity = bulletSpawnPoint.forward * LazerSpeed;
        Destroy(Charge);
        yield return new WaitForSeconds(2);
        Destroy(bullet);
    }
}
