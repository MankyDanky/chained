using UnityEngine;
using UnityEngine.AI;

public class Brute : Enemy
{
    [SerializeField] float attackDamage;
    [SerializeField] float attackRange;
    [SerializeField] float specialAttackRange;
    [SerializeField] float specialAttackCooldown;
    [SerializeField] GameObject specialAttackEffect;
    public float specialAttackTimer = 0f;
    bool stepping = false;
    bool attacking = false;
    [SerializeField] GameObject spike;
    

    protected override void Update()
    {
        specialAttackTimer += Time.deltaTime;
        if (isDead) return;
        Move();
        base.Update();
    }

    public override void Attack()
    {
        Debug.Log("Grunt attacks with " + attackDamage + " damage!");
    }

    public override void TakeDamage(float amount, Vector3 hitPoint)
    {

        base.TakeDamage(amount, hitPoint);
    }

    public override void Die()
    {
        agent.speed = 0;
        agent.isStopped = true;
        base.Die();
    }

    private void Move()
    {

        if (stepping)
        {
            agent.speed = moveSpeed;
            agent.SetDestination(player.position);
        }
        else if (Vector3.Distance(transform.position, player.position) < attackRange)
        {
            stepping = false;
            agent.speed = 0;
            attacking = true;
            animator.SetBool("isWalking", false);
            animator.SetBool("isAttacking", true);
            transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
        }
        else if (Vector3.Distance(transform.position, player.position) < specialAttackRange && specialAttackTimer >= specialAttackCooldown && !attacking)
        {
            agent.speed = 0;
            animator.SetBool("isWalking", false);
            animator.SetBool("isAttacking", false);
            animator.SetTrigger("specialAttack");
            specialAttackTimer = 0f;
        }
        else
        {
            attacking = false;
            agent.speed = 0;
            animator.SetBool("isWalking", true);
            animator.SetBool("isAttacking", false);
            animator.ResetTrigger("specialAttack");
            return;
        }
    }

    private void StartStep()
    {
        if (!attacking)
        {
            stepping = true;
        }
    }

    private void StopStep()
    {
        stepping = false;
    }

    private void SpawnSpecialAttackEffect()
    {
        Shake shake = Camera.main.GetComponent<Shake>();
        shake.start = true;
        if (Physics.Raycast(transform.position + transform.forward, Vector3.down, out RaycastHit hit, 2.0f))
        {
            Instantiate(specialAttackEffect, hit.point, Quaternion.identity);
        }
    }

    private void SpawnSpike()
    {
        if (Physics.Raycast(transform.position + transform.forward, Vector3.down, out RaycastHit hit, 2.0f))
        {
            Debug.DrawLine(transform.position + transform.forward, hit.point, Color.red, 2.0f);
            GameObject spikeInstance = Instantiate(spike, hit.point, Quaternion.identity);
            spikeInstance.transform.forward = transform.forward;
        }
    }

    void PunchPlayer()
    {
        if ((transform.position - player.position).magnitude < attackRange)
        {
            playerController.TakeDamage(attackDamage);
            playerController.ApplyImpulse(transform.forward * 10f + Vector3.up * 5f);
        }
    }
}