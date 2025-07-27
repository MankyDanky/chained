using System.Collections;
using UnityEngine;

public class SkyburstSentinel : Enemy
{
    Rigidbody rb;
    bool flying = true;
    [SerializeField] ParticleSystem[] thrusterEffects;
    [SerializeField] AudioSource slamSound;
    [SerializeField] GameObject sludgeBombPrefab;
    [SerializeField] Transform emitterSpawnPoint;
    [SerializeField] float targetYDifference = 6f;
    float sludgeBombCooldown = 1.5f;
    float lastSludgeBombTime = 0f;
    bool slamming = false;
    bool lasering = false;
    [SerializeField] GameObject slamEffect;
    [SerializeField] Transform laserSpawnPoint;
    [SerializeField] GameObject laserStartEffect;
    [SerializeField] GameObject laserEndEffect;
    [SerializeField] GameObject laserBeamEffect;
    GameObject laserStartEffectInstance;
    GameObject laserEndEffectInstance;
    GameObject laserBeamEffectInstance;

    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody>();
        player = FirstPersonController.Instance;
        StartCoroutine(Fly());
    }

    public override void Attack()
    {
        return;
    }
    protected override void Update()
    {
        base.Update();
        if (isDead || fadingIn) return;

        if (flying)
        {
            lastSludgeBombTime += Time.deltaTime;
            float distance = Vector3.Distance(new Vector3(player.position.x, transform.position.y, player.position.z), transform.position);
            if (distance > 3f)
            {
                rb.linearVelocity = (new Vector3(player.position.x, transform.position.y, player.position.z) - transform.position).normalized * 5f;
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(-rb.linearVelocity), Time.deltaTime * 2f);
            }
            else
            {
                rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, Vector3.zero, Time.deltaTime * 2f);
            }
            if (lastSludgeBombTime > sludgeBombCooldown && Random.Range(0f, 1f) < 1 * Time.deltaTime)
            {
                Instantiate(sludgeBombPrefab, emitterSpawnPoint.position, Quaternion.identity);
                lastSludgeBombTime = 0f;
            }
            if (player.position.y > transform.position.y - targetYDifference)
            {
                transform.position += Vector3.up * Time.deltaTime;
            }
        }
        else if (lasering)
        {
            if (player.position.y > transform.position.y - 3)
            {
                transform.position += Vector3.up * Time.deltaTime;
            }
            laserStartEffectInstance.transform.position = laserSpawnPoint.position;
            laserStartEffectInstance.transform.rotation = laserSpawnPoint.rotation;
            if (Physics.Raycast(laserSpawnPoint.position, -transform.forward, out RaycastHit hit, 300f))
            {
                laserEndEffectInstance.transform.position = hit.point;
                laserBeamEffectInstance.transform.localScale = new Vector3(5, hit.distance * 100, 5);
                laserBeamEffectInstance.transform.position = laserSpawnPoint.position;
                laserBeamEffectInstance.transform.LookAt(transform.position + transform.forward * -10f);
                Debug.DrawLine(laserSpawnPoint.position, hit.point, Color.red, 0.1f);
                Debug.Log($"Laser hit at {hit.point}, distance: {hit.distance}");
            }
            else
            {
                laserEndEffectInstance.transform.position = laserSpawnPoint.position - transform.forward * 300f;
                laserBeamEffectInstance.transform.localScale = new Vector3(5, 3000, 5);
                laserBeamEffectInstance.transform.position = laserSpawnPoint.position;
                laserBeamEffectInstance.transform.LookAt(laserSpawnPoint.position + transform.forward * -10f);
            }
            Vector3 direction = (transform.position - new Vector3(player.position.x, transform.position.y, player.position.z)).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 2f);

        }
    }

    public override void Die()
    {
        isDead = true;
        base.Die();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            slamSound.Play();
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Terrain"))
        {
            Instantiate(slamEffect, emitterSpawnPoint.position + Vector3.up * 0.1f, Quaternion.Euler(-90f, 0f, 0f));
            slamming = false;
        }

        if (collision.gameObject.CompareTag("Player") && slamming)
        {
            FirstPersonController playerController = player.GetComponent<FirstPersonController>();
            playerController.TakeDamage(10f);
            playerController.ApplyImpulse(transform.forward * 10f + Vector3.up * 5f);
        }

    }

    IEnumerator Fly()
    {
        flying = true;
        rb.useGravity = false;
        foreach (ParticleSystem ps in thrusterEffects)
        {
            ps.gameObject.SetActive(true);
            ps.Play();
        }
        yield return new WaitForSeconds(10f);
        flying = false;
        rb.useGravity = true;
        foreach (ParticleSystem ps in thrusterEffects)
        {
            ps.Stop();
        }
        StartCoroutine(Slam());
    }

    IEnumerator Slam()
    {
        slamming = true;
        rb.linearVelocity = Vector3.zero;
        while (slamming)
        {
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        StartCoroutine(Laser());
    }

    IEnumerator Laser()
    {
        foreach (ParticleSystem ps in thrusterEffects)
        {
            ps.Play();
        }
        laserStartEffectInstance = Instantiate(laserStartEffect, laserSpawnPoint.position, Quaternion.identity);
        laserEndEffectInstance = Instantiate(laserEndEffect, laserSpawnPoint.position + Vector3.up * 10f, Quaternion.identity);
        laserBeamEffectInstance = Instantiate(laserBeamEffect, laserSpawnPoint.position, Quaternion.identity);
        rb.useGravity = false;
        lasering = true;

        yield return new WaitForSeconds(50f);

        lasering = false;
        Destroy(laserStartEffectInstance);
        Destroy(laserEndEffectInstance);
        Destroy(laserBeamEffectInstance);

        StartCoroutine(Fly());
    }
}
