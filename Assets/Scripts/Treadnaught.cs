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
    bool spinning = false;
    bool charging = false;

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
            return;
        }
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance < stompAttackRange)
        {
            animator.SetBool("Stomping", true);
        }
        else if (distance < chargeAttackRange)
        {
            Vector3 toPlayer = (player.position - transform.position).normalized;
            float direction = Vector3.Cross(transform.forward, toPlayer).y;
            if (direction > 0)
            {

                StartCoroutine(SpinRight());
            }
            else
            {

                StartCoroutine(SpinLeft());
            }
        }
        else
        {
            animator.SetBool("Stomping", false);
        }
        base.Update();
    }

    IEnumerator SpinRight()
    {
        spinning = true;
        animator.SetBool("SpinningRight", true);
        float elapsedTime = 0f;
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float spinDuration = stateInfo.length;
        while (elapsedTime < spinDuration)
        {
            elapsedTime += Time.deltaTime;
            transform.Rotate(Vector3.up * 20f * Time.deltaTime);
            yield return null;
        }
        animator.SetBool("SpinningRight", false);
        spinning = false;
    }

    IEnumerator SpinLeft()
    {
        spinning = true;
        animator.SetBool("SpinningLeft", true);
        float elapsedTime = 0f;
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float spinDuration = stateInfo.length;
        while (elapsedTime < spinDuration)
        {
            elapsedTime += Time.deltaTime;
            transform.Rotate(Vector3.up * -20f * Time.deltaTime);
            yield return null;
        }
        animator.SetBool("SpinningLeft", false);
        spinning = false;
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
            yield return null;
        }
        yield return new WaitForSeconds(0.2f);
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
}
