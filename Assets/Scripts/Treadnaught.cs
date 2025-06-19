using System.Collections;
using UnityEngine;

public class Treadnaught : Enemy
{
    [SerializeField] float stompAttackRange = 5f;
    [SerializeField] Transform stompRightSpawn;
    [SerializeField] Transform stompLeftSpawn;
    [SerializeField] GameObject stompEffectPrefab;
    [SerializeField] float stompDamage = 30f;
    [SerializeField] float chargeAttackRange = 10f;
    [SerializeField] float chargeSpeed = 20f;
    [SerializeField] ParticleSystem[] trailParticleEmitters; // 0 for front left, 1 for front right, 2 for back left, 3 for back right
    Rigidbody rb;
    public bool spinning = false;
    bool charging = false;
    bool spinDirection = true;
    [SerializeField] GameObject rocketPrefab;
    [SerializeField] Transform leftRocketSpawnPoint;
    [SerializeField] Transform rightRocketSpawnPoint;
    [SerializeField] float rocketCooldown = 5f;
    float rocketCooldownTimer = 0f;
    [SerializeField] ParticleSystem leftShootEffect;
    [SerializeField] ParticleSystem rightShootEffect;
    [SerializeField] ParticleSystem chargeEffect;

    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody>();
    }

    public override void Attack()
    {
        return;
    }
    protected override void Update()
    {
        if (isDead) return;
        if (charging)
        {
            base.Update();
            return;
        }
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance < stompAttackRange)
        {
            animator.SetBool("Stomping", true);
            spinning = false;
            animator.SetBool("SpinningRight", false);
            animator.SetBool("SpinningLeft", false);
            for (int i = 0; i < trailParticleEmitters.Length; i++)
            {
                if (trailParticleEmitters[i] != null)
                {
                    trailParticleEmitters[i].Stop();
                }
            }
        }
        else if (distance < chargeAttackRange)
        {
            animator.SetBool("Stomping", false);
            Vector3 toPlayer = (player.position - transform.position).normalized;
            float angleToPlayer = Vector3.Angle(transform.forward, toPlayer);
            if (angleToPlayer < 15f)
            {
                spinning = false;
                animator.SetBool("SpinningRight", false);
                animator.SetBool("SpinningLeft", false);
                for (int i = 0; i < trailParticleEmitters.Length; i++)
                {
                    if (trailParticleEmitters[i] != null)
                    {
                        trailParticleEmitters[i].Stop();
                    }
                }
                StartCoroutine(Charge());
                return;
            }
            float direction = Vector3.Cross(transform.forward, toPlayer).y;
            if (direction > 0)
            {
                animator.SetBool("SpinningRight", true);
                spinning = true;
                if (!trailParticleEmitters[0].isPlaying || !trailParticleEmitters[3].isPlaying)
                {
                    trailParticleEmitters[0].Play();
                    trailParticleEmitters[3].Play();
                }
                spinDirection = true;
                transform.Rotate(Vector3.up, 100f * Time.deltaTime);
            }
            else
            {
                animator.SetBool("SpinningLeft", true);
                if (!trailParticleEmitters[1].isPlaying || !trailParticleEmitters[2].isPlaying)
                {
                    trailParticleEmitters[1].Play();
                    trailParticleEmitters[2].Play();
                }
                spinning = true;
                spinDirection = false;
                transform.Rotate(Vector3.up, -100f * Time.deltaTime);
            }
        }
        else
        {
            animator.SetBool("Stomping", false);
            spinning = false;
            for (int i = 0; i < trailParticleEmitters.Length; i++)
            {
                if (trailParticleEmitters[i] != null)
                {
                    trailParticleEmitters[i].Stop();
                }
            }
            Vector3 toPlayer = (player.position - transform.position).normalized;
            float angleToPlayer = Vector3.Angle(transform.forward, toPlayer);
            if (angleToPlayer > 25f)
            {
                float direction = Vector3.Cross(transform.forward, toPlayer).y;
                if (direction > 0)
                {
                    animator.SetBool("SpinningRight", true);
                    spinning = true;
                    if (!trailParticleEmitters[0].isPlaying || !trailParticleEmitters[3].isPlaying)
                    {
                        trailParticleEmitters[0].Play();
                        trailParticleEmitters[3].Play();
                    }
                    spinDirection = true;
                    transform.Rotate(Vector3.up, 100f * Time.deltaTime);
                }
                else
                {
                    animator.SetBool("SpinningLeft", true);
                    if (!trailParticleEmitters[1].isPlaying || !trailParticleEmitters[2].isPlaying)
                    {
                        trailParticleEmitters[1].Play();
                        trailParticleEmitters[2].Play();
                    }
                    spinning = true;
                    spinDirection = false;
                    transform.Rotate(Vector3.up, -100f * Time.deltaTime);
                }
            }
            else
            {
                animator.SetBool("SpinningRight", false);
                animator.SetBool("SpinningLeft", false);
                spinning = false;
                for (int i = 0; i < trailParticleEmitters.Length; i++)
                {
                    if (trailParticleEmitters[i] != null)
                    {
                        trailParticleEmitters[i].Stop();
                    }
                }
            }
            if (rocketCooldownTimer >= rocketCooldown)
            {
                leftShootEffect.Play();
                rightShootEffect.Play();
                Instantiate(rocketPrefab, leftRocketSpawnPoint.position, leftRocketSpawnPoint.rotation);
                Instantiate(rocketPrefab, rightRocketSpawnPoint.position, rightRocketSpawnPoint.rotation);
                rocketCooldownTimer = 0f;
            }
            else
            {
                rocketCooldownTimer += Time.deltaTime;
            }
        }
        base.Update();
    }

    IEnumerator Charge()
    {
        charging = true;
        animator.SetTrigger("Charge");
        yield return new WaitForSeconds(1.4f);
        
        // Set NavMeshAgent for charging
        agent.speed = chargeSpeed;
        agent.acceleration = 25f; // Fast acceleration for charge
        
        float elapsedTime = 0f;
        trailParticleEmitters[3].Play();
        trailParticleEmitters[2].Play();
        chargeEffect.Play();
        
        while (elapsedTime < 2f)
        {
            elapsedTime += Time.deltaTime;
            
            // Update destination to player's current position
            agent.SetDestination(player.position);
            
            yield return null;
        }
        
        chargeEffect.Stop();
        for (int i = 0; i < trailParticleEmitters.Length; i++)
        {
            if (trailParticleEmitters[i] != null)
            {
                trailParticleEmitters[i].Stop();
            }
        }
        
        agent.speed = 0; // Assuming moveSpeed is inherited from Enemy
        agent.acceleration = 8f; // Normal acceleration
        
        yield return new WaitForSeconds(1.5f);
        charging = false;
    }

    void StompRight()
    {
        Instantiate(stompEffectPrefab, stompRightSpawn.position, stompRightSpawn.rotation);
        if ((stompRightSpawn.position - player.position).magnitude < 5f)
        {
            playerController.TakeDamage(stompDamage);
            playerController.ApplyImpulse((player.position - stompRightSpawn.position) * 10f);
        }
    }

    void StompLeft()
    {
        Instantiate(stompEffectPrefab, stompLeftSpawn.position, stompLeftSpawn.rotation);
        if ((stompLeftSpawn.position - player.position).magnitude < 5f)
        {
            playerController.TakeDamage(stompDamage);
            playerController.ApplyImpulse((player.position - stompRightSpawn.position) * 10f);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (charging)
        {
            if (collision.gameObject.CompareTag("Obstacle"))
            {
                Destroy(collision.gameObject);
            } else if (collision.gameObject.CompareTag("Player"))
            {
                playerController.TakeDamage(50f);
                playerController.ApplyImpulse(transform.forward * 20f + Vector3.up * 5f);
            }
        }
    }
}
