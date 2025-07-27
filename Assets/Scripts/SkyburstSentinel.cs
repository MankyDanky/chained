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

    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody>();
        player = FirstPersonController.Instance;
        flying = true;
        rb.useGravity = false;
        foreach (ParticleSystem ps in thrusterEffects)
        {
            ps.gameObject.SetActive(true);
            ps.Play();
        }

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
            float distance = Vector3.Distance(new Vector3(player.position.x, transform.position.y, player.position.z), transform.position);
            if (distance > 3f)
            {
                rb.linearVelocity = (new Vector3(player.position.x, transform.position.y, player.position.z) - transform.position).normalized * 5f;
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(rb.linearVelocity), Time.deltaTime * 5f);
            }
            else
            {
                rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, Vector3.zero, Time.deltaTime * 5f);
            }
            if (Random.Range(0f, 1f) < 0.25f * Time.deltaTime)
            {
                Instantiate(sludgeBombPrefab, emitterSpawnPoint.position, Quaternion.identity);
            }
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
    }
}
