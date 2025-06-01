using System.Collections;
using UnityEngine;

public class Drone : Enemy
{
    [SerializeField] float attackDamage;
    [SerializeField] float attackRange;
    [SerializeField] GameObject laserPrefab;
    [SerializeField] Transform laserSpawnPoint;
    bool canShoot = true;
    [SerializeField] float laserCooldown = 1f;
    [SerializeField] float laserSpeed = 10f;
    [SerializeField] GameObject laserSound;
    [SerializeField] Transform[] arms;
    Rigidbody rb;


    protected override void Start()
    {
        rb = GetComponent<Rigidbody>();
        base.Start();
    }
    public override void Attack()
    {

    }

    protected override void Update()
    {
        if (isDead) return;
        transform.LookAt(Vector3.Lerp(transform.position + transform.forward, player.position, Time.deltaTime * 2f));
        for (int i = 0; i < arms.Length; i++)
        {
            Transform arm = arms[i];
            float targetY = -90f + Mathf.Clamp(Vector3.Dot(rb.linearVelocity, transform.forward) * 10f * (1 - 2 * (i % 2)), -60f, 60f);
            Quaternion targetRotation = Quaternion.Euler(arm.localEulerAngles.x, targetY, arm.localEulerAngles.z);
            arm.localRotation = Quaternion.Slerp(arm.localRotation, targetRotation, Time.deltaTime * 6f);
        }
        if ((player.position - transform.position).magnitude > attackRange)
        {
            rb.AddForce((player.position - transform.position).normalized * moveSpeed);
        }
        else
        {
            Debug.Log("Drone is within attack range, stopping agent.");

            if (canShoot)
            {
                Debug.Log("Drone is shooting laser at player!");
                StartCoroutine(ShootLaser());
            }
        }

        base.Update();
    }

    public override void TakeDamage(float amount, Vector3 hitPoint)
    {
        base.TakeDamage(amount, hitPoint);
        rb.AddForce((transform.position - player.position).normalized * 20f, ForceMode.Impulse); 
    }
    
    IEnumerator ShootLaser()
    {
        GameObject laser = Instantiate(laserPrefab, laserSpawnPoint.position, Quaternion.identity);
        laser.transform.LookAt(player.position);
        canShoot = false;
        Instantiate(laserSound, laserSpawnPoint.position, Quaternion.identity);
        Rigidbody laserRb = laser.GetComponent<Rigidbody>();
        float distance = Vector3.Distance(laserSpawnPoint.position, player.position);
        laserRb.linearVelocity = (player.position + Vector3.up * distance/10 - laserSpawnPoint.position).normalized * laserSpeed;
        yield return new WaitForSeconds(laserCooldown);
        canShoot = true;
    }
}
