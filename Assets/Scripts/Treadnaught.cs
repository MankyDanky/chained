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
    Rigidbody rb;
    public bool spinning = false;
    bool charging = false;
    bool spinDirection = true;

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
        if (charging || spinning)
        {
            base.Update();
            if (spinning)
            {
                if (spinDirection)
                {
                    transform.Rotate(Vector3.up, 25f * Time.deltaTime);
                }
                else
                {
                    transform.Rotate(Vector3.up, -25f * Time.deltaTime);
                }
            }
            return;
        }
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance < stompAttackRange)
        {
            animator.SetBool("Stomping", true);
        }
        else if (distance < chargeAttackRange)
        {
            animator.SetBool("Stomping", false);
            Vector3 toPlayer = (player.position - transform.position).normalized;
            float angleToPlayer = Vector3.Angle(transform.forward, toPlayer);
            if (angleToPlayer < 30f)
            {
                StartCoroutine(Charge());
                return;
            }
            float direction = Vector3.Cross(transform.forward, toPlayer).y;
            if (direction > 0)
            {
                animator.SetBool("SpinningRight", true);
                spinning = true;
                spinDirection = true;
            }
            else
            {
                animator.SetBool("SpinningLeft", true);
                spinning = true;
                spinDirection = false;
            }
        }
        else
        {
            animator.SetBool("Stomping", false);
        }
        base.Update();
    }

    void StopSpinning()
    {
        Debug.Log("Stop Spinning");
        spinning = false;
        animator.SetBool("SpinningRight", false);
        animator.SetBool("SpinningLeft", false);
    }

    IEnumerator Charge()
    {
        charging = true;
        animator.SetTrigger("Charge");
        yield return new WaitForSeconds(1f);
        float elapsedTime = 0f;
        while (elapsedTime < 2f)
        {
            elapsedTime += Time.deltaTime;
            rb.AddForce(transform.forward * chargeSpeed);
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            directionToPlayer.y = 0f;
            if (directionToPlayer.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime);
            }
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);
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
