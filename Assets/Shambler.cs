using UnityEngine;

public class Shambler : Enemy
{

    [SerializeField] private float attackRange = 2f;

    public override void Attack()
    {

    }

    protected override void Update()
    {
        base.Update();
        if ((player.position - transform.position).magnitude > attackRange)
        {
            agent.SetDestination(player.position);
            agent.speed = moveSpeed;
            animator.SetBool("isAttacking", false);
        }
        else
        {
            agent.speed = 0;
            animator.SetBool("isAttacking", true);
        }
    }
}
