using UnityEngine;
using UnityEngine.AI;

public class Brute : Enemy
{
    public float attackDamage;
    public float attackRange;
    bool stepping = false;

    protected override void Update()
    {
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
            agent.speed = 0;
            animator.SetBool("isWalking", false);
            animator.SetTrigger("attack");
        }
        else
        {
            agent.speed = 0;
            animator.SetBool("isWalking", true);
            animator.ResetTrigger("attack");
            return;
        }
    }

    private void StartStep()
    {
        stepping = true;
    }

    private void StopStep()
    {
        stepping = false;
    }
}